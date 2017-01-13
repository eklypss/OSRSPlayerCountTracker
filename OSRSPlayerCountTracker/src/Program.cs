using AngleSharp.Parser.Html;
using OSRSPlayerCountTracker.src;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;

namespace OSRSPlayerCountTracker
{
    class Program
    {
        public static int lastCount = 0;

        /// <summary>
        /// Main startup method, starts timer and keeps window open.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FileCheck();
            Timer mainTimer = new Timer { AutoReset = true, Interval = Settings.UpdateInterval, Enabled = true };
            mainTimer.Elapsed += MainTimer_Elapsed;
            mainTimer.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// Checks that DataFolder exists and creates DataFile.
        /// </summary>
        private static void FileCheck()
        {
            if (!Directory.Exists(Settings.DataFolder)) Directory.CreateDirectory(Settings.DataFolder);
            string dataFileName = string.Format("data {0}.json", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
            Settings.DataFile = Path.Combine(Settings.DataFolder, dataFileName);
            if (!File.Exists(Settings.DataFile)) File.Create(Settings.DataFile).Close();
        }

        /// <summary>
        /// Downloads SourceURI HTML and calls WebClient_DownloadStringCompleted when done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (WebClient webClient = new WebClient { Encoding = Encoding.UTF8 })
            {
                Console.WriteLine("Downloading page source..");
                webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                webClient.DownloadStringAsync(Settings.SourceURI);
            }
        }

        /// <summary>
        /// Parses HTML and creates a new DataEntry, adds it to the DataEntryList and serializes the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Console.WriteLine("Page source downloaded, parsing data..");
            var htmlParser = new HtmlParser();
            var htmlDocument = htmlParser.Parse(e.Result);
            var selector = htmlDocument.All.Where(x => x.ClassName == "player-count");
            foreach(var item in selector)
            {
                string[] textSplit = item.TextContent.Split(' ');
                DataEntry dataEntry = new DataEntry(DateTime.Now, Int32.Parse(textSplit[3]));
                Lists.DataEntryList.Add(dataEntry);
                DataSerializer ds = new DataSerializer();
                ds.SerializeData();
                Console.WriteLine("New data entry was created, date: " + dataEntry.Date + " " + dataEntry.Time + ", player count: " + dataEntry.PlayerCount + ", change: " + (dataEntry.PlayerCount - lastCount));
                lastCount = dataEntry.PlayerCount;
            }
        }
    }
}
