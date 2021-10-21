using Newtonsoft.Json;
using System.IO;
using UnityEngine;


namespace Kaizen
{
    public class FileManager
    {
        private static readonly string path = Application.persistentDataPath;

        public static void WriteJson<T>(string path, string fileName, string extension, T data)
        {
            string pathToWrite = $"{path}/{fileName}{extension}";

            JsonSerializer jsonSerializer = new JsonSerializer();
            if (File.Exists(pathToWrite))
            {
                File.Delete(pathToWrite);
            }
            StreamWriter streamWriter = new StreamWriter(pathToWrite);
            JsonWriter jsonWriter = new JsonTextWriter(streamWriter);
            jsonSerializer.Serialize(jsonWriter, data);

            jsonWriter.Close();
            streamWriter.Close();
        }

        public static T ReadJson<T>(string fileName, string extension)
        {
            string pathToRead = $"{path}/{fileName}{extension}";
            T obj = default;

            JsonSerializer jsonSerializer = new JsonSerializer();

            if (File.Exists(pathToRead))
            {
                StreamReader streamReader = new StreamReader(pathToRead);
                JsonReader jsonReader = new JsonTextReader(streamReader);
                obj = jsonSerializer.Deserialize<T>(jsonReader);
                jsonReader.Close();
                streamReader.Close();
            }

            return obj;
        }

        public static void Write<T>(string fileName, string extension, T data)
        {
            var pathToWrite = $"{path}/{fileName}{extension}";
            var json = JsonUtility.ToJson(data);

            if (File.Exists(pathToWrite))
            {
                File.Delete(pathToWrite);
            }
            File.WriteAllText(pathToWrite, json);
        }

        public static T Read<T>(string fileName, string extension)
        {
            var pathToRead = $"{path}/{fileName}{extension}";

            if (File.Exists(pathToRead))
            {
                var data = File.ReadAllText(pathToRead);
                return JsonUtility.FromJson<T>(data);
            }

            return default;
        }

        public static void Delete(string fileName, string extension)
        {
            var pathToDelete = $"{path}/{fileName}{extension}";

            if (File.Exists(pathToDelete))
            {
                File.Delete(pathToDelete);
            }
        }

        public static bool FileExists(string fileName, string extension)
        {
            string filePath = $"{path}/{fileName}{extension}";

            if (File.Exists(filePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


