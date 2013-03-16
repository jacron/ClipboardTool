using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ClipboardTool.domain
{
    class GetLinks
    {
        internal static Collection<LinkItem> get(string file)
        {
            if (file == null) return null;

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(http.*?>.*?<)",  //{<|\b})",
                RegexOptions.Singleline);

            if (m1.Count == 0) return null;

            return cleanLinks2(m1);
        }

        /// <summary>
        /// http://www.dotnetperls.com/scraping-html
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Collection<LinkItem> FindLinks(string file)
        {
            if (file == null) return null;

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            if (m1.Count == 0) return null;

            return cleanLinks(m1);
        }

        private static Collection<LinkItem> cleanLinks(MatchCollection m1)
        {
            Collection<LinkItem> list = new Collection<LinkItem>();

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;

                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                RegexOptions.Singleline);
                if (m2.Success)
                {
                    LinkItem i = new LinkItem();
                    i.Href = m2.Groups[1].Value;

                    // 4.
                    // Remove inner tags from text and add.
                    string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                    RegexOptions.Singleline);
                    i.Text = t;

                    list.Add(i);
                }
            }
            return list;
        }

        private static Collection<LinkItem> cleanLinks2(MatchCollection m1)
        {
            Collection<LinkItem> list = new Collection<LinkItem>();

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LinkItem i = new LinkItem();
                int pos = value.IndexOf(' ');
                if (pos == -1)
                {
                    pos = value.IndexOf('\n');
                }
                if (pos == -1)
                {
                    pos = value.IndexOf('\t');
                }
                if (pos != -1)
                {
                    value = value.Substring(0, pos);
                }
                pos = value.IndexOf('<');
                if (pos != -1)
                {
                    value = value.Substring(0, pos);
                }
                i.Href = value;
                list.Add(i);

            }
            return list;
        }

    }
}
