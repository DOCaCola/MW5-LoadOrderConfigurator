using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{
    /// <summary>
    /// Contains most of the background logic and operations
    /// Also has some dataobjects to keep track of various internal statuses.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ModsManager
    {
        public static ModsManager Instance { get; private set; }

        public string GameVersion = "";
        public string KnownModListGameVersion = null;

        public enum eModPathType
        {
            Program,
            Steam,
            // The Microsoft Store version stores their mods in AppData
            AppData
        }
        public ArrayByEnum<string,eModPathType> ModsPaths = new();

        public LocSettings ProgramSettings;

        // User made changes not written to files
        public bool ModSettingsTainted = false;

        // Directories found in all mod paths
        public List<string> FoundDirectories = new();
        public Dictionary<string, string> DirNameToPathDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> PathToDirNameDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Mod data as loaded from the mods' mod.json file
        public Dictionary<string, ModObject> ModDetails = new Dictionary<string, ModObject>(StringComparer.OrdinalIgnoreCase);
        // Valid mod directories
        public List<string> ModDirectories = new();
        public Dictionary<string, bool> ModEnabledList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        // As it was last loaded from file
        public Dictionary<string, bool> ModEnabledListLastState;
        public Dictionary<string, OverridingData> OverridingData = new Dictionary<string, OverridingData>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Presets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public class LastAppliedPresetModData
        {
            public bool state = false;
            public float lastLoadOrder = -1;
            public string version;
            public int buildNumber = -1;
        }
        public class LastAppliedPresetData
        {
            public long timeStamp = 0;
            public string gameVersion = "";
            public Dictionary<string, LastAppliedPresetModData> mods = null;
        }

        public LastAppliedPresetData LastAppliedPreset = null;
        // Last applied preset in ready-to-load form
        public Dictionary<string, bool> LastAppliedPresetModList = null;

        public static Color OverriddenColor = Color.FromArgb(131, 101, 0);
        public static Color OverridingColor = Color.FromArgb(80, 37, 192);
        public static Color OverriddenOveridingColor = Color.FromArgb(170,73,97);

        public static Color HighPriorityColor = Color.FromArgb(252, 54, 63);
        public static Color LowPriorityColor = Color.FromArgb(17, 137, 21);

        public static string SettingsFileName = @"Settings.json";
        public static string PresetsFileName = @"Presets.json";
        public static string LastAppliedOrderFileName = @"LastApplied.json";

        public class VortexDeploymentModData
        {
            public string fullpath;
            public string nexusModsId;
        }

        public Dictionary<string, VortexDeploymentModData> VortexDeploymentData = new Dictionary<string, VortexDeploymentModData>(StringComparer.OrdinalIgnoreCase);

        public class ModData
        {
            public float NewLoadOrder = Single.NaN;
            public float OriginalLoadOrder = Single.NaN;
            // Was the file mod.json modified by LOC before?
            public bool IsNewMod = true;

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

        static ModsManager()
        {
            Instance = new ModsManager();
        }

        ModsManager()
        {
            ProgramSettings = new LocSettings(Path.Combine(GetSettingsDirectory(), SettingsFileName));
        }

        public bool GameIsConfigured()
        {
            if (LocSettings.Instance.Data.platform == eGamePlatform.None)
                return false;

            if (LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore 
                && Utils.StringNullEmptyOrWhiteSpace(LocSettings.Instance.Data.InstallPath))
                return false;

            return true;
        }

        public string GetModListJsonFilePath()
        {
            string path;
            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.WindowsStore:
                    path = ModsPaths[eModPathType.AppData];
                    break;
                default:
                    path = ModsPaths[eModPathType.Program];
                    break;
            }

            return Path.Combine(path, @"modlist.json");
        }

        public string GetMainModPath()
        {
            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.WindowsStore:
                    return ModsPaths[eModPathType.AppData];
                default:
                    return ModsPaths[eModPathType.Program];
            }
        }

        public void LoadLastAppliedPresetData()
        {
            string lastAppliedJsonFile = GetSettingsDirectory() + Path.DirectorySeparatorChar + LastAppliedOrderFileName;


            if (!File.Exists(lastAppliedJsonFile))
            {
                return;
            }

            string modJsonText = File.ReadAllText(lastAppliedJsonFile);
            try
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                LastAppliedPreset = JsonConvert.DeserializeObject<LastAppliedPresetData>(modJsonText, jsonSettings);
            }
            catch (JsonReaderException e)
            {
                return;
            }

            Dictionary<string, bool> lastAppliedValid = new Dictionary<string, bool>();
            foreach (var curMod in LastAppliedPreset.mods)
            {
                lastAppliedValid[curMod.Key] = curMod.Value.state;
            }
            ProcessModFolderEnabledList(ref lastAppliedValid, false);
            LastAppliedPresetModList = lastAppliedValid;
        }

        public bool ShouldLoadLastApplied()
        {
            if (LastAppliedPreset == null || LastAppliedPreset.mods == null)
                return false;

            // Remove invalid mods from last loaded list.
            Dictionary<string, bool> lastMods = new();
            foreach (var curModItem in LastAppliedPreset.mods)
            {
                lastMods.Add(curModItem.Key, curModItem.Value.state);
            }
            ProcessModFolderEnabledList(ref lastMods, false);

            // Filter to enabled only mods
            List<string> lastEnabledModList = lastMods
                .Where(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            List<string> curEnabledModList = new();
            if (ModEnabledListLastState != null)
            {
                curEnabledModList = ModEnabledList
                    .Where(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();
            }

            List<string> availableModList = this.Mods
                .Select(kv => kv.Key)
                .ToList();

            var modOrderMatches = ModUtils.IsModOrderMatching(curEnabledModList, lastEnabledModList);
            bool modsWereDisabled = curEnabledModList.Count == 0 && lastEnabledModList.Count > 0;

            if (modOrderMatches && !modsWereDisabled)
                return false;

            List<string> loadOrderChangedModNames = new List<string>();
            List<string> enabledStateChangedModNames = new List<string>();

            foreach (var curCandidate in lastEnabledModList)
            {
                // Compare current load order in mod.json with the one we last saved
                string curCandidateFolderName = Path.GetFileName(curCandidate);
                bool loadOrderChanged = !FloatUtils.IsEqual(
                    LastAppliedPreset.mods[curCandidateFolderName].lastLoadOrder,
                    ModDetails[curCandidate].defaultLoadOrder);

                if (loadOrderChanged)
                {
                    loadOrderChangedModNames.Add(ModDetails[curCandidate].displayName);
                }

                bool enabledStateChanged = ModEnabledListLastState == null || !ModEnabledListLastState.ContainsKey(curCandidate) || !ModEnabledListLastState[curCandidate];

                if (enabledStateChanged)
                {
                    enabledStateChangedModNames.Add(ModDetails[curCandidate].displayName);
                }
            }
            
            if (loadOrderChangedModNames.Count > 0)
            {
                // Force refresh of the modlist so the user can see the progress so far.
                // A bit hacky putting this here
                MainForm.Instance.modObjectListView.ForceRedraw();

                var page = new TaskDialogPage()
                {
                    Caption = "Mod load order changed",
                    Icon = TaskDialogIcon.Warning,
                    AllowCancel = true,
                };

                DateTime timestamp = DateTime.UnixEpoch.AddSeconds(LastAppliedPreset.timeStamp);

                page.Buttons.Add(new TaskDialogCommandLinkButton("&Restore last applied load order", "Use the load order you applied " + timestamp.ToTimeSinceString() + ".")
                    {
                        Tag = 1
                    });
                page.Buttons.Add(new TaskDialogCommandLinkButton("&Ignore", "Use current load order.")
                {
                    Tag = 2
                });

                page.Heading = "The mod load order has changed since you last applied it.";
                var changedMods = string.Join(loadOrderChangedModNames.Count > 5 ? ", " : "\r\n", loadOrderChangedModNames);
                if (changedMods.Length == 1)
                {
                    page.Text = "The following mod is affected:\r\n" + changedMods;
                }
                else
                {
                    page.Text = "The following mods are affected:\r\n" + changedMods;
                }
                page.Text += "\r\n\r\n How would you like to proceed?";
                
                page.Footnote = new TaskDialogFootnote()
                {
                    Text = "This could occur due to an update to an installed mod or through the use of other tools that modify mod data, potentially altering the load order."
                };

                TaskDialogButton dialogResult = TaskDialog.ShowDialog(MainForm.Instance.Visible ? MainForm.Instance.Handle : 0, page);

                if (dialogResult.Tag is int resultIndex)
                    return resultIndex == 1;
            }
            else if (modsWereDisabled && enabledStateChangedModNames.Count > 0)
            {
                // Force refresh of the modlist so the user can see the progress so far.
                // A bit hacky putting this here
                MainForm.Instance.modObjectListView.ForceRedraw();

                var page = new TaskDialogPage()
                {
                    Caption = "Mod list empty",
                    Icon = TaskDialogIcon.Warning,
                    AllowCancel = true,
                };

                DateTime timestamp = DateTime.UnixEpoch.AddSeconds(LastAppliedPreset.timeStamp);

                page.Buttons.Add(new TaskDialogCommandLinkButton("&Restore last applied mod list", "Use the mod list you applied " + timestamp.ToTimeSinceString() + ".")
                {
                    Tag = 1
                });
                page.Buttons.Add(new TaskDialogCommandLinkButton("&Ignore", "Continue with empty mod list.")
                {
                    Tag = 2
                });

                page.Heading = "Your mod list has been reset or was deleted.";
                var changedMods = string.Join(enabledStateChangedModNames.Count > 5 ? ", " : "\r\n", enabledStateChangedModNames);
                page.Text = "This might have been caused as a result of a game update or due to another programs altering the mod list.\r\n\r\nThe following mods are affected:\r\n"+changedMods+"\r\n\r\n How would you like to proceed?";

                TaskDialogButton dialogResult = TaskDialog.ShowDialog(MainForm.Instance.Visible ? MainForm.Instance.Handle : 0, page);

                if (dialogResult.Tag is int resultIndex)
                    return resultIndex == 1;
            }

            return false;
        }

        public void InitModEnabledList()
        {
            ModEnabledList.Clear();
            foreach (string modDir in this.ModDirectories)
            {
                ModEnabledList[modDir] = false;
            }
        }

        // (Re)load all mod data
        // desiredMods in order they need to be loaded and enabled state
        public void ReloadModData()
        {
            ReadVortexDeploymentData();
            //Load each mods mod.json and store in Dict.
            LoadAllModDetails();
            //Combine so we have all mods in the ModList Dict for easy later use and writing to JObject
            CombineDirModList();
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
            if (KnownModListGameVersion != null)
            {
                bestAvailableVersion = KnownModListGameVersion;
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
            MainForm.Instance.toolStripStatusLabelMwVersion.Text = @"Game Version: " + bestAvailableVersion;
        }

        /// <summary>
        /// Checks for all items in the modlist if they have a possible folder on system they can point to.
        /// If not removes them from the modlist and informs user.
        /// newFoldernamesEnabledList has only foldernames as key, not full paths
        /// </summary>
        public void ProcessModFolderEnabledList(ref Dictionary<string, bool> newFolderNamesEnabledList, bool warnMissing)
        {
            Dictionary<string, bool> MissingModDirs = new();
            foreach (var item in newFolderNamesEnabledList)
            {
                if (Utils.StringNullEmptyOrWhiteSpace(item.Key))
                {
                    newFolderNamesEnabledList.Remove(item.Key);
                    continue;
                }

                // Collect listed mods that are unavailable locally
                if (!DirNameToPathDict.ContainsKey(item.Key))
                { 
                    MissingModDirs.Add(item.Key, item.Value);
                }
            }
            foreach (var missingModDir in MissingModDirs)
            {
                newFolderNamesEnabledList.Remove(missingModDir.Key);

                // We will silently ignore missing mods that are not enabled
                if (!missingModDir.Value)
                {
                    MissingModDirs.Remove(missingModDir.Key);
                }
            }
            if (warnMissing && MissingModDirs.Count > 0)
            {
                var missingMods = string.Join(MissingModDirs.Count > 5 ? ", " : "\r\n", MissingModDirs.Keys);

                TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                {
                    Text = "The mod list includes the following enabled mods which are unavailable locally:\r\n\r\n"
                           + missingMods
                           + "\r\n\r\nThese mods will be ignored.",
                    Heading = "Invalid mods in preset.",
                    Caption = "Warning",
                    Buttons =
                    {
                        TaskDialogButton.OK,
                    },
                    Icon = TaskDialogIcon.Warning,
                    DefaultButton = TaskDialogButton.OK,
                    AllowCancel = true
                });
            }

            Dictionary<string, bool> newModListDict = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var curFolderItem in newFolderNamesEnabledList)
            {
                string fullPath = DirNameToPathDict[curFolderItem.Key];
                newModListDict[fullPath] = curFolderItem.Value;
            }
            newFolderNamesEnabledList = newModListDict;
        }

        public void ProcessModNameEnabledList(ref Dictionary<string, bool> newNamesEnabledList, bool warnMissing)
        {
            Dictionary<string, bool> newModListDict = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, bool> missingMods = new();
            foreach (var curItem in newNamesEnabledList)
            {
                if (Utils.StringNullEmptyOrWhiteSpace(curItem.Key))
                {
                    newNamesEnabledList.Remove(curItem.Key);
                    continue;
                }

                bool foundMatch = false;

                // find all mods that match the name. There might be duplicates
                List<string> foundMods = new List<string>();
                foreach (var curModDetail in ModDetails)
                {
                    if (curModDetail.Value.displayName == curItem.Key)
                    {
                        foundMods.Add(curModDetail.Key);
                    }
                }

                if (foundMods.Count > 0)
                {
                    // Sort available version, so that newest is on top of the list
                    foundMods.Sort((x, y) =>
                    {
                        int compResult = string.IsNullOrWhiteSpace(ModDetails[x].version)
                            .CompareTo(string.IsNullOrWhiteSpace(ModDetails[y].version));

                        if (compResult == 0 && 
                            (!string.IsNullOrWhiteSpace(ModDetails[y].version) &&
                            !string.IsNullOrWhiteSpace(ModDetails[x].version))
                            )
                        {
                            compResult = Utils.CompareVersionStrings(ModDetails[y].version, ModDetails[x].version);
                        }

                        if (compResult == 0)
                        {
                            compResult = ModDetails[y].buildNumber.CompareTo(ModDetails[x].buildNumber);
                        }

                        return compResult;
                    });

                    foundMatch = true;
                    newModListDict.Add(foundMods[0], curItem.Value);
                }

                // Collect mods that are unavailable locally
                if (!foundMatch)
                {
                    missingMods.Add(curItem.Key, curItem.Value);
                }
            }
            foreach (var curMissingMod in missingMods)
            {
                newNamesEnabledList.Remove(curMissingMod.Key);

                // We will silently ignore missing mods that are not enabled
                if (!curMissingMod.Value)
                {
                    missingMods.Remove(curMissingMod.Key);
                }
            }
            if (warnMissing && missingMods.Count > 0)
            {
                var missingModsString = string.Join(missingMods.Count > 5 ? ", " : "\r\n", missingMods.Keys);

                TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                {
                    Text = "The mod list includes the following enabled mods which are unavailable locally:\r\n\r\n"
                           + missingModsString
                           + "\r\n\r\nThese mods will be ignored.",
                    Heading = "Invalid mods in preset.",
                    Caption = "Warning",
                    Buttons =
                    {
                        TaskDialogButton.OK,
                    },
                    Icon = TaskDialogIcon.Warning,
                    DefaultButton = TaskDialogButton.OK,
                    AllowCancel = true
                });
            }

            newNamesEnabledList = newModListDict;
        }

        public static string GetSettingsDirectory()
        {
            string appDataDir = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataDir, @"MW5LoadOrderConfigurator");
        }

        //Try and load data from previous sessions
        public bool TryLoadProgramSettings()
        {
            //Load settings from previous session:
            try
            {
                this.ProgramSettings.LoadSettings();

                UpdateGamePaths();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"ERROR: Something went wrong while loading " + SettingsFileName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            if (LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore)
                return true;

            if (!Utils.StringNullEmptyOrWhiteSpace(LocSettings.Instance.Data.InstallPath))
                return true;

            return false;
        }

        private static string GetLocalAppDataModPath()
        {
            string AppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //Put string back together
            return Path.Combine(AppDataRoaming, "MW5Mercs", "Saved", "Mods");
        }

        // Deduces mod directory locations
        public void UpdateGamePaths()
        {
            this.ModsPaths[ModsManager.eModPathType.Program] = null;
            this.ModsPaths[ModsManager.eModPathType.Steam] = null;
            this.ModsPaths[ModsManager.eModPathType.AppData] = null;

            if (LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore)
            {
                this.ModsPaths[ModsManager.eModPathType.Program] = Path.Combine(LocSettings.Instance.Data.InstallPath, "MW5Mercs", "Mods");
            }

            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Steam:
                    SetSteamWorkshopPath();
                    break;
                case eGamePlatform.WindowsStore:
                    this.ModsPaths[ModsManager.eModPathType.AppData] = GetLocalAppDataModPath();
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
            string steamAppsParentDirectory = FindSteamAppsParentDirectory(LocSettings.Instance.Data.InstallPath);
            string workshopPath = Path.Combine(steamAppsParentDirectory, "workshop", "content", "784080");
            this.ModsPaths[ModsManager.eModPathType.Steam] = workshopPath;
        }

        public void ParseDirectories()
        {
            this.FoundDirectories.Clear();

            if (LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore
                && !Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Program])
                && Directory.Exists(ModsPaths[eModPathType.Program]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Program]));
            }

            if (!Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Steam])
                && Directory.Exists(ModsPaths[eModPathType.Steam]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Steam]));
            }

            if (LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore
                && !Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.AppData])
                && Directory.Exists(ModsPaths[eModPathType.AppData]))
            {
                this.FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.AppData]));
            }
            //AddDirectoryPathsToDict();
        }

        private void AddDirectoryPathsToDict()
        {
            foreach (string curDirectory in FoundDirectories)
            {
                string directoryName = Path.GetFileName(curDirectory);
                this.DirNameToPathDict[directoryName] = curDirectory;
                this.PathToDirNameDict[curDirectory] = directoryName;
            }
        }

        public void SaveSettings()
        {
            ProgramSettings.SaveSettings();
        }

        public void WarnIfNoModList()
        {
            string modlistPath = GetModListJsonFilePath();
            if (File.Exists(modlistPath))
                return;

            TaskDialogButton result = TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
            {
                Text =                 @"The modlist.json file could not be found in"+ System.Environment.NewLine 
                    + modlistPath +@"."+System.Environment.NewLine+System.Environment.NewLine
                    +@"It is necessary to read this file in order to validate it with the correct version number the game expects." + System.Environment.NewLine + System.Environment.NewLine
                    +@"LOC will try to create the file with the correct version number when applying your profile, but there is high chance that this will fail."+System.Environment.NewLine
                    +@"It is recommended to start the game once in order to create this file before applying your mod profile.",

                Heading = "The modlist.json file could not be found.",
                Caption = "Mod list error",
                Buttons =
                {
                    TaskDialogButton.OK,
                },
                Icon = TaskDialogIcon.Warning,
                DefaultButton = TaskDialogButton.OK,
                AllowCancel = true
            });
        }

        public Dictionary<string, bool> LoadModList()
        {
            string modlistPath = GetModListJsonFilePath();

            if (!File.Exists(modlistPath))
                return null;

            JObject modListObjectObject;
            try
            {
                this.rawJson = File.ReadAllText(modlistPath);
                modListObjectObject = JObject.Parse(rawJson);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    @"There was an error trying to parse the modlist.json file in "+ System.Environment.NewLine 
                    + modlistPath +@"."+System.Environment.NewLine+System.Environment.NewLine
                    , @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            string gameVersionObj = modListObjectObject.Value<string>("gameVersion");
            if (gameVersionObj != null)
            {
                KnownModListGameVersion = gameVersionObj.ToString();
            }

            JObject modStatus = modListObjectObject.Value<JObject>("modStatus");
            if (modStatus != null)
            {
                Dictionary<string, bool> modList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                int modListIndex = 0;
                foreach (JProperty curMOD in modStatus.Properties())

                {
                    bool enabled = (bool)modStatus[curMOD.Name]?["bEnabled"];
                    modList.Add(curMOD.Name, enabled);
                    ++modListIndex;
                }

                return modList;
            }

            return null;
        }

        public void SaveToFiles()
        {
            SaveModDetails();
            SaveModListToFile();
            SaveLastAppliedModOrder();
        }

        public void ClearAll()
        {
            this.ModDirectories.Clear();
            this.Mods.Clear();
            this.ModDetails.Clear();
            this.ModEnabledList.Clear();
            this.DirNameToPathDict.Clear();
            this.OverridingData.Clear();
            this.ModsPaths[eModPathType.Program] = null;
            this.ModsPaths[eModPathType.Steam] = null;
            this.ModsPaths[eModPathType.AppData] = null;
            this.VortexDeploymentData.Clear();
        }

        private void CombineDirModList()
        {
            // First sort the directories by the default MW5 load orders
            ModDirectories.Sort((x, y) =>
            {
                if (!Mods.ContainsKey(x) || !Mods.ContainsKey(y))
                    return 0;

                // Compare Original load order
                int priorityComparison = Mods[y].OriginalLoadOrder.CompareTo(Mods[x].OriginalLoadOrder);

                // If Priority is equal, compare Folder name
                if (priorityComparison == 0)
                {
                    return PathToDirNameDict[y].CompareTo(PathToDirNameDict[x]);
                }

                return priorityComparison;
            });

            // There are sometimes "ghost" entries in the modlist.json for which there are no directories left, lets remove those.
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, bool> entry in this.ModEnabledList)
            {
                if (this.ModDirectories.Contains<string>(entry.Key))
                    continue;
                toRemove.Add(entry.Key);
            }
            foreach (string key in toRemove)
            {
                this.ModEnabledList.Remove(key);
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
            float GetOriginalLoadOrderFromObject(JObject jsonObject)
            {
                // Our saved load order
                if (jsonObject.ContainsKey("locOriginalLoadOrder"))
                {
                    return jsonObject["locOriginalLoadOrder"].Value<float>();
                }
                else if (jsonObject.ContainsKey("lotsOriginalLoadOrder"))
                {
                    // Might have been set by the MW5-LOTS mod order manager
                    return jsonObject["lotsOriginalLoadOrder"].Value<float>();
                }
                else if (jsonObject.ContainsKey("defaultLoadOrder"))
                {
                    return jsonObject["defaultLoadOrder"].Value<float>();
                }
                return 0.0f;
            }

            bool loadModSuccess = false;
            try
            {
                ModData modData = new ModData();
                ModObject modJsonDataObject = null;
                try
                {
                    string modJsonFilePath = Path.Combine(modPath, @"mod.json");
                    if (!File.Exists(modJsonFilePath))
                    {
                        return;
                    }

                    string modJsonText = File.ReadAllText(modJsonFilePath);
                    JObject modJsonObject = JObject.Parse(modJsonText);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    modJsonDataObject = JsonConvert.DeserializeObject<ModObject>(modJsonText, jsonSettings);

                    modData.NewLoadOrder = modJsonDataObject.defaultLoadOrder;
                    bool foundLoadOrder = false;
                    modData.IsNewMod = !modJsonObject.ContainsKey("locOriginalLoadOrder");

                    // Now let's be a bit overkill and try our best to find the original order of the mod
                    // Since other load order manager save these load orders very differently (or not all),
                    // try some different methods

                    // See if there is a backup file generated by MW5 Mod Organizer that has the original load order
                    // Some mods also accidentally deploy with this file
                    string modBackupJsonFilePath = Path.Combine(modPath, @"backup.json");
                    if (File.Exists(modBackupJsonFilePath))
                    {
                        try
                        {
                            string modBackupJson = File.ReadAllText(modBackupJsonFilePath);
                            JObject modBackupDetailsJ = JObject.Parse(modBackupJson);

                            bool buildAndVersionMatches = false;

                            if (modBackupDetailsJ.ContainsKey("version") &&
                                modBackupDetailsJ.ContainsKey("buildNumber"))
                            {
                                buildAndVersionMatches =
                                    String.Compare(modBackupDetailsJ["version"]?.ToString(), modJsonDataObject.version, StringComparison.Ordinal) == 0 &&
                                    String.Compare(modBackupDetailsJ["buildNumber"]?.ToString(), modJsonDataObject.buildNumber.ToString(), StringComparison.Ordinal) == 0;
                            }

                            if (buildAndVersionMatches)
                            {
                                modData.OriginalLoadOrder = GetOriginalLoadOrderFromObject(modBackupDetailsJ);
                                foundLoadOrder = true;
                            }
                        }
                        catch (Exception Ex)
                        {
                            // Silently fail
                        }
                        
                    }

                    if (!foundLoadOrder)
                    {
                        modData.OriginalLoadOrder = GetOriginalLoadOrderFromObject(modJsonObject);
                    }

                    // Determine mod origin
                    string modDirName = Path.GetFileName(modPath);

                    // Check if this might be a mod from the steam workshop
                    if (IsSteamWorkshopID(modDirName))
                    {
                        // If the mod directory name matches the store id, we can be pretty certain
                        // there are mods however, that don't have this info correctly filled
                        if (modDirName == modJsonDataObject.steamPublishedFileId.ToString())
                        {
                            modData.Origin = ModData.ModOrigin.Steam;
                        }

                        // if this looks like a steam id and the mod is stored in the steam mods directory
                        // it's certain that this is a steam mod
                        if (modData.Origin == ModData.ModOrigin.Unknown)
                        {
                            if (modPath.StartsWith(ModsPaths[eModPathType.Steam]))
                            {
                                modData.Origin = ModData.ModOrigin.Steam;
                            }
                        }
                    }

                    if (modData.Origin == ModData.ModOrigin.Unknown)
                    {
                        if (VortexDeploymentData.ContainsKey(modDirName))
                        {
                            VortexDeploymentModData vortexModData = VortexDeploymentData[modDirName];

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
                    TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                    {
                        Text = @"Error loading mod.json in : " + modPath + System.Environment.NewLine +
                               System.Environment.NewLine +
                               "The affected mod might need to be reinstalled. The mod will be skipped.",
                        Heading = "Invalid or corrupted mod.",
                        Caption = "Error",
                        Buttons =
                        {
                            TaskDialogButton.OK,
                        },
                        Icon = TaskDialogIcon.Error,
                        DefaultButton = TaskDialogButton.OK,
                        AllowCancel = true
                    });

                    return;
                }

                // Sanity check for mod files
                string pakDir = Path.Combine(modPath, "Paks");
                if (!Directory.Exists(pakDir) || Directory.GetFiles(pakDir, "*.pak").Length == 0)
                {
                    TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                    {
                        Text = @"The mod in the path" + System.Environment.NewLine +
                               modPath + System.Environment.NewLine + 
                               @"might be corrupted." + System.Environment.NewLine +
                               "The mod has a valid mod.json, but has no Pak game data files associated with it.\r\nThe affected mod might need to be reinstalled.",
                        Heading = "Invalid or corrupted mod.",
                        Caption = "Warning",
                        Buttons =
                        {
                            TaskDialogButton.OK,
                        },
                        Icon = TaskDialogIcon.Warning,
                        DefaultButton = TaskDialogButton.OK,
                        AllowCancel = true
                    });
                }

                long totalPakSize = 0;
                bool hasZeroBytePak = false;
                if (Directory.Exists(pakDir))
                {
                    foreach (string filePath in Directory.GetFiles(pakDir, "*.pak"))
                    {
                        long fileSize = LocFileUtils.GetFileSize(filePath);

                        if (fileSize == 0)
                            hasZeroBytePak = true;

                        totalPakSize += fileSize;
                    }
                }

                if (hasZeroBytePak)
                {
                    TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                    {
                        Text = @"The mod in the path" + System.Environment.NewLine +
                               modPath + System.Environment.NewLine + 
                               @"might be corrupted." + System.Environment.NewLine +
                               "The mod has one or more Pak game data files that are zero bytes in size.\r\nThe affected mod might need to be reinstalled.",
                        Heading = "Invalid or corrupted mod.",
                        Caption = "Warning",
                        Buttons =
                        {
                            TaskDialogButton.OK,
                        },
                        Icon = TaskDialogIcon.Warning,
                        DefaultButton = TaskDialogButton.OK,
                        AllowCancel = true
                    });
                }

                modData.ModFileSize = totalPakSize;
                this.Mods.Add(modPath, modData);
                this.ModDetails.Add(modPath, modJsonDataObject);
                this.ModDirectories.Add(modPath);
                string directoryName = Path.GetFileName(modPath);
                this.DirNameToPathDict[directoryName] = modPath;
                this.PathToDirNameDict[modPath] = directoryName;
                loadModSuccess = true;
            }
            finally
            {
                if (!loadModSuccess)
                {
                    if (ModEnabledList.ContainsKey(modPath))
                        ModEnabledList.Remove(modPath);
                }
            }
 
        }

        private void LoadAllModDetails()
        {
            Mods.Clear();
            ModDetails.Clear();
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

                
                float originalLoadOrder = Mods[entry.Key].OriginalLoadOrder;
                if (float.IsInteger(originalLoadOrder))
                {
                    int oloInteger = (int)originalLoadOrder;
                    modDetailsNew["locOriginalLoadOrder"] = oloInteger;
                }
                else
                {
                    modDetailsNew["locOriginalLoadOrder"] = originalLoadOrder;
                }

                float newLoadOrder = Mods[entry.Key].NewLoadOrder;
                if (float.IsInteger(newLoadOrder))
                {
                    int nloInteger = (int)newLoadOrder;
                    modDetailsNew["defaultLoadOrder"] = nloInteger;
                }
                else
                {
                    modDetailsNew["defaultLoadOrder"] = newLoadOrder;
                }

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

        public void SaveModListToFile()
        {
            string modlistJsonFilePath = GetModListJsonFilePath();
            string modlistJsonFileDir = Path.GetDirectoryName(modlistJsonFilePath);

            if (!Directory.Exists(modlistJsonFileDir))
            {
                string message = "The mod directory " + modlistJsonFileDir + " does not exist. Aborting.";
                string caption = "Error saving mod list";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                return;
            }

            JObject modListObject = null;
            // Fail silently if the current modlist.json could not be read for whatever reason
            if (File.Exists(modlistJsonFilePath))
            {
                try
                {
                    string modListJsonExisting = File.ReadAllText(modlistJsonFilePath);
                    modListObject = JObject.Parse(modListJsonExisting);
                }
                catch (Exception e)
                {

                }
            }
            
            if (modListObject == null)
            {
                modListObject = new JObject();
                modListObject["gameVersion"] = GameVersion;
            }

            JObject modStatusObject = modListObject.Value<JObject>("modStatus");
            if (modStatusObject != null)
            {
                modStatusObject.RemoveAll();
            }
            else
            {
                modStatusObject = new JObject();
                modListObject.Add("modStatus", modStatusObject);
            }

            foreach (KeyValuePair<string, bool> entry in this.ModEnabledList)
            {
                string[] temp = entry.Key.Split('\\');
                string modFolderName = temp[temp.Length - 1];

                JObject newStatus = new JObject(
                    new JProperty("bEnabled", entry.Value)
                );
                modStatusObject.Add(modFolderName, newStatus);
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(modlistJsonFilePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, modListObject);
            }
        }

        internal void SaveLastAppliedModOrder()
        {
            string lastAppliedJsonFile = GetSettingsDirectory() + Path.DirectorySeparatorChar + LastAppliedOrderFileName;

            Dictionary<string, LastAppliedPresetModData> lastAppliedModList = new Dictionary<string, LastAppliedPresetModData>();
            foreach (KeyValuePair<string, bool> entry in ModEnabledList)
            {
                string folderName = ModsManager.Instance.PathToDirNameDict[entry.Key];

                LastAppliedPresetModData lastAppliedModData = new LastAppliedPresetModData();
                lastAppliedModData.state = entry.Value;
                lastAppliedModData.lastLoadOrder = Mods[entry.Key].NewLoadOrder;
                lastAppliedModData.version = ModDetails[entry.Key].version;
                lastAppliedModData.buildNumber = ModDetails[entry.Key].buildNumber;
                lastAppliedModList[folderName] = lastAppliedModData;
            }

            JObject json = new JObject();
            json["timestamp"] = TimeProvider.System.GetUtcNow().ToUnixTimeSeconds();
            json["gameVersion"] = GameVersion;
            json["mods"] = JObject.FromObject(lastAppliedModList);

            string lastAppliedString = JsonConvert.SerializeObject(json, Formatting.Indented);

            if (File.Exists(lastAppliedJsonFile))
                File.Delete(lastAppliedJsonFile);

            StreamWriter sw = File.CreateText(lastAppliedJsonFile);
            sw.WriteLine(lastAppliedString);
            sw.Flush();
            sw.Close();
        }

        //Save presets from memory to file for use in next session.
        internal void SavePresets()
        {
            string presetsJsonFile = GetSettingsDirectory() + Path.DirectorySeparatorChar + PresetsFileName;
            string presetJsonString = JsonConvert.SerializeObject(this.Presets, Formatting.Indented);

            if (File.Exists(presetsJsonFile))
                File.Delete(presetsJsonFile);

            //Console.WriteLine(JsonString);
            StreamWriter sw = File.CreateText(presetsJsonFile);
            sw.WriteLine(presetJsonString);
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
                string message = "There was an error in decoding the presets file:\r\n\r\n"
                    + Ex.Message;
                string caption = "Presets File Decoding Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                return;
            }
            this.Presets = temp;
        }

        //Used to update the override data when a new item is added or removed to/from the mod list instead of checking all items agains each other again.
        public void UpdateNewModOverrideData(ModItem newModItem)
        {
            string modA = newModItem.FolderName;

            if (!newModItem.Enabled)
            {
                if (this.OverridingData.ContainsKey(modA))
                    this.OverridingData.Remove(modA);

                foreach (string key in this.OverridingData.Keys)
                {
                    if (OverridingData[key].overriddenBy.ContainsKey(modA))
                        OverridingData[key].overriddenBy.Remove(modA);

                    if (OverridingData[key].overrides.ContainsKey(modA))
                        OverridingData[key].overrides.Remove(modA);

                    if (OverridingData[key].overrides.Count == 0)
                        OverridingData[key].isOverriding = false;

                    if (OverridingData[key].overriddenBy.Count == 0)
                        OverridingData[key].isOverridden = false;
                }
            }
            else
            {
                if (!this.OverridingData.ContainsKey(modA))
                {
                    this.OverridingData[modA] = new OverridingData
                    {
                        mod = modA,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                }

                // check each mod for changes
                foreach (ModItem item in ModItemList.Instance.ModList)
                {
                    string modB = item.FolderName;

                    // Don't compare the same mod
                    if (modA == modB)
                        continue;

                    if (!item.Enabled)
                        continue;

                    if (!this.OverridingData.ContainsKey(modB))
                    {
                        this.OverridingData[modB] = new OverridingData
                        {
                            mod = modB,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                    }
                    RecomputeModOverridingData(newModItem, item, this.OverridingData[modA], this.OverridingData[modB]);
                }
            }

            MainForm.Instance.ColorizeListViewItems();
        }

        //See if items A and B are interacting in terms of manifest and return the intersect
        public void RecomputeModOverridingData(ModItem listItemA, ModItem listItemB, OverridingData A, OverridingData B)
        {
            if (listItemA == listItemB)
                return;

            string modA = listItemA.FolderName;
            string modB = listItemB.FolderName;

            float loadOrderA = Mods[listItemA.Path].NewLoadOrder;
            float loadOrderB = Mods[listItemB.Path].NewLoadOrder;

            //Now we have a mod that is not the mod we are looking at is enabled.
            //Lets compare the manifest!
            List<string> manifestA = this.ModDetails[this.DirNameToPathDict[modA]].manifest;
            List<string> manifestB = this.ModDetails[this.DirNameToPathDict[modB]].manifest;
            List<string> intersect = manifestA.Intersect(manifestB).ToList();

            if (!intersect.Any())
                return;

            //If we are loaded after the mod we are looking at we are overriding it.
            if (loadOrderA > loadOrderB)
            {
                if (A.mod != modB)
                {
                    A.isOverriding = true;
                    A.overrides[modB] = intersect;
                }
                if (B.mod != modA)
                {
                    B.isOverridden = true;
                    B.overriddenBy[modA] = intersect;
                }
            }
            else
            {
                if (A.mod != modB)
                {
                    A.isOverridden = true;
                    A.overriddenBy[modB] = intersect;
                }
                if (B.mod != modA)
                {
                    B.isOverriding = true;
                    B.overrides[modA] = intersect;
                }
            }
            this.OverridingData[modA] = A;
            this.OverridingData[modB] = B;
        }

        public void RecomputeOverridingData()
        {
            ////Console.WriteLine(Environment.StackTrace);
            ////Console.WriteLine("Starting Overriding data check");
            this.OverridingData.Clear();

            foreach (ModItem itemA in ModItemList.Instance.ModList)
            {
                // Skip disabled items
                if (!itemA.Enabled)
                    continue;

                string modA = itemA.FolderName;

                //Check if we already have this mod in the dict if not create an entry for it.
                if (!this.OverridingData.ContainsKey(modA))
                {
                    this.OverridingData[modA] = new OverridingData
                    {
                        mod = modA,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                }
                OverridingData A = this.OverridingData[modA];

                //Console.WriteLine("Checking: " + modA + " : " + priorityA.ToString());
                foreach (ModItem itemB in ModItemList.Instance.ModList)
                {
                    string modB = itemB.FolderName;

                    if (modA == modB)
                        continue;

                    if (!itemB.Enabled)
                        continue;

                    //If we have already seen modB in comparison to modA we don't need to compare because the comparison is bi-directionary.
                    if (
                        A.overriddenBy.ContainsKey(modB) ||
                        A.overrides.ContainsKey(modB)
                        )
                    {
                        ////Console.WriteLine("--" + modA + "has allready been compared to: " + modB);
                        continue;
                    }

                    //Check if we have already seen modB before.
                    if (this.OverridingData.ContainsKey(modB))
                    {
                        //If we have allready seen modB and we have allready compared modB and modA we don't need to compare because the comparison is bi-directionary.
                        if (
                            this.OverridingData[modB].overriddenBy.ContainsKey(modA) ||
                            this.OverridingData[modB].overrides.ContainsKey(modA)
                            )
                        {
                            ////Console.WriteLine("--" + modB + "has allready been compared to: " + modA);
                            continue;
                        }
                    }
                    else
                    {
                        //If we have not make a new modB overridingDatas
                        this.OverridingData[modB] = new OverridingData
                        {
                            mod = modB,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                    }
                    RecomputeModOverridingData(itemA, itemB, this.OverridingData[modA], this.OverridingData[modB]);
                }
            }

            MainForm.Instance.ColorizeListViewItems();
        }
    }
}
