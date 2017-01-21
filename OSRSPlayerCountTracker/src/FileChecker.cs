using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using Console = Colorful.Console;

namespace OSRSPlayerCountTracker.src
{
    public static class FileChecker
    {
        /// <summary>
        /// Creates program files if needed.
        /// </summary>
        public static void FileCheck()
        {
            if (!Directory.Exists(Settings.DataFolder)) Directory.CreateDirectory(Settings.DataFolder);
            string dataFileName = string.Format("data {0}.json", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
            Settings.DataFile = Path.Combine(Settings.DataFolder, dataFileName);
            if (!File.Exists(Settings.SettingsFile))
            {
                File.Create(Settings.SettingsFile).Close();
                Configuration config = ConfigurationManager.OpenExeConfiguration(Settings.SettingsFile);
                config.AppSettings.Settings.Add("UserAccessSecret", "not_set");
                config.AppSettings.Settings.Add("UserAccessToken", "not_set");
                config.AppSettings.Settings.Add("ConsumerSecret", "not_set");
                config.AppSettings.Settings.Add("ConsumerKey", "not_set");
                config.AppSettings.Settings.Add("ImgurClientID", "not_set");
                config.AppSettings.Settings.Add("ImgurSecret", "not_set");
                config.Save(ConfigurationSaveMode.Minimal);
                Console.WriteLine("Settings file was created, change your settings and restart the program.", Color.Red);
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Settings.SettingsFile);
                Settings.UserAccessSecret = config.AppSettings.Settings["UserAccessSecret"].Value;
                Settings.UserAccessToken = config.AppSettings.Settings["UserAccessToken"].Value;
                Settings.ConsumerSecret = config.AppSettings.Settings["ConsumerSecret"].Value;
                Settings.ConsumerKey = config.AppSettings.Settings["ConsumerKey"].Value;
                Settings.ImgurClientID = config.AppSettings.Settings["ImgurClientID"].Value;
                Settings.ImgurSecret = config.AppSettings.Settings["ImgurSecret"].Value;
                Console.WriteLine("Settings successfully loaded.", Color.LightSeaGreen);
            }
        }
    }
}