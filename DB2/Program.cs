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
        static void Main(string[] args) {


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

        private static HashSet<string> seenTweetIds = new HashSet<string>();

        private static int counter = 0;
        private static void addTweet(VisualizationEntities db, DateTime date, string handle, string tweet, string url) {
            counter++;
            Debug.Print("Counter: " + counter);
            HashSet<string> seenUrls = new HashSet<string>();
            Debug.Print("Tweet: " + tweet);
            Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string id = url.Split('/').Last();
            Tweet newTweet = new DB2.Tweet() { TweetText = tweet, Date = date, Username = handle.Split(' ').First(), TweetID = id };
            if (seenTweetIds.Contains(id) || !db.Tweets.Select(i => i.TweetID).Contains(id)) {
                db.Tweets.AddObject(newTweet);
                db.SaveChanges();
                seenTweetIds.Add(id);
            } else {
                return;
            }
            foreach (Match m in linkParser.Matches(tweet)) {
                string fullUrl = "";
                string uniqueUrl = "";
                try {
                    fullUrl = m.Value.ExpandUrl();
                    uniqueUrl = fullUrl.UnuiqeUrl();
                } catch (Exception ex) {
                    continue;
                }
                if (fullUrl.Count() > 600 || db.Websites.Any(i => i.Url == fullUrl)) {
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
                    try {
                        var page = new PageScraper(fullUrl);

                        var website = new DB2.Website() { Url = page.Url, Title = string.Concat((page.Title() ?? "").Take(300)), TweetID = newTweet.ID };
                        db.Websites.AddObject(website);
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
                        Debug.Print("Failed to scrape page: " + fullUrl);
                    }
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
