using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{ 
    public enum eGamePlatform
    {
        None,
        Epic,
        Gog,
        Steam,
        WindowsStore,
        Generic
    }

    public enum eSortOrder
    {
        HighToLow,
        LowToHigh
    }

    public class LocSettings
    {
        static public LocSettings Instance;

        public class SettingsData
        {
            public eGamePlatform platform { set; get; } = eGamePlatform.None;
            public string InstallPath { set; get; }
            public eSortOrder ListSortOrder { set; get; } = eSortOrder.HighToLow;
            public bool EnableFileWatch { set; get; } = false;
            public bool AllowDarkMode { set; get; } = true;
        }

        public SettingsData Data = new();

        private string _filePath;

        public LocSettings(string filePath)
        {
            Instance = this;
            _filePath = filePath;
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Data = JsonConvert.DeserializeObject<SettingsData>(json);
            }
        }

        public void SaveSettings()
        {
            // Write only settings that differ from their default to the settings json file
            var defaultSettings = new SettingsData();
            var changedSettings = new JObject();

            JsonSerializer serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                { Converters = { new StringEnumConverter() } });

            foreach (var property in Data.GetType().GetProperties())
            {
                var currentValue = property.GetValue(this.Data);
                var defaultValue = property.GetValue(defaultSettings);

                if (!Equals(currentValue, defaultValue))
                {
                    changedSettings[property.Name] =
                        JToken.FromObject(currentValue,serializer);
                }
            }

            if (changedSettings.Count > 0)
            {
                string settingsDir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                string json = JsonConvert.SerializeObject(changedSettings, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            else
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
        }
    }
}
