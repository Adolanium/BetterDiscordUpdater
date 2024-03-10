using System.Diagnostics;
using Microsoft.Win32;

namespace BetterDiscordUpdater;

internal class DiscordManager
{
    private static readonly string ConfigFilePath = "config.json";

    internal static void KillDiscord()
    {
        try
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var processes = Process.GetProcessesByName(config.DiscordVersion);

            foreach (var process in processes)
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to kill Discord process: {ex}");
                }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while killing Discord: {ex}");
        }
    }

    internal static void StartDiscord()
    {
        try
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
                    Logger.Warning($"Discord executable not found at: {discordExePath}");
                }
            }
            else
            {
                Logger.Warning($"No app directory found in: {discordPath}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while starting Discord: {ex}");
        }
    }

    internal static void DisableDiscordAutorun()
    {
        try
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var registryKey =
                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey != null)
            {
                var discordAutorunValue = config.DiscordVersion;
                if (registryKey.GetValue(discordAutorunValue) != null)
                    try
                    {
                        registryKey.DeleteValue(discordAutorunValue, false);
                        Logger.Info($"{config.DiscordVersion} auto-run at startup has been disabled.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to disable {config.DiscordVersion} auto-run: {ex}");
                    }
                else
                    Logger.Warning($"{config.DiscordVersion} auto-run entry not found in the registry.");

                registryKey.Close();
            }
            else
            {
                Logger.Warning("Unable to open the registry key for startup items.");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while disabling Discord auto-run: {ex}");
        }
    }
}