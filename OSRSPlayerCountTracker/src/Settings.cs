using System;
using System.IO;

namespace OSRSPlayerCountTracker.src
{
    public static class Settings
    {
        /// <summary>
        /// URI to OSRS website.
        /// </summary>
        public static Uri SourceURI = new Uri("http://oldschool.runescape.com/");

        /// <summary>
        /// Update interval in milliseconds.
        /// </summary>
        //public static int UpdateInterval = 3600000;
        public static int UpdateInterval = 3600000;

        /// <summary>
        /// Paths to data folder, data file and settings file.
        /// </summary>
        public static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "data");

        public static string DataFile = string.Empty;
        public static string SettingsFile = Path.Combine(Settings.DataFolder, @"settings.config");

        /// <summary>
        /// Twitter user details
        /// </summary>
        public static string UserAccessSecret = string.Empty;

        public static string UserAccessToken = string.Empty;
        public static string ConsumerSecret = string.Empty;
        public static string ConsumerKey = string.Empty;

        /// <summary>
        /// Imgur user details
        /// </summary>
        public static string ImgurClientID = string.Empty;

        public static string ImgurSecret = string.Empty;
    }
}