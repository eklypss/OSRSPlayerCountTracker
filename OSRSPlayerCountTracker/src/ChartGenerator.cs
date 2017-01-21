using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace OSRSPlayerCountTracker.src
{
    public static class ChartGenerator
    {
        public static async Task<Chart> CreateChart()
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