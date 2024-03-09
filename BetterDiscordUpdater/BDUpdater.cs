using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDiscordUpdater
{
    internal class BDUpdater
    {
        internal static async Task<byte[]> GetAsar()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("anfeket/betterdiscord-updater");

            var url = "https://betterdiscord.app/Download/betterdiscord.asar";
            var response = await client.GetAsync(url);

            if (response.Headers.TryGetValues("x-bd-version", out var versions))
            {
                var version = versions.FirstOrDefault();
                Console.WriteLine($"Updating to version {version}...");
            }
            else
            {
                Console.WriteLine("We were unable to determine the version, continuing...");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        internal static async Task Update(byte[] data)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var asarPath = Path.Combine(appData, "BetterDiscord", "data", "betterdiscord.asar");

            await WriteData(asarPath, data);
            await Shims(asarPath, localAppData);
        }

        internal static async Task WriteData(string path, byte[] data)
        {
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            await fileStream.WriteAsync(data, 0, data.Length);
        }

        internal static async Task Shims(string asarPath, string localAppData)
        {
            var shimDataPath = asarPath.Replace('\\', '/');
            var shimData = $"require(\"{shimDataPath}\");\nmodule.exports = require(\"./core.asar\");";

            var shimsPath = Path.Combine(localAppData, "DiscordPTB");
            var appDirs = Directory.GetDirectories(shimsPath)
                .Select(Path.GetFileName)
                .Where(name => name.StartsWith("app"))
                .OrderBy(name => name)
                .ToList();

            var lastAppDir = appDirs.Last();
            var shimsFilePath = Path.Combine(shimsPath, lastAppDir, "modules", "discord_desktop_core-1", "discord_desktop_core", "index.js");

            await File.WriteAllTextAsync(shimsFilePath, shimData);
        }
    }
}
