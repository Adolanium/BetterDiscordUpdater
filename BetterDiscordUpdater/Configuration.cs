using Newtonsoft.Json;

namespace BetterDiscordUpdater;

internal class Configuration
{
    public string DiscordVersion { get; set; }

    internal static Configuration LoadFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<Configuration>(json);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error occurred while deserializing configuration file: {ex}");
                    return new Configuration { DiscordVersion = "Discord" };
                }
            }

            var defaultConfig = new Configuration { DiscordVersion = "Discord" };
            try
            {
                var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return defaultConfig;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error occurred while creating default configuration file: {ex}");
                return defaultConfig;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error occurred while loading configuration: {ex}");
            return new Configuration { DiscordVersion = "Discord" };
        }
    }
}