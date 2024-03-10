namespace BetterDiscordUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            Logger.Info("Copying BetterDiscordUpdater.exe to AppData...");
            Installer.CopyExeToAppData();

            Logger.Info("Adding BetterDiscordUpdater.exe to startup...");
            Installer.AddExeToStartup();

            Logger.Info("Disabling Discord auto-run at startup...");
            DiscordManager.DisableDiscordAutorun();

            Logger.Info("Updating BetterDiscord...");

            Logger.Info("Killing Discord processes...");
            DiscordManager.KillDiscord();

            Logger.Info("Downloading BetterDiscord update...");
            var data = await BDUpdater.GetAsar();
            if (data != null)
            {
                Logger.Info("Applying BetterDiscord update...");
                await BDUpdater.Update(data);
                Logger.Info("BetterDiscord update applied successfully.");
            }
            else
            {
                Logger.Warning("Failed to download BetterDiscord update. Skipping update.");
            }

            Logger.Info("Starting Discord...");
            DiscordManager.StartDiscord();
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred: {ex}");
        }
    }
}