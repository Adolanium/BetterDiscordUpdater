using System.Diagnostics;
using Microsoft.Win32;

namespace BetterDiscordUpdater;

internal class Installer
{
    internal static void AddExeToStartup()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var exePath = Path.Combine(appDataPath, "BetterDiscordUpdater", "BetterDiscordUpdater.exe");

            using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key.GetValue("BetterDiscordUpdater") == null)
                {
                    key.SetValue("BetterDiscordUpdater", $"\"{exePath}\"");
                    Logger.Info("BetterDiscordUpdater.exe added to startup.");
                }
                else
                {
                    Logger.Info("BetterDiscordUpdater.exe is already in startup.");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while adding BetterDiscordUpdater.exe to startup: {ex}");
        }
    }

    internal static void CopyExeToAppData()
    {
        try
        {
            var currentExePath = Process.GetCurrentProcess().MainModule.FileName;
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var destinationExePath = Path.Combine(appDataPath, "BetterDiscordUpdater", "BetterDiscordUpdater.exe");

            if (!File.Exists(destinationExePath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(destinationExePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationExePath));
                }

                File.Copy(currentExePath, destinationExePath, true);
                Logger.Info("BetterDiscordUpdater.exe copied to AppData directory.");
            }
            else
            {
                Logger.Info("BetterDiscordUpdater.exe already exists in AppData directory. Skipping copy.");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while copying BetterDiscordUpdater.exe to AppData: {ex}");
        }
    }
}