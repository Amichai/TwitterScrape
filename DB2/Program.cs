using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DataViz;
using System.Net;
using System.Data;
using System.Data.OleDb;

namespace DB2 {
    public class Program {
        private static void addhashtagsFromTweets() {
            var db = GetDataContext();
            HashSet<string> seenHashtags = new HashSet<string>();
            var allTweets = db.Tweets.ToList();
            foreach (var tweet in allTweets) {
                foreach (var hashtag in new TweetData(tweet.TweetText).Hashtags()) {
                    Hashtag extantTag;
                    if (!seenHashtags.Contains(hashtag)) {
                        Debug.Print("new tag: " + hashtag);
                        extantTag = new Hashtag() { Tag = hashtag };
                        db.Hashtags.AddObject(extantTag);
                        db.SaveChanges();
                        seenHashtags.Add(hashtag);
                    } else {
                        extantTag = db.Hashtags.Where(i => i.Tag == hashtag).Single();
                    }

                    db.TweetHashtags.AddObject(new TweetHashtag() { TweetID = tweet.ID, HashtagID = extantTag.ID });
                    db.SaveChanges();
                }
            }
        }

        private static void clearAllHashtags() {
            var db = GetDataContext();
            foreach (var hashtag in db.Hashtags) {
                db.Hashtags.DeleteObject(hashtag);
            }
            db.SaveChanges();
            foreach (var tweetH in db.TweetHashtags) {
                db.TweetHashtags.DeleteObject(tweetH);
            }
            db.SaveChanges();
        }

        static void expandUrls() {
            var db = GetDataContext();
            var allWebsites = db.Websites.ToList();
            foreach (var w in allWebsites) {
                try {
                    var expanded = w.Url.ExpandUrl();
                    if (expanded != w.Url) {
                        w.Url = expanded;
                        db.SaveChanges();
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }

        }

        private static void addTitleAndDescription() {
            var db = GetDataContext();
            var allWebsites = db.Websites.ToList();
            foreach (var w in allWebsites) {
                if (w.LongUrlTitle == "") continue;
                Debug.Print(w.ID.ToString());
                try {
                    //w.Description = w.Url.GetDescription();
                    w.LongUrlTitle = w.Url.GetTitle();
                    Debug.Print(w.Title);
                    db.SaveChanges();
                } catch (Exception ex) {
                    //w.Description = "";
                    w.LongUrlTitle = "";
                    db.SaveChanges();
                }
            }
        }

        private static void associateTweetsAndSites() {
            var db = GetDataContext();
            foreach (var website in db.Websites.ToList()) {
                TweetWebsite tweetWebsite = new TweetWebsite() { TweetID = website.TweetID,
                WebsiteID = website.ID };
                db.TweetWebsites.AddObject(tweetWebsite);
                db.SaveChanges();
            }
        }

        static void Main(string[] args) {
            //addhashtagsFromTweets();
            //clearAllHashtags();
            //expandUrls();
            //addTitleAndDescription();

            //associateTweetsAndSites();
            
            ParseTweetFiles();
        }

        private static void ParseTweetFiles() {
            var db = GetDataContext();
            string root = @"C:\Users\Amichai\Data\documents-export-2013-06-16";
            foreach (var path in Directory.GetFiles(root)) {
                var table = LoadXLS(path, "Sheet 1");
                for (int i = 0; i < table.Rows.Count; i++) {
                    string dateString = table.Rows[i][0] as string;
                    string handle = table.Rows[i][1] as string;
                    string tweet = table.Rows[i][2] as string;
                    string url = table.Rows[i][3] as string;
                    var idx = dateString.IndexOf("at");
                    dateString = dateString.Remove(idx, 3);
                    addTweet(db, DateTime.Parse(dateString), handle, tweet, url);
                }
            }
        }

        private static int counter = 0;
        private static void addTweet(VisualizationEntities db, DateTime date, string handle, string tweet, string url) {
            if (counter++ < Properties.Settings.Default.TweetIndex - 1) {
                return;
            }
            Debug.Print("Tweet counter: " + counter);
            Debug.Print("Tweet: " + tweet);
            TweetData tweetData = new TweetData(tweet);
            string tweetId = url.Split('/').Last();
            Tweet newTweet = new DB2.Tweet() { TweetText = tweet, Date = date, Username = handle.Split(' ').First(), TweetID = tweetId };
            if (!db.Tweets.Select(i => i.TweetID).Contains(tweetId)) {
                db.Tweets.AddObject(newTweet);
                db.SaveChanges();
                Properties.Settings.Default.TweetIndex = counter;
                Properties.Settings.Default.Save();
            } else {
                return;
            }

            foreach (var hashtag in tweetData.Hashtags()) {
                Hashtag extantTag;
                if (!db.Hashtags.Select(i => i.Tag).Contains(hashtag)) {
                    Debug.Print("new tag: " + hashtag);
                    extantTag = new Hashtag() { Tag = hashtag };
                    db.Hashtags.AddObject(extantTag);
                    db.SaveChanges();
                } else {
                    extantTag = db.Hashtags.Where(i => i.Tag == hashtag).Single();
                }

                db.TweetHashtags.AddObject(new TweetHashtag() { TweetID = newTweet.ID, HashtagID = extantTag.ID });
                db.SaveChanges();
            }

            foreach (string matchedUrl in tweetData.Urls()) {
                string uniqueUrl = "";
                try {
                    uniqueUrl = matchedUrl.ExpandUrl().UnuiqeUrl();
                } catch (Exception ex) {
                    continue;
                }

                ///Find matched websites:
                var knownSite = db.Websites.Where(i => i.Url == uniqueUrl);
                if (knownSite.Count() > 0) {
                    ///
                    var tweetWebsite = new TweetWebsite() { TweetID = newTweet.ID, WebsiteID = knownSite.First().ID };
                    db.TweetWebsites.AddObject(tweetWebsite);
                    db.SaveChanges();
                    continue;
                }
                ///This is a brand new website
                if (uniqueUrl.Count() > 600 || db.Websites.Any(i => i.Url == uniqueUrl)) {
                    continue;
                }
                Debug.Print("Website: " + uniqueUrl);
                string websiteUrl;
                try {
                    var page = new PageScraper(uniqueUrl);
                    string longUrlTitle = "";
                    string description = "";

                    try {
                        longUrlTitle = page.Url.GetTitle();
                    } catch { Debug.Print("Failed to get title"); }
                    try {
                        description = page.Url.GetDescription();
                    } catch { Debug.Print("Failed to get description"); }
                    var website = new DB2.Website() {
                        Url = page.Url,
                        Title = string.Concat((page.Title() ?? "").Take(300)),
                        TweetID = newTweet.ID,
                        LongUrlTitle = string.Concat(longUrlTitle.Take(100)),
                        Description = string.Concat(description.Take(500))
                    };
                    db.Websites.AddObject(website);
                    db.SaveChanges();

                    var tweetWebsite = new TweetWebsite() { TweetID = newTweet.ID, WebsiteID = website.ID };
                    db.TweetWebsites.AddObject(tweetWebsite);
                    db.SaveChanges();
                    foreach (var m2 in page.Media()) {
                        if (m2.Link.Count() > 600 || db.Media.Any(i => i.Url == m2.Link)) {
                            continue;
                        }
                        try {
                            DB2.Medium media = new DB2.Medium() { Type = m2.Type, Url = m2.Link, SourceSiteID = website.ID };
                            if (m2.Type == "image") {
                                var request = WebRequest.Create(m2.Link);

                                using (var response = request.GetResponse())
                                using (var stream = response.GetResponseStream())
                                using (var b = System.Drawing.Bitmap.FromStream(stream)) {
                                    media.Width = b.Width;
                                    media.Height = b.Height;
                                }
                            }
                            db.Media.AddObject(media);
                            db.SaveChanges();
                            Debug.Print("Media element: " + m2.Link);
                        } catch {
                            break;
                        }
                    }
                    websiteUrl = website.Url;
                } catch {
                    Debug.Print("Failed to scrape page: " + uniqueUrl);
                }
            }
            db.SaveChanges();
        }

        public static VisualizationEntities GetDataContext() {
            //string connectionString = @"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlServerCe.3.5;provider connection string=""Data Source=c:\users\amichai\documents\visual studio 2010\Projects\DataViz\DataStore\TweetData.sdf""";
            string connectionString = @"metadata=res://*/VizData.csdl|res://*/VizData.ssdl|res://*/VizData.msl;provider=System.Data.SqlClient;provider connection string="";data source=gyudbgj7jz.database.windows.net,1433;initial catalog=Visualization;persist security info=True;user id=Amichai@gyudbgj7jz;password=love4cgR;multipleactiveresultsets=True;App=EntityFramework""";

            return new VisualizationEntities(connectionString);
        }

        private static DataTable LoadXLS(string strFile, String sheetName) {
            DataTable dtXLS = new DataTable(sheetName);
            try {
                string strConnectionString = "";
                if (strFile.Trim().EndsWith(".xlsx")) {
                    strConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", strFile);

                } else if (strFile.Trim().EndsWith(".xls")) {

                    strConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", strFile);

                }

                OleDbConnection SQLConn = new OleDbConnection(strConnectionString);

                SQLConn.Open();

                OleDbDataAdapter SQLAdapter = new OleDbDataAdapter();

                //string sql = "SELECT * FROM [" + sheetName + "$] WHERE " + column + " = " + value;
                string sql = "SELECT * FROM [" + sheetName + "$]";

                OleDbCommand selectCMD = new OleDbCommand(sql, SQLConn);

                SQLAdapter.SelectCommand = selectCMD;

                SQLAdapter.Fill(dtXLS);

                SQLConn.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            return dtXLS;

        }
    }
}
