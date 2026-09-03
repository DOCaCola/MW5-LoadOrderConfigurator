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
        public static string SettingsFileName = @"Settings.json";
        internal const string SettingsDirectoryEnvironmentVariable = "MW5_LOC_SETTINGS_DIRECTORY";
        public const int DefaultModListFontSize = 0;
        public const int MinimumModListFontSize = 10;
        public const int MaximumModListFontSize = 18;

        public static string GetSettingsDirectory()
        {
            string overriddenDirectory = Environment.GetEnvironmentVariable(SettingsDirectoryEnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(overriddenDirectory))
                return Path.GetFullPath(overriddenDirectory);

            string appDataDir = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataDir, @"MW5LoadOrderConfigurator");
        }

        static public LocSettings Instance = new LocSettings(Path.Combine(GetSettingsDirectory(), SettingsFileName));

        public class SettingsData
        {
            public eGamePlatform platform { set; get; } = eGamePlatform.None;
            public string InstallPath { set; get; }
            public eSortOrder ListSortOrder { set; get; } = eSortOrder.HighToLow;
            public bool EnableFileWatch { set; get; } = false;
            public bool AllowDarkMode { set; get; } = true;
            public int ModListFontSize { set; get; } = DefaultModListFontSize;
        }

        public bool SettingsLoaded = false;
        public SettingsData Data = new();

        private string _filePath;

        public LocSettings(string filePath)
        {
            Instance = this;
            _filePath = filePath;
            LoadSettings();
        }

        //Try and load data from previous sessions
        public bool TryLoadProgramSettings()
        {
            //Load settings from previous session:
            try
            {
                LoadSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"ERROR: Something went wrong while loading " + SettingsFileName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return TryInitializeProgramSettings();
        }

        public bool TryInitializeProgramSettings()
        {
            try
            {
                ModsManager.Instance.UpdateGamePaths();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"ERROR: Something went wrong while initializing game paths");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            if (LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore)
                return true;

            if (!Utils.StringNullEmptyOrWhiteSpace(LocSettings.Instance.Data.InstallPath))
                return true;

            return false;
        }

        public void LoadSettings()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Data = JsonConvert.DeserializeObject<SettingsData>(json);
                SettingsLoaded = true;
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

        public static int NormalizeModListFontSize(int fontSize)
        {
            if (fontSize == DefaultModListFontSize)
                return DefaultModListFontSize;

            return Math.Clamp(
                fontSize,
                MinimumModListFontSize,
                MaximumModListFontSize);
        }
    }
}
