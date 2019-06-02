using LoliSetManager;
using Parse;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoliParse
{
    public static class LoliParse
    {
        public static void Initialize(string appId, string serverAddress)
        {
            ParseClient.Initialize(new ParseClient.Configuration()
            {
                ApplicationId = appId,
                Server = serverAddress
            });
        }
        public static async Task SaveCollection(string owner, string name, IEnumerable<ItemSetInstall> sets)
        {
            ParseObject collection = new ParseObject("Collection");
            collection["Name"] = name;
            collection["Owner"] = owner;
            ParseRelation<ParseObject> relation = collection.GetRelation<ParseObject>("Sets");
            foreach (ItemSetInstall itemSetInstall in sets)
            {
                ParseObject set = new ParseObject("ItemSet");
                set["Name"] = itemSetInstall.Name;
                set["Set"] = (Dictionary<string, object>)itemSetInstall.Set;
                set["Champion"] = itemSetInstall.Champion;
                await set.SaveAsync(); // todo parallelize this
                relation.Add(set);
            }
            await collection.SaveOrUpdateAsync("Name");
        }
        public static AsyncEnumerable<ItemSetInstall> GetCollections(string[] collections)
        {
            return new AsyncEnumerable<ItemSetInstall>(async yield =>
            {
                // Get the collections
                SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
                await collections.ParallelForEachAsync(async collectionId =>
                {
                    string[] split = collectionId.Split("/");
                    if (split.Length < 2)
                    {
                        Console.WriteLine("Invalid collection ID " + collectionId);
                        return;
                    }
                    string username = split[0];
                    string collectionName = split[1];
                    ParseQuery<ParseObject> query = ParseObject.GetQuery("Collection").WhereEqualTo("Owner", username).WhereEqualTo("Name", collectionName);
                    ParseObject result = await query.FirstOrDefaultAsync();
                    ParseRelation<ParseObject> sets = result.Get<ParseRelation<ParseObject>>("Sets");
                    foreach (ParseObject row in await sets.Query.FindAsync())
                    {
                        ItemSetInstall install = new ItemSetInstall()
                        {
                            Set = row.Get<Dictionary<string, object>>("Set"),
                            Champion = row.Get<string>("Champion"),
                            Name = row.Get<string>("Name")
                        };
                        await semaphore.WaitAsync();
                        await yield.ReturnAsync(install);
                        semaphore.Release();
                    }
                });
            });
        }
    }
}
