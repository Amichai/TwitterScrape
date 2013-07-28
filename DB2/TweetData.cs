using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DB2 {
    class TweetData {
        private static Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string text;
        public TweetData(string text) {
            this.text = text;
        }
        public IEnumerable<string> Urls() {
            foreach (Match m in linkParser.Matches(text)) {
                yield return m.Value;
            }
        }

        private static Regex hashtagRegex = new Regex(@"((?:#){1}[\w\d]{1,140})");
        public IEnumerable<string> Hashtags() {
            var matches = hashtagRegex.Matches(text);
            foreach (var m in matches) {
                string tag = m.ToString();
                tag = tag.TrimStart('#');
                int result;
                if (int.TryParse(tag, out result)) {
                    continue;
                }
                yield return tag.ToUpper();
            }
        }
    }
}
