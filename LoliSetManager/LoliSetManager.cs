using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LoliSetManager
{
    public static class LoliSetManager
    {
        public const bool ENABLE_PARALLEL = false;
        private static readonly HttpClient http=new HttpClient();
        public static async Task Install(string installPath, string url, string champion, string name)
        {
            LeagueInstall install = new LeagueInstall(installPath);
            ItemSet itemSet = JsonConvert.DeserializeObject<ItemSet>(await http.GetStringAsync(url));
            if(name != null) itemSet.title = name;
            //install.InstallItemSet(itemSet, champion,name);
        }

        public static void ShowMessage(Exception ex,
            string doingWhat,
            [CallerLineNumber] int lineNumber = -1,
            [CallerMemberName] string caller = "<UNKNOWN CALLER>",
            [CallerFilePath] string path = "<UNKNOWN Path>")
        {
            Console.WriteLine("Encountered an issue while " + doingWhat + ".  If this was not expected, please submit an issue with the following message:");
            Console.WriteLine($"Path: {path}, line number: {lineNumber}, caller: {caller}");
            foreach(Exception e in ex.GetInnerExceptions())
            {
                Console.WriteLine("Caught exception " + e.GetType().ToString() + ": " + e.Message + ", at line " + lineNumber + " from " + caller + " at " + path + "." + e.StackTrace);
            }
        }

    }
}
