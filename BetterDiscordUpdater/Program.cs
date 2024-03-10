namespace BetterDiscordUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DiscordManager.KillDiscord();
        Console.WriteLine("Discord killed!");

        var data = await BDUpdater.GetAsar();

        await BDUpdater.Update(data);
        Console.WriteLine("Discord updated!");

        DiscordManager.StartDiscord();
        Console.WriteLine("Discord started!");
    }
}