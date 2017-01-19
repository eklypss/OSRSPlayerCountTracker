using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms.DataVisualization.Charting;
using AngleSharp.Parser.Html;
using Tweetinvi;
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
            FileCheck();
            lastCheck = DateTime.Now;
            mainTimer = new Timer { AutoReset = true, Interval = Settings.UpdateInterval, Enabled = false };
            checkTimer = new Timer { AutoReset = true, Interval = 1000, Enabled = true };
            mainTimer.Elapsed += MainTimer_Elapsed;
            checkTimer.Elapsed += CheckTimer_Elapsed;
            checkTimer.Start();

            if (Console.ReadKey().Key == ConsoleKey.F1)
            {
                var chart = await CreateChart();
                string chartImageFileName = string.Format("player-count-chart {0}.png", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
                chart.SaveImage(Path.Combine(Settings.DataFolder, chartImageFileName), ChartImageFormat.Png);
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        private static void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
            if (Console.ReadKey().Key == ConsoleKey.S)
            {
                Console.WriteLine("Starting main timer.", Color.Green);
                lastCheck = DateTime.Now;
                mainTimer.Start();
                MainTimer_Elapsed(null, null);
                checkTimer.Stop();
                checkTimer.Dispose();
            }
        }

        /// <summary>
        /// Checks that DataFolder exists and creates DataFile.
        /// </summary>
        private static void FileCheck()
        {
            if (!Directory.Exists(Settings.DataFolder)) Directory.CreateDirectory(Settings.DataFolder);
            string dataFileName = string.Format("data {0}.json", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
            Settings.DataFile = Path.Combine(Settings.DataFolder, dataFileName);
            //if (!File.Exists(Settings.DataFile)) File.Create(Settings.DataFile).Close();
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
                Auth.SetUserCredentials(Settings.ConsumerKey, Settings.ConsumerSecret, Settings.UserAccessSecret, Settings.UserAccessSecret);
                Console.WriteLine("Settings successfully loaded.", Color.LightSeaGreen);
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
                    //DataSerializer ds = new DataSerializer();
                    //ds.SerializeData();
                    Console.WriteLine("> New data entry was created, date: " + dataEntry.Date + " " + dataEntry.Time + ", player count: " + dataEntry.PlayerCount + ", change: " + (dataEntry.PlayerCount - lastCount), Color.LightSeaGreen);
                    lastCount = dataEntry.PlayerCount;
                }

                DateTime endTime = lastCheck.AddHours(24);
                TimeSpan timeSpan = endTime.Subtract(DateTime.Now);
                Console.WriteLine(timeSpan.TotalHours);
                if (timeSpan.TotalHours <= 0)
                {
                    lastCheck = DateTime.Now;
                    Console.WriteLine("> Day changed, creating chart, uploading and resetting..", Color.Orange);
                    var chart = await CreateChart();
                    string chartImageFileName = string.Format("player-count-chart {0}.png", DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
                    chart.SaveImage(Path.Combine(Settings.DataFolder, chartImageFileName), ChartImageFormat.Png);
                }
                else Console.WriteLine("Done at: " + lastCheck.AddHours(24).ToLongDateString() + " " + lastCheck.AddHours(24).ToLongTimeString() + " (in " + timeSpan.TotalHours + " hours)");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private static async Task<Chart> CreateChart()
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Hour", typeof(string));
            dataTable.Columns.Add("Player count", typeof(string));
            foreach (var dataEntry in Lists.DataEntryList)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow[0] = dataEntry.Time;
                dataRow[1] = dataEntry.PlayerCount;
                dataTable.Rows.Add(dataRow);
            }
            dataSet.Tables.Add(dataTable);

            Chart chart = new Chart() { DataSource = dataSet.Tables[0], Size = new Size(1920, 1080), AntiAliasing = AntiAliasingStyles.All, TextAntiAliasingQuality = TextAntiAliasingQuality.High };
            Title chartTitle = new Title() { Font = new Font("Roboto", 42), ForeColor = Color.Black, Text = "OSRS Player Counts - " + DateTime.Now.ToLongDateString() };
            chart.Titles.Add(chartTitle);
            Series series = new Series()
            {
                Name = "Series1",
                Color = Color.DeepSkyBlue,
                BorderColor = Color.Black,
                ChartType = SeriesChartType.Column,
                BorderDashStyle = ChartDashStyle.Solid,
                BorderWidth = 0,
                IsValueShownAsLabel = true,
                Font = new Font("Roboto", 13),
                LabelForeColor = Color.Black,
                XValueMember = "Hour",
                YValueMembers = "Player count",
                BackSecondaryColor = Color.Azure
            };
            chart.Series.Add(series);
            ChartArea chartArea = new ChartArea() { Name = "ChartArea1", BorderDashStyle = ChartDashStyle.Solid, AxisY = new Axis() { Minimum = 20000 }, AxisX = new Axis() { Interval = 1 } };

            chart.ChartAreas.Add(chartArea);
            chart.DataBind();
            Lists.DataEntryList.Clear();
            return chart;
        }
    }
}