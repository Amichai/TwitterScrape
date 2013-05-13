using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DataViz {
    public class MediaFetcher {
        public IEnumerable<string> Fetch(params string[] query) {
            HashSet<string> seenUrls = new HashSet<string>();
            var twitter = new TwitterSearch();
            foreach (var tweet in twitter.Search(10, 1, query)) {
                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match m in linkParser.Matches(tweet.Text)) {
                    var fullUrl = m.Value.ExpandUrl();
                    Debug.Print(fullUrl);
                    if (seenUrls.Contains(fullUrl)) {
                        continue;
                    }
                    var scraper = new PageScraper(fullUrl);
                    foreach (var img in scraper.GetImages()) {
                        yield return img;
                    }
                    seenUrls.Add(fullUrl);
                }
            }
        }

        public IEnumerable<PageScraper> GetSites(params string[] query) {
            HashSet<string> seenUrls = new HashSet<string>();
            var twitter = new TwitterSearch();
            foreach (var tweet in twitter.Search(10, 1, query)) {
                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match m in linkParser.Matches(tweet.Text)) {
                    var fullUrl = m.Value.ExpandUrl();
                    Debug.Print(fullUrl);
                    if (seenUrls.Contains(fullUrl)) {
                        continue;
                    }
                    yield return new PageScraper(fullUrl);
                }
            }
        }
    }
}
