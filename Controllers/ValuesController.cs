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
        HashSet<string> urls = new HashSet<string>() { "http://jezebel.com/rss", "http://pictorial.jezebel.com/rss", "http://themuse.jezebel.com/rss", "http://thatswhatshesaid.jezebel.com/rss", "http://theslot.jezebel.com/rss" };

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
               
                var result = cache.Get<List<Item>>(jezCache);
                if(result == null || bypassCache)
                {
                    var lst = new System.Collections.Concurrent.ConcurrentBag<Item>();
                    Parallel.ForEach(urls, url =>
                    {
                        var rssRaw = httpClient.GetStreamAsync(url).Result;
                        var xmlReader = new XmlSerializer(typeof(Rss));
                        var rssDeserialized = (Rss)xmlReader.Deserialize(rssRaw);
                        rssDeserialized.Channel.Items.ForEach(a => lst.Add(a));
                    });

                    result = lst.Where(a => !a.Category.Any(b => b.Text.Contains("deals"))).Distinct(new Item()).ToList();
                    using (var entry = cache.CreateEntry(jezCache))
                    {
                        entry.SetValue(result);
                        entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(1));
                    }
                        
                }

                var rng = new Random();
                var randomArticleNumber = rng.Next(0, result.Count);
                var randomArticle = result[randomArticleNumber];
                var resultString = $"{randomArticle.Title} by {randomArticle.Creator} {randomArticle.Link}";
                return new ContentResult() { Content = resultString, ContentType = "text", StatusCode = 200 };
                
            }
        }
    }
}
