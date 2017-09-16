using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jezebel
{
    /* 
     Licensed under the Apache License, Version 2.0

     http://www.apache.org/licenses/LICENSE-2.0
     */
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    namespace Xml2CSharp
    {
        [XmlRoot(ElementName = "category")]
        public class Category
        {
            [XmlAttribute(AttributeName = "domain")]
            public string Domain { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "guid")]
        public class Guid
        {
            [XmlAttribute(AttributeName = "isPermaLink")]
            public string IsPermaLink { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "item")]
        public class Item
        {
            [XmlElement(ElementName = "title")]
            public string Title { get; set; }
            [XmlElement(ElementName = "link")]
            public string Link { get; set; }
            [XmlElement(ElementName = "description")]
            public string Description { get; set; }
            [XmlElement(ElementName = "category")]
            public List<Category> Category { get; set; }
            [XmlElement(ElementName = "pubDate")]
            public string PubDate { get; set; }
            [XmlElement(ElementName = "guid")]
            public Guid Guid { get; set; }
            [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
            public string Creator { get; set; }
        }

        [XmlRoot(ElementName = "channel")]
        public class Channel
        {
            [XmlElement(ElementName = "title")]
            public string Title { get; set; }
            [XmlElement(ElementName = "link")]
            public string Link { get; set; }
            [XmlElement(ElementName = "description")]
            public string Description { get; set; }
            [XmlElement(ElementName = "language")]
            public string Language { get; set; }
            [XmlElement(ElementName = "item")]
            public List<Item> Items { get; set; }
        }

        [XmlRoot(ElementName = "rss")]
        public class Rss
        {
            [XmlElement(ElementName = "channel")]
            public Channel Channel { get; set; }
            [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Dc { get; set; }
            [XmlAttribute(AttributeName = "media", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Media { get; set; }
            [XmlAttribute(AttributeName = "kinja", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Kinja { get; set; }
            [XmlAttribute(AttributeName = "version")]
            public string Version { get; set; }
        }

    }

}
