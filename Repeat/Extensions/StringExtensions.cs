using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Repeat.Extensions
{
    public static class StringExtensions
    {
        public static List<string> SplitForCodeLines(this string input)
        {
            List<string> lines = new List<string> { "" };

            foreach (Match m in Regex.Matches(input, @"<code>(.+?)<\/code>|(.| )?", RegexOptions.Singleline))
            {
                if (m.Value.Contains(@"<code>") && (m.Value.Contains(@"</code>")))
                {
                    if (lines[lines.Count - 1].ToString() == "")
                    {
                        lines[lines.Count - 1] = m.Value;
                    }
                    else
                    {
                        lines.Add(m.Value);
                    }
                    lines.Add("");
                }
                else
                {
                    lines[lines.Count - 1] += m.Value;
                }
            }

            return lines;
        }
    }
}