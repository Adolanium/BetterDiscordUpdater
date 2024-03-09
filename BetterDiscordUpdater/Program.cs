using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BetterDiscordUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        static void KillDiscord()
        {
            var processes = Process.GetProcessesByName("Discord");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        static void StartDiscord()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var discordPath = Path.Combine(localAppData, "Discord");

            var appDirs = Directory.GetDirectories(discordPath)
                .Select(Path.GetFileName)
                .Where(name => name.StartsWith("app"))
                .OrderBy(name => name)
                .ToList();

            var lastAppDir = appDirs.Last();
            var discordExePath = Path.Combine(discordPath, lastAppDir, "Discord.exe");

            Process.Start(discordExePath);
        }
    }
}
