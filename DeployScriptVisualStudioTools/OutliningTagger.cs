namespace DeployScriptVisualStudioTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;

    sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        ITextBuffer Buffer;
        ITextSnapshot Snapshot;
        List<Region> Regions;

        public OutliningTagger(ITextBuffer buffer)
        {
            Buffer = buffer;
            Snapshot = buffer.CurrentSnapshot;
            Regions = new List<Region>();
            ReParse();
            Buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var currentSnapshot = spans[0].Snapshot;
            var entire = new SnapshotSpan(
                spans[0].Start,
                spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);

            var startLineNumber = entire.Start.GetContainingLine().LineNumber;
            var endLineNumber = entire.End.GetContainingLine().LineNumber;

            var currentRegions = Regions;
            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    var startPosition = startLine.Start.Position + region.StartOffset;
                    var length = (endLine.Start.Position + region.EndOffset) - startPosition;

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(currentSnapshot,
                            startLine.Start.Position + region.StartOffset, length),
                        new OutliningRegionTag(false, false, region.Ellipsis, String.Empty));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != Buffer.CurrentSnapshot)
                return;

            ReParse();
        }

        void ReParse()
        {
            var newSnapshot = Buffer.CurrentSnapshot;
            var text = newSnapshot.GetText();

            var newRegions = FindRegions(text);

            //determine the changed span, and send a changed event with the new spans
            var oldSpans = new List<Span>(
                Regions.Select(r => AsSnapshotSpan(r, Snapshot)
                                .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                                .Span));
            var newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            var oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            var newSpanCollection = new NormalizedSpanCollection(newSpans);

            //the changed regions are regions that appear in one set or the other, but not both.
            var removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            var changeStart = int.MaxValue;
            var changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            Snapshot = newSnapshot;
            Regions = newRegions;

            if (changeStart <= changeEnd)
            {
                ITextSnapshot snap = Snapshot;
                if (TagsChanged != null)
                {
                    TagsChanged(this, new SnapshotSpanEventArgs(
                        new SnapshotSpan(Snapshot, Span.FromBounds(changeStart, changeEnd))));
                }
            }
        }

        private List<Region> FindRegions(string text)
        {
            var result = new List<Region>();

            var lines = text.GetLines().ToArray();
            Region region = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var m = DeployScriptClassifier.SectionHeader.Match(lines[i]);
                if (m.Success)
                {
                    if (region != null)
                    {
                        // complete the current region
                        var endOfSectionLineNumber = FindEndOfSectionUpward(lines, i - 1);
                        region.EndLine = endOfSectionLineNumber;
                        region.EndOffset = Math.Max(lines[region.EndLine].Length - 1, 0);
                        result.Add(region);
                        region = null;
                    }

                    // start a new region
                    region = new Region
                    {
                        StartLine = i,
                        Ellipsis = m.Groups[1].Value
                    };
                }
            }

            if (region != null)
            {
                // complete the last region with EOF information
                var endOfSectionLineNumber = FindEndOfSectionUpward(lines, lines.Length - 1);
                region.EndLine = endOfSectionLineNumber;
                region.EndOffset = Math.Max(lines[region.EndLine].Length - 1, 0);
                result.Add(region);
            }

            return result;
        }

        private int FindEndOfSectionUpward(string[] lines, int startFrom)
        {
            for (int i = startFrom; i >= 0; i--)
            {
                // we are looking for non white space and non comments
                if (!String.IsNullOrWhiteSpace(lines[i]) &&
                    !lines[i].TrimStart().StartsWith("#"))
                    return i;
            }

            return 0;
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ?
                   startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }

    }

}
