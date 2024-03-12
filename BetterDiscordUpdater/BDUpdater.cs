using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BetterDiscordUpdater
{
    internal class BDUpdater
    {
        private const string ConfigFilePath = "config.json";

        internal static async Task<byte[]> GetAsar()
        {
            const int maxRetries = 3;
            const int retryDelay = 5000;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("anfeket/betterdiscord-updater");
                    var url = "https://betterdiscord.app/Download/betterdiscord.asar";
                    var response = await client.GetAsync(url);
                    return await response.Content.ReadAsByteArrayAsync();
                }
                catch
                {
                    if (retry < maxRetries - 1)
                        await Task.Delay(retryDelay);
                }
            }
            return null;
        }

        internal static async Task<bool> Update(byte[] data)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var asarPath = Path.Combine(appData, "BetterDiscord", "data", "betterdiscord.asar");
            if (!ChecksumsMatch(asarPath, data))
            {
                await WriteData(asarPath, data);
                await Shims(asarPath, localAppData);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task WriteData(string path, byte[] data)
        {
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            await fileStream.WriteAsync(data, 0, data.Length);
        }

        private static async Task Shims(string asarPath, string localAppData)
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var shimDataPath = asarPath.Replace('\\', '/');
            var shimData = $"require(\"{shimDataPath}\");\nmodule.exports = require(\"./core.asar\");";
            var shimsPath = Path.Combine(localAppData, config.DiscordVersion);
            var appDirs = Directory.GetDirectories(shimsPath).Select(Path.GetFileName).Where(name => name.StartsWith("app")).OrderBy(name => name).ToList();
            var lastAppDir = appDirs.Last();
            var shimsFilePath = Path.Combine(shimsPath, lastAppDir, "modules", "discord_desktop_core-1", "discord_desktop_core", "index.js");
            await File.WriteAllTextAsync(shimsFilePath, shimData);
        }

        private static bool ChecksumsMatch(string filePath, byte[] newData)
        {
            if (!File.Exists(filePath)) return false;
            byte[] existingData = File.ReadAllBytes(filePath);
            return GenerateChecksum(existingData) == GenerateChecksum(newData);
        }

        private static string GenerateChecksum(byte[] data)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
