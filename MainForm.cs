using Newtonsoft.Json;
using System;
using System.Collections;
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

        // Hash of the mod list currently applied to mechwarrior
        public int _ActiveModListHash = 0;
        public AsyncFileLoader _ModImageLoader = null;

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
            olvColumnModName.AspectGetter = this.ModNameGetter;
            olvColumnModAuthor.AspectGetter = this.ModAuthorGetter;
            olvColumnModVersion.AspectGetter = this.ModVersionGetter;
            olvColumnModCurLoadOrder.AspectGetter = this.ModCurLoadOrderGetter;
            olvColumnModOrgLoadOrder.AspectGetter = this.ModOrgLoadOrderGetter;
            olvColumnModFileSize.AspectGetter = this.ModFileSizeGetter;
            olvColumnModFileSize.AspectToStringConverter = FileSizeAspectConverter;
            olvColumnModFolder.AspectGetter = this.ModFolderGetter;

            olvColumnModName.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModAuthor.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModVersion.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModCurLoadOrder.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModOrgLoadOrder.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModFileSize.VisibilityChanged += OlvColumnVisibilityChanged;
            olvColumnModFolder.VisibilityChanged += OlvColumnVisibilityChanged;

            olvColumnModName.GroupKeyGetter += GroupKeyGetter;

            var dragSource = new ModDragSource();
            modObjectListView.DragSource = dragSource;

            // Selection
            RowBorderDecoration rbd = new RowBorderDecoration();
            rbd.BorderPen = new Pen(Color.FromArgb(0, 154, 223, 51));
            rbd.FillBrush = new SolidBrush(Color.FromArgb(65, 0, 143, 255));
            rbd.BoundsPadding = new Size(0, 0);
            rbd.CornerRounding = 0;
            modObjectListView.SelectedRowDecoration = rbd;

            // Hot item
            RowBorderDecoration rbdhot = new RowBorderDecoration();
            rbdhot.BorderPen = new Pen(Color.FromArgb(50, 0, 143, 255));
            rbdhot.BoundsPadding = new Size(0, 0);
            rbdhot.CornerRounding = 0;
            //rbd.FillBrush = new SolidBrush(Color.FromArgb(64, 0, 143, 255));
            rbdhot.FillGradientFrom = Color.FromArgb(16, 0, 143, 255);
            rbdhot.FillGradientTo = Color.FromArgb(16, 0, 143, 255);
            HotItemStyle his = new HotItemStyle();
            his.Decoration = rbdhot;
            modObjectListView.HotItemStyle = his;
            modObjectListView.UseHotItem = true;

            modObjectListView.BooleanCheckStateGetter = BooleanCheckStateGetter;

            bool BooleanCheckStateGetter(object rowobject)
            {
                ModItem curMod = (ModItem)rowobject;
                return curMod.Enabled;
            }

            modObjectListView.BooleanCheckStatePutter = delegate (Object rowObject, bool newValue)
            {
                ModItem curMod = (ModItem)rowObject;
                curMod.Enabled = newValue;

                var modItem = ModsManager.Instance.ModEnabledList.FirstOrDefault(x => 
                    x.ModPath.Equals(curMod.Path, StringComparison.InvariantCultureIgnoreCase));
                modItem.Enabled = newValue;

                ModsManager.Instance.UpdateNewModOverrideData(curMod);
                UpdateModCountDisplay();
                RecolorObjectListViewRows();
                modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
                CheckModConfigTainted();
                QueueSidePanelUpdate(true);
                return newValue; // return the value that you want the control to use
            };

            LocViewState._defaultViewState.WindowPosition = this.DesktopBounds;
            LocViewState._defaultViewState.listState = LocViewState.GetCurrentListViewState();
            if (LocViewState.LoadViewStateFromFile())
                LocViewState.RestoreViewState();

            UpdateColumnVisiblityMenu();

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

        private object ModFolderGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.FolderName;
        }

        private object ModFileSizeGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.FileSize;
        }

        private object ModOrgLoadOrderGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.OriginalLoadOrder;
        }

        private object ModCurLoadOrderGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.CurrentLoadOrder;
        }

        private object ModVersionGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.VersionCombined;
        }

        private object ModAuthorGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.Author;
        }

        private object GroupKeyGetter(object rowobject)
        {
            return 1;
        }

        private bool _delayedRecolorStarted = false;

        public void EnableModListDrop(bool enable)
        {
            if (enable)
            {
                if (modObjectListView.DropSink == null)
                {
                    if (!modObjectListView.FullRowSelect)
                        modObjectListView.FullRowSelect = true;
                    var dropSink = new ModDropSink();
                    dropSink.AcceptExternal = false;
                    dropSink.CanDropBetween = true;
                    dropSink.CanDropOnBackground = false;
                    dropSink.CanDropOnItem = false;
                    dropSink.CanDropOnSubItem = false;
                    dropSink.FeedbackColor = Color.Black;
                    dropSink.CanDrop += OnDropSinkOnCanDrop;
                    this.modObjectListView.DropSink = dropSink;
                }
            }
            else
            {
                modObjectListView.DropSink = null;
                if (!modObjectListView.FullRowSelect)
                    modObjectListView.FullRowSelect = true;
            }

        }

        void QueueListRecolor()
        {
            // We need to recolor the rows after column visibility has changed,
            // however we need to do that through a timer as otherwise the listview
            // gets redrawn to early
            if (!_delayedRecolorStarted)
            {
                _delayedRecolorStarted = true;
                modObjectListView.BeginUpdate();
                modObjectListView.SuspendDrawing();
            }

            timerDelayedListRecolor.Stop();
            timerDelayedListRecolor.Start();
        }

        private void OlvColumnVisibilityChanged(object sender, EventArgs e)
        {
            QueueListRecolor();
        }

        private void UpdateColumnVisiblityMenu()
        {
            authorColumnVisibilityToolStripMenuItem.Checked = olvColumnModAuthor.IsVisible;
            versionColumnVisibilityToolStripMenuItem.Checked = olvColumnModVersion.IsVisible;
            currentLoadOrderColumnVisibilityToolStripMenuItem.Checked = olvColumnModCurLoadOrder.IsVisible;
            originalLoadOrderColumnVisibilityToolStripMenuItem.Checked = olvColumnModOrgLoadOrder.IsVisible;
            fileSizeColumnVisibilityToolStripMenuItem.Checked = olvColumnModFileSize.IsVisible;
            modFolderColumnVisibilityToolStripMenuItem.Checked = olvColumnModFolder.IsVisible;
        }
        private void OnDropSinkOnCanDrop(object o, OlvDropEventArgs args)
        {
            if (_filterMode == eFilterMode.ItemFilter)
            {
                args.Effect = DragDropEffects.None;
                return;
            }
            args.Effect = DragDropEffects.Move;
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
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data is OLVDataObject)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

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

            if (e.Data is DataObject == false)
                return;

            //We only support single file drops!
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length != 1)
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

                if (fileExtension != ".zip" && fileExtension != ".rar" && fileExtension != ".7z")
                {
                    string message = "Archive format not supported. Supported formats are: .zip, rar, .7z\r\n" +
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
            var selectedItems = moveItems.Cast<OLVListItem>().ToList();
            bool anyMoved = false;
            selectedItems = selectedItems.OrderBy(i => this.modObjectListView.Items.IndexOf(i)).ToList();

            modObjectListView.BeginUpdate();
            _movingItems = true;

            if (direction == MoveDirection.Up)
            {
                List<ModItem> newList = new List<ModItem>();
                newList.Capacity = 1;
                for (int selectedItemIndex = 0; selectedItemIndex < selectedItems.Count; selectedItemIndex++)
                {
                    int currentIndex = selectedItems[selectedItemIndex].Index;
                    int newIndex = currentIndex - 1;

                    if (currentIndex == selectedItemIndex)
                        continue;

                    OLVListItem listItem = (OLVListItem)modObjectListView.Items[currentIndex];

                    ModItem curModItem = (ModItem)listItem.RowObject;
                    modObjectListView.RemoveObject(curModItem);
                    newList.Clear();
                    newList.Add(curModItem);
                    modObjectListView.InsertObjects(newIndex, newList);

                    ModItemList.Instance.ModList.Remove(curModItem);
                    ModItemList.Instance.ModList.Insert(newIndex, curModItem);

                    anyMoved = true;
                }
            }
            else
            {
                int endOffset = 1;
                List<ModItem> newList = new List<ModItem>();
                newList.Capacity = 1;

                for (int selectedItemIndex = selectedItems.Count - 1; selectedItemIndex >= 0; selectedItemIndex--)
                {
                    int currentIndex = selectedItems[selectedItemIndex].Index;
                    int newIndex = currentIndex + 1;

                    if (currentIndex == modObjectListView.Items.Count - endOffset++)
                        continue;

                    OLVListItem listItem = (OLVListItem)modObjectListView.Items[currentIndex];

                    ModItem curModItem = (ModItem)listItem.RowObject;
                    modObjectListView.RemoveObject(curModItem);
                    newList.Clear();
                    newList.Add(curModItem);
                    modObjectListView.InsertObjects(newIndex, newList);

                    ModItemList.Instance.ModList.Remove(curModItem);
                    ModItemList.Instance.ModList.Insert(newIndex, curModItem);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                ModItemList.Instance.RecomputeLoadOrders();

                ModsManager.Instance.RecomputeOverridingData();

                ColorListViewNumbers(olvColumnModCurLoadOrder.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
                RecolorObjectListViewRows();
                modObjectListView.UpdateObjects(ModItemList.Instance.ModList);

                QueueSidePanelUpdate(true);

                CheckModConfigTainted();
            }

            _movingItems = false;

            modObjectListView.EndUpdate();
        }

        public enum MovePosition { Top, Bottom };

        public void MoveListItems(ListView.SelectedListViewItemCollection moveItems, MovePosition position)
        {
            modObjectListView.BeginUpdate();
            _movingItems = true;
            bool anyMoved = false;
            var selectedItems = moveItems.Cast<OLVListItem>().ToList();
            selectedItems = selectedItems.OrderBy(i => this.modObjectListView.Items.IndexOf(i)).ToList();

            if (position == MovePosition.Top)
            {
                int listOffset = 0;
                List<ModItem> newList = new List<ModItem>();
                newList.Capacity = 1;
                foreach (var item in selectedItems)
                {
                    if (item.Index != listOffset)
                    {
                        ModItem curModItem = (ModItem)item.RowObject;

                        modObjectListView.RemoveObject(curModItem);
                        newList.Clear();
                        newList.Add(curModItem);
                        modObjectListView.InsertObjects(listOffset, newList);

                        ModItemList.Instance.ModList.Remove(curModItem);
                        ModItemList.Instance.ModList.Insert(listOffset, curModItem);

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
                    if (item.Index == modObjectListView.Items.Count - endOffset--)
                        continue;

                    ModItem curModItem = (ModItem)item.RowObject;
                    modObjectListView.RemoveObject(curModItem);
                    modObjectListView.AddObject(curModItem);

                    ModItemList.Instance.ModList.Remove(curModItem);
                    ModItemList.Instance.ModList.Add(curModItem);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                ModItemList.Instance.RecomputeLoadOrders();

                ModsManager.Instance.RecomputeOverridingData();
                ColorListViewNumbers(olvColumnModCurLoadOrder.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
                RecolorObjectListViewRows();
                modObjectListView.UpdateObjects(ModItemList.Instance.ModList);

                QueueSidePanelUpdate(true);
                CheckModConfigTainted();
            }

            _movingItems = false;

            modObjectListView.EndUpdate();
        }

        public void ApplyModSettings()
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            ModItemList.Instance.RecomputeLoadOrders();
            ModsManager.Instance.SaveToFiles();
            ModsManager.Instance.SaveLastAppliedModOrder();
            SetModConfigTainted(false);
            _ActiveModListHash = ModItemList.Instance.ModList.ComputeModListHashCode();
        }


        //For clearing the entire applications data
        public void ClearAll()
        {
            _ActiveModListHash = 0;
            listBoxOverriding.Items.Clear();
            listBoxOverriddenBy.Items.Clear();
            richTextBoxManifestOverridden.Clear();
            pictureBoxModImage.Visible = false;
            labelModNameOverrides.Text = "";
            this.modObjectListView.ClearObjects();
            this.modObjectListView.ClearCachedInfo();
            ModItemList.Instance.ModList.Clear();
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
        public void LoadAndFill(List<ModImportData> desiredMods, bool orderByDesired)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            bool prevLoadingAndFilling = LoadingAndFilling;
            this.LoadingAndFilling = true;

            //try
            {
                //ModsManager.Instance.ModEnabledList = modList;

                ModsManager.Instance.InitModEnabledList();

                List<ModImportData> orderedModList;
                // Sort by mechwarrior load order
                if (!orderByDesired)
                {
                    orderedModList = ModsManager.Instance.ModEnabledList;
                    orderedModList.Sort((x, y) =>
                    {
                        // Compare load order
                        int priorityComparison = ModsManager.Instance.ModDetails[y.ModPath].defaultLoadOrder
                            .CompareTo(ModsManager.Instance.ModDetails[x.ModPath].defaultLoadOrder);

                        // If Priority is equal, compare Folder name
                        if (priorityComparison == 0)
                        {
                            return String
                                .Compare(ModsManager.Instance.PathToDirNameDict[y.ModPath].ToString(), ModsManager.Instance.PathToDirNameDict[x.ModPath], StringComparison.InvariantCultureIgnoreCase);
                        }

                        return priorityComparison;
                    });
                }
                else
                {
                    orderedModList = ModsManager.Instance.ModEnabledList.ToList();
                    ModUtils.SwapModsToMatchFilter(ref orderedModList, desiredMods);
                }

                // set all mods to desired enabled states
                if (desiredMods != null)
                {
                    foreach (var curDesiredMod in desiredMods)
                    {
                        var curTargetItem = ModsManager.Instance.ModEnabledList.FirstOrDefault(x => 
                            x.ModPath.Equals(curDesiredMod.ModPath, StringComparison.InvariantCultureIgnoreCase));
        
                        if (curTargetItem != null)
                        {
                            curTargetItem.Enabled = curDesiredMod.Enabled;
                        }
                    }
                }

                // Get enabled mods from desired list
                for (int i = 0; i < orderedModList.Count; i++)
                {
                    bool newState = false;
                    var curModListItem = orderedModList[i];

                    var curTargetItem = ModsManager.Instance.ModEnabledList.FirstOrDefault(x => 
                        x.ModPath.Equals(curModListItem.ModPath, StringComparison.InvariantCultureIgnoreCase));

                    if (desiredMods != null && curTargetItem != null)
                    {
                        var curDesiredItem = desiredMods.FirstOrDefault(x => 
                            x.ModPath.Equals(curModListItem.ModPath, StringComparison.InvariantCultureIgnoreCase));

                        if (curDesiredItem == null)
                        {
                            continue;
                        }

                        newState = curDesiredItem.Enabled;
                    }

                    orderedModList[i].Enabled = newState;
                }

                modObjectListView.BeginUpdate();
                modObjectListView.ClearObjects();
                ModItemList.Instance.ModList.Clear();
                foreach (var entry in orderedModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
                {
                    ModItem newItem = new ModItem();
                    newItem.Enabled = entry.Enabled;
                    newItem.Path = entry.ModPath;
                    newItem.Name = ModsManager.Instance.ModDetails[entry.ModPath].displayName;
                    newItem.FolderName = ModsManager.Instance.PathToDirNameDict[entry.ModPath];
                    newItem.FileSize = ModsManager.Instance.Mods[entry.ModPath].ModFileSize;
                    newItem.Author = ModsManager.Instance.ModDetails[entry.ModPath].author;
                    newItem.CurrentLoadOrder = ModsManager.Instance.Mods[entry.ModPath].NewLoadOrder;
                    newItem.OriginalLoadOrder = ModsManager.Instance.Mods[entry.ModPath].OriginalLoadOrder;
                    newItem.Origin = ModsManager.Instance.Mods[entry.ModPath].Origin;

                    newItem.Version = ModsManager.Instance.ModDetails[entry.ModPath].version;
                    newItem.BuildNumber = ModsManager.Instance.ModDetails[entry.ModPath].buildNumber;
                    string versionString = (ModsManager.Instance.ModDetails[entry.ModPath].version + " (" +
                                            ModsManager.Instance.ModDetails[entry.ModPath].buildNumber.ToString() + ")").Trim();

                    newItem.VersionCombined = versionString;

                    ModItemList.Instance.ModList.Add(newItem);
                    modObjectListView.AddObject(newItem);
                }
                RecolorObjectListViewRows();
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
            ModItemList.Instance.RecomputeLoadOrders();

            modObjectListView.BeginUpdate();
            ModsManager.Instance.RecomputeOverridingData();
            ColorListViewNumbers(olvColumnModCurLoadOrder.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
            RecolorObjectListViewRows();
            modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            modObjectListView.EndUpdate();
            UpdateModCountDisplay();
        }

        public object ModNameGetter(object rowObject)
        {
            ModItem s = (ModItem)rowObject;
            return s.Name;
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
        public void RefreshAll(bool forceLoadLastApplied = false)
        {
            Cursor tempCursor = Cursor.Current;
            Cursor.Current = Cursors.AppStarting;
            modObjectListView.BeginUpdate();

            Point prevPosition = modObjectListView.LowLevelScrollPosition;
            List<string> prevSelected = new List<string>(modObjectListView.SelectedObjects.Count);
            foreach (ModItem selected in modObjectListView.SelectedObjects)
            {
                prevSelected.Add(selected.Path);
            }

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
                List<ModImportData> modlist = ModsManager.Instance.LoadModList();
                if (modlist != null)
                {
                    ModsManager.Instance.ProcessModImportList(ref modlist, false);
                    ModsManager.Instance.ModEnabledListLastState = modlist;
                }
                ModsManager.Instance.DetermineBestAvailableGameVersion();

                // Check if we want to load the last applied mod list

                if (!forceLoadLastApplied)
                    LoadAndFill(modlist, false);

                _ActiveModListHash = ModItemList.Instance.ModList.ComputeModListHashCode();

                ModsManager.Instance.LoadLastAppliedPresetData();

                if (forceLoadLastApplied || ModsManager.Instance.ShouldLoadLastApplied())
                {
                    // Load last saved preset
                    modlist = ModsManager.Instance.LastAppliedPresetModList;

                    LoadAndFill(modlist, true);

                    if (_ActiveModListHash != ModItemList.Instance.ModList.ComputeModListHashCode())
                        modConfigTainted = true;
                }

                FilterTextChanged();
                ModsManager.Instance.RecomputeOverridingData();
            }
            LoadPresets();
            SetModConfigTainted(modConfigTainted);

            foreach (OLVListItem curListItem in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)curListItem.RowObject;

                if (prevSelected.Contains(curModItem.Path))
                {
                    curListItem.Selected = true;
                }
            }

            modObjectListView.EndUpdate();
            modObjectListView.LowLevelScroll(prevPosition.X, prevPosition.Y);
            Cursor.Current = tempCursor;
        }

        //Saves current load order to preset.
        public void SavePreset(string name)
        {
            Dictionary<string, bool> NoPathModlist = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in ModsManager.Instance.ModEnabledList)
            {
                NoPathModlist[entry.ModFolder] = entry.Enabled;
            }
            ModsManager.Instance.Presets[name] = JsonConvert.SerializeObject(NoPathModlist, Formatting.Indented);
            ModsManager.Instance.SavePresets();
        }

        //Sets up the load order from a preset.
        private void LoadFromPreset(string name)
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

            List<string> prevSelected = new List<string>(modObjectListView.SelectedItems.Count);
            foreach (ModItem selected in modObjectListView.SelectedObjects)
            {
                prevSelected.Add(selected.Path);
            }

            modObjectListView.BeginUpdate();
            modObjectListView.ClearObjects();
            modObjectListView.ClearCachedInfo();
            ModItemList.Instance.ModList.Clear();
            ModsManager.Instance.ModDetails = new Dictionary<string, ModObject>();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.Mods.Clear();

            ModsManager.Instance.ParseDirectories();
            ModsManager.Instance.ReloadModData();
            List<ModImportData> newPresetData = new List<ModImportData>();
            foreach (var curPresetEntry in presetData)
            {
                ModImportData newImportData = new ModImportData();
                newImportData.ModFolder = curPresetEntry.Key;
                newImportData.Enabled = curPresetEntry.Value;

                newPresetData.Add(newImportData);
            }

            ModsManager.Instance.ProcessModImportList(ref newPresetData, true);
            this.LoadAndFill(newPresetData, true);
            FilterTextChanged();
            CheckModConfigTainted();
            modObjectListView.EndUpdate();

            bool firstSelected = true;

            foreach (OLVListItem curListItem in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)curListItem.RowObject;

                if (prevSelected.Contains(curModItem.Path))
                {
                    curListItem.Selected = true;
                    if (firstSelected)
                    {
                        firstSelected = false;
                        curListItem.EnsureVisible();
                    }
                }
            }
            UpdateSidePanelData(true);
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
            bool anySelected = modObjectListView.SelectedObjects.Count > 0;
            bool enabled = anySelected && _filterMode != eFilterMode.ItemFilter;
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
                    modObjectListView.UseFiltering = false;
                    modObjectListView.ModelFilter = null;

                    this._filterMode = eFilterMode.None;
                }
                else
                {
                    return;
                }

                searchFailed = false;
            }
            else
            {
                modObjectListView.ModelFilter = TextMatchFilter.Contains(modObjectListView, filtertext);
                if (!Instance.toolStripButtonFilterToggle.Checked)
                {
                    modObjectListView.UseFiltering = false;
                    // ensure that first found item is visible
                    if (modObjectListView.ModelFilter != null)
                    {
                        foreach (object originalObject in modObjectListView.Objects)
                        {
                            if (modObjectListView.ModelFilter.Filter(originalObject))
                            {
                                modObjectListView.EnsureModelVisible(originalObject);
                                searchFailed = false;
                                break;
                            }
                        }
                    }

                    _filterMode = eFilterMode.ItemHighlight;
                }
                //We are filtering by selected adding.
                else
                {
                    modObjectListView.UseFiltering = true;
                    if (modObjectListView.ModelFilter != null)
                    {
                        foreach (object originalObject in modObjectListView.Objects)
                        {
                            if (modObjectListView.ModelFilter.Filter(originalObject))
                            {
                                searchFailed = false;
                                break;
                            }
                        }
                    }

                    _filterMode = eFilterMode.ItemFilter;
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

            modObjectListView.BeginUpdate();
            modObjectListView.Invalidate();
            RecolorObjectListViewRows();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            modObjectListView.EndUpdate();

            //While filtering disable the up/down buttons (tough this should no longer be needed).
            UpdateMoveControlEnabledState();
        }

        //Check if given listviewitem can be matched to a string.
        private bool MatchItemToText(string filtertext, ListViewItem item)
        {
            if
                (
                    item.SubItems[olvColumnModName.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[olvColumnModFolder.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[olvColumnModAuthor.Index].Text.ToLower().Contains(filtertext)
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
                    modObjectListView.BeginUpdate();
                    RecolorObjectListViewRows();
                }

                if (listBoxOverriddenBy.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriding.SelectedIndex = -1;
                if (listBoxOverriddenBy.Items.Count == 0 || modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriddenBy.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriddenBy.SelectedItem;

                if (_filterMode == eFilterMode.None)
                {
                    HighlightModInList(selectedMod.ModKey);
                }

                string superMod = ModsManager.Instance.PathToDirNameDict[_sideBarSelectedModKey];

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
                    modObjectListView.EndUpdate();
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
                    modObjectListView.BeginUpdate();
                    richTextBoxManifestOverridden.SuspendDrawing();
                    RecolorObjectListViewRows();
                }

                if (listBoxOverriding.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriddenBy.SelectedIndex = -1;
                if (listBoxOverriding.Items.Count == 0 || modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriding.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriding.SelectedItem;

                if (_filterMode == eFilterMode.None)
                {
                    HighlightModInList(selectedMod.ModKey);
                }

                string superMod = ModsManager.Instance.PathToDirNameDict[_sideBarSelectedModKey];

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
                    modObjectListView.EndUpdate();
                }
            }
        }

        private void UpdateSidePanelData(bool forceUpdate)
        {
            if (modObjectListView.SelectedObjects.Count == 0)
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

            ModItem firstSelectedMod = (ModItem)modObjectListView.SelectedObjects[0];
            string selectedModPath = firstSelectedMod.Path;

            if (!forceUpdate && _sideBarSelectedModKey == selectedModPath)
                return;

            _sideBarSelectedModKey = selectedModPath;

            string selectedModFolder = firstSelectedMod.FolderName;
            // Make sure displayed label doesn't convert & to underscore
            string selectedModLabelDisplayName = firstSelectedMod.Name;
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

            if (_ModImageLoader != null)
            {
                _ModImageLoader.CancelLoad();
                _ModImageLoader = null;
            }

            pictureBoxModImage.Visible = false;

            if (File.Exists(imagePath))
            {
                _ModImageLoader = new AsyncFileLoader();
                //_ModImageLoader.ToggleDebugMode(true);

                Action<MemoryStream> onFileLoaded = (memoryStream) =>
                {
                    if (pictureBoxModImage.Image != null)
                    {
                        pictureBoxModImage.Image.Dispose();
                        pictureBoxModImage.Image = null;
                    }

                    try
                    {
                        pictureBoxModImage.Image = Image.FromStream(memoryStream);
                        pictureBoxModImage.Visible = true;
                    }
                    catch (Exception e)
                    {
                        
                    }
                };

                _ModImageLoader.LoadFileAsync(imagePath, onFileLoaded);
            }
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

            List<ModImportData> newData = importDialog.ResultData;
            ModsManager.Instance.ProcessModImportList(ref newData, true);

            importDialog.Dispose();

            if (!ModsManager.Instance.GameIsConfigured())
                return;

            modObjectListView.BeginUpdate();
            //this.ClearAll();
            List<string> prevSelected = new List<string>(modObjectListView.SelectedItems.Count);
            foreach (ModItem selected in modObjectListView.SelectedObjects)
            {
                prevSelected.Add(selected.Path);
            }
            modObjectListView.ClearObjects();
            modObjectListView.ClearCachedInfo();
            ModItemList.Instance.ModList.Clear();
            ModsManager.Instance.ModDetails.Clear();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.Mods.Clear();
            ModsManager.Instance.ParseDirectories();
            ModsManager.Instance.ReloadModData();
            ModsManager.Instance.DetermineBestAvailableGameVersion();
            this.LoadAndFill(newData, true);
            FilterTextChanged();
            CheckModConfigTainted();
            foreach (OLVListItem curListItem in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)curListItem.RowObject;

                if (prevSelected.Contains(curModItem.Path))
                {
                    curListItem.Selected = true;
                }
            }
            QueueSidePanelUpdate(true);
            modObjectListView.EndUpdate();
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

            modObjectListView.BeginUpdate();
            try
            {
                foreach (ModItem curModItem in ModItemList.Instance.ModList)
                {
                    curModItem.Enabled = true;
                }

                modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            }
            finally
            {
                modObjectListView.EndUpdate();
            }

            ModsManager.Instance.RecomputeOverridingData();
            UpdateModCountDisplay();
            RecolorObjectListViewRows();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            CheckModConfigTainted();
        }

        private void disableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (AreAllModsDisabled())
                return;

            modObjectListView.BeginUpdate();
            try
            {
                foreach (ModItem curModItem in ModItemList.Instance.ModList)
                {
                    curModItem.Enabled = false;
                }

                modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            }
            finally
            {
                modObjectListView.EndUpdate();
            }

            ModsManager.Instance.RecomputeOverridingData();
            UpdateModCountDisplay();
            RecolorObjectListViewRows();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            CheckModConfigTainted();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (OLVListItem selectedItem in modObjectListView.SelectedItems)
            {
                ModItem curModItem = (ModItem)selectedItem.RowObject;
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = curModItem.Path,
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
            string modKey = _sideBarSelectedModKey;
            string modUrl = ModsManager.Instance.ModDetails[modKey].authorURL;
            bool isValidUrl = Utils.IsUrlValid(modUrl);
            if (isValidUrl)
            {
                Process.Start(modUrl);
            }
        }

        private void linkLabelSteamId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = _sideBarSelectedModKey;
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

        private void DragDropObjectRows(int insertIndex, IList draggedItems)
        {
            // More or less a copy of OLVs Move function with a fix when moving multiple item (originalInsertIndex comparison)
            modObjectListView.BeginUpdate();
            List<int> intList = new List<int>();
            int originalInsertIndex = insertIndex;
            foreach (object modelObject in draggedItems)
            {
                if (modelObject != null)
                {
                    int num = modObjectListView.IndexOf(modelObject);
                    if (num >= 0)
                    {
                        intList.Add(num);
                        if (num <= originalInsertIndex)
                            --insertIndex;
                    }
                }
            }
            intList.Sort();
            intList.Reverse();
            try
            {
                modObjectListView.BeginUpdate();
                foreach (int index1 in intList)
                    modObjectListView.Items.RemoveAt(index1);
                modObjectListView.InsertObjects(insertIndex, draggedItems);
            }
            finally
            {
                modObjectListView.EndUpdate();
            }

            ModItemList.Instance.RecomputeLoadOrders();

            ModsManager.Instance.RecomputeOverridingData();

            modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            RecolorObjectListViewRows();
            ColorListViewNumbers(olvColumnModCurLoadOrder.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
            modObjectListView.EndUpdate();

            CheckModConfigTainted();
        }

        private void presetMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem presetMenuItem = sender as ToolStripMenuItem;
            this.LoadFromPreset(presetMenuItem.Tag.ToString());
        }

        private void savePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modObjectListView.Items.Count == 0)
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
            string modKey = _sideBarSelectedModKey;
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
            modObjectListView.DeselectAll();
            foreach (OLVListItem modListItem in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)modListItem.RowObject;

                if (curModItem.Path == modKey)
                {
                    modListItem.Selected = true;
                    modObjectListView.EnsureVisible(modListItem.Index);
                    break;
                }
            }
        }

        public void HighlightModInList(string modKey)
        {
            foreach (OLVListItem modListItem in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)modListItem.RowObject;

                if (curModItem.Path == modKey)
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
            bool showModOverrides = modObjectListView.SelectedObjects.Count == 1 && _filterMode != eFilterMode.ItemFilter;

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

                /*if (_filterMode == eFilterMode.ItemHighlight)
                {
                    string filtertext = toolStripTextFilterBox.Text.ToLower();
                    if (!string.IsNullOrWhiteSpace(filtertext) && MatchItemToText(filtertext, curItem))
                    {
                        if (!alternateColor)
                            newBackColor = _highlightColor;
                        else
                            newBackColor = _highlightColorAlternate;
                    }
                }*/

                // Color mod overrides following the currently selected mod
                if (showModOverrides)
                {
                    ModItem firstSelectedItem = (ModItem)modObjectListView.SelectedObjects[0];
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

        private int GetModCount(bool enabledOnly)
        {
            int count = 0;
            if (enabledOnly)
            {
                foreach (var curMod in ModsManager.Instance.ModEnabledList)
                {
                    if (curMod.Enabled) { count++; }
                }
            }
            else
            {
                count = ModsManager.Instance.Mods.Count;
            }

            return count;
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

        // Taint config if current mod list differs from the config on the disk
        public void CheckModConfigTainted()
        {
            SetModConfigTainted(_ActiveModListHash != ModItemList.Instance.ModList.ComputeModListHashCode());
        }

        public void SetModConfigTainted(bool modSettingsTainted)
        {
            if (ModsManager.Instance.ModSettingsTainted == modSettingsTainted)
                return;

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
            if (ModsManager.Instance.ModSettingsTainted)
            {
                var result = ShowUnappliedChangesDialog();

                if (result == eUnappliedChangesDialogResult.Apply)
                {
                    ApplyModSettings();
                }
                else if (result == eUnappliedChangesDialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            LocViewState.SaveCurrentState();
        }

        private void contextMenuItemMoveToTop_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MovePosition.Top);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[0]);
        }

        private void contextMenuItemMoveToBottom_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MovePosition.Bottom);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[^1]);
        }

        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveListItems(modObjectListView.SelectedItems, MoveDirection.Up);
        }

        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MoveListItems(modObjectListView.SelectedItems, MoveDirection.Down);
        }

        //Color the list view items based on data
        public void ColorizeListViewItems()
        {
            modObjectListView.BeginUpdate();
            ColorListViewNumbers(olvColumnModCurLoadOrder.Index, LowPriorityColor, HighPriorityColor);
            ColorListViewNumbers(olvColumnModOrgLoadOrder.Index, LowPriorityColor, HighPriorityColor);
            modObjectListView.EndUpdate();
        }

        public void ColorListViewNumbers(int subItemIndex, Color fromColor, Color toColor)
        {
            if (subItemIndex == -1)
                return;

            List<float> numbers = new List<float>();

            // Extract numbers from ListView column and find unique ones
            foreach (OLVListItem item in modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)item.RowObject;

                // Skip disabled mods
                if (!curModItem.Enabled)
                    continue;

                if (float.TryParse(item.SubItems[subItemIndex].Text, out var number))
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
            modObjectListView.BeginUpdate();
            for (int i = 0; i < modObjectListView.Items.Count; i++)
            {
                OLVListItem curListItem = (OLVListItem)modObjectListView.Items[i];
                ModItem curModItem = (ModItem)curListItem.RowObject;

                // Skip disabled mods
                if (!curModItem.Enabled)
                    continue;

                if (float.TryParse(curListItem.SubItems[subItemIndex].Text, out var number))
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
                    curListItem.SubItems[subItemIndex].ForeColor = newColor;

                    // A bit backwards. Have to refactor the function
                    if (subItemIndex == olvColumnModOrgLoadOrder.Index)
                    {
                        curModItem.ProcessedOrgLoForeColor = newColor;
                    }
                    else if (subItemIndex == olvColumnModCurLoadOrder.Index)
                    {
                        curModItem.ProcessedCurLoForeColor = newColor;
                    }
                }
            }
            modObjectListView.EndUpdate();
        }

        private void toolStripMenuItemSortDefaultLoadOrder_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModItemList.Instance.AreModsSortedByDefaultLoadOrder())
                return;

            // This sorting follows the way MW5 orders its list
            ModItemList.Instance.ModList.Sort((x, y) =>
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    (x, y) = (y, x);
                }

                // Compare Original load order
                int priorityComparison = x.OriginalLoadOrder.CompareTo(y.OriginalLoadOrder);

                // If Priority is equal, compare Folder name
                if (priorityComparison == 0)
                {
                    return String.Compare(x.FolderName, y.FolderName, StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    return priorityComparison;
                }
            });

            modObjectListView.BeginUpdate();
            IList prevSelected = modObjectListView.SelectedObjects;
            modObjectListView.ClearObjects();
            modObjectListView.AddObjects(ModItemList.Instance.ModList);

            ModItemList.Instance.RecomputeLoadOrders();

            ColorListViewNumbers(olvColumnModCurLoadOrder.Index, ModsManager.LowPriorityColor, ModsManager.HighPriorityColor);
            RecolorObjectListViewRows();
            ModsManager.Instance.RecomputeOverridingData();
            modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            FilterTextChanged();
            CheckModConfigTainted();

            modObjectListView.SelectedObjects = prevSelected;
            if (prevSelected.Count > 0)
            {
                modObjectListView.EnsureModelVisible(prevSelected[0]);
            }

            modObjectListView.EndUpdate();
            QueueSidePanelUpdate(true);
        }

        public bool AreAllModsEnabled()
        {
            for (int i = 1; i < modObjectListView.Items.Count; i++)
            {
                if (!modObjectListView.Items[i].Checked)
                    return false;
            }

            return true;
        }

        public bool AreAllModsDisabled()
        {
            for (int i = 1; i < modObjectListView.Items.Count; i++)
            {
                if (modObjectListView.Items[i].Checked)
                    return false;
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
            FilterTextChanged();

            if (toolStripButtonFilterToggle.Checked)
            {
                toolStripTextFilterBox.CueBanner = "Filter";
            }
            else
            {
                toolStripTextFilterBox.CueBanner = "Search";
            }
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
            foreach (OLVListItem selectedItem in modObjectListView.SelectedItems)
            {
                ModItem curModItem = (ModItem)selectedItem.RowObject;
                DeleteMod(curModItem.Path);
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

        private void toTopToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MovePosition.Top);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[0]);
        }

        private void toBottomToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MovePosition.Bottom);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[^1]);
        }

        private void upToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MoveDirection.Up);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[0]);
        }

        private void downToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MoveListItems(modObjectListView.SelectedItems, MoveDirection.Down);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[^1]);
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
            modObjectListView.BeginUpdate();
            this._movingItems = true;
            foreach (OLVListItem selectedItem in modObjectListView.SelectedItems)
            {
                if (newState == selectedItem.Checked)
                    continue;

                selectedItem.Checked = newState;
            }
            this._movingItems = false;

            modObjectListView.EndUpdate();
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
            e.UseCellFormatEvents = true;
        }

        private void modObjectListView_FormatCell(object sender, FormatCellEventArgs e)
        {
            ModItem modItem = (ModItem)e.Model;
            if (!modItem.Enabled)
            {
                e.SubItem.ForeColor = Color.FromArgb(142, 140, 142);
                return;
            }

            if (e.ColumnIndex == this.olvColumnModName.Index)
            {
                if (ModsManager.Instance.OverridingData.ContainsKey(modItem.FolderName))
                {
                    OverridingData a = ModsManager.Instance.OverridingData[modItem.FolderName];
                    Color newItemColor = SystemColors.WindowText;
                    if (a.isOverridden)
                    {
                        newItemColor = OverriddenColor;
                    }
                    if (a.isOverriding)
                    {
                        newItemColor = OverridingColor;
                    }
                    if (a.isOverriding && a.isOverridden)
                    {
                        newItemColor = OverriddenOveridingColor;
                    }

                    e.SubItem.ForeColor = newItemColor;
                }
            }
            else if (e.ColumnIndex == this.olvColumnModCurLoadOrder.Index)
            {
                e.SubItem.ForeColor = modItem.ProcessedCurLoForeColor;
            }
            else if (e.ColumnIndex == this.olvColumnModOrgLoadOrder.Index)
            {
                e.SubItem.ForeColor = modItem.ProcessedOrgLoForeColor;
            }
        }

        private void modObjectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMoveControlEnabledState();

            if (_movingItems)
                return;

            QueueSidePanelUpdate(false);

            RecolorObjectListViewRows();
        }

        private void modObjectListView_ModelDropped(object sender, ModelDropEventArgs e)
        {
            foreach (ModItem curSourceModItem in e.SourceModels)
            {
                int sourceItemIndex = modObjectListView.IndexOf(curSourceModItem);
                if (e.DropTargetLocation == DropTargetLocation.BelowItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex)
                        return;

                    if (sourceItemIndex - 1 == e.DropTargetIndex)
                        return;
                }
                else if (e.DropTargetLocation == DropTargetLocation.AboveItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex)
                        return;

                    // Next item after last selected
                    if (sourceItemIndex + 1 == e.DropTargetIndex)
                        return;
                }
                else
                {
                    return;
                }
            }

            int normalizedIndex = e.DropTargetIndex;
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
            {
                normalizedIndex++;
            }

            foreach (ModItem droppedMod in e.SourceModels)
            {
                ModItemList.Instance.ModList.Remove(droppedMod);
                ModItemList.Instance.ModList.Insert(e.DropTargetIndex, droppedMod);
            }
            modObjectListView.BeginUpdate();
            _movingItems = true;
            //modObjectListView.MoveObjects(normalizedIndex, e.SourceModels);

            DragDropObjectRows(normalizedIndex, e.SourceModels);

            modObjectListView.SelectObjects(e.SourceModels);
            ModItemList.Instance.RecomputeLoadOrders();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            QueueSidePanelUpdate(true);
            _movingItems = false;
            RecolorObjectListViewRows();
            modObjectListView.EndUpdate();
        }

        private void modObjectListView_BeforeSorting(object sender, BeforeSortingEventArgs e)
        {
            // Disable sorting
            //e.Canceled = true;
        }

        private void modObjectListView_DragOver(object sender, DragEventArgs e)
        {
            // Simpledropsource sets this to false..
            if (!modObjectListView.FullRowSelect)
                modObjectListView.FullRowSelect = true;
        }

        private void modObjectListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = modObjectListView.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStripMod.Show(Cursor.Position);
                }
            }
        }

        private void timerDelayedListRecolor_Tick(object sender, EventArgs e)
        {
            ColorizeListViewItems();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            RecolorObjectListViewRows();
            timerDelayedListRecolor.Stop();
            modObjectListView.EndUpdate();
            modObjectListView.ResumeDrawing();
            _delayedRecolorStarted = false;
        }

        private void columnVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            menuItem.Checked = !menuItem.Checked;

            olvColumnModAuthor.IsVisible = authorColumnVisibilityToolStripMenuItem.Checked;
            olvColumnModVersion.IsVisible = versionColumnVisibilityToolStripMenuItem.Checked;
            olvColumnModCurLoadOrder.IsVisible = currentLoadOrderColumnVisibilityToolStripMenuItem.Checked;
            olvColumnModOrgLoadOrder.IsVisible = originalLoadOrderColumnVisibilityToolStripMenuItem.Checked;
            olvColumnModFileSize.IsVisible = fileSizeColumnVisibilityToolStripMenuItem.Checked;
            olvColumnModFolder.IsVisible = modFolderColumnVisibilityToolStripMenuItem.Checked;

            modObjectListView.RebuildColumns();

            UpdateColumnVisiblityMenu();
        }

        private void modObjectListView_ColumnRightClick(object sender, ColumnClickEventArgs e)
        {
            contextMenuStripColumnOptions.Show(Cursor.Position);
        }

        private void restoreDefaultColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LocViewState.RestoreListViewState(LocViewState._defaultViewState.listState);
            UpdateColumnVisiblityMenu();

            QueueListRecolor();
        }

        private void modObjectListView_AboutToCreateGroups(object sender, CreateGroupsEventArgs e)
        {
            // With this trick we have group without header.
            // Point being that the list is smoothly scrollable when groups are used
            foreach (OLVGroup group in e.Groups)
            {
                group.State ^= GroupState.LVGS_NOHEADER;
                group.StateMask ^= GroupState.LVGS_NOHEADER;
            }
        }

        private void modObjectListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            e.Parameters.PrimarySortOrder = SortOrder.None;
        }

        private void checkModFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModCheckForm checkDialog = new ModCheckForm();
            checkDialog.ShowDialog();
        }
    }
}