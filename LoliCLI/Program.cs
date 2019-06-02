using LoliParse;
using LoliSetManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoliCLI
{
    class Program
    {
        /*
         * 
                string LeagueInstallPath;
                if (!args.GetParameter('i', "league-install-path", out LeagueInstallPath))
                {
                    Console.WriteLine("Required -i/--league-install-path not set");
                    return;

                }
                LeagueInstall league;
                try
                {
                    league = new LeagueInstall(LeagueInstallPath);
                }
                catch (LeagueNotFoundException)
                {
                    Console.WriteLine("League of Legends was not found in the directory " + LeagueInstallPath);
                    return;
                }
    */
        static async Task Main(string[] args)
        {
            ArgParser argParser = new ArgParser(args);
            if (argParser.Command == null || argParser.HasFlag('h', "help"))
            {
                Help(argParser.Command);
                return;
            }
            switch (argParser.Command)
            {
                case "build-championgg":
                    await BuildChampionGG(argParser);
                    break;
                case "install":
                    await Install(argParser);
                    break;
                default:
                    Console.WriteLine("Action " + argParser.Command + " not found");
                    Help(argParser.Command);
                    break;
            }
        }

        private static async Task Install(ArgParser args)
        {
            if (!args.GetParameter('p', "parseAddress", out string parseAddress))
            {
                Console.WriteLine("Required param parseAddress not set");
                return;
            }
            if (!args.GetParameter('a', "parseApplication", out string parseApplication))
            {
                Console.WriteLine("Required param parseApplication not set");
                return;
            }
            if (!args.GetParameter('c', "collections", out string collectionsStr))
            {
                Console.WriteLine("Required param collections not set");
                return;
            }
            if (!args.GetParameter('l', "league-of-legends-path", out string leaguePath))
            {
                Console.WriteLine("Required param league-of-legends-path not set");
                return;
            }
            LoliParse.LoliParse.Initialize(parseApplication, parseAddress);
            string[] collections = collectionsStr.Split(",");
            LeagueInstall league = new LeagueInstall(leaguePath);
            await LoliParse.LoliParse.GetCollections(collections).ParallelForEachAsync(async (ItemSetInstall install) =>
            {
                await league.InstallItemSet(install);
            });
        }

        static void Help(string command)
        {
            Console.WriteLine("Help yourself.");
        }
        private static async Task BuildChampionGG(ArgParser args)
        {
            try
            {
                SemaphoreSlim myLock = new SemaphoreSlim(1, 1);
                ChampionGGRipper.ChampionGGRipper ripper = new ChampionGGRipper.ChampionGGRipper();
                List<ItemSetInstall> itemSets = new List<ItemSetInstall>();
                await ripper.Run().ParallelForEachAsync(async (ItemSetInstall itemSetInstall) =>
                {
                    await myLock.WaitAsync();
                    itemSets.Add(itemSetInstall);
                    myLock.Release();
                });
                if (args.GetParameter('p', "parseAddress", out string parseAddress))
                {
                    if (!args.GetParameter('a', "parseApplication", out string parseApplication))
                    {
                        Console.WriteLine("Username not specified, leaving");
                        return;
                    }
                    if (!args.GetParameter('u', "user", out string username))
                    {
                        Console.WriteLine("Username not specified, leaving");
                        return;
                    }
                    if (!args.GetParameter('c', "collection", out string collectionName))
                    {
                        Console.WriteLine("Collection not specified, leaving");
                        return;
                    }
                    LoliParse.LoliParse.Initialize(parseApplication, parseAddress);
                    await LoliParse.LoliParse.SaveCollection(username, collectionName, itemSets);
                    return;
                }
                Console.WriteLine(JsonConvert.SerializeObject(itemSets));
            }
            catch (Exception e)
            {
                LoliSetManager.LoliSetManager.ShowMessage(e, "Unknown");
            }
        }
    }
}
