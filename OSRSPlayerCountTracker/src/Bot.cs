using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows.Forms.DataVisualization.Charting;
using AngleSharp.Parser.Html;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Console = Colorful.Console;

namespace OSRSPlayerCountTracker.src
{
    public class Bot
    {
        public static int lastCount = 0;
        public static DateTime lastCheck;
        public static Timer mainTimer;
        public static Timer checkTimer;

        /// <summary>
        /// Main startup method, starts timer and keeps window open.
        /// </summary>
        /// <param name="args"></param>
        public static async void Start()
        {
            FileChecker.FileCheck();
            try
            {
                Auth.SetUserCredentials(Settings.ConsumerKey, Settings.ConsumerSecret, Settings.UserAccessToken, Settings.UserAccessSecret);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            lastCheck = DateTime.Now;
            mainTimer = new Timer { AutoReset = true, Interval = Settings.UpdateInterval, Enabled = false };
            checkTimer = new Timer { AutoReset = true, Interval = 1000, Enabled = true };
            mainTimer.Elapsed += MainTimer_Elapsed;
            checkTimer.Elapsed += CheckTimer_Elapsed;
            checkTimer.Start();

            if (Console.ReadKey().Key == ConsoleKey.F1)
            {
                var chart = await ChartGenerator.CreateChart();
                string chartImageFileName = string.Format("player-count-chart {0}.png", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
                chart.SaveImage(Path.Combine(Settings.DataFolder, chartImageFileName), ChartImageFormat.Png);
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        private static void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
            {
                Console.WriteLine("Starting main timer.", Color.Green);
                lastCheck = DateTime.Now;
                mainTimer.Start();
                MainTimer_Elapsed(null, null);
                checkTimer.Stop();
                checkTimer.Dispose();
            }
            else
            {
                TimeSpan untilMidnight = DateTime.Today.AddDays(1.0) - DateTime.Now;
                Console.WriteLine(string.Format("Starting in {0} hours, {1} minutes and {2} seconds.", untilMidnight.Hours, untilMidnight.Minutes, untilMidnight.Seconds), Color.Aquamarine);
            }
        }

        /// <summary>
        /// Downloads SourceURI HTML and calls WebClient_DownloadStringCompleted when done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (WebClient webClient = new WebClient { Encoding = Encoding.UTF8 })
            {
                Console.WriteLine("Downloading page source..", Color.GreenYellow);
                webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                await webClient.DownloadStringTaskAsync(Settings.SourceURI);
            }
        }

        /// <summary>
        /// Parses HTML and creates a new DataEntry, adds it to the DataEntryList and serializes the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("Page source downloaded, parsing data..", Color.GreenYellow);
                var htmlParser = new HtmlParser();
                var htmlDocument = htmlParser.Parse(e.Result);
                var selector = htmlDocument.All.Where(x => x.ClassName == "player-count");
                foreach (var item in selector)
                {
                    string[] textSplit = item.TextContent.Split(' ');
                    DataEntry dataEntry = new DataEntry(DateTime.Now, Int32.Parse(textSplit[3]));
                    Lists.DataEntryList.Add(dataEntry);
                    Console.WriteLine(string.Format("> New data entry was created, date: {0} {1}, player count: {2}, change: {3}", dataEntry.Date, dataEntry.Time, dataEntry.PlayerCount, (dataEntry.PlayerCount - lastCount)), Color.LightSeaGreen);
                    lastCount = dataEntry.PlayerCount;
                }

                DateTime endTime = lastCheck.AddHours(12);
                TimeSpan timeSpan = endTime.Subtract(DateTime.Now);
                Console.WriteLine(timeSpan.TotalHours);
                if (timeSpan.TotalHours <= 0)
                {
                    lastCheck = DateTime.Now;
                    Console.WriteLine("> Day changed, creating chart, uploading and resetting..", Color.Orange);
                    var chart = await ChartGenerator.CreateChart();
                    string chartImageFileName = string.Format("player-count-chart {0}.png", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
                    chart.SaveImage(Path.Combine(Settings.DataFolder, chartImageFileName), ChartImageFormat.Png);
                    byte[] chartImageFile = File.ReadAllBytes(chartImageFileName);
                    var tweetMedia = Upload.UploadImage(chartImageFile);

                    var tweet = Tweet.PublishTweet("#OSRS #RuneScape Player counts for " + DateTime.Now.AddDays(-1).ToLongDateString(), new PublishTweetOptionalParameters
                    {
                        Medias = new List<IMedia> { tweetMedia }
                    });
                }
                else Console.WriteLine(string.Format("Done at: {0} {1} (in {2} hours)", endTime.ToLongDateString(), endTime.ToLongTimeString(), Math.Round(timeSpan.TotalHours, 2)), Color.LightSeaGreen);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}