namespace BetterDiscordUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DiscordManager.KillDiscord();
        Console.WriteLine("Discord killed!");
        DiscordManager.DisableStartup();
        Console.WriteLine("Discord startup disabled!");

        Installer.SetStartup();
        Console.WriteLine("Better discord updater copyed to programFiles and set startup!");
        var data = await BDUpdater.GetAsar();

        await BDUpdater.Update(data);
        Console.WriteLine("Discord updated!");

        DiscordManager.StartDiscord();
        Console.WriteLine("Discord started!");
    }
}