using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DataViz {
    public class MediaElement {
        JToken json;
        public MediaElement(JToken json) {
            this.json = json;
        }

        public string Link {
            get {
                return (string)json["link"];
            }
        }

        public string Type {
            get {
                return (string)json["type"];
            }
        }
    }
}
