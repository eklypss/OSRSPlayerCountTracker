using System;

namespace OSRSPlayerCountTracker.src
{
    /// <summary>
    /// Basic model for data entries.
    /// </summary>
    public class DataEntry
    {
        public string Date { get; protected set; }
        public string Time { get; protected set; }
        public int PlayerCount { get; set; }

        /// <summary>
        /// Constructor for converting DateTime to two different strings.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="playerCount"></param>
        public DataEntry(DateTime date, int playerCount)
        {
            Date = date.ToString("dd/MM/yyyy");
            Time = date.ToString("HH:mm:ss");
            PlayerCount = playerCount;
        }
    }
}