using Newtonsoft.Json;
using System.IO;

namespace OSRSPlayerCountTracker.src
{
    public class DataSerializer
    {
        /// <summary>
        /// Serializes Lists.DataEntryList to Settings.DataFile (JSON)
        /// </summary>
        public void SerializeData()
        {
            File.Delete(Settings.DataFile);
            using (StreamWriter file = File.CreateText(Settings.DataFile))
            {
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, Lists.DataEntryList);
                }
            }
        }
    }
}
