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
        static async Task Main(string[] args)
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
}