namespace DeployScriptVisualStudioTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    static class Extensions
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            var indexes = new List<int>();

            if (String.IsNullOrEmpty(value))
                return indexes;

            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static bool HasUnfinishedBracketInLeft(this string str, int index)
        {
            var openBrackets = 0;

            for (int i = index - 1; i >= 0; i--)
            {
                if (str[i] == ']') openBrackets--;
                if (str[i] == '[') openBrackets++;
            }

            return openBrackets != 0;
        }

        public static IEnumerable<string> GetLines(this string str)
        {
            return str.Split('\r').Select(line => line.TrimEnd('\r', '\n'));
        }

    }
}
