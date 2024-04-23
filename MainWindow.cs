using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ListView = System.Windows.Forms.ListView;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Form
    {
        static public MainWindow MainForm;
        public MainLogic logic = new MainLogic();
        //public TCPFileShare fileShare;
        bool filtered = false;
        private List<ModListItem> ListViewData = new List<ModListItem>();
        private List<ListViewItem> markedForRemoval;
        public Form4 WaitForm;
        private bool MovingItem = false;
        internal bool JustPacking = true;

        public bool LoadingAndFilling { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MainForm = this;
            //this.fileShare = new TCPFileShare(logic, this);
            this.markedForRemoval = new List<ListViewItem>();

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            this.BringToFront();
            this.Focus();
            this.KeyPreview = true;

            this.KeyDown += new KeyEventHandler(form1_KeyDown);
            this.KeyUp += new KeyEventHandler(form1_KeyUp);

            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            //start the TCP listner for TCP mod sharing
            //Disabled for now.
            //this.fileShare.Listener.RunWorkerAsync();
        }

        //called upon loading the form
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.MainIcon;
            this.logic = new MainLogic();
            if (logic.TryLoadProgramData())
            {
                LoadAndFill(false);
            }
            this.LoadPresets();
            this.SetVersionAndPlatform();

            SetupRotatingLabel();
        }

        private void SetupRotatingLabel()
        {
            this.rotatingLabel1.Text = "";                  // which can be changed by NewText property
            this.rotatingLabel1.AutoSize = false;           // adjust according to your text
            this.rotatingLabel1.NewText = "<- Low Priority/Loaded First --- High Priority/Loaded Last ->";     // whatever you want to display
            this.rotatingLabel1.ForeColor = Color.Black;    // color to display
            this.rotatingLabel1.RotateAngle = -90;          // angle to rotate
        }

        //handling key presses for hotkeys.
        private async void form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("KEY Released: " + e.KeyCode);
            if (e.KeyCode == Keys.ShiftKey)
            {
                await Task.Delay(50);
                this.button1.Text = "&UP";
                this.button2.Text = "&DOWN";
            }
        }

        private void form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("KEY Pressed: " + e.KeyCode);
            if (e.Shift)
            {
                this.button1.Text = "MOVE TO TOP";
                this.button2.Text = "MOVE TO BOTTOM";
            }
        }

        //When we hover over the manager with a file or folder
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        //When we drop a file or folder on the manager
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //We only support single file drops!
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
            {
                return;
            }
            string file = files[0];
            Console.WriteLine(file);
            //Lets see what we got here
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(file);
            bool IsDirectory = attr.ToString() == "Directory";

            if (!HandleDirectory())
            {
                HandleFile();
            }

            //Refresh button
            button6_Click(null, null);

            void HandleFile()
            {
                if (!file.Contains(".zip"))
                {
                    string message = "Only .zip files are supported. " +
                        "Please extract first and drag the folder into the application.";
                    string caption = "Unsuported File Type";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);
                    return;
                }
                //we have a zip!
                using (ZipArchive archive = ZipFile.OpenRead(file))
                {
                    bool modFound = false;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        //Console.WriteLine(entry.FullName);
                        if (entry.Name.Contains("mod.json"))
                        {
                            //we have found a mod!
                            //Console.WriteLine("MOD FOUND IN ZIP!: " + entry.FullName);
                            modFound = true;
                            break;
                        }
                    }
                    if (!modFound)
                    {
                        return;
                    }
                    //Extract mod to mods dir
                    ZipFile.ExtractToDirectory(file, logic.BasePath[0]);
                    button6_Click(null, null);
                }
            }

            //Return succes
            bool HandleDirectory()
            {
                if (!IsDirectory)
                {
                    return false;
                }
                if (!ModInDirectory(file))
                {
                    return false;
                }
                if (ModsFolderNotSet())
                {
                    return false;
                }

                string modName;
                string[] splitString = file.Split('\\');
                modName = splitString[splitString.Length - 1];
                Utils.DirectoryCopy(file, logic.BasePath[0] + "\\" + modName, true);
                return true;
            }

            bool ModInDirectory(string _file)
            {
                bool foundMod = false;
                foreach (string f in Directory.GetFiles(_file))
                {
                    if (f.Contains("mod.json"))
                    {
                        foundMod = true;
                        break;
                    }
                }

                return foundMod;
            }

            bool ModsFolderNotSet()
            {
                return Utils.StringNullEmptyOrWhiteSpace(logic.BasePath[0]);
            }
        }

        //Up button
        //Get item info, remove item, insert above, set new item as selected.
        private void button1_Click(object sender, EventArgs e)
        {
            ListView.ListViewItemCollection items = modsListView.Items;
            this.MovingItem = true;
            int i = SelectedItemIndex();
            if (i < 1)
            {
                this.MovingItem = false;
                return;
            }
            ModListItem listItem = ListViewData[i];
            items.RemoveAt(i);
            ListViewData.RemoveAt(i);

            if (Control.ModifierKeys == Keys.Shift)
            {
                //Move to top
                items.Insert(0, listItem);
                ListViewData.Insert(0, listItem);

            }
            else
            {
                //move one up
                items.Insert(i - 1, listItem);
                ListViewData.Insert(i - 1, listItem);

            }
            listItem.Selected = true;

            this.logic.GetOverridingData(this.ListViewData);
            listView1_SelectedIndexChanged(null, null);
            this.MovingItem = false;
        }

        //Down button
        //Get item info, remove item, insert below, set new item as selected.
        private void button2_Click(object sender, EventArgs e)
        {
            ListView.ListViewItemCollection items = modsListView.Items;
            this.MovingItem = true;
            int i = SelectedItemIndex();
            if (i > ListViewData.Count - 2 || i < 0)
            {
                this.MovingItem = false;
                return;
            }

            ModListItem listItem = ListViewData[i];
            items.RemoveAt(i);
            ListViewData.RemoveAt(i);

            if (Control.ModifierKeys == Keys.Shift)
            {
                //Move to bottom
                items.Insert(ListViewData.Count, listItem);
                ListViewData.Insert(ListViewData.Count, listItem);
            }
            else
            {
                //move one down
                items.Insert(i + 1, listItem);
                ListViewData.Insert(i + 1, listItem);
            }
            listItem.Selected = true;

            //Move to below when refactor is complete
            //UpdateListView();

            this.logic.GetOverridingData(ListViewData);
            listView1_SelectedIndexChanged(null, null);
            this.MovingItem = false;
        }

        //Apply button
        private void button3_Click(object sender, EventArgs e)
        {
            #region mod removal

            //Stuff for removing mods:
            if (this.markedForRemoval.Count > 0)
            {
                List<string> modNames = new List<string>();
                foreach (ListViewItem item in this.markedForRemoval)
                {
                    modNames.Add(item.SubItems[displayHeader.Index].Text);
                }

                string m = "The following mods will be permanently be removed:\n" + string.Join("\n---", modNames) + "\nARE YOU SURE?";
                string c = "Are you sure?";
                MessageBoxButtons b = MessageBoxButtons.YesNo;
                DialogResult r = MessageBox.Show(m, c, b);

                if (r == DialogResult.Yes)
                {
                    foreach (ModListItem item in markedForRemoval)
                    {
                        ListViewData.Remove(item);
                        modsListView.Items.Remove(item);
                        logic.DeleteMod(logic.DirectoryToPathDict[item.SubItems[folderHeader.Index].Text]);
                        this.logic.ModDetails.Remove(logic.DirectoryToPathDict[item.SubItems[folderHeader.Index].Text]);
                    }
                    markedForRemoval.Clear();
                }
                else if (r == DialogResult.No)
                {
                    foreach (ListViewItem item in markedForRemoval)
                    {
                        item.ForeColor = Color.Black;
                    }
                    return;
                }
            }
            #endregion

            #region Activation and Load order
            //Stuff for applying mods activation and load order:

            //Reset filter:
            this.filterBox.Text = "";
            this.filterBox_TextChanged(null, null);

            //Regenerate ModList dict
            this.logic.ModList = new Dictionary<string, bool>();

            //For each mod in the list view:
            //Check if mod enabled
            //Get its priority
            //Put mod in the ModList with it status
            //Adjust the ModDetails priority
            int length = modsListView.Items.Count;
            for (int i = 0; i < length; i++)
            {
                string modName = modsListView.Items[i].SubItems[folderHeader.Index].Text;
                string modDir = logic.DirectoryToPathDict[modName];
                try
                {
                    bool modEnabled = modsListView.Items[i].Checked;
                    int priority = modsListView.Items.Count - i;
                    this.logic.ModList[modDir] = modEnabled;
                    this.logic.ModDetails[modDir].defaultLoadOrder = priority;
                    Console.WriteLine(modDir + " : " + priority.ToString());
                }
                catch (Exception Ex)
                {
                    string message = "ERROR Mismatch between list key and details key : " + modName
                        + ". Details keys available: " + string.Join(",", this.logic.ModDetails.Keys.ToList()) + ". This mod will be skipped and the operation continued.";
                    string caption = "ERROR Key Mismatch";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);
                    continue;
                }

            }

            //Save the ModDetails to json file.
            this.logic.SaveToFiles();
            #endregion
        }

        //For clearing the entire applications data
        public void ClearAll()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            panelModInfo.Visible = false;
            if (pictureBoxModImage.Image != null)
            {
                pictureBoxModImage.Image.Dispose();
            }
            this.ListViewData.Clear();
            this.modsListView.Items.Clear();
            logic.ClearAll();
        }

        //For processing internals and updating ui after setting a vendor
        private void SetVersionAndPlatform()
        {
            bool isSteam = false;

            if (this.logic.Version > 0f)
            {
                toolStripStatusLabelMwVersion.Text = @"~RJ v." + this.logic.Version.ToString();
            }

            switch (this.logic.GamePlatform)
            {
                case MainLogic.GamePlatformEnum.Epic:
                    {
                        this.toolStripPlatformLabel.Text = "Platform: Epic Store";
                        this.button4.Enabled = true;
                        button5.Enabled = true;
                        break;
                    }
                case MainLogic.GamePlatformEnum.WindowsStore:
                    {
                        this.toolStripPlatformLabel.Text = "Platform: Windows Store";
                        this.button4.Enabled = false;
                        button5.Enabled = true;
                    }
                    break;
                case MainLogic.GamePlatformEnum.Steam:
                    {
                        this.toolStripPlatformLabel.Text = "Platform: Steam";
                        button5.Enabled = false;
                        this.button4.Enabled = true;

                        isSteam = true;
                    }
                    break;
                case MainLogic.GamePlatformEnum.Gog:
                    {
                        this.toolStripPlatformLabel.Text = "Platform: GOG";
                        this.button4.Enabled = true;
                        button5.Enabled = true;
                    }
                    break;
            }

            toolStripMenuItemOpenModFolderSteam.Visible = isSteam;
        }

        //Load mod data and fill in the list box..
        public void LoadAndFill(bool FromClipboard)
        {
            this.LoadingAndFilling = true;
            KeyValuePair<string, bool> currentEntry = new KeyValuePair<string, bool>();
            try
            {
                if (FromClipboard)
                    logic.LoadFromImportString();
                else
                    logic.LoadFromFiles();

                foreach (KeyValuePair<string, bool> entry in logic.ModList)
                {
                    if (entry.Equals(new KeyValuePair<string, bool>(null, false)))
                        continue;
                    if (entry.Key == null)
                        continue;

                    currentEntry = entry;
                    AddEntryToListViewAndData(entry);
                }
                UpdateListView();
                logic.SaveSettings();
            }
            catch (Exception e)
            {
                if (currentEntry.Key == null)
                {
                    currentEntry = new KeyValuePair<string, bool>("NULL", false);
                }
                Console.WriteLine(e.StackTrace);
                string message = "While loading " + currentEntry.Key.ToString() + "something went wrong.\n" + e.StackTrace;
                string caption = "Error Loading";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
            this.LoadingAndFilling = false;
            logic.GetOverridingData(ListViewData);
        }

        private void AddEntryToListViewAndData(KeyValuePair<string, bool> entry)
        {
            string modName = entry.Key;
            ModListItem newItem = new ModListItem
            {
                UseItemStyleForSubItems = false,
                Checked = entry.Value
            };

            for (int i = 1; i < modsListView.Columns.Count; i++)
            {
                newItem.SubItems.Add("");
            }

            switch (logic.Mods[entry.Key].Origin)
            {
                case MainLogic.ModData.ModOrigin.Steam:
                    newItem.ImageKey = "Steam";
                    break;
                case MainLogic.ModData.ModOrigin.Nexusmods:
                    newItem.ImageKey = "Nexusmods";
                    break;
                default:
                    newItem.ImageKey = "Folder";
                    break;
            }

            newItem.SubItems[displayHeader.Index].Text = logic.ModDetails[entry.Key].displayName;
            newItem.SubItems[folderHeader.Index].Text = logic.PathToDirectoryDict[modName];
            newItem.SubItems[authorHeader.Index].Text = logic.ModDetails[entry.Key].author;
            newItem.SubItems[versionHeader.Index].Text = logic.ModDetails[entry.Key].version;
            newItem.SubItems[buildHeader.Index].Text = logic.ModDetails[entry.Key].buildNumber.ToString();
            newItem.SubItems[originalLoadOrderHeader.Index].Text = logic.Mods[entry.Key].OriginalLoadOrder.ToString();

            newItem.EnsureVisible();
            newItem.Tag = entry.Key;
            ListViewData.Add(newItem);
        }

        //Fill list view from internal list of data.
        private void UpdateListView()
        {
            modsListView.Items.Clear();
            modsListView.Items.AddRange(ListViewData.ToArray());
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

        public void SetInstallDirectory(string path)
        {
            logic.InstallPath = path;
            logic.BasePath[0] = path + @"\MW5Mercs\Mods";

            bool isSteam = false;

            switch (this.logic.GamePlatform)
            {
                case MainLogic.GamePlatformEnum.Steam:
                    SetSteamWorkshopPath();
                    isSteam = true;
                    break;
                //case "GAMEPASS":
                //    SetGamepassPath();
                //    break;

                case MainLogic.GamePlatformEnum.WindowsStore:
                    string AppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    this.logic.BasePath[0] = GetBasePathFromAppDataRoaming(AppDataRoaming);
                    this.logic.CheckModDirectories();
                    break;
            }

            MainForm.toolStripMenuItemOpenModFolderSteam.Visible = isSteam;

            LoadAndFill(false);
        }

        public void SetSteamWorkshopPath()
        {
            //Split by folder depth
            List<string> splitBasePath = this.logic.BasePath[0].Split('\\').ToList<string>();

            //Find the steamapps folder
            int steamAppsIndex = splitBasePath.FindIndex(x => x.Equals("steamapps", StringComparison.OrdinalIgnoreCase));

            //Remove all past the steamapps folder
            splitBasePath.RemoveRange(steamAppsIndex + 1, splitBasePath.Count - steamAppsIndex - 1);

            //Put string back together
            this.logic.BasePath[1] = string.Join("\\", splitBasePath);

            //Point to workshop folder.
            this.logic.BasePath[1] += @"\workshop\content\784080";
        }

        //Refresh listedcheckbox
        private void button6_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        public void RefreshAll()
        {
            ClearAll();
            if (logic.TryLoadProgramData())
            {
                LoadAndFill(false);
                filterBox_TextChanged(null, null);
                logic.GetOverridingData(ListViewData);
            }
        }

        //Saves current load order to preset.
        public void SavePreset(string name)
        {
            Dictionary<string, bool> NoPathModlist = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> entry in logic.ModList)
            {
                string folderName = logic.PathToDirectoryDict[entry.Key];
                NoPathModlist[folderName] = entry.Value;
            }
            this.logic.Presets[name] = JsonConvert.SerializeObject(NoPathModlist, Formatting.Indented);
            this.logic.SavePresets();
        }

        //Sets up the load order from a preset.
        private void LoadPreset(string name)
        {
            string JsonString = logic.Presets[name];
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
                MessageBox.Show(message, caption, buttons);
                return;
            }

            this.modsListView.Items.Clear();
            this.ListViewData.Clear();
            this.logic.ModDetails = new Dictionary<string, ModObject>();
            this.logic.ModList.Clear();
            this.logic.ModList = temp;
            this.logic.Mods.Clear();
            this.LoadAndFill(true);
            this.filterBox_TextChanged(null, null);
        }

        //Load all presets from file and fill the listbox.
        private void LoadPresets()
        {
            this.logic.LoadPresets();
            RebuildPresetsMenu();
        }
        public void RebuildPresetsMenu()
        {
            // Clear all preset menu items first
            var dropDownItems = MainWindow.MainForm.presetsToolStripMenuItem.DropDownItems;

            for (int i = dropDownItems.Count - 1; i >= 0; i--)
            {
                ToolStripItem item = dropDownItems[i];
                if (item.Tag != null)
                {
                    dropDownItems.Remove(item);
                }
            }

            int menuIndex = presetsToolStripMenuItem.DropDownItems.IndexOf(toolStripMenuItemLoadPresets);
            foreach (string key in logic.Presets.Keys)
            {
                menuIndex++;

                string menuItemName = key.Replace("&", "&&");
                ToolStripItem subItem = new ToolStripMenuItem(menuItemName);
                subItem.Tag = key;
                subItem.Click += presetMenuItem_Click;
                presetsToolStripMenuItem.DropDownItems.Insert(menuIndex, subItem);
            }
        }

        #region Vendor Selection Tool Strip buttons

        //Tool strip for selecting steam as a vendor
        private void steamToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Tool strip for selecting gog as a vendor
        private void gogToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Tool strip for selecting windows store as a vendor
        private void windowsStoreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private static string GetBasePathFromAppDataRoaming(string AppDataRoaming)
        {
            //Split by folder depth
            List<string> splitBasePath = AppDataRoaming.Split('\\').ToList<string>();

            //Find the steamapps folder
            int AppDataIndex = splitBasePath.IndexOf("AppData");

            //Remove all past the steamapps folder
            splitBasePath.RemoveRange(AppDataIndex + 1, splitBasePath.Count - AppDataIndex - 1);

            //Put string back together
            return string.Join("\\", splitBasePath) + @"\Local\MW5Mercs\Saved\Mods";
        }

        //Tool strip for selecting epic store as a vendor
        private void epicStoreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //Launch game button
        private void button4_Click(object sender, EventArgs e)
        {
            switch (logic.GamePlatform)
            {
                case MainLogic.GamePlatformEnum.Epic:
                    LaunchEpicGame();
                    break;
                case MainLogic.GamePlatformEnum.Steam:
                    LaunchSteamGame();
                    break;
                case MainLogic.GamePlatformEnum.Gog:
                    LaunchGogGame();
                    break;
                case MainLogic.GamePlatformEnum.WindowsStore:
                    LaunchWindowsGame();
                    break;
            }

        }

        #region Launch Game
        private static void LaunchWindowsGame()
        {
            //Dunno how this works at all.. 
            string message = "This feature is not available in this version.";
            string caption = "Feature not available.";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }

        private void LaunchGogGame()
        {
            string Gamepath = this.logic.BasePath[0];
            Gamepath = Gamepath.Remove(Gamepath.Length - 13, 13);
            Gamepath += "MechWarrior.exe";
            try
            {
                Process.Start(Gamepath);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to launch Mechwarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        private static void LaunchEpicGame()
        {
            try
            {
                Process.Start(@"com.epicgames.launcher://apps/Hoopoe?action=launch&silent=false");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to make EPIC Games Launcher launch Mechwarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        private static void LaunchSteamGame()
        {
            try
            {
                System.Diagnostics.Process.Start(@"steam://rungameid/784080");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine(Ex.StackTrace);
                string message = "There was an error while trying to make Steam launch Mechwarrior 5.";
                string caption = "Error Launching";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }
        #endregion

        //Crude filter because to lazy to add a proper list as backup for the items.
        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            string filtertext = MainForm.filterBox.Text.ToLower();
            if (Utils.StringNullEmptyOrWhiteSpace(filtertext))
            {
                //Console.WriteLine("No filter text");
                if (this.filtered) //we are returning from filtering
                {
                    foreach (ListViewItem x in this.ListViewData)
                    {
                        x.SubItems[displayHeader.Index].BackColor = Color.White;
                        x.SubItems[folderHeader.Index].BackColor = Color.White;
                        x.SubItems[authorHeader.Index].BackColor = Color.White;
                        x.SubItems[versionHeader.Index].BackColor = Color.White;
                    }
                    UpdateListView();
                }
                else //We are not returning from a filter
                {
                    //This should never happen. We can't return from a filter we never entered.
                    // do nothing
                }
                MainForm.button1.Enabled = true;
                MainForm.button2.Enabled = true;
                this.filtered = false;
            }
            else
            {
                this.filtered = true;
                //If we are filtering with highlight
                if (MainForm.checkBox1.Checked)
                {
                    //For each item in the list
                    foreach (ListViewItem item in this.ListViewData)
                    {
                        //Check if there is a hit.
                        if (MatchItemToText(filtertext, item))
                        {
                            item.SubItems[displayHeader.Index].BackColor = Color.Yellow;
                            item.SubItems[folderHeader.Index].BackColor = Color.Yellow;
                            item.SubItems[authorHeader.Index].BackColor = Color.Yellow;
                            item.SubItems[versionHeader.Index].BackColor = Color.Yellow;
                        }
                        //if not set to white.
                        else
                        {
                            item.SubItems[displayHeader.Index].BackColor = Color.White;
                            item.SubItems[folderHeader.Index].BackColor = Color.White;
                            item.SubItems[authorHeader.Index].BackColor = Color.White;
                            item.SubItems[versionHeader.Index].BackColor = Color.White;
                        }
                    }
                }
                //We are filtering by selected adding.
                else
                {
                    //Clear the list view
                    this.modsListView.Items.Clear();
                    //For each item check if they are a hit if so add them.
                    foreach (ListViewItem item in this.ListViewData)
                    {
                        if (MatchItemToText(filtertext, item))
                        {
                            item.SubItems[displayHeader.Index].BackColor = Color.White;
                            item.SubItems[folderHeader.Index].BackColor = Color.White;
                            item.SubItems[authorHeader.Index].BackColor = Color.White;
                            item.SubItems[versionHeader.Index].BackColor = Color.White;
                            MainForm.modsListView.Items.Add(item);
                        }
                    }
                }
                //While filtering disable the up/down buttons (tough this should no longer be needed).
                MainForm.button1.Enabled = false;
                MainForm.button2.Enabled = false;
            }
        }

        //Check if given listviewitem can be matched to a string.
        private bool MatchItemToText(string filtertext, ListViewItem item)
        {
            if
                (
                    item.SubItems[displayHeader.Index].Text.ToLower().StartsWith(filtertext) ||
                    item.SubItems[folderHeader.Index].Text.ToLower().StartsWith(filtertext) ||
                    item.SubItems[authorHeader.Index].Text.ToLower().StartsWith(filtertext) ||
                    item.SubItems[displayHeader.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[folderHeader.Index].Text.ToLower().Contains(filtertext) ||
                    item.SubItems[authorHeader.Index].Text.ToLower().Contains(filtertext)
                )
            {
                return true;
            }
            return false;
        }

        //Filter or Highlight checkbox on tick action
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.modsListView.Items.Clear();
                foreach (ModListItem item in this.ListViewData)
                {
                    modsListView.Items.Add(item);
                }
            }
            this.filterBox_TextChanged(null, null);
        }

        //Mark currently selected mod for removal upon apply
        private void button5_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in modsListView.SelectedItems)
            {
                if (this.markedForRemoval.Contains(item))
                {
                    markedForRemoval.Remove(item);
                    item.SubItems[displayHeader.Index].ForeColor = Color.Black;
                    item.Selected = false;
                    logic.ColorizeListViewItems(ListViewData);
                }
                else
                {
                    this.markedForRemoval.Add(item);
                    item.SubItems[displayHeader.Index].ForeColor = Color.Red;
                    item.Selected = false;
                }
            }
        }

        //Unused
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Selected index of mods overriding the currently selected mod has changed.
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox1.ClearSelected();
            if (listBox3.Items.Count == 0 || modsListView.Items.Count == 0)
                return;

            if (listBox3.SelectedItem == null)
                return;

            string selectedMod = listBox3.SelectedItem.ToString();
            if (Utils.StringNullEmptyOrWhiteSpace(selectedMod))
                return;

            string superMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;

            if (!logic.OverrridingData.ContainsKey(superMod))
                return;

            OverridingData modData = logic.OverrridingData[superMod];

            if (!modData.overriddenBy.ContainsKey(selectedMod))
                return;

            foreach (string entry in modData.overriddenBy[selectedMod])
            {
                listBox2.Items.Add(entry);
            }
        }

        //Selected indox of mods that are beeing overriden by the currently selected mod had changed.
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox3.ClearSelected();
            if (listBox1.Items.Count == 0 || modsListView.Items.Count == 0)
                return;

            if (listBox1.SelectedItem == null)
                return;
            string selectedMod = listBox1.SelectedItem.ToString();

            if (Utils.StringNullEmptyOrWhiteSpace(selectedMod))
                return;

            string superMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;

            if (!logic.OverrridingData.ContainsKey(superMod))
                return;

            OverridingData modData = logic.OverrridingData[superMod];

            foreach (string entry in modData.overrides[selectedMod])
            {
                listBox2.Items.Add(entry);
            }
        }

        //Selected item in the list view has cahnged
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label4.Text = "";

            if (modsListView.SelectedItems.Count == 0)
            {
                panelModInfo.Visible = false;
                return;
            }

            string SelectedMod = modsListView.SelectedItems[0].SubItems[folderHeader.Index].Text;
            string SelectedModDisplayName = modsListView.SelectedItems[0].SubItems[displayHeader.Index].Text;

            if (Utils.StringNullEmptyOrWhiteSpace(SelectedMod) ||
                Utils.StringNullEmptyOrWhiteSpace(SelectedModDisplayName)
               )
            {
                panelModInfo.Visible = false;
                return;
            }

            string modPath = (string)modsListView.SelectedItems[0].Tag;
            ModObject modDetails = logic.ModDetails[modPath];

            panelModInfo.Visible = true;
            labelModName.Text = SelectedModDisplayName;
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

            string nexusModsId = logic.Mods[modPath].NexusModsId;
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

            HandleOverrding(SelectedMod);

            string imagePath = modPath + "\\Resources\\Icon128.png";

            if (pictureBoxModImage.Image != null)
            {
                pictureBoxModImage.Image.Dispose();
            }

            if (File.Exists(imagePath))
            {
                pictureBoxModImage.Image = Image.FromStream(new MemoryStream(File.ReadAllBytes(imagePath)));
            }
        }

        //Handles the showing of overrding data on select
        private void HandleOverrding(string SelectedMod)
        {
            if (logic.OverrridingData.Count == 0)
                return;

            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            this.listBox3.Items.Clear();

            this.label4.Text = SelectedMod;

            //If we select a mod that is not ticked its data is never gotten so will get an error if we don't do this.
            if (!logic.OverrridingData.ContainsKey(SelectedMod))
                return;

            OverridingData modData = logic.OverrridingData[SelectedMod];
            foreach (string orverriding in modData.overriddenBy.Keys)
            {
                this.listBox3.Items.Add(orverriding);
            }
            foreach (string overrides in modData.overrides.Keys)
            {
                this.listBox1.Items.Add(overrides);
            }
        }

        //Fires when an item is checked or unchecked.
        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //While we are removing/inserting items this will fire and we dont want that to happen when we move an item.
            if (MovingItem || this.filtered || this.LoadingAndFilling)
            {
                return;
            }

            logic.UpdateNewModOverrideData(ListViewData, ListViewData[e.Item.Index]);
            HandleOverrding(e.Item.SubItems[folderHeader.Index].Text);
        }

        //Check for mod overrding data
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.logic.GetOverridingData(ListViewData);
        }

        //On tap click?
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        #region background workers for zipping up files
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            this.logic.PackModsToZip(worker, e);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled || (string)e.Result == "ABORTED")
            {
                //we just wanna do nothing and return here.
                MainForm.WaitForm.Close();
                //MessageBox.Show("TEST123");
            }
            else
            {
                //We are actually done!
                MainForm.WaitForm.Close();

                //For when we just wanna pack and not show the dialog
                if (!JustPacking)
                {
                    JustPacking = true;
                    return;
                }

                //Returing from dialog:
                SystemSounds.Asterisk.Play();
                //Get parent dir
                string parent = Directory.GetParent(this.logic.BasePath[0]).ToString();
                string m = "Done packing mods, output in: \n" + parent + "\\Mods.zip";
                string c = "Done";
                MessageBoxButtons b = MessageBoxButtons.OK;
                MessageBox.Show(m, c, b);
                Process.Start(parent);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            logic.MonitorZipSize(worker, e);
            //We dont need to pass any results anywhere as we are just monitoring.
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                //We are actually done!
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            /*MainForm.textBox1.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                MainForm.WaitForm.textProgressBar1.Value = e.ProgressPercentage;
            });*/
        }
        #endregion

        private void shareModsViaTCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(this);
            form5.ShowDialog(this);
        }

        private void exportLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportWindow exportDialog = new ExportWindow();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            exportDialog.ShowDialog(this);
            exportDialog.Dispose();
        }

        private void importLoadOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportWindow testDialog = new ImportWindow();
            string txtResult = "";

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            testDialog.ShowDialog(this);
            txtResult = testDialog.textBox1.Text;
            testDialog.Dispose();

            if (Utils.StringNullEmptyOrWhiteSpace(txtResult) ||
                txtResult == "Paste load order clipboard here, any mods that you do not have but are in the pasted load order will be ignored.")
                return;

            Dictionary<string, bool> temp;
            try
            {
                temp = JsonConvert.DeserializeObject<Dictionary<string, bool>>(txtResult);//logic.UnScramble(txtResult));
            }
            catch (Exception Ex)
            {
                string message = "There was an error in decoding the load order string.";
                string caption = "Load Order Decoding Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
                return;
            }
            //this.ClearAll();
            this.modsListView.Items.Clear();
            this.ListViewData.Clear();
            this.logic.ModDetails = new Dictionary<string, ModObject>();
            this.logic.ModList = new Dictionary<string, bool>();
            this.logic.ModList = temp;
            this.LoadAndFill(true);
            this.filterBox_TextChanged(null, null);
        }

        private void exportmodsFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //apply current settings to file
            this.button3_Click(null, null);

            //start packing worker
            backgroundWorker1.RunWorkerAsync();
            //A little time to start up
            System.Threading.Thread.Sleep(100);
            //Start monitoring worker
            backgroundWorker2.RunWorkerAsync();

            //Show Form 4 with informing user that we are packaging mods..
            Console.WriteLine("Opening form:");
            this.WaitForm = new Form4(backgroundWorker1, backgroundWorker2);
            string message = "Packaging Mods.zip, this may take several minutes depending on the combinded size of your mods...";
            this.WaitForm.textBox1.Text = message;
            string caption = "Packing Mods.zip";
            this.WaitForm.Text = caption;
            WaitForm.ShowDialog(this);

            backgroundWorker2.CancelAsync();
            //For the rest of the code see "background"
        }

        private void openModsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.StringNullEmptyOrWhiteSpace(this.logic.BasePath[0]))
            {
                return;
            }
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = this.logic.BasePath[0],
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutDialog = new AboutWindow();

            aboutDialog.ShowDialog(this);
            aboutDialog.Dispose();
        }

        private void enableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MovingItem = true;
            foreach (ModListItem item in this.ListViewData)
            {
                item.Checked = true;
            }
            this.MovingItem = false;
            this.logic.GetOverridingData(this.ListViewData);
        }

        private void disableAllModsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MovingItem = true;
            foreach (ListViewItem item in this.modsListView.Items)
            {
                item.Checked = false;
            }
            this.MovingItem = false;
            this.logic.GetOverridingData(ListViewData);
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

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void linkLabelModAuthorUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string modUrl = logic.ModDetails[modKey].authorURL;
            bool isValidUrl = Utils.IsUrlValid(modUrl);
            if (isValidUrl)
            {
                Process.Start(modUrl);
            }
        }

        private void linkLabelSteamId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string steamUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=" + logic.ModDetails[modKey].steamPublishedFileId;
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

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsDialog = new SettingsWindow();

            settingsDialog.ShowDialog(this);
            settingsDialog.Dispose();
        }

        private void toolStripMenuItemOpenModFolderSteam_Click(object sender, EventArgs e)
        {
            if (Utils.StringNullEmptyOrWhiteSpace(this.logic.BasePath[1]))
            {
                return;
            }
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = this.logic.BasePath[1],
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
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
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
            // Retrieve the index of the insertion mark;
            int targetIndex = modsListView.InsertionMark.Index;

            // If the insertion mark is not visible, exit the method.
            if (targetIndex == -1)
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
            ModListItem draggedItem =
                (ModListItem)e.Data.GetData(typeof(ModListItem));

            ListView.ListViewItemCollection items = modsListView.Items;
            int itemIndex = draggedItem.Index;

            targetIndex = itemIndex < targetIndex ? targetIndex - 1 : targetIndex;

            if (itemIndex != targetIndex)
            {
                modsListView.SuspendLayout();
                items.RemoveAt(itemIndex);
                ListViewData.RemoveAt(itemIndex);

                items.Insert(targetIndex, draggedItem);
                ListViewData.Insert(targetIndex, draggedItem);

                this.logic.GetOverridingData(this.ListViewData);
                listView1_SelectedIndexChanged(null, null);
                modsListView.ResumeLayout();
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
            PresetSaveWindow saveDialog = new PresetSaveWindow();

            saveDialog.ShowDialog(this);
            saveDialog.Dispose();
        }

        private void deletePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PresetDeleteWindow deleteDialog = new PresetDeleteWindow();

            deleteDialog.ShowDialog(this);
            deleteDialog.Dispose();
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = (string)modsListView.SelectedItems[0].Tag;
            string nexusUrl = "https://www.nexusmods.com/mechwarrior5mercenaries/mods/" + logic.Mods[modKey].NexusModsId;

            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = nexusUrl,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}