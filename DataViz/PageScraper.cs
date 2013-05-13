using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DataViz {
    public class PageScraper {
        public string Url { get; set; }
        JObject diffBot;


        public PageScraper(string url) {
            this.Url = url;
            string diffBot_url = diffBot_base + url;
            var content = diffBot_url.GetContent();
            diffBot = JObject.Parse(content);
        }

        public IEnumerable<string> GetImages() {
            return Media().Where(i => i.Type == "image").Select(i => i.Link);
        }

        public string Title() {
            return (string)diffBot["title"];
        }

        public string Type() {
            return (string)diffBot["type"];
        }

        public string Text() {
            return (string)diffBot["text"];
        }

        public string Icon() {
            return (string)diffBot["icon"];
        }

        public string Author() {
            return (string)diffBot["author"];
        }

        public IEnumerable<MediaElement> Media() {
            JToken tok;
            if (diffBot.TryGetValue("media", out tok)) {
                foreach (var m in diffBot["media"]) {
                    yield return new MediaElement(m);
                }
            }
        }

        string diffBotToken = "845def00ee8b32409701c09590686fd5";
        string diffBot_base = @"http://www.diffbot.com/api/article?token=845def00ee8b32409701c09590686fd5&url=";
    }
}
