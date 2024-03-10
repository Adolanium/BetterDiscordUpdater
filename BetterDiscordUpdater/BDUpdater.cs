namespace BetterDiscordUpdater;

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
                if (!await CheckInternetConnectivity())
                {
                    Logger.Warning($"No internet connectivity. Retrying in {retryDelay / 1000} seconds... (Attempt {retry + 1}/{maxRetries})");
                    await Task.Delay(retryDelay);
                    continue;
                }

                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("anfeket/betterdiscord-updater");
                var url = "https://betterdiscord.app/Download/betterdiscord.asar";
                var response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("x-bd-version", out var versions))
                {
                    var version = versions.FirstOrDefault();
                    Logger.Info($"Updating to version {version}...");
                }
                else
                {
                    Logger.Warning("We were unable to determine the version, continuing...");
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error occurred while retrieving betterdiscord.asar: {ex}");

                if (retry < maxRetries - 1)
                {
                    Logger.Warning($"Retrying in {retryDelay / 1000} seconds... (Attempt {retry + 1}/{maxRetries})");
                    await Task.Delay(retryDelay);
                }
            }
        }

        Logger.Error("Failed to retrieve betterdiscord.asar after multiple attempts.");
        return null;
    }

    private static async Task<bool> CheckInternetConnectivity()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://betterdiscord.app");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    internal static async Task Update(byte[] data)
    {
        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var asarPath = Path.Combine(appData, "BetterDiscord", "data", "betterdiscord.asar");

            await WriteData(asarPath, data);
            await Shims(asarPath, localAppData);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while updating BetterDiscord: {ex}");
        }
    }

    internal static async Task WriteData(string path, byte[] data)
    {
        try
        {
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            await fileStream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while writing data to {path}: {ex}");
        }
    }

    internal static async Task Shims(string asarPath, string localAppData)
    {
        try
        {
            var config = Configuration.LoadFromFile(ConfigFilePath);
            var shimDataPath = asarPath.Replace('\\', '/');
            var shimData = $"require(\"{shimDataPath}\");\nmodule.exports = require(\"./core.asar\");";
            var shimsPath = Path.Combine(localAppData, config.DiscordVersion);
            var appDirs = Directory.GetDirectories(shimsPath)
                .Select(Path.GetFileName)
                .Where(name => name.StartsWith("app"))
                .OrderBy(name => name)
                .ToList();

            var lastAppDir = appDirs.Last();
            var shimsFilePath = Path.Combine(shimsPath, lastAppDir, "modules", "discord_desktop_core-1",
                "discord_desktop_core", "index.js");
            await File.WriteAllTextAsync(shimsFilePath, shimData);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while creating shims: {ex}");
        }
    }
}