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
        public static string ExpandUrl(this string tinyUrl) {
            string url_base = @"http://api.longurl.org/v2/expand?url=";
            string url = url_base + tinyUrl;
            var content = url.GetContent();
            var doc = new XmlDocument();
            doc.LoadXml(content);
            string expanded = doc.ChildNodes[1].ChildNodes[0].FirstChild.Value;
            return expanded;
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
    }
}
