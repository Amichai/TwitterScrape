using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace DataViz {
    public static class Util {
        static string url_base = @"http://api.longurl.org/v2/expand?url=";
        public static string ExpandUrl(this string tinyUrl) {
            //string url_base = @"http://untiny.me/api/1.0/extract/?url=";
            string allRedirects = "&all-redirects=1";
            string url = url_base + tinyUrl + allRedirects;
            var content = url.GetContent();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string expanded = doc.ChildNodes[1].ChildNodes[0].FirstChild.Value;
            return expanded;
        }

        public static string GetDescription(this string input) {
        //http://api.longurl.org/v2/expand?url=http%3A%2F%2Fbit.ly%2Frg6wp&meta-description=1
            string url = url_base + input + "&meta-description=1";
            var content = url.GetContent();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string description = doc.ChildNodes[1].ChildNodes[1].FirstChild.Value;
            return description;
        }


        public static string GetTitle(this string input) {
            //http://api.longurl.org/v2/expand?url=http%3A%2F%2Fbit.ly%2Frg6wp&meta-description=1
            string url = url_base + input + "&title=1";
            var content = url.GetContent();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string title = doc.ChildNodes[1].ChildNodes[1].FirstChild.Value;
            return title;
        }
        public static string GetContent(this string url) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            var response = req.GetResponse();
            var streamResponse = response.GetResponseStream();

            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuffer = new Char[256];
            int count = streamRead.Read(readBuffer, 0, 256);
            string content = "";
            while (count > 0) {
                String outputData = new String(readBuffer, 0, count);
                content += outputData;
                count = streamRead.Read(readBuffer, 0, 256);
            }
            streamRead.Close();
            streamResponse.Close();
            // Release the response object resources.
            streamResponse.Close();
            return content;
        }

        public static string UnuiqeUrl(this string fullUrl) {
            List<int> indices = new List<int>();
            string token = "?utm";
            if (fullUrl.Contains(token)) {
                indices.Add(fullUrl.IndexOf(token));
            }
            token = "&feature";
            if (fullUrl.Contains(token)) {
                indices.Add(fullUrl.IndexOf(token));
            }
            string unique;
            if (indices.Count() == 0) {
                var before = fullUrl.Count();
                var trimmed = fullUrl.TrimEnd('/');
                unique = trimmed;
            } else {
                var index = indices.Min();
                var uniqueUrl = string.Concat(fullUrl.Take(index));
                unique = uniqueUrl;
            }
            return unique;
        }
    }
}
