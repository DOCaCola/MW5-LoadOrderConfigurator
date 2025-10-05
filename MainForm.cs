using BrightIdeasSoftware;
using DarkModeForms;
using MW5_Mod_Manager.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static MW5_Mod_Manager.ModsManager;
using File = System.IO.File;
using ListView = System.Windows.Forms.ListView;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class MainForm : Form
    {
        static public MainForm Instance;

        public enum eFilterMode
        {
            None,
            ItemFilter,
            ItemHighlight
        }

        public eFilterMode _filterMode = eFilterMode.None;
        public bool _movingItems = false;
        string _onlineUpdateUrl = LocConstants.UrlNexusmods;
        // The mod currently displayed in the sidebar
        private static string _sideBarSelectedModKey = null;
        // Force next sidepanel update to execute
        private bool _forceSidePanelUpdate = false;
        // Mod files differ from the state displayed in the UI
        private bool _modFileStateMismatch = false;

        // Hash of the mod list currently applied to mechwarrior
        public int _ActiveModListHash = 0;
        public AsyncFileLoader _ModImageLoader = null;

        
        public static string GetSidebarSelectedModPath()
        {
            return _sideBarSelectedModKey;
        }

        public static ModObject GetSidebarSelectedModDetails()
        {
            string modKey = _sideBarSelectedModKey;
            return ModsManager.Instance.ModDetails[modKey];
        }

        public static ModData GetSidebarSelectedModData()
        {
            string modKey = _sideBarSelectedModKey;
            return ModsManager.Instance.Mods[modKey];
        }

        public static ModConflictData GetSidebarSelectedModConflictData()
        {
            string modDirName = ModsManager.Instance.PathToDirNameDict[_sideBarSelectedModKey];

            ModsManager.Instance.ModConflictData.TryGetValue(modDirName, out ModConflictData modData);
            return modData;
        }

        public MainForm()
        {
            InitializeComponent();

            DockModListForm.Instance = new();
            DockOverviewForm.Instance = new();
            DockConflictsForm.Instance = new();
            
            Instance = this;

            toolStripTextFilterBox.TextBox.PreviewKeyDown += FilterTextBoxOnPreviewKeyDown;
            toolStripTextFilterBox.TextBox.KeyPress += FilterTextBoxOnKeyPress;

            dockPanel1.SuspendLayout();

            if (LocWindowColors.DarkMode)
            {
                var darkTheme = new LocDarkTheme();
                dockPanel1.Theme = darkTheme;

                menuStrip1.SetDisableDarkMode(true);
                menuStrip1.SetDisableDarkModeChildren(true);
                toolStrip1.SetDisableDarkMode(true);
                toolStrip1.SetDisableDarkModeChildren(true);
                var darkModeCs = new DarkModeCS(this, false);
                darkModeCs.ThemeControl(toolStripTextFilterBox.Control);


                DockModListForm.Instance.modObjectListView.HeaderUsesThemes = false;
                var headerstyleb = new HeaderFormatStyle();
                headerstyleb.SetBackColor(LocWindowColors.ButtonHighlight);
                headerstyleb.SetForeColor(LocWindowColors.WindowText);
                DockModListForm.Instance.modObjectListView.HeaderFormatStyle = headerstyleb;

                toolStripButtonStartGame.Image = UiIconsDark.MW5MercsLogo;

                DockModListForm.Instance.toBottomToolStripButton.Image = UiIconsDark.Bottom;
                DockModListForm.Instance.toTopToolStripButton.Image = UiIconsDark.Top;
                DockModListForm.Instance.downToolStripButton.Image = UiIconsDark.Down;
                DockModListForm.Instance.upToolStripButton.Image = UiIconsDark.Up;

                DockModListForm.Instance.olvColumnFreeSpaceDummy.IsVisible = true;

                visualStudioToolStripExtender1.SetStyle(menuStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, darkTheme);
                visualStudioToolStripExtender1.SetStyle(toolStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, darkTheme);
                visualStudioToolStripExtender1.SetStyle(DockModListForm.Instance.toolStrip2, VisualStudioToolStripExtender.VsVersion.Vs2015, darkTheme);

                contextMenuStripColumnOptions.Renderer = darkTheme.ToolStripRenderer;
                contextMenuStripMod.Renderer = darkTheme.ToolStripRenderer;
            }
            else
            {
                var lightTheme = new LocLightTheme();
                dockPanel1.Theme = lightTheme;
                toolStrip1.RenderMode = ToolStripRenderMode.Professional;
                visualStudioToolStripExtender1.SetStyle(menuStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, lightTheme);
                visualStudioToolStripExtender1.SetStyle(toolStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, lightTheme);

                contextMenuStripColumnOptions.Renderer = lightTheme.ToolStripRenderer;
                contextMenuStripMod.Renderer = lightTheme.ToolStripRenderer;

                DockModListForm.Instance.toolStrip2.Renderer = new ToolStripTransparentRenderer();
            }

            DockOverviewForm.Instance.Show(dockPanel1, DockState.DockRight);
            DockConflictsForm.Instance.Show(dockPanel1, DockState.DockRight);
            DockOverviewForm.Instance.Show(dockPanel1, DockState.DockRight);

            DockModListForm.Instance.Show(dockPanel1, DockState.Document);
            dockPanel1.ResumeLayout(true);
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

            DockModListForm.Instance.imageListIcons.Images.Add("Steam", UiIcons.Steam);
            DockModListForm.Instance.imageListIcons.Images.Add("Nexusmods", UiIcons.Nexusmods);
            DockModListForm.Instance.imageListIcons.Images.Add("Folder", UiIcons.Folder);


            if (LocWindowColors.DarkMode)
            {
                DockModListForm.Instance.imageListIcons.Images.Add("SteamDis", UiIconsDark.SteamDis);
                DockModListForm.Instance.imageListIcons.Images.Add("NexusmodsDis", UiIconsDark.NexusmodsDis);
                DockModListForm.Instance.imageListIcons.Images.Add("FolderDis", UiIconsDark.FolderDis);
            }
            else
            {
                DockModListForm.Instance.imageListIcons.Images.Add("SteamDis", UiIcons.SteamDis);
                DockModListForm.Instance.imageListIcons.Images.Add("NexusmodsDis", UiIcons.NexusmodsDis);
                DockModListForm.Instance.imageListIcons.Images.Add("FolderDis", UiIcons.FolderDis);
            }

            DockModListForm.Instance.olvColumnModName.ImageGetter = this.ModImageGetter;
            DockModListForm.Instance.olvColumnModName.AspectGetter = this.ModNameGetter;
            DockModListForm.Instance.olvColumnModAuthor.AspectGetter = this.ModAuthorGetter;
            DockModListForm.Instance.olvColumnModVersion.AspectGetter = this.ModVersionGetter;
            DockModListForm.Instance.olvColumnModCurLoadOrder.AspectGetter = this.ModCurLoadOrderGetter;
            DockModListForm.Instance.olvColumnModOrgLoadOrder.AspectGetter = this.ModOrgLoadOrderGetter;
            DockModListForm.Instance.olvColumnModFileSize.AspectGetter = this.ModFileSizeGetter;
            DockModListForm.Instance.olvColumnModFileSize.AspectToStringConverter = FileSizeAspectConverter;
            DockModListForm.Instance.olvColumnModFolder.AspectGetter = this.ModFolderGetter;
            DockModListForm.Instance.olvColumnModFileAge.AspectGetter = this.ModFileAgeGetter;
            DockModListForm.Instance.olvColumnModFileAge.AspectToStringConverter = ModFileAgeAspectConverter;

            DockModListForm.Instance.olvColumnModName.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModAuthor.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModVersion.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModCurLoadOrder.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModOrgLoadOrder.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModFileSize.VisibilityChanged += OlvColumnVisibilityChanged;
            DockModListForm.Instance.olvColumnModFolder.VisibilityChanged += OlvColumnVisibilityChanged;

            DockModListForm.Instance.olvColumnModName.GroupKeyGetter += GroupKeyGetter;

            var dragSource = new ModDragSource();
            DockModListForm.Instance.modObjectListView.DragSource = dragSource;

            // Selection
            RowBorderDecoration rbd = new RowBorderDecoration();
            if (LocWindowColors.DarkMode)
            {
                rbd.BorderPen = new Pen(Color.FromArgb(0, 154, 223, 51));
                rbd.FillBrush = new SolidBrush(Color.FromArgb(65, 91, 173, 255));
            }
            else
            {
                rbd.BorderPen = new Pen(Color.FromArgb(0, 154, 223, 51));
                rbd.FillBrush = new SolidBrush(Color.FromArgb(65, 0, 143, 255));
            }
            rbd.BoundsPadding = new Size(0, 0);
            rbd.CornerRounding = 0;
            DockModListForm.Instance.modObjectListView.SelectedRowDecoration = rbd;

            // Hot item
            RowBorderDecoration rbdhot = new RowBorderDecoration();
            rbdhot.BorderPen = new Pen(Color.FromArgb(50, 0, 143, 255));
            rbdhot.BoundsPadding = new Size(0, 0);
            rbdhot.CornerRounding = 0;
            //rbd.FillBrush = new SolidBrush(Color.FromArgb(64, 0, 143, 255));
            if (LocWindowColors.DarkMode)
            {
                rbdhot.FillGradientFrom = Color.FromArgb(30, 0, 143, 255);
                rbdhot.FillGradientTo = Color.FromArgb(30, 0, 143, 255);
            }
            else
            {
                rbdhot.FillGradientFrom = Color.FromArgb(16, 0, 143, 255);
                rbdhot.FillGradientTo = Color.FromArgb(16, 0, 143, 255);
            }
            HotItemStyle his = new HotItemStyle();
            his.Decoration = rbdhot;
            DockModListForm.Instance.modObjectListView.HotItemStyle = his;
            DockModListForm.Instance.modObjectListView.UseHotItem = true;

            DockModListForm.Instance.modObjectListView.BooleanCheckStateGetter = BooleanCheckStateGetter;

            bool BooleanCheckStateGetter(object rowobject)
            {
                ModItem curMod = (ModItem)rowobject;
                return curMod.Enabled;
            }

            DockModListForm.Instance.modObjectListView.BooleanCheckStatePutter = delegate (Object rowObject, bool newValue)
            {
                ModItem curMod = (ModItem)rowObject;
                curMod.Enabled = newValue;

                var modItem = ModsManager.Instance.ModEnabledList.FirstOrDefault(x =>
                    x.ModPath.Equals(curMod.Path, StringComparison.InvariantCultureIgnoreCase));
                modItem.Enabled = newValue;

                ModsManager.Instance.UpdateNewModOverrideData(curMod);
                UpdateModCountDisplay();
                DockModListForm.Instance.RecolorObjectListViewRows();
                DockModListForm.Instance.modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
                CheckModConfigTainted();
                QueueSidePanelUpdate(true);
                return newValue; // return the value that you want the control to use
            };

            LocViewState._defaultViewState.WindowPosition = this.DesktopBounds;
            LocViewState._defaultViewState.listState = LocViewState.GetCurrentListViewState();
            if (LocViewState.LoadViewStateFromFile())
                LocViewState.RestoreViewState();

            CreateColumnMenus();
            AddColumnVisibilityMenuItems(toolStripMenuItemColumns.DropDownItems);
            AddColumnVisibilityMenuItems(contextMenuStripColumnOptions.Items);
            UpdateColumnVisibilityMenus();

            DockModListForm.Instance.panelColorOverridden.BackColor = LocWindowColors.ModOverriddenColor;
            DockModListForm.Instance.panelColorOverriding.BackColor = LocWindowColors.ModOverridingColor;
            DockModListForm.Instance.panelColorOverridingOverridden.BackColor = LocWindowColors.ModOverriddenOveridingColor;

            UpdateMoveControlEnabledState();

            /*Font monospaceFont = Utils.CreateBestAvailableMonospacePlatformFont(richTextBoxManifestOverridden.Font.Size);
            if (monospaceFont != null)
            {
                richTextBoxManifestOverridden.Font = monospaceFont;
            }*/

            ModsManager.Instance.ModFilesChangedEvent += InstanceOnModFilesChangedEvent;

            DockModListForm.Instance.modObjectListView.Focus();
        }

        private void InstanceOnModFilesChangedEvent(object sender, EventArgs e)
        {
            _modFileStateMismatch = true;
            StartModFilesChangedUiFeedback();
        }

        private void StartModFilesChangedUiFeedback()
        {
            toolStripButtonReload.Image = UiIcons.ReloadNotification;
            toolStripButtonReload.ToolTipText =
                "Reload settings from current mod files, reverting unapplied changes\r\n\r\nChanges to local mod files have been detected. It is recommended to reload before continuing.";
        }

        private void StopModFileChangedUiFeedback()
        {
            toolStripButtonReload.Image = UiIcons.Reload;
            toolStripButtonReload.ToolTipText = "Reload settings from current mod files, reverting unapplied changes";
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

        private object ModFileAgeGetter(object rowobject)
        {
            ModItem s = (ModItem)rowobject;
            return s.FileAge;
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
                if (DockModListForm.Instance.modObjectListView.DropSink == null)
                {
                    if (!DockModListForm.Instance.modObjectListView.FullRowSelect)
                        DockModListForm.Instance.modObjectListView.FullRowSelect = true;
                    var dropSink = new ModDropSink();
                    dropSink.AcceptExternal = false;
                    dropSink.CanDropBetween = true;
                    dropSink.CanDropOnBackground = false;
                    dropSink.CanDropOnItem = false;
                    dropSink.CanDropOnSubItem = false;
                    dropSink.FeedbackColor = LocWindowColors.ListFeedBackColor;
                    dropSink.CanDrop += OnDropSinkOnCanDrop;
                    DockModListForm.Instance.modObjectListView.DropSink = dropSink;
                }
            }
            else
            {
                DockModListForm.Instance.modObjectListView.DropSink = null;
                if (!DockModListForm.Instance.modObjectListView.FullRowSelect)
                    DockModListForm.Instance.modObjectListView.FullRowSelect = true;
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
                DockModListForm.Instance.modObjectListView.BeginUpdate();
                DockModListForm.Instance.modObjectListView.SuspendDrawing();
            }

            timerDelayedListRecolor.Stop();
            timerDelayedListRecolor.Start();
        }

        private void OlvColumnVisibilityChanged(object sender, EventArgs e)
        {
            QueueListRecolor();
        }

        // Represents a column for menu building
        private class ColumnMenuInfo
        {
            public string Name { get; }
            public OLVColumn Column { get; }

            public ColumnMenuInfo(string name, OLVColumn column)
            {
                Name = name;
                Column = column;
            }
        }

        private ColumnMenuInfo[] columnMenus;

        private void CreateColumnMenus()
        {
            columnMenus = new[]
            {
                new ColumnMenuInfo("&Author", DockModListForm.Instance.olvColumnModAuthor),
                new ColumnMenuInfo("&Version", DockModListForm.Instance.olvColumnModVersion),
                new ColumnMenuInfo("&Current Load Order", DockModListForm.Instance.olvColumnModCurLoadOrder),
                new ColumnMenuInfo("&Original Load Order", DockModListForm.Instance.olvColumnModOrgLoadOrder),
                new ColumnMenuInfo("File &Size", DockModListForm.Instance.olvColumnModFileSize),
                new ColumnMenuInfo("File A&ge", DockModListForm.Instance.olvColumnModFileAge),
                new ColumnMenuInfo("Mod &Folder", DockModListForm.Instance.olvColumnModFolder),
            };
        }

        private void AddColumnVisibilityMenuItems(ToolStripItemCollection items)
        {
            // Add column visibility items
            foreach (var col in columnMenus)
            {
                var menuItem = new ToolStripMenuItem(col.Name)
                {
                    Checked = col.Column.IsVisible,
                    CheckOnClick = true
                };

                menuItem.Click += (s, e) =>
                {
                    col.Column.IsVisible = menuItem.Checked;
                    DockModListForm.Instance.modObjectListView.RebuildColumns();
                    UpdateColumnVisibilityMenus();
                };

                items.Add(menuItem);
            }

            // Add separator
            items.Add(new ToolStripSeparator());

            // Add "Restore Defaults" item
            var restoreDefaultsItem = new ToolStripMenuItem("Restore Defaults");
            restoreDefaultsItem.Click += (s, e) =>
            {
                LocViewState.RestoreListViewState(LocViewState._defaultViewState.listState);
                UpdateColumnVisibilityMenus();
                QueueListRecolor();
            };
            items.Add(restoreDefaultsItem);
        }

        private void UpdateColumnVisibilityMenus()
        {
            IEnumerable<ToolStripItemCollection> menus = new[]
            {
                toolStripMenuItemColumns.DropDownItems,
                contextMenuStripColumnOptions.Items
            };

            foreach (var menu in menus)
            {
                foreach (ToolStripItem item in menu)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        var colInfo = columnMenus.FirstOrDefault(c => c.Name == menuItem.Text);
                        if (colInfo != null)
                        {
                            menuItem.Checked = colInfo.Column.IsVisible;
                        }
                    }
                }
            }
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

        private string ModFileAgeAspectConverter(object value)
        {
            DateTimeOffset? dateOffset = (DateTimeOffset?)value;
            if (!dateOffset.HasValue)
                return "-";
            return ((DateTimeOffset)dateOffset).ToTimeAgeString();
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
                DockModListForm.Instance.rotatingLabelTop.NewText = "Low priority »";
                toolTip1.SetToolTip(DockModListForm.Instance.rotatingLabelTop, "Mods higher in the list are loaded earlier and may get overridden by mods below them");
                DockModListForm.Instance.rotatingLabelBottom.NewText = "« High priority";
                toolTip1.SetToolTip(DockModListForm.Instance.rotatingLabelBottom, "Mods lower in the list are loaded later, and may override mods above them");
                /*DockModListForm.Instance.rotatingLabelTop.ForeColor = ModsManager.LowPriorityColor;
               DockModListForm.Instance. rotatingLabelBottom.ForeColor = ModsManager.HighPriorityColor;*/

            }
            else
            {
                DockModListForm.Instance.rotatingLabelTop.NewText = "High priority »";
                toolTip1.SetToolTip(DockModListForm.Instance.rotatingLabelTop, "Mods higher in the list are loaded later and may override mods below them");
                DockModListForm.Instance.rotatingLabelBottom.NewText = "« Low priority";
                toolTip1.SetToolTip(DockModListForm.Instance.rotatingLabelBottom, "Mods lower in the list are loaded earlier and may get overridden by mods above them");
                /*DockModListForm.Instance.rotatingLabelTop.ForeColor = ModsManager.HighPriorityColor;
                DockModListForm.Instance.rotatingLabelBottom.ForeColor = ModsManager.LowPriorityColor;*/
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
            else
            {
                targetDirectoryCleared = true;
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
            selectedItems = selectedItems.OrderBy(i => DockModListForm.Instance.modObjectListView.Items.IndexOf(i)).ToList();

            DockModListForm.Instance.modObjectListView.BeginUpdate();
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

                    OLVListItem listItem = (OLVListItem)DockModListForm.Instance.modObjectListView.Items[currentIndex];

                    ModItem curModItem = (ModItem)listItem.RowObject;
                    DockModListForm.Instance.modObjectListView.RemoveObject(curModItem);
                    newList.Clear();
                    newList.Add(curModItem);
                    DockModListForm.Instance.modObjectListView.InsertObjects(newIndex, newList);

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

                    if (currentIndex == DockModListForm.Instance.modObjectListView.Items.Count - endOffset++)
                        continue;

                    OLVListItem listItem = (OLVListItem)DockModListForm.Instance.modObjectListView.Items[currentIndex];

                    ModItem curModItem = (ModItem)listItem.RowObject;
                    DockModListForm.Instance.modObjectListView.RemoveObject(curModItem);
                    newList.Clear();
                    newList.Add(curModItem);
                    DockModListForm.Instance.modObjectListView.InsertObjects(newIndex, newList);

                    ModItemList.Instance.ModList.Remove(curModItem);
                    ModItemList.Instance.ModList.Insert(newIndex, curModItem);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                ModItemList.Instance.RecomputeLoadOrders();

                ModsManager.Instance.RecomputeOverridingData();

                DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
                ColorListViewNumbers(DockModListForm.Instance.olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
                DockModListForm.Instance.RecolorObjectListViewRows();

                QueueSidePanelUpdate(true);

                CheckModConfigTainted();
            }

            _movingItems = false;

            DockModListForm.Instance.modObjectListView.EndUpdate();
        }

        public enum MovePosition { Top, Bottom };

        public void MoveListItems(ListView.SelectedListViewItemCollection moveItems, MovePosition position)
        {
            DockModListForm.Instance.modObjectListView.BeginUpdate();
            _movingItems = true;
            bool anyMoved = false;
            var selectedItems = moveItems.Cast<OLVListItem>().ToList();
            selectedItems = selectedItems.OrderBy(i => DockModListForm.Instance.modObjectListView.Items.IndexOf(i)).ToList();

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

                        DockModListForm.Instance.modObjectListView.RemoveObject(curModItem);
                        newList.Clear();
                        newList.Add(curModItem);
                        DockModListForm.Instance.modObjectListView.InsertObjects(listOffset, newList);

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
                    if (item.Index == DockModListForm.Instance.modObjectListView.Items.Count - endOffset--)
                        continue;

                    ModItem curModItem = (ModItem)item.RowObject;
                    DockModListForm.Instance.modObjectListView.RemoveObject(curModItem);
                    DockModListForm.Instance.modObjectListView.AddObject(curModItem);

                    ModItemList.Instance.ModList.Remove(curModItem);
                    ModItemList.Instance.ModList.Add(curModItem);

                    anyMoved = true;
                }
            }

            if (anyMoved)
            {
                ModItemList.Instance.RecomputeLoadOrders();

                ModsManager.Instance.RecomputeOverridingData();
                DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
                ColorListViewNumbers(DockModListForm.Instance.olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
                DockModListForm.Instance.RecolorObjectListViewRows();

                QueueSidePanelUpdate(true);
                CheckModConfigTainted();
            }

            _movingItems = false;

            DockModListForm.Instance.modObjectListView.EndUpdate();
        }

        public void ApplyModSettings()
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            ModsManager.Instance.StopModFileWatches();
            try
            {
                ModItemList.Instance.RecomputeLoadOrders();
                ModsManager.Instance.SaveToFiles();
                ModsManager.Instance.SaveLastAppliedModOrder();
                SetModConfigTainted(false);
                _ActiveModListHash = ModItemList.Instance.ModList.ComputeModListHashCode();
            }
            finally
            {
                ModsManager.Instance.StartModFileWatches();
            }
        }


        public void ClearAll()
        {
            _ActiveModListHash = 0;

            DockConflictsForm.Instance.SuspendLayout();
            DockOverviewForm.Instance.SuspendLayout();

            DockModListForm.Instance.modObjectListView.ClearObjects();
            DockModListForm.Instance.modObjectListView.ClearCachedInfo();
            ModItemList.Instance.ModList.Clear();
            ModsManager.Instance.ClearAll();
            UpdateSidePanelData(true);
            StopModFileChangedUiFeedback();

            DockConflictsForm.Instance.ResumeLayout();
            DockOverviewForm.Instance.ResumeLayout();
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
            toolStripButtonSteamWorkshop.Visible = LocSettings.Instance.Data.platform == eGamePlatform.Steam;
            openUserModsFolderToolStripMenuItem.Visible = LocSettings.Instance.Data.platform == eGamePlatform.WindowsStore;
        }

        //Load mod data and fill in the list box...
        public void LoadAndFill(List<ModImportData> desiredMods, bool orderByDesired)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            //try
            {
                //ModsManager.Instance.ModEnabledList = modList;

                ModsManager.Instance.InitModEnabledList();

                List<ModImportData> orderedModList;
                // Sort by mechwarrior load order
                if (!orderByDesired || desiredMods == null)
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

                DockModListForm.Instance.modObjectListView.BeginUpdate();
                DockModListForm.Instance.modObjectListView.ClearObjects();
                ModItemList.Instance.ModList.Clear();
                foreach (var entry in orderedModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
                {
                    ModItem newItem = new ModItem();
                    newItem.Enabled = entry.Enabled;
                    newItem.Path = entry.ModPath;
                    newItem.Name = ModsManager.Instance.ModDetails[entry.ModPath].displayName;
                    newItem.FolderName = ModsManager.Instance.PathToDirNameDict[entry.ModPath];
                    newItem.FileSize = ModsManager.Instance.Mods[entry.ModPath].ModFileSize;
                    newItem.FileAge = ModsManager.Instance.Mods[entry.ModPath].FileAge;
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
                    DockModListForm.Instance.modObjectListView.AddObject(newItem);
                }
                DockModListForm.Instance.RecolorObjectListViewRows();
                DockModListForm.Instance.modObjectListView.EndUpdate();

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
            ModItemList.Instance.RecomputeLoadOrders();

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            ModsManager.Instance.RecomputeOverridingData();
            ColorListViewNumbers(DockModListForm.Instance.olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            DockModListForm.Instance.RecolorObjectListViewRows();
            DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            DockModListForm.Instance.modObjectListView.EndUpdate();
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
            StopModFileChangedUiFeedback();
            _modFileStateMismatch = false;
            DockModListForm.Instance.modObjectListView.BeginUpdate();

            Point prevPosition = DockModListForm.Instance.modObjectListView.LowLevelScrollPosition;
            List<string> prevSelected = new List<string>(DockModListForm.Instance.modObjectListView.SelectedObjects.Count);
            foreach (ModItem selected in DockModListForm.Instance.modObjectListView.SelectedObjects)
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

                Action redrawCallback = () =>
                {
                    DockModListForm.Instance.modObjectListView.ForceRedraw();
                };

                if (forceLoadLastApplied || ModsManager.Instance.ShouldLoadLastApplied(redrawCallback))
                {
                    // Load last saved preset
                    modlist = ModsManager.Instance.LastAppliedPresetModList;
                    DockModListForm.Instance.modObjectListView.SuspendDrawing();
                    LoadAndFill(modlist, true);
                    DockModListForm.Instance.modObjectListView.ResumeDrawing();

                    if (_ActiveModListHash != ModItemList.Instance.ModList.ComputeModListHashCode())
                        modConfigTainted = true;
                }

                FilterTextChanged();
                ModsManager.Instance.RecomputeOverridingData();
            }
            LoadPresets();
            SetModConfigTainted(modConfigTainted);

            foreach (OLVListItem curListItem in DockModListForm.Instance.modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)curListItem.RowObject;

                if (prevSelected.Contains(curModItem.Path))
                {
                    curListItem.Selected = true;
                }
            }

            DockModListForm.Instance.modObjectListView.EndUpdate();
            DockModListForm.Instance.modObjectListView.LowLevelScroll(prevPosition.X, prevPosition.Y);
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

            List<string> prevSelected = new List<string>(DockModListForm.Instance.modObjectListView.SelectedItems.Count);
            foreach (ModItem selected in DockModListForm.Instance.modObjectListView.SelectedObjects)
            {
                prevSelected.Add(selected.Path);
            }

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            DockModListForm.Instance.modObjectListView.ClearObjects();
            DockModListForm.Instance.modObjectListView.ClearCachedInfo();
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
            LoadAndFill(newPresetData, true);
            FilterTextChanged();
            CheckModConfigTainted();
            DockModListForm.Instance.modObjectListView.EndUpdate();

            bool firstSelected = true;

            foreach (OLVListItem curListItem in DockModListForm.Instance.modObjectListView.Items)
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
            // We don't really know how to properly launch the game on this platform due to the lack of owning the game.
            // Very few people do on Xbox Game Pass.
            // Let's try a few different methods.
            var appTargets = new[]
            {
                @"shell:appsFolder\PiranhaGamesInc.MechWarrior5Mercenaries_skpx0jhaqqap2!9PB86W3JK8Z5",
                @"shell:appsFolder\PiranhaGamesInc.MechWarrior5Mercenaries_skpx0jhaqqap2!App",
                @"shell:appsFolder\PiranhaGamesInc.MechWarrior5Mercenaries_skpx0jhaqqap2"
            };

            Exception lastException = null;

            foreach (var target in appTargets)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = target,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    return; // success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            // If we get here, all attempts failed
            if (lastException != null)
            {
                Console.WriteLine(lastException.Message);
                Console.WriteLine(lastException.StackTrace);
                string message = "There was an error while trying to launch MechWarrior 5.\r\n" + lastException.Message;
                string caption = "Error Launching";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string message = "There was an error while trying to launch MechWarrior 5.\r\n" + Ex.Message;
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
                string message = "There was an error while trying to launch MechWarrior 5 through Epic Games Launcher.\r\n" + Ex.Message;
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
                    FileName = SteamUtils.CreateRunGameCommand(784080),
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = @"There was an error while trying to launch Mechwarrior 5 through Steam.\r\n" + Ex.Message;
                string caption = @"Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
            }
        }
        #endregion

        public void UpdateMoveControlEnabledState()
        {
            bool anySelected = DockModListForm.Instance.modObjectListView.SelectedObjects.Count > 0;
            bool enabled = anySelected && _filterMode != eFilterMode.ItemFilter;
            DockModListForm.Instance.toTopToolStripButton.Enabled = enabled;
            DockModListForm.Instance.toBottomToolStripButton.Enabled = enabled;
            DockModListForm.Instance.upToolStripButton.Enabled = enabled;
            DockModListForm.Instance.downToolStripButton.Enabled = enabled;

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
                    DockModListForm.Instance.modObjectListView.UseFiltering = false;
                    DockModListForm.Instance.modObjectListView.ModelFilter = null;

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
                DockModListForm.Instance.modObjectListView.ModelFilter = TextMatchFilter.Contains(DockModListForm.Instance.modObjectListView, filtertext);
                if (!Instance.toolStripButtonFilterToggle.Checked)
                {
                    DockModListForm.Instance.modObjectListView.UseFiltering = false;
                    // ensure that first found item is visible
                    if (DockModListForm.Instance.modObjectListView.ModelFilter != null)
                    {
                        foreach (object originalObject in DockModListForm.Instance.modObjectListView.Objects)
                        {
                            if (DockModListForm.Instance.modObjectListView.ModelFilter.Filter(originalObject))
                            {
                                DockModListForm.Instance.modObjectListView.EnsureModelVisible(originalObject);
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
                    DockModListForm.Instance.modObjectListView.UseFiltering = true;
                    if (DockModListForm.Instance.modObjectListView.ModelFilter != null)
                    {
                        foreach (object originalObject in DockModListForm.Instance.modObjectListView.Objects)
                        {
                            if (DockModListForm.Instance.modObjectListView.ModelFilter.Filter(originalObject))
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
                toolStripTextFilterBox.ForeColor = LocWindowColors.WindowText;
                toolStripTextFilterBox.BackColor = LocWindowColors.Window;
            }

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            DockModListForm.Instance.modObjectListView.Invalidate();
            DockModListForm.Instance.RecolorObjectListViewRows();
            DockModListForm.Instance.modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            DockModListForm.Instance.modObjectListView.EndUpdate();

            //While filtering disable the up/down buttons (tough this should no longer be needed).
            UpdateMoveControlEnabledState();
        }

        public ModItem GetFirstSelectedMod()
        {
            var selectedObjects = DockModListForm.Instance.modObjectListView.SelectedObjects;
            if (selectedObjects != null && selectedObjects.Count > 0)
            {
                return (ModItem)selectedObjects[0];
            }
            return null;
        }

        private void UpdateSidePanelData(bool forceUpdate)
        {
            if (DockModListForm.Instance.modObjectListView.SelectedObjects.Count == 0)
            {
                _sideBarSelectedModKey = null;
                DockOverviewForm.Instance.pictureBoxModImage.Visible = false;
                DockOverviewForm.Instance.panelModInfo.Visible = false;
                DockConflictsForm.Instance.ClearModInfo();
                DockOverviewForm.Instance.noneSelectedPanel.Visible = true;
                return;
            }

            ModItem firstSelectedMod = GetFirstSelectedMod();
            if (firstSelectedMod == null)
                return;

            string selectedModPath = firstSelectedMod.Path;

            if (!forceUpdate && _sideBarSelectedModKey == selectedModPath)
                return;

            _sideBarSelectedModKey = selectedModPath;

            string selectedModFolder = firstSelectedMod.FolderName;
            ModObject modDetails = ModsManager.Instance.ModDetails[selectedModPath];

            DockOverviewForm.Instance.noneSelectedPanel.Visible = false;
            DockOverviewForm.Instance.panelModInfo.Visible = true;
            string selectedModLabelDisplayName = firstSelectedMod.Name.Replace("&", "&&");
            DockOverviewForm.Instance.labelModName.Text = selectedModLabelDisplayName;
            DockConflictsForm.Instance.labelModNameOverrides.Text = selectedModLabelDisplayName;
            DockOverviewForm.Instance.labelModAuthor.Text = @"Author: " + modDetails.author?.Replace("&", "&&");
            DockOverviewForm.Instance.linkLabelModAuthorUrl.Text = modDetails.authorURL.Replace("&", "&&");
            DockOverviewForm.Instance.labelModVersion.Text = @"Version: " + modDetails.version?.Replace("&", "&&");
            DockOverviewForm.Instance.labelModBuildNumber.Text = @"Build: " + modDetails.buildNumber;
            long steamId = modDetails.steamPublishedFileId;
            if (steamId > 0)
            {
                DockOverviewForm.Instance.pictureBoxSteamIcon.Visible = true;
                DockOverviewForm.Instance.labelSteamId.Visible = true;
                DockOverviewForm.Instance.linkLabelSteamId.Visible = true;
                DockOverviewForm.Instance.linkLabelSteamId.Text = steamId.ToString();
            }
            else
            {
                DockOverviewForm.Instance.pictureBoxSteamIcon.Visible = false;
                DockOverviewForm.Instance.labelSteamId.Visible = false;
                DockOverviewForm.Instance.linkLabelSteamId.Visible = false;
            }

            string nexusModsId = ModsManager.Instance.Mods[selectedModPath].NexusModsId;
            if (nexusModsId != "")
            {
                DockOverviewForm.Instance.pictureBoxNexusmodsIcon.Visible = true;
                DockOverviewForm.Instance.labelNexusmods.Visible = true;
                DockOverviewForm.Instance.linkLabelNexusmods.Visible = true;
                DockOverviewForm.Instance.linkLabelNexusmods.Text = nexusModsId;
            }
            else
            {
                DockOverviewForm.Instance.pictureBoxNexusmodsIcon.Visible = false;
                DockOverviewForm.Instance.labelNexusmods.Visible = false;
                DockOverviewForm.Instance.linkLabelNexusmods.Visible = false;
            }

            DockOverviewForm.Instance.richTextBoxModDescription.Text = modDetails.description;

            HandleOverriding(selectedModFolder);

            string imagePath = Path.Combine(selectedModPath, "Resources", "Icon128.png");

            if (_ModImageLoader != null)
            {
                _ModImageLoader.CancelLoad();
                _ModImageLoader = null;
            }

            DockOverviewForm.Instance.pictureBoxModImage.Visible = false;

            if (File.Exists(imagePath))
            {
                _ModImageLoader = new AsyncFileLoader();
                //_ModImageLoader.ToggleDebugMode(true);

                Action<MemoryStream> onFileLoaded = (memoryStream) =>
                {
                    if (DockOverviewForm.Instance.pictureBoxModImage.Image != null)
                    {
                        DockOverviewForm.Instance.pictureBoxModImage.Image.Dispose();
                        DockOverviewForm.Instance.pictureBoxModImage.Image = null;
                    }

                    try
                    {
                        DockOverviewForm.Instance.pictureBoxModImage.Image = Image.FromStream(memoryStream);
                        DockOverviewForm.Instance.pictureBoxModImage.Visible = true;
                    }
                    catch (Exception e)
                    {

                    }
                };

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _ModImageLoader.LoadFileAsync(imagePath, onFileLoaded);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        //Handles the showing of overriding data on select
        private void HandleOverriding(string selectedModPath)
        {
            DockConflictsForm.Instance.listBoxOverriding.Items.Clear();
            DockConflictsForm.Instance.listBoxOverriddenBy.Items.Clear();
            DockConflictsForm.Instance.richTextBoxManifestOverridden.Clear();

            ModItem selectedModItem = GetFirstSelectedMod();
            if (selectedModItem == null)
            {
                DockConflictsForm.Instance.SetNoneSelectedText();
                DockConflictsForm.Instance.noneSelectedPanel.Visible = true;
                return;
            }

            if (!selectedModItem.Enabled)
            {
                DockConflictsForm.Instance.SetModNotEnabledText();
                DockConflictsForm.Instance.noneSelectedPanel.Visible = true;
                return;
            }

            DockConflictsForm.Instance.noneSelectedPanel.Visible = false;
            if (ModsManager.Instance.ModConflictData.Count == 0)
                return;

            if (!ModsManager.Instance.ModConflictData.ContainsKey(selectedModPath))
                return;

            DockConflictsForm.Instance.listBoxOverriding.SuspendDrawing();
            DockConflictsForm.Instance.listBoxOverriddenBy.SuspendDrawing();
            ModConflictData modData = ModsManager.Instance.ModConflictData[selectedModPath];
            foreach (string overriding in modData.overriddenBy.Keys)
            {
                ModListBoxItem modListBoxItem = new ModListBoxItem();
                string modKey = ModsManager.Instance.DirNameToPathDict[overriding];
                modListBoxItem.DisplayName = ModsManager.Instance.ModDetails[modKey].displayName;
                modListBoxItem.ModDirName = overriding;
                modListBoxItem.ModKey = modKey;
                DockConflictsForm.Instance.listBoxOverriddenBy.Items.Add(modListBoxItem);
            }
            foreach (string overrides in modData.overrides.Keys)
            {
                ModListBoxItem modListBoxItem = new ModListBoxItem();
                string modKey = ModsManager.Instance.DirNameToPathDict[overrides];
                modListBoxItem.DisplayName = ModsManager.Instance.ModDetails[modKey].displayName;
                modListBoxItem.ModDirName = overrides;
                modListBoxItem.ModKey = modKey;
                DockConflictsForm.Instance.listBoxOverriding.Items.Add(modListBoxItem);
            }

            DockConflictsForm.Instance.listBoxOverriding.ResumeDrawing();
            DockConflictsForm.Instance.listBoxOverriddenBy.ResumeDrawing();
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

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            //this.ClearAll();
            List<string> prevSelected = new List<string>(DockModListForm.Instance.modObjectListView.SelectedItems.Count);
            foreach (ModItem selected in DockModListForm.Instance.modObjectListView.SelectedObjects)
            {
                prevSelected.Add(selected.Path);
            }
            DockModListForm.Instance.modObjectListView.ClearObjects();
            DockModListForm.Instance.modObjectListView.ClearCachedInfo();
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
            foreach (OLVListItem curListItem in DockModListForm.Instance.modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)curListItem.RowObject;

                if (prevSelected.Contains(curModItem.Path))
                {
                    curListItem.Selected = true;
                }
            }
            QueueSidePanelUpdate(true);
            DockModListForm.Instance.modObjectListView.EndUpdate();
        }

        private void importLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportLoadOrder();
        }

        private void openModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModsManager.Instance.ModsPaths[eModPathType.Program] == null || Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Program].FullPath))
                return;

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.Program].FullPath,
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

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            try
            {
                foreach (ModItem curModItem in ModItemList.Instance.ModList)
                {
                    curModItem.Enabled = true;
                }

                DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            }
            finally
            {
                DockModListForm.Instance.modObjectListView.EndUpdate();
            }

            ModsManager.Instance.RecomputeOverridingData();
            UpdateModCountDisplay();
            DockModListForm.Instance.RecolorObjectListViewRows();
            DockModListForm.Instance.modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            CheckModConfigTainted();
        }

        private void disableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (AreAllModsDisabled())
                return;

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            try
            {
                foreach (ModItem curModItem in ModItemList.Instance.ModList)
                {
                    curModItem.Enabled = false;
                }

                DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            }
            finally
            {
                DockModListForm.Instance.modObjectListView.EndUpdate();
            }

            ModsManager.Instance.RecomputeOverridingData();
            UpdateModCountDisplay();
            DockModListForm.Instance.RecolorObjectListViewRows();
            DockModListForm.Instance.modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            CheckModConfigTainted();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (OLVListItem selectedItem in DockModListForm.Instance.modObjectListView.SelectedItems)
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

            if (ModsManager.Instance.ModsPaths[eModPathType.Steam] == null || Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.Steam].FullPath))
                return;

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.Steam].FullPath,
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

        private void presetMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem presetMenuItem = sender as ToolStripMenuItem;
            this.LoadFromPreset(presetMenuItem.Tag.ToString());
        }

        private void savePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DockModListForm.Instance.modObjectListView.Items.Count == 0)
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
        public void SelectModInList(string modKey)
        {
            DockModListForm.Instance.modObjectListView.DeselectAll();
            foreach (OLVListItem modListItem in DockModListForm.Instance.modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)modListItem.RowObject;

                if (curModItem.Path == modKey)
                {
                    modListItem.Selected = true;
                    DockModListForm.Instance.modObjectListView.EnsureVisible(modListItem.Index);
                    break;
                }
            }
        }

        public void HighlightModInList(string modKey)
        {
            foreach (OLVListItem modListItem in DockModListForm.Instance.modObjectListView.Items)
            {
                ModItem curModItem = (ModItem)modListItem.RowObject;

                if (curModItem.Path == modKey)
                {
                    foreach (ListViewItem.ListViewSubItem subItem in modListItem.SubItems)
                    {
                        if (modListItem.Index % 2 == 1)
                        {
                            subItem.BackColor = LocWindowColors.ListModHighlightColorAlternate;
                            curModItem.ProcessedRowBackColor = LocWindowColors.ListModHighlightColorAlternate;
                        }
                        else
                        {
                            subItem.BackColor = LocWindowColors.ListModHighlightColor;
                            curModItem.ProcessedRowBackColor = LocWindowColors.ListModHighlightColor;
                        }
                    }
                    break;
                }
            }
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
                toolStripButtonApply.ForeColor = LocWindowColors.ControlText;
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
            var selectedItems = DockModListForm.Instance.modObjectListView.SelectedObjects;
            MoveListItems(DockModListForm.Instance.modObjectListView.SelectedItems, MovePosition.Top);
            DockModListForm.Instance.modObjectListView.SelectedObjects = selectedItems;
            DockModListForm.Instance.modObjectListView.EnsureModelVisible(selectedItems[0]);
        }

        private void contextMenuItemMoveToBottom_Click(object sender, EventArgs e)
        {
            var selectedItems = DockModListForm.Instance.modObjectListView.SelectedObjects;
            MoveListItems(DockModListForm.Instance.modObjectListView.SelectedItems, MovePosition.Bottom);
            DockModListForm.Instance.modObjectListView.SelectedObjects = selectedItems;
            DockModListForm.Instance.modObjectListView.EnsureModelVisible(selectedItems[^1]);
        }

        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveListItems(DockModListForm.Instance.modObjectListView.SelectedItems, MoveDirection.Up);
        }

        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MoveListItems(DockModListForm.Instance.modObjectListView.SelectedItems, MoveDirection.Down);
        }

        //Color the list view items based on data
        public void ColorizeListViewItems()
        {
            DockModListForm.Instance.modObjectListView.BeginUpdate();
            ColorListViewNumbers(DockModListForm.Instance.olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            ColorListViewNumbers(DockModListForm.Instance.olvColumnModOrgLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            DockModListForm.Instance.modObjectListView.EndUpdate();
        }

        public void ColorListViewNumbers(int subItemIndex, Color fromColor, Color toColor)
        {
            if (subItemIndex == -1)
                return;

            List<float> numbers = new List<float>();

            // Extract numbers from ListView column and find unique ones
            foreach (OLVListItem item in DockModListForm.Instance.modObjectListView.Items)
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
            DockModListForm.Instance.modObjectListView.BeginUpdate();
            for (int i = 0; i < DockModListForm.Instance.modObjectListView.Items.Count; i++)
            {
                OLVListItem curListItem = (OLVListItem)DockModListForm.Instance.modObjectListView.Items[i];
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
                    if (subItemIndex == DockModListForm.Instance.olvColumnModOrgLoadOrder.Index)
                    {
                        curModItem.ProcessedOrgLoForeColor = newColor;
                    }
                    else if (subItemIndex == DockModListForm.Instance.olvColumnModCurLoadOrder.Index)
                    {
                        curModItem.ProcessedCurLoForeColor = newColor;
                    }
                }
            }
            DockModListForm.Instance.modObjectListView.EndUpdate();
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

            DockModListForm.Instance.modObjectListView.BeginUpdate();
            IList prevSelected = DockModListForm.Instance.modObjectListView.SelectedObjects;
            DockModListForm.Instance.modObjectListView.ClearObjects();
            DockModListForm.Instance.modObjectListView.AddObjects(ModItemList.Instance.ModList);

            ModItemList.Instance.RecomputeLoadOrders();

            ModsManager.Instance.RecomputeOverridingData();
            DockModListForm.Instance.modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            ColorListViewNumbers(DockModListForm.Instance.olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            DockModListForm.Instance.RecolorObjectListViewRows();
            FilterTextChanged();
            CheckModConfigTainted();

            DockModListForm.Instance.modObjectListView.SelectedObjects = prevSelected;
            if (prevSelected.Count > 0)
            {
                DockModListForm.Instance.modObjectListView.EnsureModelVisible(prevSelected[0]);
            }

            DockModListForm.Instance.modObjectListView.EndUpdate();
            QueueSidePanelUpdate(true);
        }

        public bool AreAllModsEnabled()
        {
            for (int i = 1; i < DockModListForm.Instance.modObjectListView.Items.Count; i++)
            {
                if (!DockModListForm.Instance.modObjectListView.Items[i].Checked)
                    return false;
            }

            return true;
        }

        public bool AreAllModsDisabled()
        {
            for (int i = 1; i < DockModListForm.Instance.modObjectListView.Items.Count; i++)
            {
                if (DockModListForm.Instance.modObjectListView.Items[i].Checked)
                    return false;
            }

            return true;
        }

        private void openUserModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ModsManager.Instance.GameIsConfigured())
                return;

            if (ModsManager.Instance.ModsPaths[eModPathType.AppData] == null || Utils.StringNullEmptyOrWhiteSpace(ModsManager.Instance.ModsPaths[eModPathType.AppData].FullPath))
                return;

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ModsManager.Instance.ModsPaths[eModPathType.AppData].FullPath,
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
            toolStripTextFilterBox.TextBox.Focus();
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
                ModsManager.Instance.StopModFileWatches();
                try
                {
                    if (FileOperationUtils.DeleteFile(modKey, true, this.Handle))
                    {
                        RefreshAll();
                    }
                }
                finally
                {
                    ModsManager.Instance.StartModFileWatches();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (OLVListItem selectedItem in DockModListForm.Instance.modObjectListView.SelectedItems)
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

                    ModsManager.Instance.StopModFileWatches();
                    try
                    {
                        CopyModFromFolder(fbd.SelectedPath);
                        RefreshAll(true);
                    }
                    finally
                    {
                        ModsManager.Instance.StartModFileWatches();
                    }
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
                ModsManager.Instance.StopModFileWatches();
                try
                {
                    List<string> extractedModDirNames = ExtractModFromArchive(selectedZipFile);
                    if (extractedModDirNames == null || extractedModDirNames.Count == 0)
                        return;
                    RefreshAll(true);
                }
                finally
                {
                    ModsManager.Instance.StartModFileWatches();
                }
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
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.M)
            {
                LaunchGame();
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.R)
            {
                RefreshAll(false);
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.I)
            {
                ImportLoadOrder();
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.E)
            {
                ExportLoadOrder();
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.F)
            {
                toolStripTextFilterBox.TextBox.Focus();
                e.Handled = true;
                return;
            }
        }

        public void SetSelectedModEnabledState(bool newState)
        {
            DockModListForm.Instance.modObjectListView.BeginUpdate();
            this._movingItems = true;
            foreach (OLVListItem selectedItem in DockModListForm.Instance.modObjectListView.SelectedItems)
            {
                if (newState == selectedItem.Checked)
                    continue;

                selectedItem.Checked = newState;
            }
            this._movingItems = false;

            DockModListForm.Instance.modObjectListView.EndUpdate();
        }

        private void enableModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSelectedModEnabledState(true);
        }

        private void disableModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSelectedModEnabledState(false);
        }

        private void timerDelayedListRecolor_Tick(object sender, EventArgs e)
        {
            ColorizeListViewItems();
            DockModListForm.Instance.modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            DockModListForm.Instance.RecolorObjectListViewRows();
            timerDelayedListRecolor.Stop();
            DockModListForm.Instance.modObjectListView.EndUpdate();
            DockModListForm.Instance.modObjectListView.ResumeDrawing();
            _delayedRecolorStarted = false;
        }

        private void checkModFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModCheckForm checkDialog = new ModCheckForm();
            checkDialog.ShowDialog();
        }


        private bool ReloadButtonBlinkState = false;

        private void timerReloadButtonBlink_Tick(object sender, EventArgs e)
        {
            if (ReloadButtonBlinkState)
            {
                toolStripButtonReload.ForeColor = Color.OrangeRed;
            }
            else
            {
                toolStripButtonReload.ForeColor = LocWindowColors.ControlText;
            }

            ReloadButtonBlinkState = !ReloadButtonBlinkState;
        }

        private void toolStripTextFilterBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (_filterMode == eFilterMode.ItemHighlight)
                {
                    DockModListForm.Instance.modObjectListView.UseFiltering = false;
                    if (DockModListForm.Instance.modObjectListView.ModelFilter != null)
                    {
                        object foundObject = null;
                        bool lastObject = true;

                        var objectList = DockModListForm.Instance.modObjectListView.Objects.Cast<object>().ToList();
                        foreach (object originalObject in objectList.ReverseIterate())
                        {
                            if (!DockModListForm.Instance.modObjectListView.ModelFilter.Filter(originalObject))
                                continue;

                            bool currentIsSelected = DockModListForm.Instance.modObjectListView.IsSelected(originalObject);
                            if (lastObject)
                            {
                                lastObject = false;
                                if (currentIsSelected)
                                {
                                    continue;
                                }
                            }

                            if (currentIsSelected)
                                break;

                            foundObject = originalObject;
                        }

                        if (foundObject != null)
                        {
                            DockModListForm.Instance.modObjectListView.EnsureModelVisible(foundObject);
                            DockModListForm.Instance.modObjectListView.SelectedObject = foundObject;
                        }
                    }
                }
            }
        }

        private void toolStripTextFilterBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Avoids error sound when pressing the enter key
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                return;
            }

            // Ignore repeated Ctrl+F input
            if (e.KeyChar == '\u0006')
            {
                e.Handled = true;
                return;
            }
        }

        private void toolStripTextFilterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                e.Handled = true;
                return;
            }
        }

        private void FilterTextBoxOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                if (!string.IsNullOrEmpty(toolStripTextFilterBox.Text))
                {
                    toolStripTextFilterBox.Text = String.Empty;
                }
                else
                {
                    DockModListForm.Instance.modObjectListView.Focus();
                }
                e.Handled = true;
            }
        }

        private void FilterTextBoxOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape/* && !string.IsNullOrEmpty(toolStripTextFilterBox.Text)*/)
            {
                e.IsInputKey = true;
            }
        }

        private void toolStripButtonSteamWorkshop_Click(object sender, EventArgs e)
        {
            var runUrl = SteamUtils.IsSteamRunning()
                ? "steam://url/SteamWorkshopPage/784080"
                : "https://steamcommunity.com/app/784080/workshop/";
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = runUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void runMechWarrior5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchGame();
        }

        private void toolStripMenuItemOverviewWindowToggle_Click(object sender, EventArgs e)
        {
            DockOverviewForm.Instance.Show(dockPanel1);
        }

        private void toolStripMenuItemConflictWindowToggle_Click(object sender, EventArgs e)
        {
            DockConflictsForm.Instance.Show(dockPanel1);
        }
    }
}