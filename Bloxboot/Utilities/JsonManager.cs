using System;
using System.IO;

namespace Bloxboot.Utilities
{
    public class JsonManager
    {
        public virtual string RobloxPath => "/Applications/Roblox.app";
        public virtual string ClientSettingsPath => Path.Combine(RobloxPath, "Contents/MacOS/ClientSettings");
        public virtual string FileLocation => Path.Combine(ClientSettingsPath, "ClientAppSettings.json");

        private string jsonData;

        public void Add(string key, bool value)
        {
            AddData($"\"{key}\": {value.ToString().ToLower()}");
        }

        public void Add(string key, int value)
        {
            AddData($"\"{key}\": {value}");
        }

        public void Add(string key, string value)
        {
            AddData($"\"{key}\": \"{value}\"");
        }

        private void AddData(string data)
        {
            jsonData = jsonData != null ? jsonData + ",\n" + data : "{\n" + data;
        }

        public void WriteFile()
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                jsonData += "\n}";
                File.WriteAllText(FileLocation, jsonData);
            }
            else
            {
                Console.WriteLine("No data to write. Add data before calling WriteFile.");
            }
        }
    }
}
