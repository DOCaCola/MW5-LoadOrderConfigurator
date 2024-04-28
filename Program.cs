using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static MW5_Mod_Manager.MainLogic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Forms.Application;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }

    /// <summary>
    /// Contains most of the background logic and operations
    /// Also has some dataobjects to keep track of various internal statuses.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class MainLogic
    {
        public float Version = 0f;

        public enum eGamePlatform
        {
            None,
            Epic,
            Gog,
            Steam,
            WindowsStore
        }

        public eGamePlatform GamePlatform = eGamePlatform.None;
        public string InstallPath = "";
        public string GameVersion = "";

        public enum eModPathType
        {
            Main,
            Steam,
            // Apparently WindowsStore version can store mods in Local AppData
            AppData
        }
        public ArrayByEnum<string,eModPathType> ModsPaths = new();

        public ProgramData ProgramSettings = new();

        // User made changes not written to files
        public bool ModSettingsTainted = false;

        public JObject parent;
        // Directories found in all mod paths
        public List<string> FoundDirectories = new();
        public Dictionary<string, string> DirectoryToPathDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> PathToDirectoryDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Mod data as loaded from the mods' mod.json file
        public Dictionary<string, ModObject> ModDetails = new Dictionary<string, ModObject>(StringComparer.OrdinalIgnoreCase);
        // Valid mod directories
        public List<string> ModDirectories = new();
        public Dictionary<string, bool> ModList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, OverridingData> OverrridingData = new Dictionary<string, OverridingData>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Presets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static Color OverriddenColor = Color.FromArgb(131, 101, 0);
        public static Color OverridingColor = Color.FromArgb(80, 37, 192);
        public static Color OverriddenOveridingColor = Color.FromArgb(170,73,97);

        public static Color HighPriorityColor = Color.FromArgb(252, 54, 63);
        public static Color LowPriorityColor = Color.FromArgb(17, 137, 21);

        public static string SettingsFileName = @"Settings.json";
        public static string PresetsFileName = @"Presets.json";

        public class VortexDeploymentModData
        {
            public string fullpath;
            public string nexusModsId;
        }

        public Dictionary<string, VortexDeploymentModData> VortexDeploymentData = new Dictionary<string, VortexDeploymentModData>(StringComparer.OrdinalIgnoreCase);

        public class ModData
        {
            public float OriginalLoadOrder = Single.NaN;

            public enum ModOrigin
            {
                Unknown,
                Steam,
                Nexusmods
            }

            public ModOrigin Origin = ModOrigin.Unknown;
            public string NexusModsId = "";
            // Mod's pak file size
            public long ModFileSize = 0;
        }

        public Dictionary<string, ModData> Mods = new Dictionary<string, ModData>();

        public string rawJson;


        public bool GameIsConfigured()
        {
            if (GamePlatform == eGamePlatform.None)
                return false;

            if (Utils.StringNullEmptyOrWhiteSpace(InstallPath))
                return false;

            return true;
        }

        public string GetModListJsonFilePath()
        {
            string path;
            switch (GamePlatform)
            {
                case eGamePlatform.WindowsStore:
                    path = ModsPaths[eModPathType.AppData];
                    break;
                default:
                    path = ModsPaths[eModPathType.Main];
                    break;
            }

            return Path.Combine(path, @"modlist.json");
        }

        /// <summary>
        /// Starts suquence to load all mods from folders, loads modlist, combines modlist with found folders structure
        /// and loads details of each found mod.
        /// </summary>
        public void LoadFromFiles()
        {
            CheckMainModDirectoryExists();
            //find all mod directories and parse them into just folder names:
            ParseDirectories();
            //parse modlist.json
            ModListParser();

            ReadVortexDeploymentData();
            //Load each mods mod.json and store in Dict.
            LoadAllModDetails();
            //Combine so we have all mods in the ModList Dict for easy later use and writing to JObject
            CombineDirModList();
            DetermineBestAvailableGameVersion();
        }

        private void ReadVortexDeploymentData()
        {
            // Check for vortex (nexus mods) manager vortex.deployment.json
            foreach (string curModPath in this.ModsPaths)
            {
                if (Utils.StringNullEmptyOrWhiteSpace(curModPath))
                    continue;

                string vortexDeploymentFile = Path.Combine(curModPath, @"vortex.deployment.json");

                if (File.Exists(vortexDeploymentFile))
                {
                    string vortexModDataJson = File.ReadAllText(vortexDeploymentFile);
                    JObject vortexModData = JObject.Parse(vortexModDataJson);

                    JToken vortexFileData = vortexModData["files"];

                    if (vortexFileData == null)
                        continue;

                    foreach (JToken modFileData in vortexFileData)
                    {
                        JToken relPathToken = modFileData["relPath"];
                        if (relPathToken == null)
                            continue;

                        string vortexRelPath = modFileData["relPath"].ToString();

                        int index = vortexRelPath.IndexOf('\\');
                        if (index < 0)
                            continue;

                        string modFolderName = vortexRelPath.Substring(0, index);

                        if (Utils.StringNullEmptyOrWhiteSpace(modFolderName))
                            continue;

                        JToken vortexSource = modFileData["source"];
                        if (vortexSource == null)
                            continue;

                        // Looking for part of a path like Advanced Zoom-412-1-2-6-1679946838
                        string nexusModsId = null;
                        try
                        {
                            Regex regexObj = new Regex(@".*?-([\d]+)-[\d-]+-[\d]{10}",
                                RegexOptions.Multiline);
                            Match regexMatch = regexObj.Match(vortexSource.ToString());
                            if (regexMatch.Success)
                            {
                                nexusModsId = regexMatch.Groups[1].Value;
                            }

                        }
                        catch (ArgumentException ex)
                        {
                            // Syntax error in the regular expression
                        }

                        if (nexusModsId == null)
                            continue;

                        VortexDeploymentModData newVortexData = new();
                        newVortexData.nexusModsId = nexusModsId;
                        newVortexData.fullpath = Path.Combine(curModPath, modFolderName);

                        VortexDeploymentData[modFolderName] = newVortexData;
                    }
                }
            }
        }

        public void DetermineBestAvailableGameVersion()
        {
            string bestAvailableVersion = "0";

            // We will trust the game version from modlist.json if it exists.
            if ((this.parent != null && this.parent.ContainsKey("gameVersion")))
            {
                bestAvailableVersion = this.parent["gameVersion"].ToString();
            }
            else
            {
                // Otherwise we have to fall back to the highest available version in the loaded mods
                foreach (ModObject mod in ModDetails.Values)
                {
                    int versionCompare = Utils.CompareVersionStrings(bestAvailableVersion, mod.gameVersion);
                    if (versionCompare < 0)
                    {
                        bestAvailableVersion = mod.gameVersion;
                    }
                }
            }

            this.GameVersion = bestAvailableVersion;
            MainWindow.MainForm.toolStripStatusLabelMwVersion.Text = @"Game Version: " + bestAvailableVersion;
        }

        /// <summary>
        /// Used to load mods when using a preset or importing a load order string.
        /// Starts sequence to load all mods from folders, loads modlist, checks mod folder names against their possible paths
        /// and adds those paths, combines modlist with found folders structure and loads details of each found mod.
        /// </summary>
        public void LoadFromImportString()
        {
            //find all mod directories and parse them into just folder names:
            ParseDirectories();
            //We need to check if the mod we want to load from a preset is actually present on the system.
            CheckModDirPresent();
            //We are coming from an string of just modfolder names and no directory paths in the modlist object
            //so we need to convert using the DirectoryToPathDict
            AddPathsToModList();
            //Load each mods mod.json and store in Dict.
            LoadAllModDetails();
            //Combine so we have all mods in the ModList Dict for easy later use and writing to JObject
            CombineDirModList();
        }

        /// <summary>
        /// Checks for all items in the modlist if they have a possible folder on system they can point to.
        /// If not removes them from the modlist and informs user.
        /// </summary>
        private void CheckModDirPresent()
        {
            Dictionary<string, bool> MissingModDirs = new();
            foreach (var item in this.ModList)
            {
                if (Utils.StringNullEmptyOrWhiteSpace(item.Key))
                {
                    ModList.Remove(item.Key);
                    continue;
                }

                // Collect listed mods that are unavailable locally
                if (!DirectoryToPathDict.ContainsKey(item.Key))
                { 
                    MissingModDirs.Add(item.Key, item.Value);
                }
            }
            foreach (var missingModDir in MissingModDirs)
            {
                this.ModList.Remove(missingModDir.Key);

                // We will silently ignore missing mods that are not enabled
                if (!missingModDir.Value)
                {
                    MissingModDirs.Remove(missingModDir.Key);
                }
            }
            if (MissingModDirs.Count > 0)
            {
                string message = "The mod list includes the following enabled mods which are unavailable locally:\r\n\r\n"
                    + string.Join("\r\n", MissingModDirs.Keys)
                    + "\r\n\r\nThese mods will be ignored.";
                string caption = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Matches each folder name in the modlist to a folder on system.
        /// Then replaces the old modlist with a new one keyed by full folder path.
        /// </summary>
        private void AddPathsToModList()
        {
            Dictionary<string, bool> newModList = new Dictionary<string, bool>();
            foreach (string key in this.ModList.Keys)
            {
                string fullPath = DirectoryToPathDict[key];
                newModList[fullPath] = ModList[key];
            }
            this.ModList = newModList;
        }

        private void CheckMainModDirectoryExists()
        {
            if (Utils.StringNullEmptyOrWhiteSpace(this.ModsPaths[eModPathType.Main]))
            {
                return;
            }
            if (Directory.Exists(this.ModsPaths[eModPathType.Main]))
            {
                return;
            }
            string message = @"Mods folder does not exist in : " + this.ModsPaths[eModPathType.Main] + System.Environment.NewLine 
                             + @"Do you want to create it?";
            string caption = @"Mods folder does not exist";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult Result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                Directory.CreateDirectory(this.ModsPaths[eModPathType.Main]);
            }
        }

        public string GetSettingsDirectory()
        {
            string appDataDir = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataDir, @"MW5LoadOrderConfigurator");
        }

        //Try and load data from previous sessions
        public bool TryLoadProgramSettings()
        {
            //Load settings from previous session:
            string settingsDir = GetSettingsDirectory();
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            try
            {
                string json = File.ReadAllText(Path.Combine(settingsDir, SettingsFileName));
                this.ProgramSettings = JsonConvert.DeserializeObject<ProgramData>(json);

                if (!Utils.StringNullEmptyOrWhiteSpace(ProgramSettings.platform))
                {
                    if (!Enum.TryParse(ProgramSettings.platform, out eGamePlatform platform))
                    {
                        platform = eGamePlatform.None;
                    }

                    GamePlatform = platform;
                }

                if (!Utils.StringNullEmptyOrWhiteSpace(ProgramSettings.InstallPath) &&
                    Directory.Exists(ProgramSettings.InstallPath))
                {
                    SetGameInstallPath(ProgramSettings.InstallPath);
                }
                if (ProgramSettings.version > 0)
                {
                    Version = ProgramSettings.version;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"ERROR: Something went wrong while loading " + SettingsFileName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            if (!Utils.StringNullEmptyOrWhiteSpace(this.ModsPaths[eModPathType.Main]))
                return true;
            return false;
        }

        private static string GetLocalAppDataModPath()
        {
            string AppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //Put string back together
            return Path.Combine(AppDataRoaming, "MW5Mercs", "Saved", "Mods");
        }

        // Sets the game install path, deduces mod directory locations
        public void SetGameInstallPath(string path)
        {
            this.InstallPath = path;

            this.ModsPaths[MainLogic.eModPathType.Main] = Path.Combine(path, "MW5Mercs", "Mods");

            switch (this.GamePlatform)
            {
                case MainLogic.eGamePlatform.Steam:
                    SetSteamWorkshopPath();
                    break;
                case MainLogic.eGamePlatform.WindowsStore:
                    this.ModsPaths[MainLogic.eModPathType.AppData] = GetLocalAppDataModPath();
                    break;
            }
        }

        public static string FindSteamAppsParentDirectory(string path)
        {
            string currentDirectory = Path.GetDirectoryName(path);

            while (currentDirectory != null)
            {
                if (Path.GetFileName(currentDirectory).Equals("steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    return currentDirectory;
                }
                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            return null;
        }

        public void SetSteamWorkshopPath()
        {
            string steamAppsParentDirectory = FindSteamAppsParentDirectory(this.InstallPath);
            string workshopPath = Path.Combine(steamAppsParentDirectory, "workshop", "content", "784080");
            this.ModsPaths[MainLogic.eModPathType.Steam] = workshopPath;
        }

        //Delete a mod dir from system.
        internal void DeleteMod(string modDir)
        {
            string directory = modDir;
            Directory.Delete(directory, true);
        }

        //parse all directories in the basepath mods folder or steam workshop mods folder.
        private void ParseDirectories()
        {
            this.FoundDirectories.Clear();

            if (!Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Main]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Main]));
            }

            if (!Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Steam]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Steam]));
            }

            if (!Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.AppData]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.AppData]));
            }
            AddDirectoryPathsToDict();
        }

        private void AddDirectoryPathsToDict()
        {
            foreach (string curDirectory in FoundDirectories)
            {
                string directoryName = Path.GetFileName(curDirectory);
                this.DirectoryToPathDict[directoryName] = curDirectory;
                this.PathToDirectoryDict[curDirectory] = directoryName;
            }
        }

        public void SaveSettings()
        {
            string settingsDir = GetSettingsDirectory();

            ProgramSettings.InstallPath = this.InstallPath;
            this.ProgramSettings.platform = this.GamePlatform.ToString();
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            using (StreamWriter sw = new StreamWriter(Path.Combine(settingsDir, SettingsFileName)))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this.ProgramSettings);
            }
        }

        public void ModListParser()
        {
            string modlistPath = GetModListJsonFilePath();
            try
            {
                this.rawJson = File.ReadAllText(modlistPath);
                this.parent = JObject.Parse(rawJson);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    @"The modlist.json file could not be found in"+ System.Environment.NewLine 
                    + modlistPath +@"."+System.Environment.NewLine+System.Environment.NewLine
                    +@"It is necessary to read this file in order to validate it with the correct version number the game expects." + System.Environment.NewLine + System.Environment.NewLine
                    +@"LOC will try to create the file with the correct version number when applying your profile, but there is high chance that this will fail."+System.Environment.NewLine
                    +@"It is recommended to start the game once in order to create this file before applying your mod profile.",
                    @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            JObject modStatus = this.parent.Value<JObject>("modStatus");
            if (modStatus != null)
            {
                foreach (JProperty mod in modStatus.Properties())
                {
                    bool enabled = (bool)this.parent["modStatus"][mod.Name]["bEnabled"];
                    if (this.DirectoryToPathDict.TryGetValue(mod.Name, out string modDir))
                    {
                        this.ModList.Add(modDir, enabled);
                    }
                }
            }
        }

        public void SaveToFiles()
        {
            UpdateModlistJObject();
            SaveModDetails();
            SaveModListJson();
        }

        //TODO Fix
        public void ClearAll()
        {
            this.ModDirectories.Clear();
            this.Mods.Clear();
            this.ModDetails.Clear();
            this.ModList.Clear();
            this.DirectoryToPathDict.Clear();
            this.OverrridingData.Clear();
            this.ModsPaths[eModPathType.Main] = null;
            this.ModsPaths[eModPathType.Steam] = null;
            this.ModsPaths[eModPathType.AppData] = null;
            this.VortexDeploymentData.Clear();
            
        }

        //Check if the mod dir is already present in data loaded from modlist.json, if not add it.
        private void CombineDirModList()
        {
            // First sort the directory by the default MW5 load orders
            ModDirectories.Sort((x, y) =>
            {
                if (!Mods.ContainsKey(x) || !Mods.ContainsKey(y))
                    return 0;

                // Compare Original load order
                int priorityComparison = Mods[y].OriginalLoadOrder.CompareTo(Mods[x].OriginalLoadOrder);

                // If Priority is equal, compare Folder name
                if (priorityComparison == 0)
                {
                    return PathToDirectoryDict[y].CompareTo(PathToDirectoryDict[x]);
                }
                else
                {
                    return priorityComparison;
                }
            });

            foreach (string modDir in this.ModDirectories)
            {
                if (this.ModList.ContainsKey(modDir))
                    continue;

                ModList[modDir] = false;
            }
            //Turns out there are sometimes "ghost" entries in the modlist.json for which there are no directories left, lets remove those.
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, bool> entry in this.ModList)
            {
                if (this.ModDirectories.Contains<string>(entry.Key))
                    continue;
                toRemove.Add(entry.Key);
            }
            foreach (string key in toRemove)
            {
                this.ModList.Remove(key);
            }
        }

        public static bool IsSteamWorkshopID(string input)
        {
            if (String.IsNullOrEmpty(input))
                return false;

            // Check if all characters in the string are digits
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            // Do a sanity check on workshop id
            // The oldest (known) mechwarrior workshop mod has id 2494637209
             if (input.Length < 10)
                 return false;

            return true;
        }

        private void LoadModDetails(string modPath)
        {
            bool loadModSuccess = false;
            try
            {
                ModData modData = new ModData();
                ModObject modDetails = null;
                try
                {
                    string modJsonFilePath = Path.Combine(modPath, @"mod.json");
                    string modJson = File.ReadAllText(modJsonFilePath);
                    JObject modDetailsJ = JObject.Parse(modJson);

                    modDetails = modDetailsJ.ToObject<ModObject>();

                    if (modDetailsJ.ContainsKey("locOriginalLoadOrder"))
                    {
                        modData.OriginalLoadOrder = modDetails.locOriginalLoadOrder;
                    }
                    else if (modDetailsJ.ContainsKey("lotsOriginalLoadOrder"))
                    {
                        // Might have been set by the MW5-LOTS mod order manager
                        modData.OriginalLoadOrder = modDetailsJ["lotsOriginalLoadOrder"].Value<float>();
                    }
                    else
                    {
                        modData.OriginalLoadOrder = modDetails.defaultLoadOrder;
                    }

                    // Determine mod origin
                    string modDir = this.PathToDirectoryDict[modPath];

                    // Check if this might be a mod from the steam workshop
                    if (IsSteamWorkshopID(modDir))
                    {
                        // If the mod directory name matches the store id, we can be pretty certain
                        // there are mods however, that don't have this info correctly filled
                        if (modDir == modDetails.steamPublishedFileId.ToString())
                        {
                            modData.Origin = ModData.ModOrigin.Steam;
                        }

                        // if this looks like a steam id and the mod is stored in the steam mods directory
                        // it's certain that this is a steam mod
                        if (modData.Origin == ModData.ModOrigin.Unknown)
                        {
                            if (modPath.StartsWith(MainWindow.MainForm.logic.ModsPaths[eModPathType.Steam]))
                            {
                                modData.Origin = ModData.ModOrigin.Steam;
                            }
                        }
                    }

                    if (modData.Origin == ModData.ModOrigin.Unknown)
                    {
                        string modDirectoryName = PathToDirectoryDict[modPath];
                        if (VortexDeploymentData.ContainsKey(modDirectoryName))
                        {
                            VortexDeploymentModData vortexModData = VortexDeploymentData[modDirectoryName];

                            modData.Origin = ModData.ModOrigin.Nexusmods;
                            modData.NexusModsId = vortexModData.nexusModsId;
                        }

                    }

                    // Fallback
                    if (modData.Origin == ModData.ModOrigin.Unknown)
                    {
                        if (File.Exists(modPath + @"\__folder_managed_by_vortex"))
                        {
                            modData.Origin = ModData.ModOrigin.Nexusmods;
                        }
                    }
                }
                catch (Exception e)
                {
                    string message = @"Error loading mod.json in : " + modPath + System.Environment.NewLine +
                                     System.Environment.NewLine +
                                     "The folder will be skipped. If this is not a mod folder you can ignore ths message.";
                    string caption = "Error Loading mod.json";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);

                    return;
                }

                // Sanity check for mod files
                string pakDir = modPath + @"\Paks";
                if (!Directory.Exists(pakDir) || Directory.GetFiles(pakDir, "*.pak").Length == 0)
                {
                    string message = @"The mod in the path" + System.Environment.NewLine +
                                     modPath + System.Environment.NewLine + 
                                     @"might be corrupted." + System.Environment.NewLine +
                                     "The mod has a valid mod.json, but has no Pak game data files associated with it.";
                    string caption = "Loading mod";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons, MessageBoxIcon.Warning);
                }

                long totalPakSize = 0;
                bool hasZeroBytePak = false;
                if (Directory.Exists(pakDir))
                {
                    foreach (string filePath in Directory.GetFiles(pakDir, "*.pak"))
                    {
                        long fileSize = new FileInfo(filePath).Length;

                        if (fileSize == 0)
                        {
                            hasZeroBytePak = true;
                            break;
                        }

                        totalPakSize += fileSize;
                    }
                }

                if (hasZeroBytePak)
                {
                    string message = @"The mod in the path" + System.Environment.NewLine +
                                     modPath + System.Environment.NewLine + 
                                     @"might be corrupted." + System.Environment.NewLine +
                                     "The mod has one or more Pak game data files that are zero bytes in size.";
                    string caption = "Error Loading mod";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                }

                modData.ModFileSize = totalPakSize;
                this.Mods.Add(modPath, modData);
                this.ModDetails.Add(modPath, modDetails);
                this.ModDirectories.Add(modPath);
                loadModSuccess = true;
            }
            finally
            {
                if (!loadModSuccess)
                {
                    if (ModList.ContainsKey(modPath))
                        ModList.Remove(modPath);
                }
            }
 
        }

        private void LoadAllModDetails()
        {
            foreach (string modDir in this.FoundDirectories)
            {
                LoadModDetails(modDir);
            }
        }

        public void SaveModDetails()
        {
            foreach (KeyValuePair<string, ModObject> entry in this.ModDetails)
            {
                string modJsonPath = entry.Key + @"\mod.json";

                // Make sure the file still exists, in case the mod was deleted
                if (!File.Exists(modJsonPath))
                    continue;

                string modJsonExisting = File.ReadAllText(modJsonPath);
                JObject modDetailsNew = JObject.Parse(modJsonExisting);

                //JObject modDetailsUpdate = JObject.FromObject(entry.Value);
                modDetailsNew["locOriginalLoadOrder"] = Mods[entry.Key].OriginalLoadOrder;
                modDetailsNew["defaultLoadOrder"] = entry.Value.defaultLoadOrder;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                using (StreamWriter sw = new StreamWriter(modJsonPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //modDetailsNew.WriteTo(writer);

                    serializer.Serialize(writer, modDetailsNew);
                }
            }
        }

        public void UpdateModlistJObject()
        {
            if (this.parent == null)
            {
                this.parent = new JObject();
                this.parent["gameVersion"] = GameVersion;
            }

            JObject modStatusObject = this.parent.Value<JObject>("modStatus");
            if (modStatusObject != null)
            {
                modStatusObject.RemoveAll();
            }
            else
            {
                this.parent.Add("modStatus", new JObject());
            }
            
            foreach (KeyValuePair<string, bool> entry in this.ModList)
            {
                string[] temp = entry.Key.Split('\\');
                string modFolderName = temp[temp.Length - 1];
                AddModToModlistJObject(modFolderName, entry.Value);
            }
        }

        public void SaveModListJson()
        {
            string modlistJsonFilePath = GetModListJsonFilePath();

            if (File.Exists(modlistJsonFilePath))
            {
                string modListJsonExisting = File.ReadAllText(modlistJsonFilePath);
                JObject modListNew = JObject.Parse(modListJsonExisting);

                modListNew["modStatus"] = parent["modStatus"];

                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                using (StreamWriter sw = new StreamWriter(modlistJsonFilePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, modListNew);
                }
            }
            else
            {
                string jsonString = this.parent.ToString();
                StreamWriter sw = File.CreateText(modlistJsonFilePath);
                sw.WriteLine(jsonString);
                sw.Flush();
                sw.Close();
            }

        }

        public void AddModToModlistJObject(string ModName, bool status)
        {
            JObject modStatus = this.parent["modStatus"] as JObject;
            JObject newStatus = new JObject(
                new JProperty("bEnabled", status)
            );
            modStatus.Add(ModName, newStatus);
        }

        public void SetModInJObject(string ModName, bool status)
        {
            this.parent["modStatus"][ModName]["bEnabled"] = status;
        }

        #region pack mods to zip

        public void ThreadProc()
        {
            //Get parent dir
            string parent = Directory.GetParent(this.ModsPaths[eModPathType.Main]).ToString();
            //Check if Mods.zip allready exists delete it if so, we need to do this else the ZipFile lib will error.
            if (File.Exists(parent + "\\Mods.zip"))
            {
                File.Delete(parent + "\\Mods.zip");
            }
            ZipFile.CreateFromDirectory(this.ModsPaths[eModPathType.Main], parent + "\\Mods.zip", CompressionLevel.Fastest, false);
        }

        public void PackModsToZip(BackgroundWorker worker, DoWorkEventArgs e)
        {
            //Console.WriteLine("Starting zip compression");
            string parent = Directory.GetParent(this.ModsPaths[eModPathType.Main]).ToString();

            Thread t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
            while (t.IsAlive)
            {
                System.Threading.Thread.Sleep(500);
                if (worker.CancellationPending || e.Cancel)
                {
                    t.Interrupt();
                    t.Join();
                    e.Result = "ABORTED";
                    if (File.Exists(parent + "\\Mods.zip"))
                    {
                        File.Delete(parent + "\\Mods.zip");
                    }
                    return;
                }
                Thread.Yield();
            }
            //Open folder where we stored the zip file
            e.Result = "DONE";
        }

        #endregion pack mods to zip

        //Reset the overriding data between two mods and check if after mods are still overriding/being overriden
        public void ResetOverrdingBetweenMods(ModListViewItem listItemA, ModListViewItem listItemB)
        {
            string modA = listItemA.SubItems[MainWindow.MainForm.folderHeader.Index].Text;
            string modB = listItemB.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

            if (this.OverrridingData.ContainsKey(modA))
            {
                if (this.OverrridingData[modA].overriddenBy.ContainsKey(modB))
                    this.OverrridingData[modA].overriddenBy.Remove(modB);
                if (this.OverrridingData[modA].overrides.ContainsKey(modB))
                    this.OverrridingData[modA].overrides.Remove(modB);
                if (this.OverrridingData[modA].overrides.Count == 0)
                    this.OverrridingData[modA].isOverriding = false;
                if (this.OverrridingData[modA].overriddenBy.Count == 0)
                    this.OverrridingData[modA].isOverridden = false;
            }
            if (this.OverrridingData.ContainsKey(modA))
            {
                if (this.OverrridingData[modB].overriddenBy.ContainsKey(modA))
                    this.OverrridingData[modB].overriddenBy.Remove(modA);
                if (this.OverrridingData[modB].overrides.ContainsKey(modA))
                    this.OverrridingData[modB].overrides.Remove(modA);
                if (this.OverrridingData[modB].overrides.Count == 0)
                    this.OverrridingData[modB].isOverriding = false;
                if (this.OverrridingData[modB].overriddenBy.Count == 0)
                    this.OverrridingData[modB].isOverridden = false;
            }
            //Console.WriteLine("ResetOverrdingBetweenMods modA: " + modA + " " + this.OverrridingData[modA].isOverriding + " " + this.OverrridingData[modA].isOverriden);
            //Console.WriteLine("ResetOverrdingBetweenMods modB: " + modB + " " + this.OverrridingData[modB].isOverriding + " " + this.OverrridingData[modB].isOverriden);
        }

        //Save presets from memory to file for use in next session.
        internal void SavePresets()
        {
            string JsonFile = GetSettingsDirectory() + Path.DirectorySeparatorChar + PresetsFileName;
            string JsonString = JsonConvert.SerializeObject(this.Presets, Formatting.Indented);

            if (File.Exists(JsonFile))
                File.Delete(JsonFile);

            //Console.WriteLine(JsonString);
            StreamWriter sw = File.CreateText(JsonFile);
            sw.WriteLine(JsonString);
            sw.Flush();
            sw.Close();
        }

        //Load prests from file
        public void LoadPresets()
        {
            string JsonFile = GetSettingsDirectory() + Path.DirectorySeparatorChar + PresetsFileName;
            //parse to dict of strings.

            if (!File.Exists(JsonFile))
                return;

            Dictionary<string, string> temp;
            try
            {
                string json = File.ReadAllText(JsonFile);
                temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                //Console.WriteLine("OUTPUT HERE!");
                //Console.WriteLine(JsonConvert.SerializeObject(temp, Formatting.Indented));
            }
            catch (Exception Ex)
            {
                string message = "There was an error in decoding the presets file!";
                string caption = "Presets File Decoding Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                return;
            }
            this.Presets = temp;
        }

        //Used to update the override data when a new item is added or removed to/from the mod list instead of checking all items agains each other again.
        public void UpdateNewModOverrideData(List<ModListViewItem> items, ModListViewItem newListItem)
        {
            string modA = newListItem.SubItems[MainWindow.MainForm.folderHeader.Index].Text;
            ////Console.WriteLine("UpdateNewModOverrideData");
            ////Console.WriteLine("Mod checked or unchecked: " + modA);

            if (!newListItem.Checked)
            {
                ////Console.WriteLine("--Unchecked");
                if (this.OverrridingData.ContainsKey(modA))
                    this.OverrridingData.Remove(modA);

                foreach (string key in this.OverrridingData.Keys)
                {
                    if (OverrridingData[key].overriddenBy.ContainsKey(modA))
                        OverrridingData[key].overriddenBy.Remove(modA);

                    if (OverrridingData[key].overrides.ContainsKey(modA))
                        OverrridingData[key].overrides.Remove(modA);

                    if (OverrridingData[key].overrides.Count == 0)
                        OverrridingData[key].isOverriding = false;

                    if (OverrridingData[key].overriddenBy.Count == 0)
                        OverrridingData[key].isOverridden = false;
                }
            }
            else
            {
                ////Console.WriteLine("--Unchecked");
                if (!this.OverrridingData.ContainsKey(modA))
                {
                    this.OverrridingData[modA] = new OverridingData
                    {
                        mod = modA,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                }

                //check each mod for changes
                foreach (ModListViewItem item in items)
                {
                    string modB = item.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

                    //Again dont compare mods to themselves.
                    if (modA == modB)
                        continue;

                    if (!this.OverrridingData.ContainsKey(modB))
                    {
                        this.OverrridingData[modB] = new OverridingData
                        {
                            mod = modB,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                    }
                    GetModOverridingData(newListItem, item, items.Count, this.OverrridingData[modA], this.OverrridingData[modB]);
                }
            }

            ColorizeListViewItems(items);
        }

        //used to update the overriding data when a mod is moved ONE up or ONE down.
        public void UpdateModOverridingdata(List<ModListViewItem> items, ModListViewItem movedModItem, bool movedUp)
        {
            string modA = movedModItem.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

            //Console.WriteLine("UpdateModOverridingdata");
            //Console.WriteLine("--" + modA);

            int indexToCheck = 0;
            if (movedUp)
                indexToCheck = movedModItem.Index + 1;
            else
                indexToCheck = movedModItem.Index - 1;

            ModListViewItem listItemB = items[indexToCheck];
            string modB = listItemB.SubItems[MainWindow.MainForm.folderHeader.Index].Text;
            //Console.WriteLine("++" + modB);

            if (!this.OverrridingData.ContainsKey(modA))
            {
                this.OverrridingData[modA] = new OverridingData
                {
                    mod = modA,
                    overrides = new Dictionary<string, List<string>>(),
                    overriddenBy = new Dictionary<string, List<string>>()
                };
            }
            if (!this.OverrridingData.ContainsKey(modB))
            {
                this.OverrridingData[modB] = new OverridingData
                {
                    mod = modB,
                    overrides = new Dictionary<string, List<string>>(),
                    overriddenBy = new Dictionary<string, List<string>>()
                };
            }

            ResetOverrdingBetweenMods(movedModItem, listItemB);

            GetModOverridingData(movedModItem, items[indexToCheck], items.Count, OverrridingData[modA], OverrridingData[modA]);

            OverridingData A = OverrridingData[modA];
            OverridingData B = OverrridingData[modB];

            ColorizeListViewItems(items);
        }

        //See if items A and B are interacting in terms of manifest and return the intersect
        public void GetModOverridingData(ModListViewItem listItemA, ModListViewItem listItemB, int itemCount, OverridingData A, OverridingData B)
        {
            string modA = listItemA.SubItems[MainWindow.MainForm.folderHeader.Index].Text;
            string modB = listItemB.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

            if (modA == modB)
                return;

            int priorityA = itemCount - listItemA.Index;
            int priorityB = itemCount - listItemB.Index;

            //Now we have a mod that is not the mod we are looking at is enbabled.
            //Lets compare the manifest!
            List<string> manifestA = this.ModDetails[this.DirectoryToPathDict[modA]].manifest;
            List<string> manifestB = this.ModDetails[this.DirectoryToPathDict[modB]].manifest;
            List<string> intersect = manifestA.Intersect(manifestB).ToList();

            //If the intersects elements are greater then zero we have shared parts of the manifest
            if (intersect.Count() == 0)
                return;

            ////Console.WriteLine("---Intersection: " + modB + " : " + priorityB.ToString());

            //If we are loaded after the mod we are looking at we are overriding it.
            if (priorityA > priorityB)
            {
                if (!(A.mod == modB))
                {
                    A.isOverriding = true;
                    A.overrides[modB] = intersect;
                }
                if (!(B.mod == modA))
                {
                    B.isOverridden = true;
                    B.overriddenBy[modA] = intersect;
                }
            }
            else
            {
                if (!(A.mod == modB))
                {
                    A.isOverridden = true;
                    A.overriddenBy[modB] = intersect;
                }
                if (!(B.mod == modA))
                {
                    B.isOverriding = true;
                    B.overrides[modA] = intersect;
                }
            }
            this.OverrridingData[modA] = A;
            this.OverrridingData[modB] = B;
        }

        //Return a dict of all overriden mods with a list of overriden files as values.
        //else returns an empty string.
        public void GetOverridingData(List<ModListViewItem> items)
        {
            ////Console.WriteLine(Environment.StackTrace);
            ////Console.WriteLine("Starting Overriding data check");
            this.OverrridingData.Clear();

            foreach (ModListViewItem itemA in items)
            {
                //We only wanna check this for items actually enabled.
                if (!itemA.Checked)
                    continue;

                string modA = itemA.SubItems[MainWindow.MainForm.folderHeader.Index].Text;
                int priorityA = items.Count - items.IndexOf(itemA);

                //Check if we allready have this mod in the dict if not create an entry for it.
                if (!this.OverrridingData.ContainsKey(modA))
                {
                    this.OverrridingData[modA] = new OverridingData
                    {
                        mod = modA,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                }
                OverridingData A = this.OverrridingData[modA];

                //Console.WriteLine("Checking: " + modA + " : " + priorityA.ToString());
                foreach (ModListViewItem itemB in items)
                {
                    string modB = itemB.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

                    if (modA == modB)
                        continue;

                    if (!itemB.Checked)
                        continue;

                    //If we have allready seen modb in comparison to modA we don't need to compare because the comparison is bi-directionary.
                    if (
                        A.overriddenBy.ContainsKey(modB) ||
                        A.overrides.ContainsKey(modB)
                        )
                    {
                        ////Console.WriteLine("--" + modA + "has allready been compared to: " + modB);
                        continue;
                    }

                    //Check if we have allready seen modB before.
                    if (this.OverrridingData.ContainsKey(modB))
                    {
                        //If we have allready seen modB and we have allready compared modB and modA we don't need to compare because the comparison is bi-directionary.
                        if (
                            this.OverrridingData[modB].overriddenBy.ContainsKey(modA) ||
                            this.OverrridingData[modB].overrides.ContainsKey(modA)
                            )
                        {
                            ////Console.WriteLine("--" + modB + "has allready been compared to: " + modA);
                            continue;
                        }
                    }
                    else
                    {
                        //If we have not make a new modB overridingDatas
                        this.OverrridingData[modB] = new OverridingData
                        {
                            mod = modB,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                    }
                    GetModOverridingData(itemA, itemB, items.Count, this.OverrridingData[modA], this.OverrridingData[modB]);
                }
            }

            #region debug output

            //Debug output
            //foreach(string key in this.OverrridingData.Keys)
            //{
            //    //Console.WriteLine("MOD: " + key);
            //    //Console.WriteLine("--Overriden:");
            //    foreach (string mod in OverrridingData[key].overriddenBy.Keys)
            //    {
            //        //Console.WriteLine("----" + OverrridingData[key].isOverriden);
            //    }
            //    //Console.WriteLine("--Overrides:");
            //    foreach (string mod in OverrridingData[key].overrides.Keys)
            //    {
            //        //Console.WriteLine("----" + OverrridingData[key].isOverriding);
            //    }
            //}

            #endregion debug output

            ColorizeListViewItems(items);
        }

        //Check color of a single mod.
        public void ColorItemOnOverrdingData(ModListViewItem listItem)
        {
            ColorizeListViewItems(new List<ModListViewItem>() { listItem });
        }

        //Color the list view items based on data
        public void ColorizeListViewItems(List<ModListViewItem> items)
        {
            MainWindow.MainForm.modsListView.BeginUpdate();
            foreach (ModListViewItem item in items)
            {
                string modName = item.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

                //marked for removal so don't color.
                if (item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor == Color.Red)
                {
                    continue;
                }

                bool modEnabled = MainWindow.MainForm.logic.ModList[item.Tag.ToString()];

                if (modEnabled)
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].Font = new Font(MainWindow.MainForm.modsListView.Font, MainWindow.MainForm.modsListView.Font.Style | FontStyle.Bold);  
                }
                else
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].Font = new Font(MainWindow.MainForm.modsListView.Font, MainWindow.MainForm.modsListView.Font.Style);  
                }

                foreach (ListViewItem.ListViewSubItem curItem in item.SubItems)
                {
                    curItem.ForeColor = modEnabled ? SystemColors.WindowText : Color.Gray;
                }

                if (!modEnabled)
                {
                    continue;
                }

                if (!this.OverrridingData.ContainsKey(modName))
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = SystemColors.WindowText;

                    continue;
                }
                OverridingData A = OverrridingData[modName];
                Color newItemColor = SystemColors.WindowText;
                if (A.isOverridden)
                {
                    newItemColor = OverriddenColor;
                }
                if (A.isOverriding)
                {
                    newItemColor = OverridingColor;
                }
                if (A.isOverriding && A.isOverridden)
                {
                    newItemColor = OverriddenOveridingColor;
                }

                item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = newItemColor;
            }

            MainWindow.MainForm.ColorListViewNumbers(MainWindow.MainForm.ModListData, MainWindow.MainForm.currentLoadOrderHeader.Index, LowPriorityColor, HighPriorityColor);
            MainWindow.MainForm.ColorListViewNumbers(MainWindow.MainForm.ModListData, MainWindow.MainForm.originalLoadOrderHeader.Index, LowPriorityColor, HighPriorityColor);
            MainWindow.MainForm.modsListView.EndUpdate();
        }

        //Monitor the size of a given zip file
        public void MonitorZipSize(BackgroundWorker worker, DoWorkEventArgs e)
        {
            string zipFile = Directory.GetParent(this.ModsPaths[eModPathType.Main]).ToString() + "\\Mods.zip";
            long folderSize = Utils.DirSize(new DirectoryInfo(ModsPaths[eModPathType.Main]));
            //zip usually does about 60 percent but we dont wanna complete at like 85 or 90 lets overestimate
            long compressedFolderSize = (long)Math.Round(folderSize * 0.35);
            //Console.WriteLine("Starting file size monitor, FolderSize: " + compressedFolderSize.ToString());
            while (!e.Cancel && !worker.CancellationPending)
            {
                while (!File.Exists(zipFile))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                long zipFileSize = new FileInfo(zipFile).Length;
                int progress = Math.Min((int)((zipFileSize * (long)100) / compressedFolderSize), 100);
                //Console.WriteLine("--" + zipFileSize.ToString());
                //Console.WriteLine("--" + progress.ToString());
                worker.ReportProgress(progress);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}