using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoliSetManager
{
    public static class HTMLGetter
    {
        public static readonly HtmlWeb web=new HtmlWeb();
        private static Dictionary<string, HtmlDocument> cachedDocuments = new Dictionary<string, HtmlDocument>();
        public static async Task<HtmlDocument> GetURL(string url)
        {
            //Console.WriteLine($"GET {url}");
            if (cachedDocuments.TryGetValue(url, out HtmlDocument doc))
            {
                return doc;
            }
            doc = await web.LoadFromWebAsync(url);
            cachedDocuments[url] = doc;
            return doc;
        }
    }
}
