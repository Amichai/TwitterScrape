using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataStore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DataViz;
using Tweet = DataStore.Tweet;
using System.Net;
using System.Data;
using System.Data.OleDb;
using DB2;

namespace csvParser {
    class Program {
        static void Main(string[] args) {
            //parseTable();
            //imageSizes();
            //removeDuplicates();
            
            var db = DB2.Program.GetDataContext();
            //var sorted = db.Websites.OrderByDescending(i => i.HitCount);
            //var searchitem = db.Websites.Where(i => i.Url.Contains("records"));

            //foreach (var w in sorted) {
            //    Debug.Print(w.UniqueUrl);
            //}

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

        private static void removeDuplicates() {
            var db = DataStore.DataUtil.GetDataContext();

            foreach (var w in db.Websites) {
                List<int> indices = new List<int>();
                string token = "?utm";
                if (w.Url.Contains(token)) {
                    indices.Add(w.Url.IndexOf(token));
                }
                token = "&feature";
                if (w.Url.Contains(token)) {
                    indices.Add(w.Url.IndexOf(token));
                }
                string unique;
                if (indices.Count() == 0) {
                    var before = w.Url.Count();
                    var trimmed = w.Url.TrimEnd('/');
                    if (before == trimmed.Count()) continue;
                    unique = trimmed;
                } else {
                    var index = indices.Min();
                    var uniqueUrl = string.Concat(w.Url.Take(index));
                    Debug.Print(w.Url + " " + uniqueUrl);
                    unique = uniqueUrl;
                }
                ///CHeck if we saw this alreay.
                var seenBefore = db.Websites.Where(i => i.UniqueUrl == unique).SingleOrDefault();
                if (seenBefore == null) {
                    ///new site
                    w.UniqueUrl = unique;
                    w.HitCount = 1;
                } else {
                    //increment
                    seenBefore.HitCount++;
                    db.DeleteObject(w);
                }
                db.SaveChanges();
            }
        }

        private static void imageSizes() {
            var db = DataStore.DataUtil.GetDataContext();

            foreach (var m2 in db.Media) {
                if (m2.Type == "image" && (m2.Width == null || m2.Height == null)) {
                    var request = WebRequest.Create(m2.Url);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var b = System.Drawing.Bitmap.FromStream(stream)) {
                        m2.Width = b.Width;
                        m2.Height = b.Height;
                        int area = b.Width * b.Height;
                    }
                    Debug.Print(m2.Url + " " + m2.Width.ToString() + " " + m2.Height.ToString());
                    db.SaveChanges();
                }
            }
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

        private static void parsexslxTable() {
            
        }

        //private static void parseTable() {
        //    //string path = @"C:\Users\Amichai\Data\New feed items (1)(2).csv";
        //    string root = @"C:\Users\Amichai\Data\documents-export-2013-05-22\Feed\Dashboard";
        //    foreach (var path in Directory.GetFiles(root)) {
        //        if (path.Split('.').Last() != "csv") continue;
        //        var f = new StreamReader(path);
        //        var text = f.ReadToEnd();
        //        var db = DataStore.DataUtil.GetDataContext();
        //        var lines = text.Split('\n');
        //        var csv = new CsvHelper.CsvReader(new StreamReader(path));
        //        while (csv.Read()) {
        //            var record = csv.CurrentRecord;
        //            var dateString = record[0];
        //            var idx = dateString.IndexOf("at");
        //            dateString = dateString.Remove(idx, 3);
        //            addTweet(db, DateTime.Parse(dateString), record[1], record[2], record[3]);
        //        }
        //    }
        //}

        private static void clearTweets(TweetDataEntities db) {
            foreach (var tweet in db.Tweet2) {
                db.DeleteObject(tweet);
            }
            db.SaveChanges();

        }
        private static int counter = 0;
        private static void addTweet(VisualizationEntities db, DateTime date, string handle, string tweet, string url) {
            counter++;
            Debug.Print("Counter: " + counter);
            HashSet<string> seenUrls = new HashSet<string>();
            Debug.Print("Tweet: " + tweet);
            Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string id = url.Split('/').Last();
            DB2.Tweet newTweet = new DB2.Tweet() { TweetText = tweet, Date = date, Username = handle.Split(' ').First(), TweetID = id };
            if (!db.Tweets.Select(i => i.TweetID).Contains(id)) {
                db.Tweets.AddObject(newTweet);
                db.SaveChanges();
            } else {
                return;
            }
            foreach (Match m in linkParser.Matches(tweet)) {
                string fullUrl = "";
                string uniqueUrl = "";
                try {
                    fullUrl = m.Value.ExpandUrl();
                    uniqueUrl = fullUrl.UnuiqeUrl();
                } catch {
                    continue;
                }
                if (fullUrl.Count() > 300 || db.Websites.Any(i => i.Url == fullUrl)) {
                    continue;
                }
                Debug.Print("Website: " + fullUrl);
                string websiteUrl;
                var seen = db.Websites.Where(i => i.Url == uniqueUrl).SingleOrDefault();
                if (seen != null) {
                    seen.HitCount++;
                    websiteUrl = seen.Url;
                    db.SaveChanges();
                } else {
                    var page = new PageScraper(fullUrl);
                    var website = new DB2.Website() { Url = page.Url, Title = page.Title() };
                    db.Websites.AddObject(website);
                    db.SaveChanges();
                    foreach (var m2 in page.Media()) {
                        if (m2.Link.Count() > 300 || db.Media.Any(i => i.Url == m2.Link)) {
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

                        }
                    }
                    websiteUrl = website.Url;
                }
            }
            db.SaveChanges();
        }
    }
}
