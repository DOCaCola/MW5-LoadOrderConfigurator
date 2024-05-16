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
using System.Text;
using BrightIdeasSoftware;
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

        eFilterMode _filterMode = eFilterMode.None;
        public List<ListViewItem> ModListData = new List<ListViewItem>();
        private bool _movingItems = false;
        string _onlineUpdateUrl = LocConstants.UrlNexusmods;
        // The mod currently displaed in the sidebar
        private static string _sideBarSelectedModKey = string.Empty;
        // Force next sidepanel update to execute
        private bool _forceSidePanelUpdate = false;

        static Color _highlightColor = Color.FromArgb(200, 253, 213);
        static Color _highlightColorAlternate = Color.FromArgb(189, 240, 202);

        public static Color _OverriddenBackColor = Color.FromArgb(255, 242, 203);
        public static Color _OverriddenBackColorAlternate = Color.FromArgb(247, 234, 196);
        public static Color _OverridingBackColor = Color.FromArgb(235, 225, 255);
        public static Color _OverridingBackColorAlternate = Color.FromArgb(226, 217, 245);

        private TypedObjectListView<ModItem> _modList = null;
        private TypedColumn<ModItem> _modNameColumn = null;

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

            this.Text += @" " + GetVersion();

            _modList = new TypedObjectListView<ModItem>(this.modObjectListView);
            _modNameColumn = new TypedColumn<ModItem>(this.olvColumnModName);

            imageListIcons.Images.Add("Steam", UiIcons.Steam);
            imageListIcons.Images.Add("SteamDis", UiIcons.SteamDis);
            imageListIcons.Images.Add("Nexusmods", UiIcons.Nexusmods);
            imageListIcons.Images.Add("NexusmodsDis", UiIcons.NexusmodsDis);
            imageListIcons.Images.Add("Folder", UiIcons.Folder);
            imageListIcons.Images.Add("FolderDis", UiIcons.FolderDis);

            olvColumnModName.ImageGetter = this.ModImageGetter;
            olvColumnModFileSize.AspectToStringConverter = FileSizeAspectConverter;

            var dragSource = new SimpleDragSource();
            dragSource.RefreshAfterDrop = false;
            this.modObjectListView.DragSource = dragSource;
            var dropSink = new SimpleDropSink();
            dropSink.AcceptExternal = false;
            dropSink.CanDropBetween = true;
            dropSink.CanDropOnBackground = false;
            dropSink.CanDropOnItem = false;
            dropSink.CanDropOnSubItem = false;
            dropSink.FeedbackColor = Color.Black;
            dropSink.CanDrop += (o, args) => { args.Effect = DragDropEffects.Move; };
            this.modObjectListView.DropSink = dropSink;

            /*
            this.modObjectListView.CustomSorter = delegate(OLVColumn column, SortOrder order) {
                this.modObjectListView.ListViewItemSorter = new ColumnComparer(
                    this.isEmergencyColumn, SortOrder.Descending, column, order);
            };*/

            modsListView.SetDoubleBuffered();

            panelColorOverridden.BackColor = ModsManager.OverriddenColor;
            panelColorOverriding.BackColor = ModsManager.OverridingColor;
            panelColorOverridingOverridden.BackColor = ModsManager.OverriddenOveridingColor;

            toolStrip2.Renderer = new ToolStripTransparentRenderer();

            UpdateMoveControlEnabledState();

            /*Font monospaceFont = Utils.CreateBestAvailableMonospacePlatformFont(richTextBoxManifestOverridden.Font.Size);
            if (monospaceFont != null)
            {
                richTextBoxManifestOverridden.Font = monospaceFont;
            }*/
        }


        private string FileSizeAspectConverter(object value)
        {
            long size = (long)value;
            return Utils.BytesToHumanReadableString(size);
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

                _onlineUpdateUrl = updateData["updateUrl"].ToString();
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

        public void UpdatePriorityLabels()
        {
            if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh)
            {
                rotatingLabelTop.NewText = "Low priority »";
                toolTip1.SetToolTip(rotatingLabelTop, "Mods higher in the list are loaded earlier and may get overridden by mods below them");
                rotatingLabelBottom.NewText = "« High priority";
                toolTip1.SetToolTip(rotatingLabelBottom, "Mods lower in the list are loaded later, and may override mods above them");
                /*rotatingLabelTop.ForeColor = ModsManager.LowPriorityColor;
                rotatingLabelBottom.ForeColor = ModsManager.HighPriorityColor;*/

            }
            else
            {
                rotatingLabelTop.NewText = "High priority »";
                toolTip1.SetToolTip(rotatingLabelTop, "Mods higher in the list are loaded later and may override mods below them");
                rotatingLabelBottom.NewText = "« Low priority";
                toolTip1.SetToolTip(rotatingLabelBottom, "Mods lower in the list are loaded earlier and may get overridden by mods above them");
                /*rotatingLabelTop.ForeColor = ModsManager.HighPriorityColor;
                rotatingLabelBottom.ForeColor = ModsManager.LowPriorityColor;*/
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

            string destinationPath = Path.GetFullPath(Path.Combine(ModsManager.Instance.GetMainModPath(), Path.GetFileName(path)));

            bool targetDirectoryCleared = false;
            if (Directory.Exists(destinationPath))
            {
                DialogResult dialogResult = MessageBox.Show("The target directory " + destinationPath
                    + " already exists. It has to be deleted before the copy operation can begin."
                    + "\r\n\r\nAre you sure you want to continue?",
                    "Mod Directory already exists",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    if (FileOperationUtils.DeleteFile(destinationPath, true, this.Handle))
                    {
                        targetDirectoryCleared = true;
                    }
                }

            }

            if (!targetDirectoryCleared)
                return false;

            return FileOperationUtils.CopyDirectory(path, ModsManager.Instance.GetMainModPath(), this.Handle);
        }

        public List<string> ExtractModFromArchive(string filePath)
        {
            ExtractForm extractForm = new ExtractForm();
            extractForm.ArchiveFilePath = filePath;
            extractForm.OutputFolderPath = ModsManager.Instance.GetMainModPath();

            bool result = extractForm.ShowDialog(this) != DialogResult.Cancel;

            List<string> extractedModDirs = extractForm.ExtractedModDirNames;
            extractForm.Dispose();

            return extractedModDirs;
        }

        public void QueueSidePanelUpdate(bool forceUpdate)
        {
            if (forceUpdate && !_forceSidePanelUpdate)
            {
                _forceSidePanelUpdate = true;
            }

            timerOverviewUpdateDelay.Stop();
            timerOverviewUpdateDelay.Start();
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

            if (ModsManager.Instance.ModSettingsTainted)
            {
                if (ShowChangesNeedToBeAppliedDialog())
                    ApplyModSettings();
                else
                    return;
            }

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

                List<string> extractedModDirNames = ExtractModFromArchive(filePath);
                if (extractedModDirNames == null || extractedModDirNames.Count == 0)
                    return;
            }

            RefreshAll(true);
        }

        public enum MoveDirection { Up, Down };

        public void MoveListItems(ListView.SelectedListViewItemCollection moveItems, MoveDirection direction)
        {
            var selectedItems = moveItems.Cast<ListViewItem>().ToList();
            bool anyMoved = false;
            selectedItems = selectedItems.OrderBy(i => this.modsListView.Items.IndexOf(i)).ToList();

            modsListView.BeginUpdate();
            _movingItems = true;

            if (direction == MoveDirection.Up)
            {
                for (int selectedItemIndex = 0; selectedItemIndex < selectedItems.Count; selectedItemIndex++)
                {
                    int currentIndex = selectedItems[selectedItemIndex].Index;
                    int newIndex = currentIndex - 1;

                    if (currentIndex == selectedItemIndex)
                        continue;

                    ListViewItem listItem = ModListData[currentIndex];
                    modsListView.Items.RemoveAt(currentIndex);
                    ModListData.RemoveAt(currentIndex);

                    modsListView.Items.Insert(newIndex, listItem);
                    ModListData.Insert(newIndex, listItem);

                    anyMoved = true;
                }
            }
            else
            {
                int endOffset = 1;
                for (int selectedItemIndex = selectedItems.Count - 1; selectedItemIndex >= 0; selectedItemIndex--)
                {
                    int currentIndex = selectedItems[selectedItemIndex].Index;
                    int newIndex = currentIndex + 1;

                    if (currentIndex == modsListView.Items.Count - endOffset++)
                        continue;

                    ListViewItem listItem = ModListData[currentIndex];
                    modsListView.Items.RemoveAt(currentIndex);
                    ModListData.RemoveAt(currentIndex);

                    modsListView.Items.Insert(newIndex, listItem);
                    ModListData.Insert(newIndex, listItem);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                RecomputeLoadOrdersAndUpdateList();
                ModsManager.Instance.GetOverridingData(this.ModListData);

                modListView_SelectedIndexChanged(null, null);

                SetModConfigTainted(true);
            }

            _movingItems = false;

            QueueSidePanelUpdate(true);
            modsListView.EndUpdate();
        }

        public enum MovePosition { Top, Bottom };

        public void MoveListItems(ListView.SelectedListViewItemCollection moveItems, MovePosition position)
        {
            modsListView.BeginUpdate();
            _movingItems = true;
            bool anyMoved = false;
            var selectedItems = moveItems.Cast<ListViewItem>().ToList();
            selectedItems = selectedItems.OrderBy(i => this.modsListView.Items.IndexOf(i)).ToList();

            if (position == MovePosition.Top)
            {
                int listOffset = 0;
                foreach (var item in selectedItems)
                {
                    if (item.Index != listOffset)
                    {
                        modsListView.Items.Remove(item);
                        ModListData.Remove(item);

                        modsListView.Items.Insert(listOffset, item);
                        ModListData.Insert(listOffset, item);

                        anyMoved = true;
                    }
                    ++listOffset;
                }
            }
            else
            {
                int endOffset = selectedItems.Count;
                foreach (var item in selectedItems)
                {
                    if (item.Index == modsListView.Items.Count - endOffset--)
                        continue;

                    modsListView.Items.Remove(item);
                    ModListData.Remove(item);

                    modsListView.Items.Add(item);
                    ModListData.Add(item);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                RecomputeLoadOrdersAndUpdateList();
                ModsManager.Instance.GetOverridingData(this.ModListData);

                modListView_SelectedIndexChanged(null, null);
                SetModConfigTainted(true);
            }

            _movingItems = false;

            QueueSidePanelUpdate(true);
            modsListView.EndUpdate();
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
            this.ModListData.Clear();
            this.modsListView.Items.Clear();
            ModsManager.Instance.ClearAll();
            UpdateSidePanelData(true);
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
                case eGamePlatform.Generic:
                    {
                        this.toolStripPlatformLabel.Text = @"Platform: MW5";
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
                        // Compare load order
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
                if (desiredMods != null)
                {
                    foreach (var curDesiredMod in desiredMods)
                    {
                        ModsManager.Instance.ModEnabledList[curDesiredMod.Key] = curDesiredMod.Value;
                    }
                }

                for (int i = 0; i < orderedModList.Count; i++)
                {
                    bool newState = false;
                    var curModListItem = orderedModList[i];
                    if (desiredMods != null && desiredMods.ContainsKey(curModListItem.Key))
                    {
                        newState = desiredMods[curModListItem.Key];
                    }

                    orderedModList[i] = new KeyValuePair<string, bool>(curModListItem.Key, newState);
                }

                modsListView.BeginUpdate();
                ModListData.Clear();
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

                modObjectListView.BeginUpdate();
                foreach (KeyValuePair<string, bool> entry in orderedModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
                {
                    if (entry.Equals(new KeyValuePair<string, bool>(null, false)))
                        continue;
                    if (entry.Key == null)
                        continue;

                    ModItem newItem = new ModItem();
                    newItem.Enabled = ModsManager.Instance.ModEnabledList[entry.Key];
                    newItem.Path = entry.Key;
                    newItem.Name = ModsManager.Instance.ModDetails[entry.Key].displayName;
                    newItem.FolderName = ModsManager.Instance.PathToDirNameDict[entry.Key];
                    newItem.FileSize = ModsManager.Instance.Mods[entry.Key].ModFileSize;
                    newItem.Author = ModsManager.Instance.ModDetails[entry.Key].author;
                    newItem.CurrentLoadOrder = (int)ModsManager.Instance.Mods[entry.Key].NewLoadOrder;
                    newItem.OriginalLoadOrder = (int)ModsManager.Instance.Mods[entry.Key].OriginalLoadOrder;
                    newItem.Origin = ModsManager.Instance.Mods[entry.Key].Origin;

                    modObjectListView.AddObject(newItem);
                }
                modObjectListView.EndUpdate();

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

        public object ModImageGetter(object rowObject)
        {
            ModItem s = (ModItem)rowObject;

            switch (s.Origin)
            {
                case ModsManager.ModData.ModOrigin.Steam:
                    if (s.Enabled)
                        return "Steam";
                    else
                        return "SteamDis";

                case ModsManager.ModData.ModOrigin.Nexusmods:
                    if (s.Enabled)
                        return "Nexusmods";
                    else
                        return "NexusmodsDis";

                default:
                    if (s.Enabled)
                        return "Folder";
                    else
                        return "FolderDis";


            }
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

            string versionString = (ModsManager.Instance.ModDetails[entry.Key].version + " (" +
                                    ModsManager.Instance.ModDetails[entry.Key].buildNumber.ToString() + ")").Trim();

            newItem.SubItems[displayHeader.Index].Text = ModsManager.Instance.ModDetails[entry.Key].displayName;
            newItem.SubItems[folderHeader.Index].Text = ModsManager.Instance.PathToDirNameDict[modName];
            newItem.SubItems[authorHeader.Index].Text = ModsManager.Instance.ModDetails[entry.Key].author;
            newItem.SubItems[versionHeader.Index].Text = versionString;
            newItem.SubItems[currentLoadOrderHeader.Index].Text = ModsManager.Instance.Mods[entry.Key].NewLoadOrder.ToString();
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

        public void RefreshAll(bool forceLoadLastApplied = false)
        {
            Cursor tempCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            modsListView.BeginUpdate();
            ClearAll();
            bool modConfigTainted = false;
            if (ModsManager.Instance.TryLoadProgramSettings())
            {
                UpdatePriorityLabels();
                SetVersionAndPlatform();
                ModsManager.Instance.WarnIfNoModList();
                ModsManager.Instance.ParseDirectories();
                ModsManager.Instance.ReloadModData();

                // load modlist.json
                Dictionary<string, bool> modlist = ModsManager.Instance.LoadModList();
                if (modlist != null)
                {
                    ModsManager.Instance.ProcessModFolderEnabledList(ref modlist, false);
                    ModsManager.Instance.ModEnabledListLastState = modlist;
                }
                ModsManager.Instance.DetermineBestAvailableGameVersion();

                // Check if we want to load the last applied mod list
                Dictionary<string, bool> loadModlist = modlist;

                LoadAndFill(loadModlist, false);

                ModsManager.Instance.LoadLastAppliedPresetData();

                if (forceLoadLastApplied || ModsManager.Instance.ShouldLoadLastApplied())
                {
                    // Load last saved preset
                    loadModlist = ModsManager.Instance.LastAppliedPresetModList;

                    LoadAndFill(loadModlist, true);
                    modConfigTainted = true;
                }

                FilterTextChanged();
                ModsManager.Instance.GetOverridingData(ModListData);
            }
            LoadPresets();
            SetModConfigTainted(modConfigTainted);
            modsListView.EndUpdate();
            Cursor.Current = tempCursor;
        }

        //Saves current load order to preset.
        public void SavePreset(string name)
        {
            Dictionary<string, bool> NoPathModlist = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
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
            Dictionary<string, bool> presetData;
            try
            {
                presetData = JsonConvert.DeserializeObject<Dictionary<string, bool>>(JsonString);
            }
            catch (Exception Ex)
            {
                string message = "There was an error in decoding the load order string.";
                string caption = "Load Order Decoding Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                return;
            }

            presetData.ReverseIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh);

            modsListView.BeginUpdate();
            this.modsListView.Items.Clear();
            this.ModListData.Clear();
            ModsManager.Instance.ModDetails = new Dictionary<string, ModObject>();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.Mods.Clear();

            ModsManager.Instance.ParseDirectories();
            ModsManager.Instance.ReloadModData();
            ModsManager.Instance.ProcessModFolderEnabledList(ref presetData, true);
            this.LoadAndFill(presetData, true);
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
                var result = ShowUnappliedChangesDialog();

                if (result == eUnappliedChangesDialogResult.Apply)
                {
                    ApplyModSettings();
                }
                else if (result == eUnappliedChangesDialogResult.Cancel)
                {
                    return;
                }
            }

            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Epic:
                    LaunchGameEpic();
                    break;
                case eGamePlatform.Steam:
                    LanchGameSteam();
                    break;
                case eGamePlatform.Generic:
                case eGamePlatform.Gog:
                    LaunchGameGeneric();
                    break;
                case eGamePlatform.WindowsStore:
                    LaunchGameMicrosoftStore();
                    break;
            }
        }

        //Launch game button
        private void buttonStartGame_Click(object sender, EventArgs e)
        {


        }

        #region Launch Game
        private static void LaunchGameMicrosoftStore()
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

        private void LaunchGameGeneric()
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

        private static void LaunchGameEpic()
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

        private static void LanchGameSteam()
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

        private void UpdateMoveControlEnabledState()
        {
            bool anySelected = modsListView.SelectedItems.Count > 0;
            bool enabled = anySelected && _filterMode == eFilterMode.None;
            toTopToolStripButton.Enabled = enabled;
            toBottomToolStripButton.Enabled = enabled;
            upToolStripButton.Enabled = enabled;
            downToolStripButton.Enabled = enabled;

            moveupToolStripMenuItem.Enabled = enabled;
            movedownToolStripMenuItem.Enabled = enabled;
            contextMenuItemMoveToTop.Enabled = enabled;
            contextMenuItemMoveToBottom.Enabled = enabled;
        }

        private void FilterTextChanged()
        {
            bool searchFailed = true;
            string filtertext = toolStripTextFilterBox.Text.ToLower();
            if (Utils.StringNullEmptyOrWhiteSpace(filtertext))
            {
                if (this._filterMode != eFilterMode.None)
                {
                    // end filtering
                    modsListView.BeginUpdate();
                    if (this._filterMode == eFilterMode.ItemFilter)
                        ReloadListViewFromData();
                    RecolorListViewRows();
                    modsListView.EndUpdate();
                    UpdateMoveControlEnabledState();
                    this._filterMode = eFilterMode.None;
                }

                searchFailed = false;
            }
            else
            {
                if (!Instance.toolStripButtonFilterToggle.Checked)
                {
                    // ensure that first found item is visible
                    foreach (ListViewItem item in this.ModListData)
                    {
                        if (MatchItemToText(filtertext, item))
                        {
                            item.EnsureVisible();
                            searchFailed = false;
                            break;
                        }
                    }

                    _filterMode = eFilterMode.ItemHighlight;
                    RecolorListViewRows();
                }
                //We are filtering by selected adding.
                else
                {
                    _filterMode = eFilterMode.ItemFilter;
                    //Clear the list view
                    this.modsListView.Items.Clear();
                    Instance.modsListView.BeginUpdate();
                    RecolorListViewRows();
                    foreach (ListViewItem item in this.ModListData)
                    {
                        bool prevLoadingAndFilling = LoadingAndFilling;
                        LoadingAndFilling = true;
                        if (MatchItemToText(filtertext, item))
                        {
                            searchFailed = false;
                            Instance.modsListView.Items.Add(item);
                        }

                        LoadingAndFilling = prevLoadingAndFilling;
                    }
                    Instance.modsListView.EndUpdate();
                }
            }
            toolStripButtonClearFilter.Enabled = toolStripTextFilterBox.Text.Length > 0;

            if (searchFailed)
            {
                toolStripTextFilterBox.ForeColor = Color.White;
                toolStripTextFilterBox.BackColor = Color.FromArgb(252, 104, 99);
            }
            else
            {
                toolStripTextFilterBox.ForeColor = SystemColors.WindowText;
                toolStripTextFilterBox.BackColor = SystemColors.Window;
            }

            //While filtering disable the up/down buttons (tough this should no longer be needed).
            UpdateMoveControlEnabledState();
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

        private void AppendContentPathToMainfestList(string contentPath, ref StringBuilder sb)
        {
            sb.Append(@"\b ");
            sb.Append(Path.GetFileName(contentPath));
            sb.Append(@" \b0 ");

            sb.Append(@" (" + Utils.RtfEscape(Path.GetDirectoryName(contentPath)) + @")");
            sb.Append(@" \line ");
        }

        //Selected index of mods overriding the currently selected mod has changed.
        private void listBoxOverriddenBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool startedListUpdate = false;
            try
            {
                if (_filterMode == eFilterMode.None)
                {
                    startedListUpdate = true;
                    richTextBoxManifestOverridden.SuspendDrawing();
                    modsListView.BeginUpdate();
                    RecolorListViewRows();
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

                if (_filterMode == eFilterMode.None)
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

                var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modData.overriddenBy[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry, ref sb);
                }
                sb.Append(@"}");
                richTextBoxManifestOverridden.Rtf = sb.ToString();
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.ResumeDrawing();
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
                if (_filterMode == eFilterMode.None)
                {
                    startedListUpdate = true;
                    modsListView.BeginUpdate();
                    richTextBoxManifestOverridden.SuspendDrawing();
                    RecolorListViewRows();
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

                if (_filterMode == eFilterMode.None)
                {
                    HighlightModInList(selectedMod.ModKey);
                }

                string superMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;

                if (!ModsManager.Instance.OverridingData.ContainsKey(superMod))
                    return;

                OverridingData modData = ModsManager.Instance.OverridingData[superMod];

                var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modData.overrides[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry, ref sb);
                }
                sb.Append(@"}");
                richTextBoxManifestOverridden.Rtf = sb.ToString();
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.ResumeDrawing();
                    modsListView.EndUpdate();
                }
            }
        }

        private void UpdateSidePanelData(bool forceUpdate)
        {
            if (modsListView.SelectedItems.Count == 0)
            {
                _sideBarSelectedModKey = string.Empty;
                labelModNameOverrides.Text = string.Empty;
                pictureBoxModImage.Visible = false;
                panelModInfo.Visible = false;
                richTextBoxManifestOverridden.Clear();
                listBoxOverriddenBy.Items.Clear();
                listBoxOverriding.Items.Clear();
                return;
            }

            string selectedModPath = (string)modsListView.SelectedItems[0].Tag;

            if (!forceUpdate && _sideBarSelectedModKey == selectedModPath)
                return;

            _sideBarSelectedModKey = selectedModPath;

            string selectedModFolder = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;
            // Make sure displayed label doesn't convert & to underscore
            string selectedModLabelDisplayName = modsListView.SelectedItems[0].SubItems[displayHeader.Index].Text;
            selectedModLabelDisplayName = selectedModLabelDisplayName.Replace("&", "&&");

            ModObject modDetails = ModsManager.Instance.ModDetails[selectedModPath];

            panelModInfo.Visible = true;
            labelModName.Text = selectedModLabelDisplayName;
            labelModNameOverrides.Text = selectedModLabelDisplayName;
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

            string nexusModsId = ModsManager.Instance.Mods[selectedModPath].NexusModsId;
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

            HandleOverriding(selectedModFolder);

            string imagePath = Path.Combine(selectedModPath, "Resources", "Icon128.png");

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

        private void modListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMoveControlEnabledState();

            if (_filterMode == eFilterMode.None)
                RecolorListViewRows();

            if (_movingItems)
                return;

            QueueSidePanelUpdate(false);
        }

        //Handles the showing of overriding data on select
        private void HandleOverriding(string SelectedMod)
        {
            if (ModsManager.Instance.OverridingData.Count == 0)
                return;

            this.listBoxOverriding.Items.Clear();
            this.listBoxOverriddenBy.Items.Clear();
            this.richTextBoxManifestOverridden.Clear();

            if (!ModsManager.Instance.OverridingData.ContainsKey(SelectedMod))
                return;

            listBoxOverriding.SuspendDrawing();
            listBoxOverriddenBy.SuspendDrawing();
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
            listBoxOverriding.ResumeDrawing();
            listBoxOverriddenBy.ResumeDrawing();
        }

        private void modListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //While we are removing/inserting items this will fire and we dont want that to happen when we move an item.
            if (_movingItems || this.LoadingAndFilling)
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

        private void ExportLoadOrder()
        {
            ExportForm exportDialog = new ExportForm();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            exportDialog.ShowDialog(this);
            exportDialog.Dispose();
        }

        private void exportLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportLoadOrder();
        }

        private void ImportLoadOrder()
        {
            ImportForm importDialog = new ImportForm();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (importDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Dictionary<string, bool> newData = importDialog.ResultData;
            if (importDialog.ResultDataType == ImportForm.eResultDataType.DirNames)
            {
                ModsManager.Instance.ProcessModFolderEnabledList(ref newData, true);
            }
            else if (importDialog.ResultDataType == ImportForm.eResultDataType.ModNames)
            {
                ModsManager.Instance.ProcessModNameEnabledList(ref newData, true);
            }

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

        private void importLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportLoadOrder();
        }

        private void openModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Program]))
                return;

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

            if (AreAllModsEnabled())
                return;

            modsListView.BeginUpdate();

            this._movingItems = true;
            foreach (ListViewItem item in this.ModListData)
            {
                item.Checked = true;
            }
            this._movingItems = false;

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

            if (AreAllModsDisabled())
                return;

            modsListView.BeginUpdate();

            this._movingItems = true;
            foreach (ListViewItem item in this.modsListView.Items)
            {
                item.Checked = false;
            }
            this._movingItems = false;

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
            foreach (ListViewItem selectedItem in modsListView.SelectedItems)
            {
                string path = (string)selectedItem.Tag;
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
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Steam]))
                return;

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

            if (_filterMode != eFilterMode.None)
                return;

            _movingItems = true;
            DoDragDrop(modsListView.SelectedItems, DragDropEffects.Move);
            _movingItems = false;

            QueueSidePanelUpdate(true);
        }

        private void modsListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
            {
                e.Effect = DragDropEffects.None;
            }

            e.Effect = e.AllowedEffect;
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

            if (!e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
                return;

            // Retrieve the dragged item.
            var draggedItems = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));


            DragDropRows(insertIndex, draggedItems);

            modsListView.InsertionMark.Index = -1;
        }

        private void modsListView_DragLeave(object sender, EventArgs e)
        {
            modsListView.InsertionMark.Index = -1;
        }

        private void DragDropRows(int insertIndex, ListView.SelectedListViewItemCollection draggedItems)
        {
            modsListView.BeginUpdate();
            foreach (ListViewItem draggedItem in draggedItems)
            {
                int oldIndex = draggedItem.Index;

                // If AppearsAfterItem is enabled, insert one item down.
                int targetIndex =
                    modsListView.InsertionMark.AppearsAfterItem
                        ? insertIndex + 1
                        : insertIndex;

                ListViewItem newItem = (ListViewItem)draggedItem.Clone();
                ModListData.Insert(targetIndex, newItem);
                modsListView.Items.Insert(targetIndex, newItem);
                newItem.Selected = true;

                ModListData.Remove(draggedItem);
                modsListView.Items.Remove(draggedItem);

                // Bottom to top reverses the order for multiple items.
                if (oldIndex > insertIndex)
                {
                    insertIndex++;
                }
            }

            RecomputeLoadOrdersAndUpdateList();
            ModsManager.Instance.GetOverridingData(this.ModListData);

            modListView_SelectedIndexChanged(null, null);
            modsListView.EndUpdate();

            SetModConfigTainted(true);
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
                        if (modListItem.Index % 2 == 1)
                        {
                            subItem.BackColor = _highlightColorAlternate;
                        }
                        else
                        {
                            subItem.BackColor = _highlightColor;
                        }
                    }
                    break;
                }
            }
        }

        public void RecolorObjectListViewRows()
        {
            bool showModOverrides = modObjectListView.SelectedItems.Count == 1 && _filterMode == eFilterMode.None;

            bool anyUpdated = false;
            for (int i = 0; i <= modObjectListView.Items.Count - 1; ++i)
            {
                OLVListItem curItem = (OLVListItem)modObjectListView.Items[i];
                ModItem curModItem = (ModItem)curItem.RowObject;

                bool alternateColor = i % 2 == 1;
                Color newBackColor = SystemColors.Window;
                if (alternateColor)
                {
                    newBackColor = Color.FromArgb(246, 245, 246);
                }

                if (_filterMode == eFilterMode.ItemHighlight)
                {
                    string filtertext = toolStripTextFilterBox.Text.ToLower();
                    if (!string.IsNullOrWhiteSpace(filtertext) && MatchItemToText(filtertext, curItem))
                    {
                        if (!alternateColor)
                            newBackColor = _highlightColor;
                        else
                            newBackColor = _highlightColorAlternate;
                    }
                }

                // Color mod overrides following the currently selected mod
                if (showModOverrides)
                {
                    OLVListItem firstSelected = (OLVListItem)modObjectListView.SelectedItems[0];
                    ModItem firstSelectedItem = (ModItem)firstSelected.RowObject;
                    string selectedModPath = firstSelectedItem.Path;
                    string selectedModFolder = ModsManager.Instance.PathToDirNameDict[selectedModPath];
                    if (ModsManager.Instance.OverridingData.ContainsKey(selectedModFolder))
                    {
                        OverridingData modData = ModsManager.Instance.OverridingData[selectedModFolder];
                        bool foundMatch = false;
                        foreach (string overriding in modData.overriddenBy.Keys)
                        {
                            string modKey = ModsManager.Instance.DirNameToPathDict[overriding];
                            if (modKey == curModItem.Path)
                            {
                                if (!alternateColor)
                                    newBackColor = _OverridingBackColor;
                                else
                                    newBackColor = _OverridingBackColorAlternate;
                                foundMatch = true;
                                break;
                            }
                        }

                        if (!foundMatch)
                        {
                            foreach (string overrides in modData.overrides.Keys)
                            {
                                string modKey = ModsManager.Instance.DirNameToPathDict[overrides];
                                if (modKey == curModItem.Path)
                                {
                                    if (!alternateColor)
                                        newBackColor = _OverriddenBackColor;
                                    else
                                        newBackColor = _OverriddenBackColorAlternate;
                                    break;
                                }
                            }
                        }
                    }
                }

                curModItem.ProcessedRowBackColor = newBackColor;

                foreach (OLVListSubItem subItem in curItem.SubItems)
                {
                    if (subItem.BackColor != newBackColor)
                    {
                        if (!anyUpdated)
                        {
                            anyUpdated = true;
                            this.modObjectListView.BeginUpdate();
                        }
                        subItem.BackColor = newBackColor;
                    }

                }
            }

            if (anyUpdated)
                this.modObjectListView.EndUpdate();
        }

        public void RecolorListViewRows()
        {
            bool showModOverrides = modsListView.SelectedItems.Count == 1 && _filterMode == eFilterMode.None;

            bool anyUpdated = false;
            for (int i = 0; i <= ModListData.Count - 1; ++i)
            {
                ListViewItem curItem = ModListData[i];

                bool alternateColor = i % 2 == 1;
                Color newBackColor = SystemColors.Window;
                if (alternateColor)
                {
                    newBackColor = Color.FromArgb(246, 245, 246);
                }

                if (_filterMode == eFilterMode.ItemHighlight)
                {
                    string filtertext = toolStripTextFilterBox.Text.ToLower();
                    if (!string.IsNullOrWhiteSpace(filtertext) && MatchItemToText(filtertext, curItem))
                    {
                        if (!alternateColor)
                            newBackColor = _highlightColor;
                        else
                            newBackColor = _highlightColorAlternate;
                    }
                }

                // Color mod overrides following the currently selected mod
                if (showModOverrides)
                {
                    string selectedModPath = (string)modsListView.SelectedItems[0].Tag;
                    string selectedModFolder = ModsManager.Instance.PathToDirNameDict[selectedModPath];
                    if (ModsManager.Instance.OverridingData.ContainsKey(selectedModFolder))
                    {
                        OverridingData modData = ModsManager.Instance.OverridingData[selectedModFolder];
                        bool foundMatch = false;
                        foreach (string overriding in modData.overriddenBy.Keys)
                        {
                            ModListBoxItem modListBoxItem = new ModListBoxItem();
                            string modKey = ModsManager.Instance.DirNameToPathDict[overriding];
                            if (modKey == (string)curItem.Tag)
                            {
                                if (!alternateColor)
                                    newBackColor = _OverridingBackColor;
                                else
                                    newBackColor = _OverridingBackColorAlternate;
                                foundMatch = true;
                                break;
                            }
                        }

                        if (!foundMatch)
                        {
                            foreach (string overrides in modData.overrides.Keys)
                            {
                                ModListBoxItem modListBoxItem = new ModListBoxItem();
                                string modKey = ModsManager.Instance.DirNameToPathDict[overrides];
                                if (modKey == (string)curItem.Tag)
                                {
                                    if (!alternateColor)
                                        newBackColor = _OverriddenBackColor;
                                    else
                                        newBackColor = _OverriddenBackColorAlternate;
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (ListViewItem.ListViewSubItem subItem in curItem.SubItems)
                {
                    if (subItem.BackColor != newBackColor)
                    {
                        if (!anyUpdated)
                        {
                            anyUpdated = true;
                            this.modsListView.BeginUpdate();
                        }
                        subItem.BackColor = newBackColor;
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
            bool isDefaultSorted = AreModsSortedByDefaultLoadOrder();

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
                    ModsManager.Instance.Mods[modKey].NewLoadOrder = curLoadOrder;
                    --curLoadOrder;
                }
                else
                {
                    ModsManager.Instance.Mods[modKey].NewLoadOrder = ModsManager.Instance.Mods[modKey].OriginalLoadOrder;
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
                        ModsManager.Instance.Mods[modListItem.Tag.ToString()].NewLoadOrder.ToString();
            }

            ColorListViewNumbers(ModListData, MainForm.Instance.currentLoadOrderHeader.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
            RecolorListViewRows();
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

        public enum eUnappliedChangesDialogResult
        {
            Apply,
            DontApply,
            Cancel
        }

        private eUnappliedChangesDialogResult ShowUnappliedChangesDialog()
        {
            // Create the page which we want to show in the dialog.
            TaskDialogButton btnCancel = TaskDialogButton.Cancel;
            TaskDialogButton btnApply = new TaskDialogButton("&Apply");
            TaskDialogButton btnDontApply = new TaskDialogButton("Do&n't apply");

            var page = new TaskDialogPage()
            {
                Caption = "MechWarrior 5 Load Order Configurator",
                Heading = "Do you want to apply your changes to the MechWarrior 5 mod list?",
                /*Text = "You have unapplied changes to your mod list.",*/
                Buttons =
                {
                    btnCancel,
                    btnApply,
                    btnDontApply
                }
            };

            // Show a modal dialog, then check the result.
            TaskDialogButton result = TaskDialog.ShowDialog(this, page);

            if (result == btnApply)
                return eUnappliedChangesDialogResult.Apply;
            if (result == btnDontApply)
                return eUnappliedChangesDialogResult.DontApply;

            return eUnappliedChangesDialogResult.Cancel;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ModsManager.Instance.ModSettingsTainted)
                return;

            var result = ShowUnappliedChangesDialog();

            if (result == eUnappliedChangesDialogResult.Apply)
            {
                ApplyModSettings();
            }
            else if (result == eUnappliedChangesDialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void contextMenuItemMoveToTop_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MovePosition.Top);
        }

        private void contextMenuItemMoveToBottom_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MovePosition.Bottom);
        }

        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MoveDirection.Up);
        }

        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MoveListItems(modsListView.SelectedItems, MoveDirection.Down);
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

            if (AreModsSortedByDefaultLoadOrder())
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

        public bool AreAllModsEnabled()
        {
            for (int i = 1; i < ModListData.Count; i++)
            {
                if (!ModListData[i].Checked)
                    return false;
            }

            return true;
        }

        public bool AreAllModsDisabled()
        {
            for (int i = 1; i < ModListData.Count; i++)
            {
                if (ModListData[i].Checked)
                    return false;
            }

            return true;
        }

        public bool AreModsSortedByDefaultLoadOrder()
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
                    if (int.Parse(ModListData[i - 1].SubItems[originalLoadOrderHeader.Index].Text) > int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) ||
                        (int.Parse(ModListData[i - 1].SubItems[originalLoadOrderHeader.Index].Text) == int.Parse(ModListData[i].SubItems[originalLoadOrderHeader.Index].Text) &&
                         string.Compare(ModListData[i - 1].SubItems[folderHeader.Index].Text, ModListData[i].SubItems[folderHeader.Index].Text) > 0))
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private void openUserModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.AppData]))
                return;

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
            CheckForNewVersion();
            if (!ModsManager.Instance.GameIsConfigured())
            {
                ShowSettingsDialog();
                // Also calls refresh all. We end up calling refreshall twice
                // and may get the recovery dialog twice if we don't return early
                return;
            }

            RefreshAll();
        }

        private void DeleteMod(string modKey)
        {
            // If the directory already vanished (i.e. deleted by the user externally)
            if (!Directory.Exists(modKey))
            {
                RefreshAll();
                return;
            }

            // Create the page which we want to show in the dialog.
            TaskDialogButton btnCancel = TaskDialogButton.Cancel;
            TaskDialogButton btnDelete = new TaskDialogButton("&Remove");

            var page = new TaskDialogPage()
            {
                Caption = "Remove mod",
                Heading = "Remove " + ModsManager.Instance.ModDetails[modKey].displayName + "?",
                Text = "This will delete the directory\r\n" + modKey,
                Icon = TaskDialogIcon.Warning,
                Buttons =
                {
                    btnCancel,
                    btnDelete,
                },
                AllowCancel = true
            };

            // Show a modal dialog, then check the result.
            TaskDialogButton result = TaskDialog.ShowDialog(this, page);

            if (result == btnDelete)
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

        private void toolStripMenuItemInstallFromFolder_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModsManager.Instance.ModSettingsTainted)
            {
                if (ShowChangesNeedToBeAppliedDialog())
                    ApplyModSettings();
                else
                    return;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a mod folder to install";
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
                    RefreshAll(true);
                }
            }
        }

        private bool ShowChangesNeedToBeAppliedDialog()
        {
            // Create the page which we want to show in the dialog.
            TaskDialogButton btnCancel = TaskDialogButton.Cancel;
            TaskDialogButton btnApply = new TaskDialogButton("&Apply");

            var page = new TaskDialogPage()
            {
                Caption = "MechWarrior 5 Load Order Configurator",
                Heading = "Apply pending changes to mod list?",
                Text = "Pending mod list changes need be applied before you can continue.",
                Buttons =
                {
                    btnCancel,
                    btnApply,
                }
            };

            // Show a modal dialog, then check the result.
            TaskDialogButton result = TaskDialog.ShowDialog(this, page);

            return result == btnApply;
        }

        private void toolStripMenuItemInstallArchive_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModsManager.Instance.ModSettingsTainted)
            {
                if (ShowChangesNeedToBeAppliedDialog())
                    ApplyModSettings();
                else
                    return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mod Archives|*.zip;*.7z;*.rar|All files (*.*)|*.*";
            openFileDialog.Title = "Select a mod archive to install";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedZipFile = openFileDialog.FileName;

                List<string> extractedModDirNames = ExtractModFromArchive(selectedZipFile);
                if (extractedModDirNames == null || extractedModDirNames.Count == 0)
                    return;
                RefreshAll(true);
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
            string updateUrl = _onlineUpdateUrl;
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = updateUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void modsListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == this.enabledHeader.Index)
            {
                e.NewWidth = this.modsListView.Columns[e.ColumnIndex].Width;
                e.Cancel = true;
            }
        }

        private void toTopToolStripButton_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MovePosition.Top);

            if (modsListView.SelectedItems.Count > 0)
                modsListView.SelectedItems[0].EnsureVisible();
        }

        private void toBottomToolStripButton_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MovePosition.Bottom);
            if (modsListView.SelectedItems.Count > 0)
                modsListView.SelectedItems[^1].EnsureVisible();

        }

        private void upToolStripButton_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MoveDirection.Up);
            if (modsListView.SelectedItems.Count > 0)
                modsListView.SelectedItems[0].EnsureVisible();

        }

        private void downToolStripButton_Click(object sender, EventArgs e)
        {
            MoveListItems(modsListView.SelectedItems, MoveDirection.Down);
            if (modsListView.SelectedItems.Count > 0)
                modsListView.SelectedItems[^1].EnsureVisible();

        }

        private void timerOverviewUpdateDelay_Tick(object sender, EventArgs e)
        {
            UpdateSidePanelData(_forceSidePanelUpdate);
            _forceSidePanelUpdate = false;
            timerOverviewUpdateDelay.Stop();
        }

        private void reloadModDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                ApplyModSettings();
                return;
            }

            if (e.Control && e.KeyCode == Keys.M)
            {
                LaunchGame();
                return;
            }

            if (e.Control && e.KeyCode == Keys.R)
            {
                RefreshAll(false);
                return;
            }

            if (e.Control && e.KeyCode == Keys.I)
            {
                ImportLoadOrder();
                return;
            }

            if (e.Control && e.KeyCode == Keys.E)
            {
                ExportLoadOrder();
                return;
            }
        }

        public void SetSelectedModEnabledState(bool newState)
        {
            bool tainted = false;
            modsListView.BeginUpdate();
            this._movingItems = true;
            foreach (ListViewItem selectedItem in modsListView.SelectedItems)
            {
                if (newState == selectedItem.Checked)
                    continue;

                tainted = true;
                selectedItem.Checked = newState;
                string key = selectedItem.Tag as string;
                ModsManager.Instance.ModEnabledList[key] = newState;
            }
            this._movingItems = false;

            if (tainted)
            {
                ModsManager.Instance.GetOverridingData(this.ModListData);
                UpdateModCountDisplay();
                RecomputeLoadOrdersAndUpdateList();
                SetModConfigTainted(true);
            }

            modsListView.EndUpdate();
        }

        private void enableModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSelectedModEnabledState(true);
        }

        private void disableModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSelectedModEnabledState(false);
        }

        private void modObjectListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            ModItem curModItem = (ModItem)e.Item.RowObject;
            e.Item.BackColor = curModItem.ProcessedRowBackColor;
        }

        private void modObjectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_movingItems)
                return;

            RecolorObjectListViewRows();

            //modObjectListView.RefreshObjects(ModItemList.Instance.ModList);

            /*foreach (OLVListItem curItem in modObjectListView.Items)
            {
                
                curItem.BackColor = Color.Red;
            }*/
        }

        private void modObjectListView_ModelDropped(object sender, ModelDropEventArgs e)
        {
            ModItem firstMod = e.SourceModels[0] as ModItem;
            int firstModIndex = modObjectListView.IndexOf(firstMod);

            if (firstModIndex == e.DropTargetIndex)
                return;

            int normalizedIndex = e.DropTargetIndex;
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
            {
                normalizedIndex++;
            }
            if (firstModIndex == normalizedIndex)
                return;

            foreach (ModItem droppedMod in e.SourceModels)
            {
                //ModItemList.Instance.ModList.Remove(droppedMod);
                //ModItemList.Instance.ModList.Insert(e.DropTargetIndex, droppedMod);
            }
            modObjectListView.BeginUpdate();
            _movingItems = true;
            modObjectListView.MoveObjects(normalizedIndex, e.SourceModels);

            modObjectListView.SelectObjects(e.SourceModels);

            ModItemList.Instance.RecomputeLoadOrders();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            modObjectListView.Sort();
            _movingItems = false;
            RecolorObjectListViewRows();
            modObjectListView.EndUpdate();
        }

        private void modObjectListView_BeforeSorting(object sender, BeforeSortingEventArgs e)
        {
            // Disable sorting
            e.Canceled = true;
        }

        private void modObjectListView_DragOver(object sender, DragEventArgs e)
        {
            // Simpledropsource sets this to false..
            modObjectListView.FullRowSelect = true;
        }
    }
}