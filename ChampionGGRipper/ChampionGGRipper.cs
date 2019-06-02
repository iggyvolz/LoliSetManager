using HtmlAgilityPack;
using LoliSetManager;
using System;
using System.Collections.Async;
using System.Threading;
using System.Threading.Tasks;

namespace ChampionGGRipper
{
    public class ChampionGGRipper : ItemSetRipper
    {
        private static string _version = null;
        private static SemaphoreSlim versionGetLock = new SemaphoreSlim(1, 1);
        public static async Task<string> GetVersion()
        {
            if(_version != null)
            {
                return _version;
            }
            await versionGetLock.WaitAsync(); // make sure two processes aren't getting the URL at the same time
            if(_version == null)
            {
                // No one got it yet, we're stuck with the dirty work
                HtmlDocument doc = await HTMLGetter.GetURL("https://champion.gg/faq/");
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class=\"analysis-holder\"]//strong[1]");
                _version = node.InnerText;
            }
            versionGetLock.Release();
            return _version;
        }

        public readonly AsyncEnumerable<SetDescription> GetSetDescriptions = new AsyncEnumerable<SetDescription>(async yield =>
             {
                 HtmlNodeCollection nodes;
                 try
                 {
                     HtmlDocument doc = await HTMLGetter.GetURL("https://champion.gg");
                     nodes = doc.DocumentNode.SelectNodes("//*[@id=\"home\"]//a[preceding-sibling::*]");
                 }
                 catch(Exception e)
                 {
                     LoliSetManager.LoliSetManager.ShowMessage(e, "Getting item set listings from Champion.GG");
                     return;
                 }
                 SemaphoreSlim myLock = new SemaphoreSlim(1, 1);
                 foreach(HtmlNode node in nodes)
                 {
                     try
                     {
                         SetDescription setDescription = new SetDescription(await GetVersion(), node);
                         await myLock.WaitAsync();
                         await yield.ReturnAsync(setDescription);
                         myLock.Release();
                     }
                     catch (Exception e)
                     {
                         LoliSetManager.LoliSetManager.ShowMessage(e, "Getting an item set from Champion.GG");
                     }
                 }
             });
        protected override async Task Runner(AsyncEnumerator<ItemSetInstall>.Yield yield)
        {
            SemaphoreSlim myLock = new SemaphoreSlim(1, 1);
            await GetSetDescriptions.ParallelForEachAsync(async (SetDescription setDescription) =>
            {
                try
                {
                    ItemSetInstall iSet = await setDescription.GetItemSet();
                    await myLock.WaitAsync();
                    await yield.ReturnAsync(iSet);
                    myLock.Release();
                }
                catch(Exception e)
                {
                    LoliSetManager.LoliSetManager.ShowMessage(e, "Parsing "+setDescription.ToString()+" from Champion.GG");
                }
            });

        }
    }
}
