using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using static MW5_Mod_Manager.ModsManager;
using File = System.IO.File;
using ListView = System.Windows.Forms.ListView;
using System.Net.Http;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class MainForm : Form
    {
        static public MainForm Instance;

        enum eFilterMode
        {
            None,
            ItemFilter,
            ItemHighlight
        }

        eFilterMode FilterMode = eFilterMode.None;
        public List<ListViewItem> ModListData = new List<ListViewItem>();
        private bool MovingItem = false;
        internal bool JustPacking = true;
        string OnlineUpdateUrl = LocConstants.UrlNexusmods;

        static Color HighlightColor = Color.FromArgb(200, 253, 213);

        public bool LoadingAndFilling { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
        }


        public string GetVersion()
        {
            Version versionInfo = typeof(MainForm).GetTypeInfo().Assembly.GetName().Version;
            return versionInfo.Major.ToString() + @"." + versionInfo.Minor.ToString();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.MainIcon;

            RefreshAll();
            /*if (ModsManager.Instance.TryLoadProgramSettings())
            {
                ModsManager.Instance.ParseDirectories();
                ModsManager.Instance.ReloadModData();
                var modList = ModsManager.Instance.LoadModList();
                ModsManager.Instance.ProcessModFolderList(ref modList);
                LoadAndFill(modList, false);
                ModsManager.Instance.LoadLastAppliedPresetData();
            }
            this.LoadPresets();
            this.SetVersionAndPlatform();*/

            this.Text += @" " + GetVersion();

            modsListView.SetDoubleBuffered();

            /*rotatingLabelLowPriority.ForeColor = MainLogic.LowPriorityColor;
            rotatingLabelHighPriority.ForeColor = MainLogic.HighPriorityColor;*/

            panelColorOverridden.BackColor = ModsManager.OverriddenColor;
            panelColorOverriding.BackColor = ModsManager.OverridingColor;
            panelColorOverridingOverridden.BackColor = ModsManager.OverriddenOveridingColor;

            CheckForNewVersion();
            UpdatePriorityLabels();
        }

        private void ProcessUpdateCheckData(string updateJson)
        {
            try
            {
                JObject updateData = JObject.Parse(updateJson);

                if (!updateData.ContainsKey("version"))
                    return;

                string onlineVersion = updateData["version"].ToString();

                if (Utils.CompareVersionStrings(onlineVersion, GetVersion()) == 1)
                {
                    // New version available
                    toolStripStatusLabelUpdate.Visible = true;
                    toolStripStatusLabelUpdate.Text = "A new version of Load Order Configurator is available. Click here";
                }

                if (!updateData.ContainsKey("updateUrl"))
                    return;

                OnlineUpdateUrl = updateData["updateUrl"].ToString();
            }
            catch
            {

            }
        }

        private async void CheckForNewVersion()
        {
            string jsonData = string.Empty;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    jsonData = await httpClient.GetStringAsync(LocConstants.UrlUpdateCheck);
                }
            }
            catch (Exception ex)
            {
                return;
            }

            Instance.Invoke(new Action(() =>
            {
                ProcessUpdateCheckData(jsonData);
            }));
        }

        private const string HighPrioTooltip = "Mods are loaded later and override mods that were loaded earlier.";
        private const string LowPrioTooltip = "Mods are loaded earlier and may get overriden by mods loading after them.";

        public void UpdatePriorityLabels()
        {
            if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh)
            {
                rotatingLabelTop.NewText = "Lowest priority »";
                toolTip1.SetToolTip(rotatingLabelTop, LowPrioTooltip);
                rotatingLabelBottom.NewText = "« Highest priority";
                toolTip1.SetToolTip(rotatingLabelBottom, HighPrioTooltip);
                
            }
            else
            {
                rotatingLabelTop.NewText = "Highest priority »";
                toolTip1.SetToolTip(rotatingLabelTop, HighPrioTooltip);
                rotatingLabelBottom.NewText = "« Lowest priority";
                toolTip1.SetToolTip(rotatingLabelBottom, LowPrioTooltip);
            }
        }

        //When we hover over the manager with a file or folder
        void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        public bool CopyModFromFolder(string path)
        {
            if (Utils.IsSubdirectory(path, ModsManager.Instance.GetMainModPath()))
            {
                MessageBox.Show(@"The source folder is within in the mod directory. Operation aborted.", @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!File.Exists(Path.Combine(path, "mod.json")))
            {
                MessageBox.Show(@"This doesn't seem to be a valid mod directory. Operation aborted.", @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return FileOperationUtils.CopyDirectory(path, ModsManager.Instance.GetMainModPath(), this.Handle);
        }

        public bool ExtractModFromArchive(string filePath)
        {
            ExtractForm extractForm = new ExtractForm();
            extractForm.ArchiveFilePath = filePath;
            extractForm.OutputFolderPath = ModsManager.Instance.GetMainModPath();

            bool result = extractForm.ShowDialog(this) != DialogResult.Cancel;
            extractForm.Dispose();

            return result;
        }

        //When we drop a file or folder on the manager
        void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            //We only support single file drops!
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
            {
                return;
            }
            string filePath = files[0];

            //Lets see what we got here
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(filePath);
            bool isDirectory = ((attr & FileAttributes.Directory) == FileAttributes.Directory);

            if (isDirectory)
            {
                CopyModFromFolder(filePath);
            }
            else
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (fileExtension != "zip" || fileExtension != "rar" || fileExtension != "7z")
                {
                    string message = "Archive format not supported. Supported formats are: .zip, rar, .7z" +
                                     "Please extract the mod first and drag the mod folder into the application.";
                    string caption = "Unsupported Archive Type";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons, MessageBoxIcon.Asterisk);
                    return;
                }

                if (!ExtractModFromArchive(filePath))
                    return;
            }

            RefreshAll();
        }

        private void MoveItemUp(int itemIndex, bool moveToTop)
        {
            ListView.ListViewItemCollection items = modsListView.Items;
            this.MovingItem = true;
            int i = itemIndex;
            if (i < 1)
            {
                this.MovingItem = false;
                return;
            }
            ListViewItem listItem = ModListData[i];
            items.RemoveAt(i);
            ModListData.RemoveAt(i);

            SetModConfigTainted(true);

            if (moveToTop)
            {
                //Move to top
                items.Insert(0, listItem);
                ModListData.Insert(0, listItem);

            }
            else
            {
                //move one up
                items.Insert(i - 1, listItem);
                ModListData.Insert(i - 1, listItem);

            }
            listItem.Selected = true;
            modsListView.EnsureVisible(listItem.Index);

            RecomputeLoadOrdersAndUpdateList();
            ModsManager.Instance.GetOverridingData(this.ModListData);
            modListView_SelectedIndexChanged(null, null);
            this.MovingItem = false;
        }

        private void MoveItemDown(int itemIndex, bool moveToTop)
        {
            ListView.ListViewItemCollection items = modsListView.Items;
            this.MovingItem = true;
            int i = itemIndex;
            if (i > ModListData.Count - 2 || i < 0)
            {
                this.MovingItem = false;
                return;
            }

            ListViewItem listItem = ModListData[i];
            items.RemoveAt(i);
            ModListData.RemoveAt(i);

            SetModConfigTainted(true);

            if (moveToTop)
            {
                //Move to bottom
                items.Insert(ModListData.Count, listItem);
                ModListData.Insert(ModListData.Count, listItem);
            }
            else
            {
                //move one down
                items.Insert(i + 1, listItem);
                ModListData.Insert(i + 1, listItem);
            }
            listItem.Selected = true;
            modsListView.EnsureVisible(listItem.Index);

            RecomputeLoadOrdersAndUpdateList();
            ModsManager.Instance.GetOverridingData(ModListData);
            modListView_SelectedIndexChanged(null, null);
            this.MovingItem = false;
        }

        public void ApplyModSettings()
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            RecomputeLoadOrders();
            ModsManager.Instance.SaveToFiles();
            ModsManager.Instance.SaveLastAppliedModOrder();
            SetModConfigTainted(false);
        }


        //For clearing the entire applications data
        public void ClearAll()
        {
            listBoxOverriding.Items.Clear();
            listBoxOverriddenBy.Items.Clear();
            richTextBoxManifestOverridden.Clear();
            pictureBoxModImage.Visible = false;
            labelModNameOverrides.Text = "";
            ClearModSidePanel();
            this.ModListData.Clear();
            this.modsListView.Items.Clear();
            ModsManager.Instance.ClearAll();
        }

        //For processing internals and updating ui after setting a vendor
        private void SetVersionAndPlatform()
        {
            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Epic:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: Epic Store";
                        this.toolStripButtonStartGame.Enabled = true;
                        break;
                    }
                case eGamePlatform.WindowsStore:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: Microsoft Store/Xbox Game Pass";
                        this.toolStripButtonStartGame.Enabled = true;
                    }
                    break;
                case eGamePlatform.Steam:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: Steam";
                        this.toolStripButtonStartGame.Enabled = true;
                    }
                    break;
                case eGamePlatform.Gog:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: GOG.com";
                        this.toolStripButtonStartGame.Enabled = true;
                    }
                    break;
                default:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: None";
                        this.toolStripButtonStartGame.Enabled = false;
                    }
                    break;
            }

            openModsFolderToolStripMenuItem.Visible = LocSettings.Instance.Data.platform != eGamePlatform.WindowsStore;
            toolStripMenuItemOpenModFolderSteam.Visible = LocSettings.Instance.Data.platform == eGamePlatform.Steam;
            openUserModsFolderToolStripMenuItem.Visible = LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore;
        }

        //Load mod data and fill in the list box...
        public void LoadAndFill(Dictionary<string, bool> desiredMods, bool orderByDesired)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            bool prevLoadingAndFilling = LoadingAndFilling;
            this.LoadingAndFilling = true;

            //try
            {
                //ModsManager.Instance.ModEnabledList = modList;

                ModsManager.Instance.InitModEnabledList();

                List<KeyValuePair<string, bool>> orderedModList;
                // Sort by mechwarrior load order
                if (!orderByDesired)
                {
                    orderedModList = ModsManager.Instance.ModEnabledList.ToList();
                    orderedModList.Sort((x, y) =>
                    {
                        // Compare Original load order
                        int priorityComparison = ModsManager.Instance.ModDetails[y.Key].defaultLoadOrder
                            .CompareTo(ModsManager.Instance.ModDetails[x.Key].defaultLoadOrder);

                        // If Priority is equal, compare Folder name
                        if (priorityComparison == 0)
                        {
                            return ModsManager.Instance.PathToDirNameDict[y.Key].ToString()
                                .CompareTo(ModsManager.Instance.PathToDirNameDict[x.Key]);
                        }

                        return priorityComparison;
                    });
                }
                else
                {
                    orderedModList = ModsManager.Instance.ModEnabledList.ToList();
                    ModUtils.SwapModsToMatchFilter(ref orderedModList, desiredMods.ToList());
                }

				// set all mods to desired enabled states
                foreach (var curDesiredMod in desiredMods)
                {
                    ModsManager.Instance.ModEnabledList[curDesiredMod.Key] = curDesiredMod.Value;
                }

                for (int i = 0; i < orderedModList.Count; i++)
                {
                    bool newState = false;
                    var curModListItem = orderedModList[i];
                    if (desiredMods.ContainsKey(curModListItem.Key))
                    {
                        newState = desiredMods[curModListItem.Key];
                    }

                    orderedModList[i] = new KeyValuePair<string, bool>(curModListItem.Key, newState);
                }

                modsListView.BeginUpdate();
                foreach (KeyValuePair<string, bool> entry in orderedModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
                {
                    if (entry.Equals(new KeyValuePair<string, bool>(null, false)))
                        continue;
                    if (entry.Key == null)
                        continue;

                    AddEntryToListViewAndData(entry);
                }
                ReloadListViewFromData();
                modsListView.EndUpdate();
                ModsManager.Instance.SaveSettings();
            }
            /*catch (Exception e)
            {
                if (currentEntry.Key == null)
                {
                    currentEntry = new KeyValuePair<string, bool>("NULL", false);
                }
                Console.WriteLine(e.StackTrace);
                string message = "There was an error trying to load mod " + currentEntry.Key.ToString() + ".\r\n\r\n" + e.StackTrace;
                string caption = "Error Loading";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }*/
            this.LoadingAndFilling = prevLoadingAndFilling;
            RecomputeLoadOrdersAndUpdateList();
            ModsManager.Instance.GetOverridingData(ModListData);
            UpdateModCountDisplay();
        }

        private void AddEntryToListViewAndData(KeyValuePair<string, bool> entry)
        {
            string modName = entry.Key;
            ListViewItem newItem = new ListViewItem
            {
                UseItemStyleForSubItems = false,
                Checked = entry.Value
            };

            for (int i = 1; i < modsListView.Columns.Count; i++)
            {
                newItem.SubItems.Add("");
            }

            switch (ModsManager.Instance.Mods[entry.Key].Origin)
            {
                case ModsManager.ModData.ModOrigin.Steam:
                    newItem.ImageKey = "Steam";
                    break;
                case ModsManager.ModData.ModOrigin.Nexusmods:
                    newItem.ImageKey = "Nexusmods";
                    break;
                default:
                    newItem.ImageKey = "Folder";
                    break;
            }

            string versionString = (ModsManager.Instance.ModDetails[entry.Key].version + " (" +
                                    ModsManager.Instance.ModDetails[entry.Key].buildNumber.ToString() + ")").Trim();

            newItem.SubItems[displayHeader.Index].Text = ModsManager.Instance.ModDetails[entry.Key].displayName;
            newItem.SubItems[folderHeader.Index].Text = ModsManager.Instance.PathToDirNameDict[modName];
            newItem.SubItems[authorHeader.Index].Text = ModsManager.Instance.ModDetails[entry.Key].author;
            newItem.SubItems[versionHeader.Index].Text = versionString;
            newItem.SubItems[currentLoadOrderHeader.Index].Text = ModsManager.Instance.ModDetails[entry.Key].defaultLoadOrder.ToString();
            newItem.SubItems[originalLoadOrderHeader.Index].Text = ModsManager.Instance.Mods[entry.Key].OriginalLoadOrder.ToString();
            newItem.SubItems[fileSizeHeader.Index].Text = Utils.BytesToHumanReadableString(ModsManager.Instance.Mods[entry.Key].ModFileSize);

            newItem.Tag = entry.Key;
            ModListData.Add(newItem);
        }

        //Fill list view from internal list of data.
        private void ReloadListViewFromData()
        {
            modsListView.BeginUpdate();
            modsListView.Items.Clear();
            bool prevLoadingAndFilling = LoadingAndFilling;
            LoadingAndFilling = true;
            modsListView.Items.AddRange(ModListData.ToArray());
            LoadingAndFilling = prevLoadingAndFilling;
            modsListView.EndUpdate();
        }

        //gets the index of the selected item in listview1.
        private int SelectedItemIndex()
        {
            int index = -1;
            var SelectedItems = modsListView.SelectedItems;
            if (SelectedItems.Count == 0)
            {
                return index;
            }

            index = modsListView.SelectedItems[0].Index;

            if (index < 0)
            {
                return -1;
            }
            return index;
        }

        public void RefreshAll()
        {
            Cursor tempCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            modsListView.BeginUpdate();
            ClearAll();
            if (ModsManager.Instance.TryLoadProgramSettings())
            {
                ModsManager.Instance.ParseDirectories();
                ModsManager.Instance.ReloadModData();
                var modList = ModsManager.Instance.LoadModList();
                ModsManager.Instance.DetermineBestAvailableGameVersion();
                ModsManager.Instance.ProcessModFolderList(ref modList, true);
                LoadAndFill(modList, false);

                FilterTextChanged();
                ModsManager.Instance.GetOverridingData(ModListData);

                ModsManager.Instance.LoadLastAppliedPresetData();
                ModsManager.Instance.CheckPrevAppliedPresetDataAgainstCurrentMods();
            }
            LoadPresets();
            SetVersionAndPlatform();
            SetModConfigTainted(false);
            modsListView.EndUpdate();
            Cursor.Current = tempCursor;
        }

        //Saves current load order to preset.
        public void SavePreset(string name)
        {
            Dictionary<string, bool> NoPathModlist = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> entry in ModsManager.Instance.ModEnabledList)
            {
                string folderName = ModsManager.Instance.PathToDirNameDict[entry.Key];
                NoPathModlist[folderName] = entry.Value;
            }
            ModsManager.Instance.Presets[name] = JsonConvert.SerializeObject(NoPathModlist, Formatting.Indented);
            ModsManager.Instance.SavePresets();
        }

        //Sets up the load order from a preset.
        private void LoadPreset(string name)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            string JsonString = ModsManager.Instance.Presets[name];
            Dictionary<string, bool> temp;
            try
            {
                temp = JsonConvert.DeserializeObject<Dictionary<string, bool>>(JsonString);
            }
            catch (Exception Ex)
            {
                string message = "There was an error in decoding the load order string.";
                string caption = "Load Order Decoding Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                return;
            }

            temp.ReverseIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh);

            modsListView.BeginUpdate();
            this.modsListView.Items.Clear();
            this.ModListData.Clear();
            ModsManager.Instance.ModDetails = new Dictionary<string, ModObject>();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.Mods.Clear();

            ModsManager.Instance.ParseDirectories();
            ModsManager.Instance.ReloadModData();
            ModsManager.Instance.ProcessModFolderList(ref temp, true);
            this.LoadAndFill(temp, true);
            FilterTextChanged();
            SetModConfigTainted(true);
            modsListView.EndUpdate();
        }

        //Load all presets from file and fill the listbox.
        private void LoadPresets()
        {
            ModsManager.Instance.LoadPresets();
            RebuildPresetsMenu();
        }
        public void RebuildPresetsMenu()
        {
            // Clear all preset menu items first
            var dropDownItems = MainForm.Instance.presetsToolStripMenuItem.DropDownItems;

            for (int i = dropDownItems.Count - 1; i >= 0; i--)
            {
                ToolStripItem item = dropDownItems[i];
                if (item.Tag != null)
                {
                    dropDownItems.Remove(item);
                }
            }

            int menuIndex = presetsToolStripMenuItem.DropDownItems.IndexOf(toolStripMenuItemLoadPresets);
            foreach (string key in ModsManager.Instance.Presets.Keys)
            {
                menuIndex++;

                string menuItemName = key.Replace("&", "&&");
                ToolStripItem subItem = new ToolStripMenuItem(menuItemName);
                subItem.Tag = key;
                subItem.Click += presetMenuItem_Click;
                presetsToolStripMenuItem.DropDownItems.Insert(menuIndex, subItem);
            }
        }

        private void LaunchGame()
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModsManager.Instance.ModSettingsTainted)
            {
                DialogResult result =
                    MessageBox.Show(
                        @"You have unapplied changes to your mod list." + System.Environment.NewLine + System.Environment.NewLine
                        + "Do you want to apply your changes before starting?",
                        @"Unapplied changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    ApplyModSettings();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Epic:
                    LaunchEpicGame();
                    break;
                case eGamePlatform.Steam:
                    LaunchSteamGame();
                    break;
                case eGamePlatform.Gog:
                    LaunchGogGame();
                    break;
                case eGamePlatform.WindowsStore:
                    LaunchMicrosoftStoreGame();
                    break;
            }
        }

        //Launch game button
        private void buttonStartGame_Click(object sender, EventArgs e)
        {


        }

        #region Launch Game
        private static void LaunchMicrosoftStoreGame()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = @"shell:appsFolder\PiranhaGamesInc.MechWarrior5Mercenaries_skpx0jhaqqap2!9PB86W3JK8Z5",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to launch MechWarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }

        private void LaunchGogGame()
        {
            string gamePath = Path.Combine(LocSettings.Instance.Data.InstallPath, "MechWarrior.exe");
            try
            {
                Process.Start(gamePath);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to launch MechWarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }

        private static void LaunchEpicGame()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = @"com.epicgames.launcher://apps/Hoopoe?action=launch&silent=false",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to make Epic Games Launcher launch Mechwarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }

        private static void LaunchSteamGame()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = @"steam://rungameid/784080",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = @"There was an error while trying to launch Mechwarrior 5 through Steam.";
                string caption = @"Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void SetMoveControlsEnabled(bool enabled)
        {
            moveupToolStripMenuItem.Enabled = enabled;
            movedownToolStripMenuItem.Enabled = enabled;
            contextMenuItemMoveToTop.Enabled = enabled;
            contextMenuItemMoveToBottom.Enabled = enabled;
        }

        private void FilterTextChanged()
        {
            string filtertext = Instance.toolStripTextFilterBox.Text.ToLower();
            if (Utils.StringNullEmptyOrWhiteSpace(filtertext))
            {
                if (this.FilterMode != eFilterMode.None)
                {
                    // end filtering
                    modsListView.BeginUpdate();
                    UnhighlightAllMods();
                    ReloadListViewFromData();
                    modsListView.EndUpdate();
                    SetMoveControlsEnabled(true);
                    this.FilterMode = eFilterMode.None;
                }
            }
            else
            {
                if (!Instance.toolStripButtonFilterToggle.Checked)
                {
                    FilterMode = eFilterMode.ItemHighlight;
                    bool anyUpdated = false;
                    foreach (ListViewItem item in this.ModListData)
                    {
                        if (MatchItemToText(filtertext, item))
                        {
                            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                            {
                                if (subItem.BackColor != HighlightColor)
                                {
                                    if (!anyUpdated)
                                    {
                                        anyUpdated = true;
                                        modsListView.BeginUpdate();
                                    }

                                    subItem.BackColor = HighlightColor;
                                }
                            }
                        }
                        else
                        {
                            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                            {
                                if (subItem.BackColor != SystemColors.Window)
                                {
                                    if (!anyUpdated)
                                    {
                                        anyUpdated = true;
                                        modsListView.BeginUpdate();
                                    }

                                    subItem.BackColor = SystemColors.Window;
                                }

                            }
                        }
                    }
                    if (anyUpdated)
                        modsListView.EndUpdate();
                }
                //We are filtering by selected adding.
                else
                {
                    FilterMode = eFilterMode.ItemFilter;
                    //Clear the list view
                    this.modsListView.Items.Clear();
                    Instance.modsListView.BeginUpdate();
                    UnhighlightAllMods();
                    foreach (ListViewItem item in this.ModListData)
                    {

                        bool prevLoadingAndFilling = LoadingAndFilling;
                        LoadingAndFilling = true;
                        if (MatchItemToText(filtertext, item))
                        {
                            Instance.modsListView.Items.Add(item);
                        }

                        LoadingAndFilling = prevLoadingAndFilling;
                    }
                    Instance.modsListView.EndUpdate();
                }
                //While filtering disable the up/down buttons (tough this should no longer be needed).
                SetMoveControlsEnabled(false);
            }
            toolStripButtonClearFilter.Enabled = toolStripTextFilterBox.Text.Length > 0;
        }

        //Check if given listviewitem can be matched to a string.
        private bool MatchItemToText(string filtertext, ListViewItem item)
        {
            if
                (
                    item.SubItems[displayHeader.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[folderHeader.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[authorHeader.Index].Text.ToLower().Contains(filtertext)
                )
            {
                return true;
            }
            return false;
        }

        private void AppendContentPathToMainfestList(string contentPath)
        {
            if (!string.IsNullOrWhiteSpace(richTextBoxManifestOverridden.Text))
            {
                richTextBoxManifestOverridden.AppendText("\r\n");
            }

            richTextBoxManifestOverridden.SelectionFont = new Font(richTextBoxManifestOverridden.Font, FontStyle.Bold);
            richTextBoxManifestOverridden.AppendText(Path.GetFileName(contentPath));

            richTextBoxManifestOverridden.SelectionFont = richTextBoxManifestOverridden.Font;
            richTextBoxManifestOverridden.AppendText(@" (" + Path.GetDirectoryName(contentPath) + @")");
        }

        //Selected index of mods overriding the currently selected mod has changed.
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool startedListUpdate = false;
            try
            {
                if (FilterMode == eFilterMode.None)
                {
                    startedListUpdate = true;
                    richTextBoxManifestOverridden.Suspend();
                    modsListView.BeginUpdate();
                    UnhighlightAllMods();
                }

                if (listBoxOverriddenBy.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriding.SelectedIndex = -1;
                if (listBoxOverriddenBy.Items.Count == 0 || modsListView.Items.Count == 0)
                    return;

                if (listBoxOverriddenBy.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriddenBy.SelectedItem;

                if (FilterMode == eFilterMode.None)
                {
                    HighlightModInList(selectedMod.ModKey);
                }

                if (startedListUpdate)
                {
                    modsListView.EndUpdate();
                }

                string superMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;

                if (!ModsManager.Instance.OverridingData.ContainsKey(superMod))
                    return;

                OverridingData modData = ModsManager.Instance.OverridingData[superMod];

                if (!modData.overriddenBy.ContainsKey(selectedMod.ModDirName))
                    return;

                foreach (string entry in modData.overriddenBy[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry);
                }
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.Resume();
                    modsListView.EndUpdate();
                }
            }
        }

        //Selected index of mods that are being overriden by the currently selected mod had changed.
        private void listBoxOverriding_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool startedListUpdate = false;
            try
            {
                if (FilterMode == eFilterMode.None)
                {
                    startedListUpdate = true;
                    modsListView.BeginUpdate();
                    richTextBoxManifestOverridden.Suspend();
                    UnhighlightAllMods();
                }

                if (listBoxOverriding.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriddenBy.SelectedIndex = -1;
                if (listBoxOverriding.Items.Count == 0 || modsListView.Items.Count == 0)
                    return;

                if (listBoxOverriding.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriding.SelectedItem;

                if (FilterMode == eFilterMode.None)
                {
                    HighlightModInList(selectedMod.ModKey);
                }

                string superMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;

                if (!ModsManager.Instance.OverridingData.ContainsKey(superMod))
                    return;

                OverridingData modData = ModsManager.Instance.OverridingData[superMod];

                foreach (string entry in modData.overrides[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry);
                }
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.Resume();
                    modsListView.EndUpdate();
                }
            }
        }



        public void ClearModSidePanel()
        {
            labelModNameOverrides.Text = "";
            pictureBoxModImage.Visible = false;
            panelModInfo.Visible = false;
            richTextBoxManifestOverridden.Clear();
            listBoxOverriddenBy.Items.Clear();
            listBoxOverriding.Items.Clear();
        }

        private void modListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FilterMode == eFilterMode.None)
                UnhighlightAllMods();

            if (modsListView.SelectedItems.Count == 0)
            {
                ClearModSidePanel();
                return;
            }

            string SelectedMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;
            string SelectedModDisplayName = modsListView.SelectedItems[0].SubItems[displayHeader.Index].Text;

            if (Utils.StringNullEmptyOrWhiteSpace(SelectedMod) ||
                Utils.StringNullEmptyOrWhiteSpace(SelectedModDisplayName)
               )
            {
                ClearModSidePanel();
                return;
            }

            string modPath = (string)modsListView.SelectedItems[0].Tag;
            ModObject modDetails = ModsManager.Instance.ModDetails[modPath];

            panelModInfo.Visible = true;
            labelModName.Text = SelectedModDisplayName;
            labelModNameOverrides.Text = SelectedModDisplayName;
            labelModAuthor.Text = @"Author: " + modDetails.author;
            linkLabelModAuthorUrl.Text = modDetails.authorURL;
            labelModVersion.Text = @"Version: " + modDetails.version;
            labelModBuildNumber.Text = @"Build: " + modDetails.buildNumber;
            long steamId = modDetails.steamPublishedFileId;
            if (steamId > 0)
            {
                pictureBoxSteamIcon.Visible = true;
                labelSteamId.Visible = true;
                linkLabelSteamId.Visible = true;
                linkLabelSteamId.Text = steamId.ToString();
            }
            else
            {
                pictureBoxSteamIcon.Visible = false;
                labelSteamId.Visible = false;
                linkLabelSteamId.Visible = false;
            }

            string nexusModsId = ModsManager.Instance.Mods[modPath].NexusModsId;
            if (nexusModsId != "")
            {
                pictureBoxNexusmodsIcon.Visible = true;
                labelNexusmods.Visible = true;
                linkLabelNexusmods.Visible = true;
                linkLabelNexusmods.Text = nexusModsId;
            }
            else
            {
                pictureBoxNexusmodsIcon.Visible = false;
                labelNexusmods.Visible = false;
                linkLabelNexusmods.Visible = false;
            }

            richTextBoxModDescription.Text = modDetails.description;

            HandleOverriding(SelectedMod);

            string imagePath = Path.Combine(modPath, "Resources", "Icon128.png");

            bool imageLoadSuccess = false;
            if (File.Exists(imagePath))
            {
                try
                {
                    pictureBoxModImage.Image = Image.FromStream(new MemoryStream(File.ReadAllBytes(imagePath)));
                    imageLoadSuccess = true;
                }
                catch
                {
                    // Fail image load silently
                }
            }
            pictureBoxModImage.Visible = imageLoadSuccess;
        }

        //Handles the showing of overriding data on select
        private void HandleOverriding(string SelectedMod)
        {
            if (ModsManager.Instance.OverridingData.Count == 0)
                return;

            this.listBoxOverriding.Items.Clear();
            this.listBoxOverriddenBy.Items.Clear();
            this.richTextBoxManifestOverridden.Clear();

            //If we select a mod that is not ticked its data is never gotten so will get an error if we don't do this.
            if (!ModsManager.Instance.OverridingData.ContainsKey(SelectedMod))
                return;

            listBoxOverriding.Suspend();
            listBoxOverriddenBy.Suspend();
            OverridingData modData = ModsManager.Instance.OverridingData[SelectedMod];
            foreach (string overriding in modData.overriddenBy.Keys)
            {
                ModListBoxItem modListBoxItem = new ModListBoxItem();
                string modKey = ModsManager.Instance.DirNameToPathDict[overriding];
                modListBoxItem.DisplayName = ModsManager.Instance.ModDetails[modKey].displayName;
                modListBoxItem.ModDirName = overriding;
                modListBoxItem.ModKey = modKey;
                this.listBoxOverriddenBy.Items.Add(modListBoxItem);
            }
            foreach (string overrides in modData.overrides.Keys)
            {
                ModListBoxItem modListBoxItem = new ModListBoxItem();
                string modKey = ModsManager.Instance.DirNameToPathDict[overrides];
                modListBoxItem.DisplayName = ModsManager.Instance.ModDetails[modKey].displayName;
                modListBoxItem.ModDirName = overrides;
                modListBoxItem.ModKey = modKey;
                this.listBoxOverriding.Items.Add(modListBoxItem);
            }
            listBoxOverriding.Resume();
            listBoxOverriddenBy.Resume();
        }

        private void modListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //While we are removing/inserting items this will fire and we dont want that to happen when we move an item.
            if (MovingItem || this.LoadingAndFilling)
            {
                return;
            }

            // set mod enabled state
            ModsManager.Instance.ModEnabledList[e.Item.Tag.ToString()] = e.Item.Checked;

            RecomputeLoadOrdersAndUpdateList();

            ModsManager.Instance.UpdateNewModOverrideData(ModListData, ModListData[e.Item.Index]);
            HandleOverriding(e.Item.SubItems[folderHeader.Index].Text);
            UpdateModCountDisplay();
            SetModConfigTainted(true);
        }

        //Check for mod overrding data
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ModsManager.Instance.GetOverridingData(ModListData);
        }

        private void exportLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportForm exportDialog = new ExportForm();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            exportDialog.ShowDialog(this);
            exportDialog.Dispose();
        }

        private void importLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportForm importDialog = new ImportForm();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (importDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            Dictionary<string, bool> newData = importDialog.ResultData.ReverseIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh);
            ModsManager.Instance.ProcessModFolderList(ref newData, true);
            importDialog.Dispose();

            if (!ModsManager.Instance.GameIsConfigured())
                return;

            modsListView.BeginUpdate();
            //this.ClearAll();
            this.modsListView.Items.Clear();
            this.ModListData.Clear();
            ModsManager.Instance.ModDetails.Clear();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.Mods.Clear();
            ModsManager.Instance.ParseDirectories();
            ModsManager.Instance.ReloadModData();
            ModsManager.Instance.DetermineBestAvailableGameVersion();
            this.LoadAndFill(newData, true);
            FilterTextChanged();
            SetModConfigTainted(true);
            modsListView.EndUpdate();
        }

        private void openModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Program]))
            {
                return;
            }
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.Program],
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Win32Exception win32Exception)
            {
                Console.WriteLine(win32Exception.Message);
                Console.WriteLine(win32Exception.StackTrace);
                string message = "There was an error trying to open the mod directory. The folder does not exist, is not valid or was not set.";
                string caption = "Error Opening Mods Folder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutDialog = new AboutForm();

            aboutDialog.ShowDialog(this);
            aboutDialog.Dispose();
        }

        private void enableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            modsListView.BeginUpdate();

            this.MovingItem = true;
            foreach (ListViewItem item in this.ModListData)
            {
                item.Checked = true;
            }
            this.MovingItem = false;

            foreach (var key in ModsManager.Instance.ModEnabledList.Keys)
            {
                ModsManager.Instance.ModEnabledList[key] = true;
            }

            ModsManager.Instance.GetOverridingData(this.ModListData);
            UpdateModCountDisplay();
            RecomputeLoadOrdersAndUpdateList();
            SetModConfigTainted(true);

            modsListView.EndUpdate();
        }

        private void disableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            modsListView.BeginUpdate();

            this.MovingItem = true;
            foreach (ListViewItem item in this.modsListView.Items)
            {
                item.Checked = false;
            }
            this.MovingItem = false;

            foreach (var key in ModsManager.Instance.ModEnabledList.Keys)
            {
                ModsManager.Instance.ModEnabledList[key] = false;
            }

            ModsManager.Instance.GetOverridingData(ModListData);
            UpdateModCountDisplay();
            RecomputeLoadOrdersAndUpdateList();
            SetModConfigTainted(true);

            modsListView.EndUpdate();
        }

        private void modsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = modsListView.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripMod.Show(Cursor.Position);
                }
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = (string)modsListView.SelectedItems[0].Tag;

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = path,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabelModAuthorUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string modUrl = ModsManager.Instance.ModDetails[modKey].authorURL;
            bool isValidUrl = Utils.IsUrlValid(modUrl);
            if (isValidUrl)
            {
                Process.Start(modUrl);
            }
        }

        private void linkLabelSteamId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string steamUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + ModsManager.Instance.ModDetails[modKey].steamPublishedFileId;
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = steamUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void richTextBoxModDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            bool isValidUrl = Utils.IsUrlValid(e.LinkText);
            if (isValidUrl)
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }

        private void ShowSettingsDialog()
        {
            SettingsForm settingsDialog = new SettingsForm();

            settingsDialog.ShowDialog(this);
            settingsDialog.Dispose();
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void toolStripMenuItemOpenModFolderSteam_Click(object sender, EventArgs e)
        {
            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Steam]))
            {
                return;
            }
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.Steam],
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Win32Exception win32Exception)
            {
                Console.WriteLine(win32Exception.Message);
                Console.WriteLine(win32Exception.StackTrace);
                string message = "While trying to open the mods folder, windows has encountered an error. Your folder does not exist, is not valid or was not set.";
                string caption = "Error Opening Mods Folder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        private void modsListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            modsListView.AllowDrop = true;

            if (FilterMode != eFilterMode.None)
                return;

            MovingItem = true;
            DoDragDrop(e.Item, DragDropEffects.Move);
            MovingItem = false;
        }

        private void modsListView_DragEnter(object sender, DragEventArgs e)
        {
            if (MovingItem) e.Effect = e.AllowedEffect;
        }

        private void modsListView_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint =
                modsListView.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            int targetIndex = modsListView.InsertionMark.NearestIndex(targetPoint);

            // Confirm that the mouse pointer is not over the dragged item.
            if (targetIndex > -1)
            {
                // Determine whether the mouse pointer is to the left or
                // the right of the midpoint of the closest item and set
                // the InsertionMark.AppearsAfterItem property accordingly.
                Rectangle itemBounds = modsListView.GetItemRect(targetIndex);
                if (targetPoint.Y > itemBounds.Top + (itemBounds.Height / 2))
                {
                    modsListView.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    modsListView.InsertionMark.AppearsAfterItem = false;
                }
            }

            // Set the location of the insertion mark. If the mouse is
            // over the dragged item, the targetIndex value is -1 and
            // the insertion mark disappears.
            modsListView.InsertionMark.Index = targetIndex;
        }

        private void modsListView_DragDrop(object sender, DragEventArgs e)
        {
            modsListView.AllowDrop = false;
            // Retrieve the index of the insertion mark;
            int insertIndex = modsListView.InsertionMark.Index;

            // If the insertion mark is not visible, exit the method.
            if (insertIndex == -1)
            {
                return;
            }

            // If the insertion mark is to the right of the item with
            // the corresponding index, increment the target index.
            /*if (modsListView.InsertionMark.AppearsAfterItem) 
            {
                targetIndex++;
            }*/

            // Retrieve the dragged item.
            ListViewItem draggedItem =
                (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            int itemIndex = draggedItem.Index;

            int targetIndex =
                modsListView.InsertionMark.AppearsAfterItem
                    ? insertIndex + 1
                    : insertIndex;

            if (itemIndex != targetIndex)
            {
                modsListView.BeginUpdate();

                ListViewItem newItem = (ListViewItem)draggedItem.Clone();
                ModListData.Insert(targetIndex, newItem);
                modsListView.Items.Insert(targetIndex, newItem);
                newItem.Selected = true;

                ModListData.Remove(draggedItem);
                modsListView.Items.Remove(draggedItem);
                RecomputeLoadOrdersAndUpdateList();
                ModsManager.Instance.GetOverridingData(this.ModListData);

                modListView_SelectedIndexChanged(null, null);
                modsListView.EndUpdate();

                SetModConfigTainted(true);
            }

            modsListView.InsertionMark.Index = -1;
        }

        private void modsListView_DragLeave(object sender, EventArgs e)
        {
            modsListView.InsertionMark.Index = -1;
        }

        private void presetMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem presetMenuItem = sender as ToolStripMenuItem;
            this.LoadPreset(presetMenuItem.Tag.ToString());
        }

        private void savePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModListData.Count == 0)
            {
                MessageBox.Show(@"No configured mods. Nothing to save as preset.", @"No mods", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            PresetSaveForm saveDialog = new PresetSaveForm();

            saveDialog.ShowDialog(this);
            saveDialog.Dispose();
        }

        private void deletePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PresetDeleteForm deleteDialog = new PresetDeleteForm();

            deleteDialog.ShowDialog(this);
            deleteDialog.Dispose();
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string nexusUrl = "https://www.nexusmods.com/mechwarrior5mercenaries/mods/" + ModsManager.Instance.Mods[modKey].NexusModsId;

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = nexusUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        public void SelectModInList(string modKey)
        {
            foreach (ListViewItem modListItem in ModListData)
            {
                if (modListItem.Tag.ToString() == modKey)
                {
                    modListItem.Selected = true;
                    modsListView.EnsureVisible(modListItem.Index);
                    break;
                }
            }
        }

        public void HighlightModInList(string modKey)
        {
            foreach (ListViewItem modListItem in ModListData)
            {
                if (modListItem.Tag.ToString() == modKey)
                {
                    foreach (ListViewItem.ListViewSubItem subItem in modListItem.SubItems)
                    {
                        subItem.BackColor = HighlightColor;
                    }
                    break;
                }
            }
        }

        public void UnhighlightAllMods()
        {
            bool anyUpdated = false;
            foreach (ListViewItem modListItem in ModListData)
            {
                foreach (ListViewItem.ListViewSubItem subItem in modListItem.SubItems)
                {
                    if (subItem.BackColor != SystemColors.Window)
                    {
                        if (!anyUpdated)
                        {
                            anyUpdated = true;
                            this.modsListView.BeginUpdate();
                        }
                        subItem.BackColor = SystemColors.Window;
                    }

                }
            }

            if (anyUpdated)
                this.modsListView.EndUpdate();
        }

        private int GetModCount(bool enabledOnly)
        {
            int count = 0;
            if (enabledOnly)
            {
                foreach (bool curModState in ModsManager.Instance.ModEnabledList.Values)
                {
                    if (curModState) { count++; }
                }
            }
            else
            {
                count = ModsManager.Instance.Mods.Count;
            }

            return count;
        }

        public void RecomputeLoadOrders(bool restoreLoadOrdersOfDisabled = false)
        {
            // If the list is sorted according to MW5's default load order,
            // we can reset everyting to the default load order
            bool isDefaultSorted = IsSortedByDefaultLoadOrder();

            int curLoadOrder = GetModCount(restoreLoadOrdersOfDisabled);

            // Reorder modlist by recreating it...
            Dictionary<string, bool> newModList = new Dictionary<string, bool>();

            foreach (ListViewItem curModListItem in ModListData.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
            {
                string modKey = curModListItem.Tag.ToString();
                bool modEnabled = ModsManager.Instance.ModEnabledList[modKey];
                newModList[modKey] = modEnabled;
                if (!isDefaultSorted && (!restoreLoadOrdersOfDisabled || modEnabled))
                {
                    ModsManager.Instance.ModDetails[modKey].defaultLoadOrder = curLoadOrder;
                    --curLoadOrder;
                }
                else
                {
                    ModsManager.Instance.ModDetails[modKey].defaultLoadOrder = ModsManager.Instance.ModDetails[modKey].locOriginalLoadOrder;
                }
            }

            ModsManager.Instance.ModEnabledList = newModList;
        }

        public void RecomputeLoadOrdersAndUpdateList()
        {
            RecomputeLoadOrders();

            modsListView.BeginUpdate();
            foreach (ListViewItem modListItem in ModListData)
            {
                modListItem.SubItems[currentLoadOrderHeader.Index].Text =
                        ModsManager.Instance.ModDetails[modListItem.Tag.ToString()].defaultLoadOrder.ToString();
            }

            MainForm.Instance.ColorListViewNumbers(ModListData, MainForm.Instance.currentLoadOrderHeader.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
            modsListView.EndUpdate();
        }

        private void listBoxOverriding_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxOverriding.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                ModListBoxItem modListBoxItem = listBoxOverriding.Items[index] as ModListBoxItem;
                SelectModInList(modListBoxItem.ModKey);
            }
        }

        private void listBoxOverriddenBy_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxOverriddenBy.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                ModListBoxItem modListBoxItem = listBoxOverriddenBy.Items[index] as ModListBoxItem;
                SelectModInList(modListBoxItem.ModKey);
            }
        }

        public void UpdateModCountDisplay()
        {
            toolStripStatusLabelModCountTotal.Text = @"Total: " + GetModCount(false);
            toolStripStatusLabelModsActive.Text = @"Active: " + GetModCount(true);
        }

        public void SetModConfigTainted(bool modSettingsTainted)
        {
            ModsManager.Instance.ModSettingsTainted = modSettingsTainted;
            if (modSettingsTainted)
            {
                toolStripButtonApply.ForeColor = Color.OrangeRed;
                toolStripButtonApply.Font = new Font(Instance.toolStrip1.Font, Instance.toolStrip1.Font.Style | FontStyle.Bold);

            }
            else
            {
                toolStripButtonApply.ForeColor = SystemColors.ControlText;
                toolStripButtonApply.Font = new Font(Instance.toolStrip1.Font, Instance.toolStrip1.Font.Style);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ModsManager.Instance.ModSettingsTainted)
                return;

            DialogResult result =
                MessageBox.Show(
                    @"You have unapplied changes to your mod list." + System.Environment.NewLine + System.Environment.NewLine
                    + @"Do you want to apply your changes before quitting?",
                    @"Unapplied changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.Yes)
            {
                ApplyModSettings();
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void contextMenuItemMoveToTop_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in modsListView.SelectedItems)
            {
                MoveItemUp(item.Index, true);
            }
        }

        private void contextMenuItemMoveToBottom_Click(object sender, EventArgs e)
        {
            MoveItemDown(SelectedItemIndex(), true);
        }

        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in modsListView.SelectedItems)
            {
                MoveItemUp(item.Index, false);
            }
        }

        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveItemDown(SelectedItemIndex(), false);
        }

        public void ColorListViewNumbers(List<ListViewItem> listViewItems, int subItemIndex, Color fromColor, Color toColor)
        {
            List<int> numbers = new List<int>();

            // Extract numbers from ListView column and find unique ones
            foreach (ListViewItem item in listViewItems)
            {
                // Skip disabled mods
                if (!ModsManager.Instance.ModEnabledList[item.Tag.ToString()])
                    continue;

                int number;
                if (int.TryParse(item.SubItems[subItemIndex].Text, out number))
                {
                    if (!numbers.Contains(number))
                    {
                        numbers.Add(number);
                    }
                }
            }

            if (numbers.Count == 0)
                return;

            numbers.Sort();

            // Color the ListView items based on sorted unique numbers
            modsListView.BeginUpdate();
            for (int i = 0; i < listViewItems.Count; i++)
            {
                // Skip disabled mods
                if (!ModsManager.Instance.ModEnabledList[listViewItems[i].Tag.ToString()])
                    continue;

                int number;
                if (int.TryParse(listViewItems[i].SubItems[subItemIndex].Text, out number))
                {
                    Color newColor;
                    if (numbers.Count == 1)
                    {
                        newColor = fromColor;
                    }
                    else
                    {
                        int index = numbers.IndexOf(number);
                        double ratio = (double)index / (numbers.Count - 1);
                        newColor = Utils.InterpolateColor(fromColor, toColor, ratio);
                    }
                    listViewItems[i].SubItems[subItemIndex].ForeColor = newColor;
                }
            }
            modsListView.EndUpdate();
        }

        private void toolStripMenuItemSortDefaultLoadOrder_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            // This sorting follows the way MW5 orders its list

            modsListView.BeginUpdate();
            ModListData.Sort((x, y) =>
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    (x, y) = (y, x);
                }

                // Compare Original load order
                int priorityComparison = int.Parse(x.SubItems[originalLoadOrderHeader.Index].Text).CompareTo(int.Parse(y.SubItems[originalLoadOrderHeader.Index].Text));

                // If Priority is equal, compare Folder name
                if (priorityComparison == 0)
                {
                    return x.SubItems[folderHeader.Index].Text.CompareTo(y.SubItems[folderHeader.Index].Text);
                }
                else
                {
                    return priorityComparison;
                }
            });

            ReloadListViewFromData();
            RecomputeLoadOrdersAndUpdateList();
            ModsManager.Instance.GetOverridingData(this.ModListData);
            FilterTextChanged();
            modListView_SelectedIndexChanged(null, null);
            SetModConfigTainted(true);
            int selectedItemIndex = SelectedItemIndex();
            if (selectedItemIndex != -1)
            {
                modsListView.EnsureVisible(selectedItemIndex);
            }

            modsListView.EndUpdate();
        }

        public bool IsSortedByDefaultLoadOrder()
        {
            for (int i = 1; i < ModListData.Count; i++)
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    if (int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) > int.Parse(ModListData[i - 1].SubItems[originalLoadOrderHeader.Index].Text) ||
                        (int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) == int.Parse(ModListData[i - 1].SubItems[originalLoadOrderHeader.Index].Text) &&
                         string.Compare(ModListData[i].SubItems[folderHeader.Index].Text, ModListData[i - 1].SubItems[folderHeader.Index].Text) > 0))
                    {
                        return false;
                    }
                }
                else
                {
                    if (int.Parse(ModListData[i-1].SubItems[originalLoadOrderHeader.Index].Text) > int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) ||
                        (int.Parse(ModListData[i-1].SubItems[originalLoadOrderHeader.Index].Text) == int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) &&
                         string.Compare(ModListData[i-1].SubItems[folderHeader.Index].Text, ModListData[i].SubItems[folderHeader.Index].Text) > 0))
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private void openUserModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.AppData]))
            {
                return;
            }
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.AppData],
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Win32Exception win32Exception)
            {
                Console.WriteLine(win32Exception.Message);
                Console.WriteLine(win32Exception.StackTrace);
                string message = "While trying to open the mods folder, windows has encountered an error. Your folder does not exist, is not valid or was not set.";
                string caption = "Error Opening Mods Folder";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        private void toolStripButtonReload_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void toolStripButtonUp_Click(object sender, EventArgs e)
        {
            MoveItemUp(SelectedItemIndex(), Control.ModifierKeys == Keys.Shift);
        }

        private void toolStripButtonDown_Click(object sender, EventArgs e)
        {
            MoveItemDown(SelectedItemIndex(), Control.ModifierKeys == Keys.Shift);
        }

        private void toolStripButtonApply_Click(object sender, EventArgs e)
        {
            ApplyModSettings();
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            LaunchGame();
        }

        private void toolStripTextFilterBox_TextChanged(object sender, EventArgs e)
        {
            FilterTextChanged();
        }

        private void toolStripButtonClearFilter_Click(object sender, EventArgs e)
        {
            toolStripTextFilterBox.Text = "";
            toolStripTextFilterBox.Focus();
        }

        private void toolStripButtonFilterToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (!toolStripButtonFilterToggle.Checked)
            {
                ReloadListViewFromData();
            }
            FilterTextChanged();
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
            {
                ShowSettingsDialog();
            }
        }

        private void DeleteMod(string modKey)
        {
            // If the directory already vanished (i.e. deleted by the user externally)
            if (!Directory.Exists(modKey))
            {
                RefreshAll();
                return;
            }

            DialogResult dialogResult = MessageBox.Show("The mod " + ModsManager.Instance.ModDetails[modKey].displayName
                                                                   + " will be removed. This will delete the directory\r\n" + modKey
                                                                   + "\r\n\r\nAre you sure you want to continue?",
                "Delete Mod",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                if (FileOperationUtils.DeleteFile(modKey, true, this.Handle))
                {
                    RefreshAll();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in modsListView.SelectedItems)
            {
                DeleteMod(selectedItem.Tag.ToString());
            }
        }

        private void toolStripMenuItemImportFromFolder_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a mod folder for import";
                fbd.UseDescriptionForTitle = true;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !Utils.StringNullEmptyOrWhiteSpace(fbd.SelectedPath))
                {
                    if (!File.Exists(Path.Combine(fbd.SelectedPath, "mod.json")))
                    {
                        MessageBox.Show(@"No mod.json found." + System.Environment.NewLine + System.Environment.NewLine +
                                        @"This doesn't appear to be a valid mod folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    CopyModFromFolder(fbd.SelectedPath);
                    RefreshAll();
                }
            }
        }

        private void toolStripMenuItemImportArchive_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mod Archives|*.zip;*.7z;*.rar|All files (*.*)|*.*";
            openFileDialog.Title = "Select a mod archive for import";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedZipFile = openFileDialog.FileName;

                ExtractModFromArchive(selectedZipFile);
                RefreshAll();
            }
        }

        private void toolStripMenuItemNexusmodsLink_Click(object sender, EventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://www.nexusmods.com/mechwarrior5mercenaries/mods/1085",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void toolStripStatusLabelUpdate_Click(object sender, EventArgs e)
        {
            string updateUrl = OnlineUpdateUrl;
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = updateUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}