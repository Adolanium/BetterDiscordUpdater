namespace BetterDiscordUpdater;

internal class Logger
{
    private static readonly string LogFilePath = "log.txt";

    public static void Info(string message)
    {
        Log(message, "INFO");
    }

    public static void Warning(string message)
    {
        Log(message, "WARNING");
    }

    public static void Error(string message)
    {
        Log(message, "ERROR");
    }

    private static void Log(string message, string level)
    {
        var timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        var logMessage = $"{timestamp} [{level}] {message}";

        Console.WriteLine(logMessage);

        try
        {
            if (!File.Exists(LogFilePath)) File.Create(LogFilePath).Dispose();

            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write log message to file: {ex.Message}");
        }
    }
}