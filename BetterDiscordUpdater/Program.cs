using Microsoft.Win32;
using System.Diagnostics;

namespace BetterDiscordUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Disabling Discord auto-run at startup...");
            DiscordManager.DisableDiscordAutorun();

            Console.WriteLine("Copying BetterDiscordUpdater.exe to AppData...");
            CopyExeToAppData();

            Console.WriteLine("Adding BetterDiscordUpdater.exe to startup...");
            AddExeToStartup();

            Console.WriteLine("Updating BetterDiscord...");

            Console.WriteLine("Killing Discord processes...");
            DiscordManager.KillDiscord();
            Console.WriteLine("Discord processes terminated.");

            Console.WriteLine("Downloading BetterDiscord update...");
            var data = await BDUpdater.GetAsar();
            Console.WriteLine("BetterDiscord update downloaded successfully.");

            Console.WriteLine("Applying BetterDiscord update...");
            await BDUpdater.Update(data);
            Console.WriteLine("BetterDiscord update applied successfully.");

            Console.WriteLine("Starting Discord...");
            DiscordManager.StartDiscord();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void CopyExeToAppData()
    {
        var currentExePath = Process.GetCurrentProcess().MainModule.FileName;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var destinationExePath = Path.Combine(appDataPath, "BetterDiscordUpdater", "BetterDiscordUpdater.exe");

        if (!Directory.Exists(Path.GetDirectoryName(destinationExePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destinationExePath));
        }

        File.Copy(currentExePath, destinationExePath, true);
        Console.WriteLine("BetterDiscordUpdater.exe copied to AppData directory.");
    }

    private static void AddExeToStartup()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var exePath = Path.Combine(appDataPath, "BetterDiscordUpdater", "BetterDiscordUpdater.exe");

        using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
        {
            if (key.GetValue("BetterDiscordUpdater") == null)
            {
                key.SetValue("BetterDiscordUpdater", $"\"{exePath}\"");
                Console.WriteLine("BetterDiscordUpdater.exe added to startup.");
            }
            else
            {
                Console.WriteLine("BetterDiscordUpdater.exe is already in startup.");
            }
        }
    }
}