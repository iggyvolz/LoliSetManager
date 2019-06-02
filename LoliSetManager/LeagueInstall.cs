using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoliSetManager
{
    public class LeagueNotFoundException : Exception { }
    public class LeagueInstall
    {
        private readonly DirectoryInfo installDir;
        public LeagueInstall(DirectoryInfo installDir)
        {
            if (!installDir.GetFiles().Any(f => f.Name == "LeagueClient.exe"))
            {
                throw new LeagueNotFoundException();
            }
            this.installDir = installDir;
        }
        public LeagueInstall(string installDir) : this(new DirectoryInfo(installDir))
        {

        }
        SemaphoreSlim iolock = new SemaphoreSlim(1, 1);
        public async Task InstallItemSet(ItemSetInstall install)
        {
            await Task.Run(() =>
            {
                DirectoryInfo cwd = installDir;
                cwd = cwd.CreateSubdirectory("Config");
                if (install.Champion == null)
                {
                    cwd = cwd.CreateSubdirectory("Global");
                }
                else
                {
                    cwd = cwd.CreateSubdirectory("Champions");
                    cwd = cwd.CreateSubdirectory(install.Champion);
                }
                cwd = cwd.CreateSubdirectory("Recommended");
                iolock.Wait();
                File.WriteAllText(Path.Combine(cwd.FullName, "Loli_" + install.Name + ".json"), JsonConvert.SerializeObject(install.Set));
                iolock.Release();
            });
        }
    }
}
