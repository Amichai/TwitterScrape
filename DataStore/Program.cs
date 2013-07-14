using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataViz;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Drawing;

namespace DataStore {
    public class Program {
        static void Main(string[] args) {
            TweetDataEntities db;
            MediaFetcher fetcher = new MediaFetcher();
            db = DataUtil.GetDataContext();
            getSites(db, "data", "visualization", "-rt");
            //popularSites();
            //MediaElements(db);
            //addImageSizes(db);

        }

        private static void addImageSizes(TweetDataEntities db){
            foreach(var m in db.Media){
                if(m.ImageArea == null && m.Type == "image"){
                    try {
                        var request = WebRequest.Create(m.Url);

                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        using (var b = Bitmap.FromStream(stream)) {
                            int area = b.Width * b.Height;
                            m.ImageArea = area;
                         }
                        db.SaveChanges();
                    }
                    catch{
                    
                    }
                }
            }
        }

        public static Dictionary<string,int> MediaElements(TweetDataEntities db) {
            Dictionary<string,int> imagesSize = new Dictionary<string,int>();
            foreach (var a in db.Media) {
                if (a.ImageArea == null) continue;
                imagesSize[a.Url] = a.ImageArea.Value;
            }
            //var ordered = imagesSize.OrderByDescending(i => i.Value);
            return imagesSize;
        }

        private static void popularSites(TweetDataEntities db) {
            Dictionary<string, int> siteCounter = new Dictionary<string, int>();
            foreach (var a in db.Tweets) {
                if (a.LinkSite == null) continue;
                var website = db.Websites.Where(i => i.Url == a.LinkSite).First();
                string title = website.Title;
                if (title == null) { continue; }
                if (siteCounter.ContainsKey(title)) {
                    siteCounter[title]++;
                } else {
                    siteCounter[title] = 1;
                }
            }
            var ordered = siteCounter.OrderByDescending(i => i.Value);
        }

        private static void getSites(TweetDataEntities db, params string[] query) {
            HashSet<string> seenUrls = new HashSet<string>();
            var twitter = new TwitterSearch();
            int count = 0;
            foreach (var tweet in twitter.Search(100, 10, query)) {
                Debug.Print("Tweet number: " + (++count).ToString());
                if (db.Tweets.Any(i => i.TweetID == tweet.TweetID)) {
                    continue;
                }
                Tweet t = new Tweet() { Text = tweet.Text, TweetID = tweet.TweetID };
                Debug.Print("Tweet: " + tweet.Text);
                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match m in linkParser.Matches(tweet.Text)) {
                    string fullUrl = "";
                    try {
                        fullUrl = m.Value.ExpandUrl();
                    } catch {
                        continue;
                    }
                    if (db.Websites.Any(i => i.Url == fullUrl)) {
                        continue;
                    }
                    Debug.Print("Website: " + fullUrl);
                    var page = new PageScraper(fullUrl);
                    var website = new Website() { Url = page.Url, Title = page.Title() };
                    db.Websites.AddObject(website);
                    foreach (var m2 in page.Media()) {
                        if (db.Media.Any(i => i.Url == m2.Link)) {
                            continue;
                        }
                        Medium media = new Medium() { Type = m2.Type, Url = m2.Link, SourceSite = website.Url };
                        if (m2.Type == "image") {
                            var request = WebRequest.Create(m2.Link);

                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            using (var b = Bitmap.FromStream(stream)) {
                                int area = b.Width * b.Height;
                                media.ImageArea = area;
                            }
                        }
                        db.Media.AddObject(media);
                        Debug.Print("Media element: " + m2.Link);
                    }
                    t.LinkSite = website.Url;
                   
                }
                db.Tweets.AddObject(t);
                db.SaveChanges();
            }
        }
    }
}
