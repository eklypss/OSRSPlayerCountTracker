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
        public static int UpdateInterval = 1800000;

        /// <summary>
        /// Paths to data folder and data file.
        /// </summary>
        public static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "data");
        public static string DataFile = string.Empty;
    }
}
