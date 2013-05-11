namespace DeployScriptVisualStudioTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using System.Text.RegularExpressions;
    using System.Diagnostics;

    sealed class DeployScriptClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;

        List<RegexTag> RegexTags = new List<RegexTag>();
        ClassificationTag tagValue;

        internal static Regex SectionHeader = new Regex(@"^\s*\[([^\]]+)\]\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        

        public DeployScriptClassifier(ITextBuffer buffer, IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;

            RegexTags.Add(new RegexTag(@"^\s*(#.*)$", BuildTag(typeService, PredefinedClassificationTypeNames.Comment), isPredominant: true));
            RegexTags.Add(new RegexTag(SectionHeader, BuildTag(typeService, "SectionHeader")));
            RegexTags.Add(new RegexTag(@"^\s*(exec|include|print)\b", BuildTag(typeService, PredefinedClassificationTypeNames.Keyword)));
            RegexTags.Add(new RegexTag(@"((?<!\\)%(\\%|[^%])+%)", BuildTag(typeService, "Variable")));

            tagValue = BuildTag(typeService, PredefinedClassificationTypeNames.String);
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var snapShot = spans[0].Snapshot;
            foreach (var span in spans)
            {
                var startLine = span.Start.GetContainingLine();
                var endLine = span.End.GetContainingLine();

                var startLineNumber = startLine.LineNumber;
                var endLineNumber = endLine.LineNumber;

                for (int i = startLineNumber; i <= endLineNumber; i++)
                {
                    var line = spans[0].Snapshot.GetLineFromLineNumber(i);
                    var text = line.GetText();
                    var predominantTagFound = false;

                    foreach (var item in RegexTags)
                    {
                        var matches = item.Pattern.Matches(text);
                        if (matches.Count > 0)
                        {
                            foreach (Match m in matches)
                            {
                                var target = m.Groups[1];
                                var location = new SnapshotSpan(snapShot, line.Start.Position + target.Index, target.Length);

                                yield return new TagSpan<ClassificationTag>(location, item.Tag);
                            }

                            if (item.IsPredominant)
                            {
                                predominantTagFound = true;
                                break;
                            }
                        }
                    }

                    if (predominantTagFound) break;

                    // values
                    foreach (var index in text.AllIndexesOf("="))                   
                    {
                        // is it an equal sign inside an unclosed bracket?
                        // like this xxx[a=b]
                        // and not like this xxx[] = 
                        if (!text.HasUnfinishedBracketInLeft(index))
                        {
                            var value = text.Substring(index + 1);
                            var location = new SnapshotSpan(snapShot, line.Start.Position + index + 1, value.Length);
                            yield return new TagSpan<ClassificationTag>(location, tagValue);
                            break;
                        }
                    }
                }
            }
        }

        private ClassificationTag BuildTag(IClassificationTypeRegistryService typeService, string type)
        {
            return new ClassificationTag(typeService.GetClassificationType(type));
        }

#pragma warning disable 0067
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
