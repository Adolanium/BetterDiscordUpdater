using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDiscordUpdater
{
    internal class DiscordManager
    {
        private static readonly string ConfigFilePath = "config.json";

        internal static void KillDiscord()
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var processes = Process.GetProcessesByName(config.DiscordVersion);
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        internal static void StartDiscord()
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var discordPath = Path.Combine(localAppData, config.DiscordVersion);

            var appDirs = Directory.GetDirectories(discordPath)
                .Select(Path.GetFileName)
                .Where(name => name.StartsWith("app"))
                .OrderBy(name => name)
                .ToList();

            var lastAppDir = appDirs.Last();
            var discordExePath = Path.Combine(discordPath, lastAppDir, "DiscordPTB.exe");

            var processStartInfo = new ProcessStartInfo(discordExePath);
            processStartInfo.UseShellExecute = true;
            Process.Start(processStartInfo);

            Environment.Exit(0);
        }

    }
}