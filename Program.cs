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

        public enum GamePlatformEnum
        {
            None,
            Epic,
            Gog,
            Steam,
            WindowsStore
        }

        public GamePlatformEnum GamePlatform = GamePlatformEnum.None;
        public string InstallPath = "";
        public string[] BasePath = new string[2];
        public ProgramData ProgramData = new ProgramData();

        public JObject parent;
        public List<string> Directories = new List<string>();
        public Dictionary<string, string> DirectoryToPathDict = new Dictionary<string, string>();
        public Dictionary<string, string> PathToDirectoryDict = new Dictionary<string, string>();

        // Mod data as loaded from the mods' mod.json file
        public Dictionary<string, ModObject> ModDetails = new Dictionary<string, ModObject>();
        public Dictionary<string, bool> ModList = new Dictionary<string, bool>();
        public Dictionary<string, OverridingData> OverrridingData = new Dictionary<string, OverridingData>();
        public Dictionary<string, string> Presets = new Dictionary<string, string>();

        public struct ModData
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

            public ModData()
            {
            }
        }

        public Dictionary<string, ModData> Mods = new Dictionary<string, ModData>();

        public bool CreatedModlist = false;

        public bool InterruptSearch = false;

        public string rawJson;

        /// <summary>
        /// Starts suquence to load all mods from folders, loads modlist, combines modlist with found folders structure
        /// and loads details of each found mod.
        /// </summary>
        public void LoadFromFiles()
        {
            //Check if the Mods directory exits:
            CheckModDirectories();
            //find all mod directories and parse them into just folder names:
            ParseDirectories();
            //parse modlist.json
            ModListParser();
            //Combine so we have all mods in the ModList Dict for easy later use and writing to JObject
            CombineDirModList();
            //Load each mods mod.json and store in Dict.
            LoadModDetails();
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
            //Combine so we have all mods in the ModList Dict for easy later use and writing to JObject
            CombineDirModList();
            //Load each mods mod.json and store in Dict.
            LoadModDetails();
        }

        /// <summary>
        /// Checks for all items in the modlist if they have a possible folder on system they can point to.
        /// If not removes them from the modlist and imforms user.
        /// </summary>
        private void CheckModDirPresent()
        {
            List<string> MissingModDirs = new List<string>();
            foreach (string key in this.ModList.Keys)
            {
                if (Utils.StringNullEmptyOrWhiteSpace(key))
                {
                    ModList.Remove(key);
                    continue;
                }
                //If the folder that this mod needs is not present warn user and remove.
                if (!DirectoryToPathDict.ContainsKey(key))
                {
                    MissingModDirs.Add(key);
                }
            }
            foreach (string key in MissingModDirs)
            {
                this.ModList.Remove(key);
            }
            if (MissingModDirs.Count > 0)
            {
                string message = "ERROR Mods folder not found for the following mods:\n"
                    + string.Join("\n", MissingModDirs)
                    + "\nThese mods will skipped.";
                string caption = "ERROR Finding Mod Directories";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
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

        //TODO Write summary
        /// <summary>
        /// Checks if the set mods directory exists, if not creates one.
        /// </summary>
        /// <returns></returns>
        public void CheckModDirectories()
        {
            CheckMainModDirectory();
            CheckSteamModDirectory();
        }

        private void CheckSteamModDirectory()
        {
            if (Utils.StringNullEmptyOrWhiteSpace(this.BasePath[1]))
            {
                return;
            }
            if (Directory.Exists(this.BasePath[1]))
            {
                return;
            }
            string message = "ERROR Steam Mods folder does not exist in : " + this.BasePath[1] + "\r\nDo you want to create it?";
            string caption = "ERROR Loading";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult Result = MessageBox.Show(message, caption, buttons);
            if (Result == DialogResult.Yes)
            {
                Directory.CreateDirectory(BasePath[1]);
            }
        }

        private void CheckMainModDirectory()
        {
            if (Utils.StringNullEmptyOrWhiteSpace(this.BasePath[0]))
            {
                return;
            }
            if (Directory.Exists(this.BasePath[0]))
            {
                return;
            }
            string message = "ERROR Mods folder does not exits in : " + this.BasePath[0] + "\r\nDo you want to create it?";
            string caption = "ERROR Loading";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult Result = MessageBox.Show(message, caption, buttons);
            if (Result == DialogResult.Yes)
            {
                Directory.CreateDirectory(BasePath[0]);
            }
        }

        //Try and load data from previous sessions
        public bool TryLoadProgramData()
        {
            //Load install dir from previous session:
            string appDataDir = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string settingsDir = Path.Combine(appDataDir, @"MW5LoadOrderManager");
            if (!File.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            try
            {
                string json = File.ReadAllText(settingsDir + @"\ProgramData.json");
                this.ProgramData = JsonConvert.DeserializeObject<ProgramData>(json);

                Console.WriteLine("Finshed loading ProgramData.json:"
                    + " Platform: " + this.ProgramData.platform
                    + " Version: " + this.ProgramData.version
                    + " Installdir: " + this.ProgramData.ModPaths);

                if (!Utils.StringNullEmptyOrWhiteSpace(this.ProgramData.ModPaths[0]))
                {
                    BasePath[0] = ProgramData.ModPaths[0];
                }
                if (!Utils.StringNullEmptyOrWhiteSpace(ProgramData.ModPaths[1]))
                {
                    BasePath[1] = ProgramData.ModPaths[1];
                }
                if (!Utils.StringNullEmptyOrWhiteSpace(ProgramData.platform))
                {
                    if (!Enum.TryParse(ProgramData.platform, out GamePlatformEnum platform))
                    {
                        platform = GamePlatformEnum.None;
                    }

                    GamePlatform = platform;
                }

                if (!Utils.StringNullEmptyOrWhiteSpace(ProgramData.InstallPath))
                {
                    InstallPath = ProgramData.InstallPath;
                }
                if (ProgramData.version > 0)
                {
                    Version = ProgramData.version;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Something went wrong while loading ProgramData.json");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            if (this.BasePath[0] != null && this.BasePath[0] != "")
                return true;
            return false;
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
            this.Directories.Clear();

            //Check if basepath is there
            if (BasePath == null)
                return;

            HandleMainModDirectories();
            HandleSteamModDirectories();
            AddDirectoryPathsToDict();
        }

        private void AddDirectoryPathsToDict()
        {
            for (int i = 0; i < Directories.Count; i++)
            {
                string directory = this.Directories[i];
                Directories[i] = directory;

                //We wanna keep a dict of the directory name pointing to its path because later on we want to look up the directory
                //path based on just a folder name from the mods.json.
                string[] temp = directory.Split('\\');
                string directoryName = temp[temp.Length - 1];
                this.DirectoryToPathDict[directoryName] = directory;
                this.PathToDirectoryDict[directory] = directoryName;
            }
        }

        private void HandleMainModDirectories()
        {
            if (Utils.StringNullEmptyOrWhiteSpace(BasePath[0]))
            {
                return;
            }
            this.Directories.AddRange(Directory.GetDirectories(BasePath[0]));
        }

        private void HandleSteamModDirectories()
        {
            if (Utils.StringNullEmptyOrWhiteSpace(BasePath[1]))
            {
                return;
            }
            this.Directories.AddRange(Directory.GetDirectories(BasePath[1]));
        }

        public void SaveSettings()
        {
            string settingsDir = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MW5LoadOrderManager";

            ProgramData.InstallPath = this.InstallPath;
            this.ProgramData.ModPaths = this.BasePath;
            this.ProgramData.platform = this.GamePlatform.ToString();
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            using (StreamWriter sw = new StreamWriter(settingsDir + @"\ProgramData.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this.ProgramData);
            }
        }

        public void ModListParser()
        {
            try
            {
                this.rawJson = File.ReadAllText(BasePath[0] + @"\modlist.json");
                this.parent = JObject.Parse(rawJson);
            }
            catch (Exception e)
            {
                string message = "ERROR loading modlist.json in : " + this.BasePath[0] + ". It will be created after locating possible mod directories.";
                string caption = "ERROR Loading";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult Result = MessageBox.Show(message, caption, buttons);
                this.CreatedModlist = true;
                return;
            }
            foreach (JProperty mod in this.parent.Value<JObject>("modStatus").Properties())
            {
                bool enabled = (bool)this.parent["modStatus"][mod.Name]["bEnabled"];
                if (this.DirectoryToPathDict.TryGetValue(mod.Name, out string modDir))
                {
                    this.ModList.Add(modDir, enabled);
                }
            }
        }

        public void SaveToFiles()
        {
            UpdateJObject();
            SaveModDetails();
            SaveModListJson();
        }

        //TODO Fix
        public void ClearAll()
        {
            this.Mods.Clear();
            this.ModDetails = new Dictionary<string, ModObject>();
            this.ModList = new Dictionary<string, bool>();
            this.DirectoryToPathDict = new Dictionary<string, string>();
            this.OverrridingData = new Dictionary<string, OverridingData>();
            this.BasePath = new string[2] { "", "" };
        }

        //Check if the mod dir is already present in data loaded from modlist.json, if not add it.
        private void CombineDirModList()
        {
            foreach (string modDir in this.Directories)
            {
                if (this.ModList.ContainsKey(modDir))
                    continue;

                ModList[modDir] = false;
            }
            //Turns out there are sometimes "ghost" entries in the modlist.json for witch there are no directories left, lets remove those.
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, bool> entry in this.ModList)
            {
                if (this.Directories.Contains<string>(entry.Key))
                    continue;
                toRemove.Add(entry.Key);
            }
            foreach (string key in toRemove)
            {
                this.ModList.Remove(key);
            }
            if (this.CreatedModlist)
            {
                UpdateJObject();
                SaveModListJson();
            }
        }

        private void LoadModDetails()
        {
            foreach (string modDir in this.Directories)
            {
                try
                {
                    string modJsonFilePath = modDir + @"\mod.json";
                    string modJson = File.ReadAllText(modJsonFilePath);
                    JObject modDetailsJ = JObject.Parse(modJson);

                    ModObject modDetails = modDetailsJ.ToObject<ModObject>();

                    this.ModDetails.Add(modDir, modDetails);

                    ModData modData = new ModData();

                    if (modDetailsJ.ContainsKey("lomOriginalLoadOrder"))
                    {
                        modData.OriginalLoadOrder = modDetails.lomOriginalLoadOrder;
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

                    if (MainWindow.MainForm.logic.GamePlatform == GamePlatformEnum.Steam)
                    {
                        if (modDir.StartsWith(MainWindow.MainForm.logic.BasePath[1]))
                        {
                            modData.Origin = ModData.ModOrigin.Steam;
                        }
                    }

                    if (modData.Origin == ModData.ModOrigin.Unknown)
                    {
                        // Check for vortex (nexus mods) manager hardlinks
                        List<string> hardlinks = HardlinkUtils.HardLinkHelper.GetHardLinks(modJsonFilePath);
                        if (hardlinks.Count > 0)
                        {
                            foreach (string hardlinkPath in hardlinks)
                            {
                                // Looking for part of a path like C:\\Users\\XYZ\\AppData\\Roaming\\Vortex\\mechwarrior5mercenaries\\mods\\Advanced Zoom-412-1-2-6-1679946838\\advanced_zoom\\mod.json
                                bool foundMatch = false;
                                try {
                                    Regex regexObj = new Regex(@"\\[^\\]{2,}?-([\d]+)-[\d-]+-[\d]{10}\\", RegexOptions.Multiline);
                                    Match regexMatch = regexObj.Match(hardlinkPath);
                                    if (regexMatch.Success)
                                    {
                                        modData.NexusModsId = regexMatch.Groups[1].Value;
                                        foundMatch = true;
                                    }

                                } catch (ArgumentException ex) {
                                    // Syntax error in the regular expression
                                }

                                if (foundMatch)
                                {
                                    modData.Origin = ModData.ModOrigin.Nexusmods;
                                    break;
                                }
                            }
                        }
                    }
                    
                    if (modData.Origin == ModData.ModOrigin.Unknown)
                    {
                        if (File.Exists(modDir + @"\__folder_managed_by_vortex"))
                        {
                            modData.Origin = ModData.ModOrigin.Nexusmods;
                        }
                    }

                    this.Mods.Add(modDir, modData);
                }
                catch (Exception e)
                {
                    string message = "ERROR loading mod.json in : " + modDir +
                        " folder will be skipped. " +
                        " If this is not a mod folder you can ignore ths message.";
                    string caption = "ERROR Loading";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);

                    if (ModList.ContainsKey(modDir))
                    {
                        ModList.Remove(modDir);
                    }
                    if (ModDetails.ContainsKey(modDir))
                    {
                        ModDetails.Remove(modDir);
                    }
                }
            }
        }

        public void SaveModDetails()
        {
            foreach (KeyValuePair<string, ModObject> entry in this.ModDetails)
            {
                string modJsonPath = entry.Key + @"\mod.json";

                string modJsonExisting = File.ReadAllText(modJsonPath);
                JObject modDetailsNew = JObject.Parse(modJsonExisting);

                //JObject modDetailsUpdate = JObject.FromObject(entry.Value);
                modDetailsNew["lomOriginalLoadOrder"] = Mods[entry.Key].OriginalLoadOrder;
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

        public void UpdateJObject()
        {
            if (this.parent == null)
            {
                this.parent = new JObject();
                this.parent.Add("modStatus", JObject.Parse(@"{}"));
            }
            this.parent.Value<JObject>("modStatus").RemoveAll();
            foreach (KeyValuePair<string, bool> entry in this.ModList)
            {
                string[] temp = entry.Key.Split('\\');
                string modFolderName = temp[temp.Length - 1];
                AddModToJObject(modFolderName, entry.Value);
            }
        }

        public void SaveModListJson()
        {
            string jsonString = this.parent.ToString();
            StreamWriter sw = File.CreateText(BasePath[0] + @"\modlist.json");
            sw.WriteLine(jsonString);
            sw.Flush();
            sw.Close();
        }

        public void AddModToJObject(string ModName, bool status)
        {
            //ugly but I'm lazy today
            if (status)
            {
                (this.parent["modStatus"] as JObject).Add(ModName, JObject.Parse(@"{""bEnabled"": true}"));
            }
            else
            {
                (this.parent["modStatus"] as JObject).Add(ModName, JObject.Parse(@"{""bEnabled"": false}"));
            }
        }

        public void SetModInJObject(string ModName, bool status)
        {
            this.parent["modStatus"][ModName]["bEnabled"] = status;
        }

        #region pack mods to zip

        public void ThreadProc()
        {
            //Get parent dir
            string parent = Directory.GetParent(this.BasePath[0]).ToString();
            //Check if Mods.zip allready exists delete it if so, we need to do this else the ZipFile lib will error.
            if (File.Exists(parent + "\\Mods.zip"))
            {
                File.Delete(parent + "\\Mods.zip");
            }
            ZipFile.CreateFromDirectory(this.BasePath[0], parent + "\\Mods.zip", CompressionLevel.Fastest, false);
        }

        public void PackModsToZip(BackgroundWorker worker, DoWorkEventArgs e)
        {
            //Console.WriteLine("Starting zip compression");
            string parent = Directory.GetParent(this.BasePath[0]).ToString();

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
                    this.OverrridingData[modA].isOverriden = false;
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
                    this.OverrridingData[modB].isOverriden = false;
            }
            //Console.WriteLine("ResetOverrdingBetweenMods modA: " + modA + " " + this.OverrridingData[modA].isOverriding + " " + this.OverrridingData[modA].isOverriden);
            //Console.WriteLine("ResetOverrdingBetweenMods modB: " + modB + " " + this.OverrridingData[modB].isOverriding + " " + this.OverrridingData[modB].isOverriden);
        }

        //Save presets from memory to file for use in next session.
        internal void SavePresets()
        {
            string JsonFile = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MW5LoadOrderManager\presets.json";
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
            string JsonFile = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MW5LoadOrderManager\presets.json";
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
                MessageBox.Show(message, caption, buttons);
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
                        OverrridingData[key].isOverriden = false;
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
                    B.isOverriden = true;
                    B.overriddenBy[modA] = intersect;
                }
            }
            else
            {
                if (!(A.mod == modB))
                {
                    A.isOverriden = true;
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
            foreach (ModListViewItem item in items)
            {
                string modName = item.SubItems[MainWindow.MainForm.folderHeader.Index].Text;

                //marked for removal so don't color.
                if (item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor == Color.Red)
                {
                    continue;
                }

                if (item.Checked)
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].Font = new Font(MainWindow.MainForm.modsListView.Font, MainWindow.MainForm.modsListView.Font.Style | FontStyle.Bold);  
                }
                else
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].Font = new Font(MainWindow.MainForm.modsListView.Font, MainWindow.MainForm.modsListView.Font.Style);  
                }

                foreach (ListViewItem.ListViewSubItem curItem in item.SubItems)
                {
                    curItem.ForeColor = item.Checked ? Color.Black : Color.Gray;
                }

                if (!item.Checked)
                {
                    continue;
                }

                ////Console.WriteLine("Coloring mod: " + mod);
                if (!this.OverrridingData.ContainsKey(modName))
                {
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = Color.Black;
                    ////Console.WriteLine("Black");

                    continue;
                }
                OverridingData A = OverrridingData[modName];
                if (A.isOverriden)
                {
                    ////Console.WriteLine("OrangeRed");
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = Color.OrangeRed;
                }
                if (A.isOverriding)
                {
                    ////Console.WriteLine("Green");
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = Color.Green;
                }
                if (A.isOverriding && A.isOverriden)
                {
                    ////Console.WriteLine("Orange");
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = Color.Orange;
                }
                if (!A.isOverriding && !A.isOverriden)
                {
                    ////Console.WriteLine("Black");
                    item.SubItems[MainWindow.MainForm.displayHeader.Index].ForeColor = Color.Black;
                }
            }
        }

        //Monitor the size of a given zip file
        public void MonitorZipSize(BackgroundWorker worker, DoWorkEventArgs e)
        {
            string zipFile = Directory.GetParent(this.BasePath[0]).ToString() + "\\Mods.zip";
            long folderSize = Utils.DirSize(new DirectoryInfo(BasePath[0]));
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