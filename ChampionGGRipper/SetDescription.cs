using HtmlAgilityPack;
using LoliSetManager;
using System;
using System.Collections.Async;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionGGRipper
{
    public class SetDescription
    {
        private readonly string Version;
        public readonly string Champion;
        public readonly string ChampionSafe;
        public readonly string Position;
        private readonly string Url;

        public SetDescription(string version, HtmlNode node)
        {
            Version = version;
            Champion = node.SelectSingleNode(node.ParentNode.XPath + "/a[1]/div/span").InnerText.Trim();
            Position = node.InnerText.Trim();
            Url = node.GetAttributeValue("href", null);
            ChampionSafe = Url.Split("/").Reverse().ToArray()[1];
        }
        public override string ToString()
        {
            return $"Champion.GG description for champion: {Champion}, position: {Position}";
        }
        public async Task<ItemSetInstall> GetItemSet()
        {
            ItemSet set = new ItemSet() { title = $"CGG {Position} {Version}", sortrank = 1 };
            HtmlDocument doc = await HTMLGetter.GetURL($"https://champion.gg{Url}");
            HtmlNodeCollection builds = doc.DocumentNode.SelectNodes("//div[@class=\"build-wrapper\"]");
            HtmlNode build = builds[0];
            string[] imgSrcs = build.SelectNodes(build.XPath + "//img").Select((HtmlNode node) => node.GetAttributeValue("src", null).Split("/").Last().Split(".").First()).ToArray();
            set.blocks = builds.Select((HtmlNode node) => new ItemSet.Block()
            {
                type = node.PreviousSibling.PreviousSibling.InnerText + " (" + node.SelectSingleNode(node.XPath + "/div[@class=\"build-text\"]").InnerText.Trim() + ")",
                items = build.SelectNodes(build.XPath + "//img").Select((HtmlNode img) =>
                    (ItemSet.Block.Item)img // img element
                        .GetAttributeValue("src", null) // //ddragon.leagueoflegends.com/cdn/9.10.1/img/item/1413.png
                        .Split("/")
                        .Last() // 1413.png
                        .Split(".")
                        .First() // 1413
                    ).ToArray()
            }).ToArray();

            return new ItemSetInstall() { Champion = ChampionSafe, Set = set, Name = $"{Champion}_championgg_{Position.ToLower()}" };
        }
    }
}