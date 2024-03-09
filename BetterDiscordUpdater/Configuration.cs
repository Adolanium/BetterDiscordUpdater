using Newtonsoft.Json;

namespace BetterDiscordUpdater;

internal class Configuration
{
    public string DiscordVersion { get; set; }

    internal static Configuration LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Configuration>(json);
        }
        else
        {
            var defaultConfig = new Configuration { DiscordVersion = "Discord" };
            var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(filePath, json);
            return defaultConfig;
        }
    }
}