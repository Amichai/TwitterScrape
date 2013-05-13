using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace DataViz {
    public class TwitterSearch {
        const string base_url = @"http://search.twitter.com/search.json?";
        public IEnumerable<Tweet> Search(int results, int pagesToReturn, params string[] query) {
            for (int page = 1; page < pagesToReturn; page++) {
                var toAppend = string.Format(@"rpp={0}&page={1}&result_type=mixed&q=", results, page);
                string url = base_url + toAppend;
                foreach (var q in query) {
                    url += q + "%20";
                }
                string content = url.GetContent();
                JObject result = JObject.Parse(content);
                foreach (var a in result["results"]) {
                    yield return new Tweet() { Text = (string)a["text"], TweetID = (string)a["id_str"] };
                }
            }
        }

        public IEnumerable<Tweet> Search(int results, string until, params string[] query) {
            var toAppend = string.Format(@"rpp={0}&until={1}&result_type=mixed&q=", results, until);
            string url = base_url + toAppend;
            foreach (var q in query) {
                url += q + "%20";
            }
            string content = url.GetContent();
            Debug.Print("Twitter response: " + content);
            JObject result = JObject.Parse(content);
            foreach (var a in result["results"]) {
                yield return new Tweet() { Text = (string)a["text"], TweetID = (string)a["id_str"] };
            }
        }
    }

    public class Tweet {
        public string Text { get; set; }
        public string TweetID { get; set; }

    }
}
