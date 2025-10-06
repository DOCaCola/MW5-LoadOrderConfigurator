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
using SharpCompress.Common;

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

        // General info about a specific mod directory
        public class ModPathInfo
        {
            public string FullPath = null;
            public FileSystemWatcherAsync<eModPathType> FolderWatcher = null;
        }
        public ArrayByEnum<ModPathInfo, eModPathType> ModsPaths = new();

        
        // Input data when importing a mod from file/clipboard/last load order file
        public class ModImportData
        {
            public string ModPath;
            public string ModFolder;
            public string ModName;
            // Version and build is currently unused. Potentially for future use
            public string Version;
            public int Build = -1;
            public float LoadOrder = Single.NaN;
            public bool Enabled = false;
            public bool Available = false;
        }

        public LocSettings ProgramSettings;

        // User made changes not written to files
        public bool ModSettingsTainted = false;

        // Directories found in all mod paths
        public List<string> FoundDirectories = new();
        public Dictionary<string, string> DirNameToPathDict = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> PathToDirNameDict = new(StringComparer.OrdinalIgnoreCase);

        // Mod data as loaded from the mods' mod.json file
        public Dictionary<string, ModObject> ModDetails = new(StringComparer.OrdinalIgnoreCase);
        // Valid mod directories
        public List<string> ModDirectories = new();

        public List<ModImportData> ModEnabledList = new();
        // As it was last loaded from file
        public List<ModImportData> ModEnabledListLastState;
        public Dictionary<string, ModConflictData> ModConflictData = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Presets = new(StringComparer.OrdinalIgnoreCase);

        // Triggered when critical mod files were changed that would require a file reload
        public event EventHandler ModFilesChangedEvent;

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
        public List<ModImportData> LastAppliedPresetModList = null;

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
            // timestamp with age of mod files
            public DateTimeOffset? FileAge = null;
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
                    path = ModsPaths[eModPathType.AppData].FullPath;
                    break;
                default:
                    path = ModsPaths[eModPathType.Program].FullPath;
                    break;
            }

            return Path.Combine(path, @"modlist.json");
        }

        public string GetMainModPath()
        {
            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.WindowsStore:
                    return ModsPaths[eModPathType.AppData].FullPath;
                default:
                    return ModsPaths[eModPathType.Program].FullPath;
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

            List<ModImportData> lastAppliedValid = new();
            foreach (var curMod in LastAppliedPreset.mods)
            {
                ModImportData newImportData = new();
                newImportData.ModFolder = curMod.Key;
                newImportData.Enabled = curMod.Value.state;

                lastAppliedValid.Add(newImportData);
            }
            ProcessModImportList(ref lastAppliedValid, false);
            LastAppliedPresetModList = lastAppliedValid;
        }

        public bool ShouldLoadLastApplied(Action listRefreshCallback)
        {
            if (LastAppliedPreset == null || LastAppliedPreset.mods == null)
                return false;

            // Remove invalid mods from last loaded list.
            List<ModImportData> lastMods = new();
            foreach (var curModItem in LastAppliedPreset.mods)
            {
                ModImportData newImportData = new ModImportData();
                newImportData.ModFolder = curModItem.Key;
                newImportData.Enabled = curModItem.Value.state;

                lastMods.Add(newImportData);
            }
            ProcessModImportList(ref lastMods, false);

            // Filter to enabled only mods
            List<string> lastEnabledModList = lastMods
                .Where(kv => kv.Enabled && kv.Available)
                .Select(kv => kv.ModPath)
                .ToList();

            List<string> curEnabledModList = new();
            if (ModEnabledListLastState != null)
            {
                curEnabledModList = ModEnabledList
                    .Where(kv => kv.Enabled && kv.Available)
                    .Select(kv => kv.ModPath)
                    .ToList();
            }

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

                ModImportData enabledListItem = ModEnabledListLastState?.FirstOrDefault(x =>
                    x.ModPath.Equals(curCandidate, StringComparison.InvariantCultureIgnoreCase));

                bool enabledStateChanged = ModEnabledListLastState == null || enabledListItem == null || !enabledListItem.Enabled;

                if (enabledStateChanged)
                {
                    enabledStateChangedModNames.Add(ModDetails[curCandidate].displayName);
                }
            }

            if (loadOrderChangedModNames.Count > 0)
            {
                listRefreshCallback();

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
                listRefreshCallback();

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
                page.Text = "This might have been caused as a result of a game update or due to another programs altering the mod list.\r\n\r\nThe following mods are affected:\r\n" + changedMods + "\r\n\r\n How would you like to proceed?";

                TaskDialogButton dialogResult = TaskDialog.ShowDialog(MainForm.Instance.Visible ? MainForm.Instance.Handle : 0, page);

                if (dialogResult.Tag is int resultIndex)
                    return resultIndex == 1;
            }

            return false;
        }

        public void RenewModEnabledList()
        {
            ModEnabledList.Clear();
            foreach (string modDir in this.ModDirectories)
            {
                ModImportData newImportData = new ModImportData();
                newImportData.ModPath = modDir;
                newImportData.ModFolder = Path.GetFileName(modDir);
                newImportData.Available = true;
                ModEnabledList.Add(newImportData);
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
            foreach (ModPathInfo curModInfo in this.ModsPaths)
            {
                if (curModInfo == null || Utils.StringNullEmptyOrWhiteSpace(curModInfo.FullPath))
                    continue;

                string vortexDeploymentFile = Path.Combine(curModInfo.FullPath, @"vortex.deployment.json");

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
                        newVortexData.fullpath = Path.Combine(curModInfo.FullPath, modFolderName);

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
        /// newFoldernamesEnabledList has only foldernames, doesn't contain full paths yet
        /// </summary>
        public void ProcessModImportList(ref List<ModImportData> modImportList, bool warnMissing)
        {
            List<string> missingMods = new List<string>();
            foreach (var curImportItem in modImportList)
            {
                // We either have the name of the mod or the name of the mod folder, so let's try to complete the missing one
                bool hasFolder = !Utils.StringNullEmptyOrWhiteSpace(curImportItem.ModFolder);
                bool hasName = !Utils.StringNullEmptyOrWhiteSpace(curImportItem.ModName);

                bool foundMod = false;
                if (hasFolder)
                {
                    if (DirNameToPathDict.TryGetValue(curImportItem.ModFolder, out var modPath))
                    {
                        curImportItem.Available = true;
                        curImportItem.ModPath = modPath;
                        curImportItem.ModFolder = Path.GetFileName(modPath);
                        foundMod = true;
                    }
                }

                if (hasName)
                {

                    // find all mods that match the name. There might be duplicates
                    List<string> foundLocalMods = new List<string>();
                    foreach (var curModDetail in ModDetails)
                    {
                        if (curModDetail.Value.displayName == curImportItem.ModName)
                        {
                            foundLocalMods.Add(curModDetail.Key);
                        }
                    }

                    if (foundLocalMods.Count > 0)
                    {
                        // Sort available version, so that newest is on top of the list
                        foundLocalMods.Sort((x, y) =>
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

                        foundMod = true;
                        curImportItem.ModPath = foundLocalMods[0];
                        curImportItem.ModFolder = ModsManager.Instance.PathToDirNameDict[foundLocalMods[0]];
                        curImportItem.Available = true;
                    }
                }

                if (!foundMod)
                {
                    if (curImportItem.Enabled)
                    {
                        string modName = String.Empty;
                        if (!string.IsNullOrWhiteSpace(curImportItem.ModName))
                            modName = curImportItem.ModName;
                        else if (!string.IsNullOrWhiteSpace(curImportItem.ModFolder))
                            modName = curImportItem.ModFolder;

                        missingMods.Add(modName);
                    }
                    continue;
                }
            }
            if (warnMissing && missingMods.Count > 0)
            {
                var missingModsString = string.Join(missingMods.Count > 5 ? ", " : "\r\n", missingMods);

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

            modImportList.RemoveAll(x => !x.Available);
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
            string appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataRoaming, "MW5Mercs", "Saved", "Mods");
        }

        enum ModFileAction
        {
            Changed,
            Created,
            Deleted,
            Renamed
        }

        private int _fileWatchStopCounter = 0;

        public void StartModFileWatches()
        {
            if (_fileWatchStopCounter == 0)
            {

                foreach (ModPathInfo curModInfo in this.ModsPaths)
                {
                    curModInfo?.FolderWatcher?.StartWatching();
                }
            }
        }
        public void StopModFileWatches()
        {
            if (_fileWatchStopCounter > 0)
            {
                _fileWatchStopCounter--;
                if (_fileWatchStopCounter == 0)
                {
                    foreach (ModPathInfo curModInfo in this.ModsPaths)
                    {
                        curModInfo?.FolderWatcher?.StopWatching();
                    }
                }
            }
        }

        private void ModFilesChanged(ModFileAction action, string path, string oldPath, eModPathType modPathType)
        {
            bool IsPathOfInterest(string pathOfInterest, bool fileMissing)
            {
                if (string.IsNullOrWhiteSpace(pathOfInterest))
                    return false;

                if (string.Equals(Path.GetFileName(pathOfInterest), "mod.json", StringComparison.OrdinalIgnoreCase))
                {
                    if (fileMissing || File.Exists(pathOfInterest))
                    {
                        if (LocFileUtils.IsDirectSubdirectory(ModsPaths[modPathType].FullPath, Path.GetDirectoryName(pathOfInterest)))
                        {
                            return true;
                        }
                    }
                }

                // Check directory types
                if (LocFileUtils.IsDirectSubdirectory(ModsPaths[modPathType].FullPath, pathOfInterest))
                {
                    if (action == ModFileAction.Deleted)
                    {
                        // We are only interested in tracked mod directories
                        foreach (string curModDirectory in this.ModDirectories)
                        {
                            if (string.Equals(pathOfInterest, curModDirectory, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                    else if (action == ModFileAction.Created || (action == ModFileAction.Renamed && !fileMissing))
                    {
                        // Check if there is a newly created mod directory which contains a mod.json
                        return File.Exists(Path.Combine(pathOfInterest, "mod.json"));
                    }
                }

                return false;
            }

            if (!IsPathOfInterest(path, action == ModFileAction.Deleted) && !IsPathOfInterest(oldPath, true))
                return;

            ModFilesChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void ClearGamePaths()
        {
            ModsPaths[eModPathType.Program]?.FolderWatcher?.Dispose();
            ModsPaths[eModPathType.Program] = null;
            ModsPaths[eModPathType.Steam]?.FolderWatcher?.Dispose();
            ModsPaths[eModPathType.Steam] = null;
            ModsPaths[eModPathType.AppData]?.FolderWatcher?.Dispose();
            ModsPaths[eModPathType.AppData] = null;
        }

        // Deduces mod directory locations
        public void UpdateGamePaths()
        {
            ClearGamePaths();

            if (LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore)
            {
                string modPath = Path.Combine(LocSettings.Instance.Data.InstallPath, "MW5Mercs", "Mods");
                this.ModsPaths[ModsManager.eModPathType.Program] = CreateModPathInfo(modPath, eModPathType.Program);
            }

            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Steam:
                    string steamAppsParentDirectory = FindSteamAppsParentDirectory(LocSettings.Instance.Data.InstallPath);
                    string workshopPath = Path.Combine(steamAppsParentDirectory, "workshop", "content", "784080");
                    this.ModsPaths[ModsManager.eModPathType.Steam] = CreateModPathInfo(workshopPath, eModPathType.Steam);
                    break;
                case eGamePlatform.WindowsStore:
                    string appDataPath = GetLocalAppDataModPath();
                    this.ModsPaths[ModsManager.eModPathType.AppData] = CreateModPathInfo(appDataPath, eModPathType.AppData);
                    break;
            }
        }

        private ModPathInfo CreateModPathInfo(string path, eModPathType pathType)
        {
            NotifyFilters notifyFilters = NotifyFilters.CreationTime
                                          | NotifyFilters.DirectoryName
                                          | NotifyFilters.FileName
                                          | NotifyFilters.LastWrite
                                          | NotifyFilters.Size;

            var modPathInfo = new ModPathInfo
            {
                FullPath = path,
            };

            if (ProgramSettings.Data.EnableFileWatch && Directory.Exists(path))
            {
                var folderWatcher = new FileSystemWatcherAsync<eModPathType>(path, pathType, true, notifyFilters, _fileWatchStopCounter != 0);
                var customObject = folderWatcher.CustomObject;

                folderWatcher.Changed += (sender, e) => ModFilesChanged(ModFileAction.Changed, e.FullPath, null, customObject);
                folderWatcher.Created += (sender, e) => ModFilesChanged(ModFileAction.Created, e.FullPath, null, customObject);
                folderWatcher.Deleted += (sender, e) => ModFilesChanged(ModFileAction.Deleted, e.FullPath, null, customObject);
                folderWatcher.Renamed += (sender, e) => ModFilesChanged(ModFileAction.Renamed, e.FullPath, e.OldFullPath, customObject);

                modPathInfo.FolderWatcher = folderWatcher;
            }

            return modPathInfo;
        }

        public static string FindSteamAppsParentDirectory(string path)
        {
            string currentDirectory = Path.GetDirectoryName(path);

            while (currentDirectory != null)
            {
                if (string.Equals(Path.GetFileName(currentDirectory), "steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    return currentDirectory;
                }
                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            return null;
        }

        public void ParseDirectories()
        {
            FoundDirectories.Clear();

            if (LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore
                && !Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Program]?.FullPath)
                && Directory.Exists(ModsPaths[eModPathType.Program]?.FullPath))
            {
                FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Program]?.FullPath));
            }

            if (!Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.Steam]?.FullPath)
                && Directory.Exists(ModsPaths[eModPathType.Steam]?.FullPath))
            {
                FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.Steam]?.FullPath));
            }

            if (LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore
                && !Utils.StringNullEmptyOrWhiteSpace(ModsPaths[eModPathType.AppData]?.FullPath)
                && Directory.Exists(ModsPaths[eModPathType.AppData]?.FullPath))
            {
                FoundDirectories.AddRange(Directory.GetDirectories(ModsPaths[eModPathType.AppData]?.FullPath));
            }
            //AddDirectoryPathsToDict();
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
                Text = @"The modlist.json file could not be found in" + System.Environment.NewLine
                    + modlistPath + @"." + System.Environment.NewLine + System.Environment.NewLine
                    + @"It is necessary to read this file in order to validate it with the correct version number the game expects." + System.Environment.NewLine + System.Environment.NewLine
                    + @"LOC will try to create the file with the correct version number when applying your profile, but there is high chance that this will fail." + System.Environment.NewLine
                    + @"It is recommended to start the game once in order to create this file before applying your mod profile.",

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

        public List<ModImportData> LoadModList()
        {
            string modlistPath = GetModListJsonFilePath();

            if (!File.Exists(modlistPath))
                return null;

            JObject modListObjectObject;
            try
            {
                rawJson = File.ReadAllText(modlistPath);
                modListObjectObject = JObject.Parse(rawJson);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    @"There was an error trying to parse the modlist.json file in " + System.Environment.NewLine
                    + modlistPath + @"." + System.Environment.NewLine + System.Environment.NewLine
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
                List<ModImportData> modList = new List<ModImportData>(modStatus.Properties().Count());
                foreach (JProperty curMOD in modStatus.Properties())

                {
                    bool enabled = (bool)modStatus[curMOD.Name]?["bEnabled"];

                    ModImportData newImportData = new ModImportData();
                    newImportData.ModFolder = curMOD.Name;
                    newImportData.Enabled = enabled;

                    modList.Add(newImportData);
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
            this.ModConflictData.Clear();
            ClearGamePaths();
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
                    return string.Compare(PathToDirNameDict[y], PathToDirNameDict[x], StringComparison.OrdinalIgnoreCase);
                }

                return priorityComparison;
            });
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
            float? GetOriginalLoadOrderFromObject(JObject jsonObject)
            {
                // Our saved load order
                if (jsonObject.ContainsKey("locOriginalLoadOrder"))
                {
                    return jsonObject["locOriginalLoadOrder"].Value<float>();
                }
                else if (jsonObject.ContainsKey("lotsOriginalLoadOrder"))
                {
                    // Might have been set by the "MW5-LOTS" mod order manager
                    return jsonObject["lotsOriginalLoadOrder"].Value<float>();
                }
                else if (jsonObject.ContainsKey("defaultLoadOrder"))
                {
                    return jsonObject["defaultLoadOrder"].Value<float>();
                }
                return null;
            }

            JObject TryReadBackupFile(string filePath, ModObject modJsonDataObject)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        string modBackupJson = File.ReadAllText(filePath);
                        JObject modBackupDetailsJ = JObject.Parse(modBackupJson);

                        if (modBackupDetailsJ.ContainsKey("displayName") && modBackupDetailsJ.ContainsKey("version") && modBackupDetailsJ.ContainsKey("buildNumber"))
                        {
                            // Check if the backup file was created from this mod and that the version matches
                            bool isValidBackup =
                                string.Compare(modBackupDetailsJ["displayName"]?.ToString(), modJsonDataObject.displayName, StringComparison.Ordinal) == 0 &&
                                string.Compare(modBackupDetailsJ["version"]?.ToString(), modJsonDataObject.version, StringComparison.Ordinal) == 0 &&
                                string.Compare(modBackupDetailsJ["buildNumber"]?.ToString(), modJsonDataObject.buildNumber.ToString(), StringComparison.Ordinal) == 0;

                            if (isValidBackup)
                            {
                                return modBackupDetailsJ;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Silently fail
                    }
                }
                return null;
            }

            DateTimeOffset? GetFileAge(string modPath)
            {
                string paksPath = Path.Combine(modPath, "Paks");
                string resourcesPath = Path.Combine(modPath, "Resources");

                // Get *.pak files
                var pakFiles = Directory.Exists(paksPath)
                    ? Directory.EnumerateFiles(paksPath, "*.pak", SearchOption.AllDirectories)
                    : Enumerable.Empty<string>();

                // Get *.json files
                var jsonFiles = Directory.Exists(resourcesPath)
                    ? Directory.EnumerateFiles(resourcesPath, "*.json", SearchOption.AllDirectories)
                    : Enumerable.Empty<string>();

                var allFiles = pakFiles.Concat(jsonFiles);

                return allFiles
                    .Select(file => (DateTimeOffset?)DateTime.SpecifyKind(new FileInfo(file).LastWriteTimeUtc, DateTimeKind.Utc))
                    .OrderByDescending(d => d)
                    .FirstOrDefault();
            }

            bool loadModSuccess = false;
            try
            {
                string modJsonFilePath = Path.Combine(modPath, "mod.json");
                if (!File.Exists(modJsonFilePath))
                {
                    return;
                }

                ModData modData = new ModData();
                ModObject modJsonDataObject = null;
                try
                {
                    string modJsonText = File.ReadAllText(modJsonFilePath);
                    JObject modJsonObject = JObject.Parse(modJsonText);

                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    modJsonDataObject = JsonConvert.DeserializeObject<ModObject>(modJsonText, jsonSettings);

                    modData.NewLoadOrder = modJsonDataObject.defaultLoadOrder;
                    modData.IsNewMod = !modJsonObject.ContainsKey("locOriginalLoadOrder");

                    // Now let's be a bit overkill and try our best to find the original order of the mod
                    // Since other load order manager save these load orders very differently (or not all),
                    // try some different methods
                    float? originalLoadOrder = null;

                    // Only try backup files if locOriginalLoadOrder is not present in mod.json
                    if (modData.IsNewMod)
                    {
                        // "MW5 Mod Organizer" backup file
                        // Some mods also accidentally deploy with this file
                        JObject moBackupFile = TryReadBackupFile(Path.Combine(modPath, "backup.json"), modJsonDataObject);
                        if (moBackupFile != null)
                        {
                            originalLoadOrder = GetOriginalLoadOrderFromObject(moBackupFile);
                        }
                        if (!originalLoadOrder.HasValue)
                        {
							// "MW5 Linux Modder" backup file
                            JObject linuxBackupFile = TryReadBackupFile(Path.Combine(modPath, "mod.json.bak"), modJsonDataObject);
                            if (linuxBackupFile != null)
                            {
                                originalLoadOrder = GetOriginalLoadOrderFromObject(linuxBackupFile);
                            }
                        }
                    }
                    // Always fallback to mod.json if not found in backup
                    if (!originalLoadOrder.HasValue)
                    {
                        originalLoadOrder = GetOriginalLoadOrderFromObject(modJsonObject);
                    }

                    modData.OriginalLoadOrder = originalLoadOrder ?? 0f;

                    // Determine mod origin
                    string modDirName = Path.GetFileName(modPath);

                    // Check if this might be a mod from the steam workshop
                    if (IsSteamWorkshopID(modDirName))
                    {
                        // If the mod directory name matches the store id, we can be pretty certain.
                        // There are mods however, that have this field incorrectly filled
                        if (string.Equals(modDirName, modJsonDataObject.steamPublishedFileId.ToString(), StringComparison.Ordinal))
                        {
                            modData.Origin = ModData.ModOrigin.Steam;
                        }

                        // if this looks like a steam id and the mod is stored in the steam mods directory
                        // it's certain that this is a steam mod
                        if (modData.Origin == ModData.ModOrigin.Unknown)
                        {
                            if (modPath.StartsWith(ModsPaths[eModPathType.Steam]?.FullPath, StringComparison.OrdinalIgnoreCase))
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
                    if (modData.Origin == ModData.ModOrigin.Unknown &&
                        File.Exists(Path.Combine(modPath, "__folder_managed_by_vortex")))
                    {
                        modData.Origin = ModData.ModOrigin.Nexusmods;
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

                // Calculate pak file size and
                // do basic pak sanity checks. Warn user if something looks off
                string pakDir = Path.Combine(modPath, "Paks");
                if (modJsonDataObject.manifest?.Count > 0 &&
                    (!Directory.Exists(pakDir) || !Directory.EnumerateFiles(pakDir, "*.pak").Any()))
                {
                    TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                    {
                        Text = @"The mod in the path" + Environment.NewLine +
                               modPath + Environment.NewLine +
                               @"might be corrupted." + Environment.NewLine +
                               "The mod has a valid mod.json with a file manifest, but has no Pak game data files associated with it.\r\nThe affected mod might need to be reinstalled.",
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

                // Zero-byte pak check
                bool hasZeroBytePak = Directory.Exists(pakDir) &&
                    Directory.EnumerateFiles(pakDir, "*.pak").Any(f => LocFileUtils.GetFileSize(f) == 0);
                if (hasZeroBytePak)
                {
                    TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
                    {
                        Text = @"The mod in the path" + Environment.NewLine +
                               modPath + Environment.NewLine +
                               @"might be corrupted." + Environment.NewLine +
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

                // Get mod file size
                var skipFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    // We want to skip files that are potentially different between installs of the same mod
                    // to not get different sizes for different users/computers
                    "__folder_managed_by_vortex", "mod.json", "mod.json.bak", "backup.json"
                };
                modData.ModFileSize = Directory.EnumerateFiles(modPath, "*", SearchOption.AllDirectories)
                    .Where(f => !skipFiles.Contains(Path.GetFileName(f)))
                    .Sum(f => LocFileUtils.GetFileSize(f));


                modData.FileAge = GetFileAge(modPath);

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
                    var itemToRemove = ModEnabledList.FirstOrDefault(x =>
                        string.Equals(x.ModPath, modPath, StringComparison.InvariantCultureIgnoreCase));

                    if (itemToRemove != null)
                    {
                        ModEnabledList.Remove(itemToRemove);
                    }
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
            var serializer = new JsonSerializer { Formatting = Formatting.Indented };

            foreach (var entry in ModDetails)
            {
                string modJsonPath = Path.Combine(entry.Key, "mod.json");

                // Make sure the file still exists, in case the mod was deleted in the meantime
                if (!File.Exists(modJsonPath))
                    continue;

                //try
                {
                    string modJsonExisting = File.ReadAllText(modJsonPath);
                    JObject modDetailsNew = JObject.Parse(modJsonExisting);

                    if (!Mods.TryGetValue(entry.Key, out var modData))
                        continue;

                    bool needsUpdate = false;

                    float originalLoadOrder = modData.OriginalLoadOrder;
                    JToken currentOlo = modDetailsNew["locOriginalLoadOrder"];
                    JToken newOlo = float.IsInteger(originalLoadOrder)
                        ? new JValue((int)originalLoadOrder)
                        : new JValue(originalLoadOrder);

                    if (!JToken.DeepEquals(currentOlo, newOlo))
                    {
                        modDetailsNew["locOriginalLoadOrder"] = newOlo;
                        needsUpdate = true;
                    }

                    float newLoadOrder = modData.NewLoadOrder;
                    JToken currentNlo = modDetailsNew["defaultLoadOrder"];
                    JToken newNlo = float.IsInteger(newLoadOrder)
                        ? new JValue((int)newLoadOrder)
                        : new JValue(newLoadOrder);

                    if (!JToken.DeepEquals(currentNlo, newNlo))
                    {
                        modDetailsNew["defaultLoadOrder"] = newNlo;
                        needsUpdate = true;
                    }

                    // Only write if something changed
                    if (needsUpdate)
                    {
                        using (var sw = new StreamWriter(modJsonPath))
                        using (var writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, modDetailsNew);
                        }
                    }
                }
                /*catch (Exception ex)
                {
                    // Log or show error, but continue with other mods
                    Console.WriteLine($"Error saving mod details for {modJsonPath}: {ex.Message}");
                }*/
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

            foreach (var entry in this.ModEnabledList)
            {
                JObject newStatus = new JObject(
                    new JProperty("bEnabled", entry.Enabled)
                );
                modStatusObject.Add(entry.ModFolder, newStatus);
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
            foreach (var entry in ModEnabledList)
            {
                string folderName = entry.ModFolder;

                LastAppliedPresetModData lastAppliedModData = new LastAppliedPresetModData();
                lastAppliedModData.state = entry.Enabled;
                lastAppliedModData.lastLoadOrder = Mods[entry.ModPath].NewLoadOrder;
                lastAppliedModData.version = ModDetails[entry.ModPath].version;
                lastAppliedModData.buildNumber = ModDetails[entry.ModPath].buildNumber;
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

        // Save presets to file
        internal void SavePresets()
        {
            string presetsFilePath = Path.Combine(GetSettingsDirectory(), PresetsFileName);
            string tempFilePath = presetsFilePath + ".tmp";

            try
            {
                string presetJsonString = JsonConvert.SerializeObject(this.Presets, Formatting.Indented);

                // Write to a temporary file first
                File.WriteAllText(tempFilePath, presetJsonString);

                // Replace the original file with the new one (atomic operation)
                File.Copy(tempFilePath, presetsFilePath, overwrite: true);
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"There was an error saving the presets file:\r\n\r\n{ex.Message}",
                    "Presets File Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Load presets file
        public void LoadPresets()
        {
            // Load the presets file from the settings directory
            string presetsFilePath = Path.Combine(GetSettingsDirectory(), PresetsFileName);

            if (!File.Exists(presetsFilePath))
                return;

            try
            {
                string json = File.ReadAllText(presetsFilePath);
                var loadedPresets = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                if (loadedPresets != null)
                {
                    Presets = loadedPresets;
                }
                else
                {
                    MessageBox.Show(
                        "The presets file is empty or invalid.",
                        "Presets File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"There was an error decoding the presets file:\r\n\r\n{ex.Message}",
                    "Presets File Decoding Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        //Used to update the override data when a new item is added or removed to/from the mod list instead of checking all items agains each other again.
        public void UpdateNewModOverrideData(ModItem newModItem)
        {
            string modAPath = newModItem.FolderName;

            if (!newModItem.Enabled)
            {
                ModConflictData.Remove(modAPath);

                foreach (string key in ModConflictData.Keys)
                {
                    var conflictData = ModConflictData[key];
                    conflictData.overriddenBy.Remove(modAPath);
                    conflictData.overrides.Remove(modAPath);

                    if (conflictData.overrides.Count == 0)
                        conflictData.isOverriding = false;

                    if (conflictData.overriddenBy.Count == 0)
                        conflictData.isOverridden = false;
                }
            }
            else
            {
                if (!ModConflictData.TryGetValue(modAPath, out ModConflictData conflictDataA))
                    {
                    conflictDataA = new ModConflictData
                    {
                        modPath = modAPath,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                    ModConflictData[modAPath] = conflictDataA;
                }

                // Check each mod for changes
                foreach (ModItem item in ModItemList.Instance.ModList)
                {
                    string modBPath = item.FolderName;

                    if (modAPath == modBPath)
                        continue;

                    if (!item.Enabled)
                        continue;

                    if (!ModConflictData.TryGetValue(modBPath, out ModConflictData conflictDataB))
                    {
                        conflictDataB = new ModConflictData
                        {
                            modPath = modBPath,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                        ModConflictData[modBPath] = conflictDataB;
                    }
                    RecomputeModConflictData(newModItem, item, conflictDataA, conflictDataB);
                }
            }

            MainForm.Instance.ColorizeListViewItems();
        }

        // Compares the manifests of both mods to find intersecting files.
        public void RecomputeModConflictData(ModItem listItemA, ModItem listItemB, ModConflictData conflictDataA, ModConflictData conflictDataB)
        {
            if (listItemA == listItemB)
                return;

            string modAPath = listItemA.FolderName;
            string modBPath = listItemB.FolderName;

            // Retrieve current load orders
            float loadOrderA = Mods[listItemA.Path].NewLoadOrder;
            float loadOrderB = Mods[listItemB.Path].NewLoadOrder;

            // Retrieve manifests for both mods
            if (!DirNameToPathDict.TryGetValue(modAPath, out var modAFullPath) ||
                !DirNameToPathDict.TryGetValue(modBPath, out var modBFullPath))
                return;

            var manifestA = ModDetails.TryGetValue(modAFullPath, out var modObjA) ? modObjA.manifest : null;
            var manifestB = ModDetails.TryGetValue(modBFullPath, out var modObjB) ? modObjB.manifest : null;

            if (manifestA == null || manifestB == null)
                return;

            // Find intersecting files (case-insensitive)
            var intersect = manifestA.Intersect(manifestB, StringComparer.OrdinalIgnoreCase).ToList();
            if (intersect.Count == 0)
                return;

            // Determine which mod overrides the other
            bool aOverridesB = loadOrderA > loadOrderB ||
                       (loadOrderA == loadOrderB &&
                        string.Compare(modAPath, modBPath, StringComparison.InvariantCultureIgnoreCase) > 0);

            if (aOverridesB)
            {
                // A overrides B
                if (conflictDataA.modPath != modBPath)
                {
                    conflictDataA.isOverriding = true;
                    conflictDataA.overrides[modBPath] = intersect;
                }
                if (conflictDataB.modPath != modAPath)
                {
                    conflictDataB.isOverridden = true;
                    conflictDataB.overriddenBy[modAPath] = intersect;
                }
            }
            else
            {
                // B overrides A
                if (conflictDataA.modPath != modBPath)
                {
                    conflictDataA.isOverridden = true;
                    conflictDataA.overriddenBy[modBPath] = intersect;
                }
                if (conflictDataB.modPath != modAPath)
                {
                    conflictDataB.isOverriding = true;
                    conflictDataB.overrides[modAPath] = intersect;
                }
            }

            ModConflictData[modAPath] = conflictDataA;
            ModConflictData[modBPath] = conflictDataB;
        }

        public void RecomputeOverridingData()
        {
            ModConflictData.Clear();

            // Iterate over all enabled mods to compute their override relationships
            foreach (ModItem itemA in ModItemList.Instance.ModList)
            {
                if (!itemA.Enabled)
                    continue;

                string modAPath = itemA.FolderName;

                // Ensure an entry exists for modA in the conflict data dictionary
                if (!ModConflictData.TryGetValue(modAPath, out ModConflictData conflictDataA))
                {
                    conflictDataA = new ModConflictData
                    {
                        modPath = modAPath,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                    ModConflictData[modAPath] = conflictDataA;
                }

                // Compare modA with every other enabled mod to determine conflicts
                foreach (ModItem itemB in ModItemList.Instance.ModList)
                {
                    string modBPath = itemB.FolderName;

                    if (modAPath == modBPath)
                        continue;

                    if (!itemB.Enabled)
                        continue;

                    // Avoid redundant comparisons: if modA and modB have already been compared, skip
                    if (
                        conflictDataA.overriddenBy.ContainsKey(modBPath) ||
                        conflictDataA.overrides.ContainsKey(modBPath)
                        )
                    {
                        continue;
                    }

                    // If modB already has conflict data, check if this mod pair was already compared
                    if (!ModConflictData.TryGetValue(modBPath, out ModConflictData conflictDataB))
                    {
                        conflictDataB = new ModConflictData
                        {
                            modPath = modBPath,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                        ModConflictData[modBPath] = conflictDataB;
                    }
                    else
                    {
                        if (
                            conflictDataB.overriddenBy.ContainsKey(modAPath) ||
                            conflictDataB.overrides.ContainsKey(modAPath)
                            )
                        {
                            // This mod pair has already been processed
                            continue;
                        }
                    }
                    RecomputeModConflictData(itemA, itemB, conflictDataA, conflictDataB);
                }
            }

            MainForm.Instance.ColorizeListViewItems();
        }
    }
}
