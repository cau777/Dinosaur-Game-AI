using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Spotboo.Unity.Methods
{
    public static class SaveSystem
    {
        public static void TestFile(string filename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
            }

            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
            }
        }

        public static void SaveJson(object data, string filename)
        {
            TestFile(filename);
            string str = JsonUtility.ToJson(data);

            StreamWriter writer = new StreamWriter(filename);
            writer.Write(str);
            writer.Close();
        }

        public static async Task SaveJsonAsync(object data, string filename)
        {
            TestFile(filename);
            string str = JsonUtility.ToJson(data);

            StreamWriter writer = new StreamWriter(filename);
            await writer.WriteAsync(str);
            writer.Close();
        }

        public static T LoadJson<T>(string filename)
        {
            TestFile(filename);

            StreamReader reader = new StreamReader(filename);
            string str = reader.ReadToEnd();
            reader.Close();

            try
            {
                return JsonUtility.FromJson<T>(str);
            }
            catch
            {
                return default;
            }
        }
        
        public static T LoadJson<T>(string filename, T defaultValue)
        {
            TestFile(filename);

            StreamReader reader = new StreamReader(filename);
            string str = reader.ReadToEnd();
            reader.Close();

            try
            {
                return JsonUtility.FromJson<T>(str);
            }
            catch
            {
                return defaultValue;
            }
        }
        
        public static async Task<T> LoadJsonAsync<T>(string filename)
        {
            TestFile(filename);

            StreamReader reader = new StreamReader(filename);
            string str = await reader.ReadToEndAsync();
            reader.Close();

            try
            {
                return JsonUtility.FromJson<T>(str);
            }
            catch
            {
                return default;
            }
        }
        
        public static async Task<T> LoadJsonAsync<T>(string filename, T defaultValue)
        {
            TestFile(filename);

            StreamReader reader = new StreamReader(filename);
            string str = await reader.ReadToEndAsync();
            reader.Close();

            try
            {
                return JsonUtility.FromJson<T>(str);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}