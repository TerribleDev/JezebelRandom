using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using jezebel.Xml2CSharp;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace jezebel.Controllers
{
    public class ValuesController : Controller
    {
        IMemoryCache cache;
        const string jezCache = "jezebelRss";

        public ValuesController(IMemoryCache cache)
        {
            this.cache = cache;
        }
        [Route("jezme")]
        [HttpGet]
        public async Task<IActionResult> Get(bool bypassCache)
        {
            using (var httpClient = new HttpClient())
            {
                var result = cache.Get<Rss>(jezCache);
                if(result == null || bypassCache)
                {
                    var rssRaw = await httpClient.GetStreamAsync("http://jezebel.com/rss");
                    var xmlReader = new XmlSerializer(typeof(Rss));
                    result = (Rss)xmlReader.Deserialize(rssRaw);
                    using (var entry = cache.CreateEntry(jezCache))
                    {
                        entry.SetValue(result);
                        entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                    }
                        
                }

                var rng = new Random();
                var randomArticleNumber = rng.Next(0, result.Channel.Items.Count);
                var randomArticle = result.Channel.Items[randomArticleNumber];
                var resultString = $"{randomArticle.Title} by {randomArticle.Creator} {randomArticle.Link}";
                return new ContentResult() { Content = resultString, ContentType = "text", StatusCode = 200 };
                
            }
        }
    }
}
