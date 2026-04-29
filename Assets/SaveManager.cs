using System;
using System.IO;
using System.Text.Json;
using GEngine.Core;

namespace GEngine.Assets
{
    public static class SaveManager
    {
        public static string RootPath;
        internal static void Initialize()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            RootPath = Path.Combine(appData, Config.GameName);
            try
            {
                if (!Directory.Exists(RootPath))
                {
                    Directory.CreateDirectory(RootPath);
                    Directory.CreateDirectory(Path.Combine(RootPath, "userData"));
                }
            }
            catch { }
        }
        public static void Save(string fileName, object data)
        {
            string directory = Path.Combine (RootPath, "userData");
            string fullPath = Path.Combine(directory, fileName);
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(fullPath, json);
        }
        public static T Load<T>(string fileName)
        {
            string fullPath = Path.Combine(RootPath, "userData", fileName);
            if (!File.Exists(fullPath)) return default;
            string jsonFile = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<T>(jsonFile);
        }
    }
}
