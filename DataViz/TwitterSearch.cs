using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataViz {
    public class TwitterSearch {
        const string base_url = @"http://search.twitter.com/search.json?rpp=100&result_type=mixed&q=";
        public IEnumerable<string> Search(params string[] query) {
            string url = base_url;
            foreach (var q in query) {
                url += q + "%20";
            }
            string content = url.GetContent();
            JObject result = JObject.Parse(content);
            foreach(var a in result["results"]){
                yield return (string)a["text"];
            }
        }
    }
}
