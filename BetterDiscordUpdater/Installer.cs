using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Reflection;

namespace BetterDiscordUpdater;

internal class Installer
{
    internal static void SetStartup()
    {
        try
        {
            string srcFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> filesToCopy = new List<string> { "\\BetterDiscordUpdater.exe", "\\config.json" };
            string dstFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BetterDiscordUpdater";
            Directory.CreateDirectory(dstFolder);

            foreach (string file in filesToCopy)
            {
                try
                {
                    File.Copy(srcFolder + file, dstFolder + file, true);
                }
                catch
                {
                    throw new Exception("could not copy files to progreamFiles");
                }
            }

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("BetterDiscordUpdater", dstFolder + filesToCopy[0]);
        }
        catch 
        {
            throw new Exception("SetStartup failed");
        }
    }
}