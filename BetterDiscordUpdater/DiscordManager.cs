using Microsoft.Win32;
using System.Diagnostics;

namespace BetterDiscordUpdater;

internal class DiscordManager
{
    private static readonly string ConfigFilePath = "config.json";

    internal static void KillDiscord()
    {
        var config = Configuration.LoadFromFile(ConfigFilePath);
        var processes = Process.GetProcessesByName(config.DiscordVersion);
        foreach (var process in processes) process.Kill();
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

        if (appDirs.Count > 0)
        {
            var lastAppDir = appDirs.Last();
            var discordExePath = Path.Combine(discordPath, lastAppDir, $"{config.DiscordVersion}.exe");

            if (File.Exists(discordExePath))
            {
                var processStartInfo = new ProcessStartInfo(discordExePath);
                processStartInfo.UseShellExecute = true;
                Process.Start(processStartInfo);
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Discord executable not found at: {discordExePath}");
            }
        }
        else
        {
            Console.WriteLine($"No app directory found in: {discordPath}");
        }
    }
    
    internal static void DisableStartup()
    {
        RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        rk.DeleteValue("Discord", false);
    }
}