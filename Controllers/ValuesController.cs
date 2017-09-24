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
        const string breitbartCache = "breitRss";
        HashSet<string> breitbartUrls = new HashSet<string>() { "http://www.breitbart.com/california/feed/", "http://www.breitbart.com/texas/feed/", "http://www.breitbart.com/jerusalem/feed/", "http://www.breitbart.com/london/feed/" };

        public ValuesController(IMemoryCache cache)
        {
            this.cache = cache;
        }
        [Route("jezme")]
        [HttpGet]
        public async Task<IActionResult> Get(bool bypassCache)
        {
            var result = cache.Get<List<Item>>(jezCache);
            if(result == null || bypassCache)
            {
                result = await GetRssItems(urls);
            }
            var rng = new Random();
            var randomArticleNumber = rng.Next(0, result.Count);
            var randomArticle = result[randomArticleNumber];
            var resultString = $"{randomArticle.Title} by {randomArticle.Creator} {randomArticle.Link}";
            return new ContentResult() { Content = resultString, ContentType = "text", StatusCode = 200 }; 
        }
        [Route("breitbartme")]
        [HttpGet]
        public async Task<IActionResult> GetBreit(bool bypassCache)
        {
            var result = cache.Get<List<Item>>(breitbartCache);
            if(result == null || bypassCache)
            {
                result = await GetRssItems(breitbartUrls);
            }
            var rng = new Random();
            var randomArticleNumber = rng.Next(0, result.Count);
            var randomArticle = result[randomArticleNumber];
            var resultString = $"{randomArticle.Title} by {randomArticle.Creator} {randomArticle.Link}";
            return new ContentResult() { Content = resultString, ContentType = "text", StatusCode = 200 }; 
        }

        private async Task<List<Item>> GetRssItems(IEnumerable<string> urls)
        {
            using(var httpClient = new HttpClient())
            {
                var lst = new System.Collections.Concurrent.ConcurrentBag<Item>();
                var tasks = urls.Select(url => httpClient.GetStreamAsync(url).ContinueWith(result => 
                {
                        var xmlReader = new XmlSerializer(typeof(Rss));
                        var rssDeserialized = (Rss)xmlReader.Deserialize(result.Result);
                        rssDeserialized.Channel.Items.ForEach(a=>lst.Add(a));
                }));
                await Task.WhenAll(tasks);   
                return  lst.Where(a => !a.Category.Any(b => b.Text.Contains("deals"))).Distinct(new Item()).ToList();
            }
        }
    }
}
