using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace ResearchBot.Util
{
    public class JsonWrapper
    {
        /// <summary>
        /// Read a JSON from a file
        /// </summary>
        /// <param name="path">The path where the file is</param>
        /// <returns>A JObject containing the JSON of the file</returns>
        public static JObject ReadJSON(string path)
        {
            // Opens the file
            using (StreamReader file = File.OpenText(path))
            // Reads the json from the file
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                // Deserialize the JSON into the JObject
                return (JObject)JToken.ReadFrom(reader);
            }
        }

        public static JObject JsonChooseRandomProject(string path)
        {
            // Opens the file
            using (StreamReader file = File.OpenText(path))
            // Reads the json from the file
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                // Deserialize the JSON into the JObject
                JProperty[] json = (JToken.ReadFrom(reader)).Values<JProperty>().ToArray();
                JArray category = (json[new Random().Next(0, json.Length)].Value as JArray);

                return ((JObject)category[new Random().Next(0, category.Count)]);
            }
        }
    }
}