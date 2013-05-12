using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DataViz {
    class Program {
        static void Main(string[] args) {
            var twitter = new TwitterSearch();
            foreach (var tweet in twitter.Search("data","visualization")) {
                Console.WriteLine(tweet);
                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match m in linkParser.Matches(tweet)) {
                    var fullUrl = m.Value.ExpandUrl();
                    var scraper = new PageScraper(fullUrl);
                    foreach (var img in scraper.GetImages()) {
                        Debug.Print(img);
                    }
                }
            }
        }
    }
}
