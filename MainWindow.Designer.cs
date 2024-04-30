using System;
using System.Drawing;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            openFileDialog1 = new OpenFileDialog();
            toolStripPlatformLabel = new ToolStripStatusLabel();
            modsListView = new ListView();
            enabledHeader = new ColumnHeader();
            displayHeader = new ColumnHeader();
            authorHeader = new ColumnHeader();
            versionHeader = new ColumnHeader();
            currentLoadOrderHeader = new ColumnHeader();
            originalLoadOrderHeader = new ColumnHeader();
            fileSizeHeader = new ColumnHeader();
            folderHeader = new ColumnHeader();
            imageListIcons = new ImageList(components);
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            textProgressBarBindingSource = new BindingSource(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemImportArchive = new ToolStripMenuItem();
            toolStripMenuItemImportFromFolder = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            importLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
            exportLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            exportmodsFolderToolStripMenuItem1 = new ToolStripMenuItem();
            shareModsViaTCPToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripMenuItemSettings = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            presetsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemLoadPresets = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            savePresetToolStripMenuItem = new ToolStripMenuItem();
            deletePresetToolStripMenuItem = new ToolStripMenuItem();
            modsToolStripMenuItem = new ToolStripMenuItem();
            enableAllModsToolStripMenuItem = new ToolStripMenuItem();
            disableAllModsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripMenuItemSortDefaultLoadOrder = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            openModsFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemOpenModFolderSteam = new ToolStripMenuItem();
            openUserModsFolderToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemNexusmodsLink = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabelModsActive = new ToolStripStatusLabel();
            toolStripStatusLabelModCountTotal = new ToolStripStatusLabel();
            toolStripStatusLabelMwVersion = new ToolStripStatusLabel();
            contextMenuStripMod = new ContextMenuStrip(components);
            moveupToolStripMenuItem = new ToolStripMenuItem();
            movedownToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            contextMenuItemMoveToTop = new ToolStripMenuItem();
            contextMenuItemMoveToBottom = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            tabPage1 = new TabPage();
            richTextBoxManifestOverridden = new RichTextBox();
            splitContainer2 = new SplitContainer();
            listBoxOverriding = new ListBox();
            label6 = new Label();
            listBoxOverriddenBy = new ListBox();
            label5 = new Label();
            labelModNameOverrides = new Label();
            label7 = new Label();
            tabPageModInfo = new TabPage();
            pictureBoxModImage = new PictureBox();
            panelModInfo = new Panel();
            pictureBoxNexusmodsIcon = new PictureBox();
            labelNexusmods = new Label();
            linkLabelNexusmods = new LinkLabel();
            pictureBoxSteamIcon = new PictureBox();
            label1 = new Label();
            richTextBoxModDescription = new RichTextBox();
            labelSteamId = new Label();
            linkLabelSteamId = new LinkLabel();
            linkLabelModAuthorUrl = new LinkLabel();
            labelModBuildNumber = new Label();
            labelModVersion = new Label();
            labelModAuthor = new Label();
            labelModName = new Label();
            tabControl1 = new TabControl();
            toolTip1 = new ToolTip(components);
            rotatingLabelHighPriority = new RotatingLabel();
            rotatingLabelLowPriority = new RotatingLabel();
            panelColorLegend = new Panel();
            label8 = new Label();
            label4 = new Label();
            label2 = new Label();
            panelColorOverridingOverridden = new Panel();
            panelColorOverriding = new Panel();
            panelColorOverridden = new Panel();
            splitContainerMain = new SplitContainer();
            toolStrip1 = new ToolStrip();
            toolStripButtonApply = new ToolStripButton();
            toolStripButtonStartGame = new ToolStripButton();
            toolStripSeparator10 = new ToolStripSeparator();
            toolStripButtonReload = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            toolStripTextFilterBox = new ToolStripTextBox();
            toolStripButtonClearFilter = new ToolStripButton();
            toolStripButtonFilterToggle = new ToolStripButton();
            toolStripSeparator11 = new ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            contextMenuStripMod.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabPageModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).BeginInit();
            panelModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).BeginInit();
            tabControl1.SuspendLayout();
            panelColorLegend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // toolStripPlatformLabel
            // 
            toolStripPlatformLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripPlatformLabel.Name = "toolStripPlatformLabel";
            toolStripPlatformLabel.Size = new Size(16, 19);
            toolStripPlatformLabel.Text = "-";
            // 
            // modsListView
            // 
            modsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modsListView.CheckBoxes = true;
            modsListView.Columns.AddRange(new ColumnHeader[] { enabledHeader, displayHeader, authorHeader, versionHeader, currentLoadOrderHeader, originalLoadOrderHeader, fileSizeHeader, folderHeader });
            modsListView.FullRowSelect = true;
            modsListView.GridLines = true;
            modsListView.LabelWrap = false;
            modsListView.Location = new Point(30, 3);
            modsListView.MultiSelect = false;
            modsListView.Name = "modsListView";
            modsListView.RightToLeft = RightToLeft.No;
            modsListView.Size = new Size(795, 454);
            modsListView.SmallImageList = imageListIcons;
            modsListView.TabIndex = 10;
            modsListView.UseCompatibleStateImageBehavior = false;
            modsListView.View = View.Details;
            modsListView.ItemChecked += modListView_ItemChecked;
            modsListView.ItemDrag += modsListView_ItemDrag;
            modsListView.SelectedIndexChanged += modListView_SelectedIndexChanged;
            modsListView.DragDrop += modsListView_DragDrop;
            modsListView.DragEnter += modsListView_DragEnter;
            modsListView.DragOver += modsListView_DragOver;
            modsListView.DragLeave += modsListView_DragLeave;
            modsListView.MouseClick += modsListView_MouseClick;
            // 
            // enabledHeader
            // 
            enabledHeader.Tag = "";
            enabledHeader.Text = "";
            enabledHeader.Width = 38;
            // 
            // displayHeader
            // 
            displayHeader.Tag = "";
            displayHeader.Text = "Display Name";
            displayHeader.Width = 260;
            // 
            // authorHeader
            // 
            authorHeader.Tag = "";
            authorHeader.Text = "Author";
            authorHeader.Width = 90;
            // 
            // versionHeader
            // 
            versionHeader.Tag = "";
            versionHeader.Text = "Version";
            versionHeader.Width = 72;
            // 
            // currentLoadOrderHeader
            // 
            currentLoadOrderHeader.Text = "Current Load Order";
            currentLoadOrderHeader.Width = 40;
            // 
            // originalLoadOrderHeader
            // 
            originalLoadOrderHeader.Text = "Default Load Order";
            originalLoadOrderHeader.Width = 40;
            // 
            // fileSizeHeader
            // 
            fileSizeHeader.Text = "Size";
            // 
            // folderHeader
            // 
            folderHeader.Tag = "";
            folderHeader.Text = "Mod Folder";
            folderHeader.Width = 110;
            // 
            // imageListIcons
            // 
            imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageListIcons.ImageStream = (ImageListStreamer)resources.GetObject("imageListIcons.ImageStream");
            imageListIcons.TransparentColor = Color.Transparent;
            imageListIcons.Images.SetKeyName(0, "Folder");
            imageListIcons.Images.SetKeyName(1, "Steam");
            imageListIcons.Images.SetKeyName(2, "Nexusmods");
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, presetsToolStripMenuItem, modsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1167, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemImportArchive, toolStripMenuItemImportFromFolder, toolStripSeparator12, importLoadOrderToolStripMenuItem1, exportLoadOrderToolStripMenuItem1, toolStripSeparator7, exportmodsFolderToolStripMenuItem1, shareModsViaTCPToolStripMenuItem, toolStripSeparator2, toolStripMenuItemSettings, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItemImportArchive
            // 
            toolStripMenuItemImportArchive.Name = "toolStripMenuItemImportArchive";
            toolStripMenuItemImportArchive.Size = new Size(189, 22);
            toolStripMenuItemImportArchive.Text = "Import from &archive...";
            toolStripMenuItemImportArchive.Click += toolStripMenuItemImportArchive_Click;
            // 
            // toolStripMenuItemImportFromFolder
            // 
            toolStripMenuItemImportFromFolder.Name = "toolStripMenuItemImportFromFolder";
            toolStripMenuItemImportFromFolder.Size = new Size(189, 22);
            toolStripMenuItemImportFromFolder.Text = "Import from &folder...";
            toolStripMenuItemImportFromFolder.Click += toolStripMenuItemImportFromFolder_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(186, 6);
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            importLoadOrderToolStripMenuItem1.Size = new Size(189, 22);
            importLoadOrderToolStripMenuItem1.Text = "&Import load order...";
            importLoadOrderToolStripMenuItem1.Click += importLoadOrderToolStripMenuItem1_Click;
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            exportLoadOrderToolStripMenuItem1.Size = new Size(189, 22);
            exportLoadOrderToolStripMenuItem1.Text = "&Export load order...";
            exportLoadOrderToolStripMenuItem1.Click += exportLoadOrderToolStripMenuItem1_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(186, 6);
            toolStripSeparator7.Visible = false;
            // 
            // exportmodsFolderToolStripMenuItem1
            // 
            exportmodsFolderToolStripMenuItem1.Name = "exportmodsFolderToolStripMenuItem1";
            exportmodsFolderToolStripMenuItem1.Size = new Size(189, 22);
            exportmodsFolderToolStripMenuItem1.Text = "Export &mods folder";
            exportmodsFolderToolStripMenuItem1.Visible = false;
            exportmodsFolderToolStripMenuItem1.Click += exportmodsFolderToolStripMenuItem1_Click;
            // 
            // shareModsViaTCPToolStripMenuItem
            // 
            shareModsViaTCPToolStripMenuItem.Name = "shareModsViaTCPToolStripMenuItem";
            shareModsViaTCPToolStripMenuItem.Size = new Size(189, 22);
            shareModsViaTCPToolStripMenuItem.Text = "Share mods via &TCP";
            shareModsViaTCPToolStripMenuItem.Visible = false;
            shareModsViaTCPToolStripMenuItem.Click += shareModsViaTCPToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(186, 6);
            // 
            // toolStripMenuItemSettings
            // 
            toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            toolStripMenuItemSettings.Size = new Size(189, 22);
            toolStripMenuItemSettings.Text = "&Settings";
            toolStripMenuItemSettings.Click += toolStripMenuItemSettings_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(186, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(189, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // presetsToolStripMenuItem
            // 
            presetsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemLoadPresets, toolStripSeparator4, savePresetToolStripMenuItem, deletePresetToolStripMenuItem });
            presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            presetsToolStripMenuItem.Size = new Size(56, 20);
            presetsToolStripMenuItem.Text = "&Presets";
            // 
            // toolStripMenuItemLoadPresets
            // 
            toolStripMenuItemLoadPresets.Enabled = false;
            toolStripMenuItemLoadPresets.Name = "toolStripMenuItemLoadPresets";
            toolStripMenuItemLoadPresets.Size = new Size(151, 22);
            toolStripMenuItemLoadPresets.Text = "Load Preset:";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(148, 6);
            // 
            // savePresetToolStripMenuItem
            // 
            savePresetToolStripMenuItem.Name = "savePresetToolStripMenuItem";
            savePresetToolStripMenuItem.Size = new Size(151, 22);
            savePresetToolStripMenuItem.Text = "&Save Preset...";
            savePresetToolStripMenuItem.Click += savePresetToolStripMenuItem_Click;
            // 
            // deletePresetToolStripMenuItem
            // 
            deletePresetToolStripMenuItem.Name = "deletePresetToolStripMenuItem";
            deletePresetToolStripMenuItem.Size = new Size(151, 22);
            deletePresetToolStripMenuItem.Text = "&Delete Preset...";
            deletePresetToolStripMenuItem.Click += deletePresetToolStripMenuItem_Click;
            // 
            // modsToolStripMenuItem
            // 
            modsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { enableAllModsToolStripMenuItem, disableAllModsToolStripMenuItem, toolStripSeparator3, toolStripMenuItemSortDefaultLoadOrder, toolStripSeparator8, openModsFolderToolStripMenuItem, toolStripMenuItemOpenModFolderSteam, openUserModsFolderToolStripMenuItem });
            modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            modsToolStripMenuItem.Size = new Size(49, 20);
            modsToolStripMenuItem.Text = "&Mods";
            // 
            // enableAllModsToolStripMenuItem
            // 
            enableAllModsToolStripMenuItem.Name = "enableAllModsToolStripMenuItem";
            enableAllModsToolStripMenuItem.Size = new Size(208, 22);
            enableAllModsToolStripMenuItem.Text = "&Enable all";
            enableAllModsToolStripMenuItem.Click += enableAllModsToolStripMenuItem_Click;
            // 
            // disableAllModsToolStripMenuItem
            // 
            disableAllModsToolStripMenuItem.Name = "disableAllModsToolStripMenuItem";
            disableAllModsToolStripMenuItem.Size = new Size(208, 22);
            disableAllModsToolStripMenuItem.Text = "&Disable all";
            disableAllModsToolStripMenuItem.Click += disableAllModsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(205, 6);
            // 
            // toolStripMenuItemSortDefaultLoadOrder
            // 
            toolStripMenuItemSortDefaultLoadOrder.Name = "toolStripMenuItemSortDefaultLoadOrder";
            toolStripMenuItemSortDefaultLoadOrder.Size = new Size(208, 22);
            toolStripMenuItemSortDefaultLoadOrder.Text = "&Sort by default load order";
            toolStripMenuItemSortDefaultLoadOrder.Click += toolStripMenuItemSortDefaultLoadOrder_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(205, 6);
            // 
            // openModsFolderToolStripMenuItem
            // 
            openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            openModsFolderToolStripMenuItem.Size = new Size(208, 22);
            openModsFolderToolStripMenuItem.Text = "&Open Mods Folder";
            openModsFolderToolStripMenuItem.Click += openModsFolderToolStripMenuItem_Click;
            // 
            // toolStripMenuItemOpenModFolderSteam
            // 
            toolStripMenuItemOpenModFolderSteam.Name = "toolStripMenuItemOpenModFolderSteam";
            toolStripMenuItemOpenModFolderSteam.Size = new Size(208, 22);
            toolStripMenuItemOpenModFolderSteam.Text = "Open S&team Mods folder";
            toolStripMenuItemOpenModFolderSteam.Visible = false;
            toolStripMenuItemOpenModFolderSteam.Click += toolStripMenuItemOpenModFolderSteam_Click;
            // 
            // openUserModsFolderToolStripMenuItem
            // 
            openUserModsFolderToolStripMenuItem.Name = "openUserModsFolderToolStripMenuItem";
            openUserModsFolderToolStripMenuItem.Size = new Size(208, 22);
            openUserModsFolderToolStripMenuItem.Text = "Open &User Mods folder";
            openUserModsFolderToolStripMenuItem.Click += openUserModsFolderToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemNexusmodsLink, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(40, 20);
            helpToolStripMenuItem.Text = "&Info";
            // 
            // toolStripMenuItemNexusmodsLink
            // 
            toolStripMenuItemNexusmodsLink.Name = "toolStripMenuItemNexusmodsLink";
            toolStripMenuItemNexusmodsLink.Size = new Size(180, 22);
            toolStripMenuItemNexusmodsLink.Text = "Visit on &Nexusmods";
            toolStripMenuItemNexusmodsLink.Click += toolStripMenuItemNexusmodsLink_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(180, 22);
            aboutToolStripMenuItem.Text = "Ab&out";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabelModsActive, toolStripStatusLabelModCountTotal, toolStripPlatformLabel, toolStripStatusLabelMwVersion });
            statusStrip1.Location = new Point(0, 555);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1167, 24);
            statusStrip1.TabIndex = 36;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(1058, 19);
            toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabelModsActive
            // 
            toolStripStatusLabelModsActive.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelModsActive.Name = "toolStripStatusLabelModsActive";
            toolStripStatusLabelModsActive.Size = new Size(16, 19);
            toolStripStatusLabelModsActive.Text = "-";
            // 
            // toolStripStatusLabelModCountTotal
            // 
            toolStripStatusLabelModCountTotal.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelModCountTotal.Name = "toolStripStatusLabelModCountTotal";
            toolStripStatusLabelModCountTotal.Size = new Size(16, 19);
            toolStripStatusLabelModCountTotal.Text = "-";
            // 
            // toolStripStatusLabelMwVersion
            // 
            toolStripStatusLabelMwVersion.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelMwVersion.Margin = new Padding(0, 3, 30, 2);
            toolStripStatusLabelMwVersion.Name = "toolStripStatusLabelMwVersion";
            toolStripStatusLabelMwVersion.Size = new Size(16, 19);
            toolStripStatusLabelMwVersion.Text = "-";
            // 
            // contextMenuStripMod
            // 
            contextMenuStripMod.Items.AddRange(new ToolStripItem[] { moveupToolStripMenuItem, movedownToolStripMenuItem, toolStripSeparator6, contextMenuItemMoveToTop, contextMenuItemMoveToBottom, toolStripSeparator5, openFolderToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStripMod.Name = "contextMenuStripMod";
            contextMenuStripMod.Size = new Size(162, 148);
            // 
            // moveupToolStripMenuItem
            // 
            moveupToolStripMenuItem.Name = "moveupToolStripMenuItem";
            moveupToolStripMenuItem.Size = new Size(161, 22);
            moveupToolStripMenuItem.Text = "Move &up";
            moveupToolStripMenuItem.Click += moveupToolStripMenuItem_Click;
            // 
            // movedownToolStripMenuItem
            // 
            movedownToolStripMenuItem.Name = "movedownToolStripMenuItem";
            movedownToolStripMenuItem.Size = new Size(161, 22);
            movedownToolStripMenuItem.Text = "Move &down";
            movedownToolStripMenuItem.Click += movedownToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(158, 6);
            // 
            // contextMenuItemMoveToTop
            // 
            contextMenuItemMoveToTop.Name = "contextMenuItemMoveToTop";
            contextMenuItemMoveToTop.Size = new Size(161, 22);
            contextMenuItemMoveToTop.Text = "Move to &top";
            contextMenuItemMoveToTop.Click += contextMenuItemMoveToTop_Click;
            // 
            // contextMenuItemMoveToBottom
            // 
            contextMenuItemMoveToBottom.Name = "contextMenuItemMoveToBottom";
            contextMenuItemMoveToBottom.Size = new Size(161, 22);
            contextMenuItemMoveToBottom.Text = "Move to &bottom";
            contextMenuItemMoveToBottom.Click += contextMenuItemMoveToBottom_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(158, 6);
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new Size(161, 22);
            openFolderToolStripMenuItem.Text = "Open &Folder";
            openFolderToolStripMenuItem.Click += openFolderToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(161, 22);
            deleteToolStripMenuItem.Text = "D&elete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(richTextBoxManifestOverridden);
            tabPage1.Controls.Add(splitContainer2);
            tabPage1.Controls.Add(labelModNameOverrides);
            tabPage1.Controls.Add(label7);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(326, 455);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Overrides";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBoxManifestOverridden
            // 
            richTextBoxManifestOverridden.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxManifestOverridden.Location = new Point(9, 164);
            richTextBoxManifestOverridden.Name = "richTextBoxManifestOverridden";
            richTextBoxManifestOverridden.ReadOnly = true;
            richTextBoxManifestOverridden.Size = new Size(309, 294);
            richTextBoxManifestOverridden.TabIndex = 28;
            richTextBoxManifestOverridden.Text = "";
            richTextBoxManifestOverridden.WordWrap = false;
            // 
            // splitContainer2
            // 
            splitContainer2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new Point(9, 36);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(listBoxOverriding);
            splitContainer2.Panel1.Controls.Add(label6);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(listBoxOverriddenBy);
            splitContainer2.Panel2.Controls.Add(label5);
            splitContainer2.Size = new Size(309, 100);
            splitContainer2.SplitterDistance = 152;
            splitContainer2.TabIndex = 27;
            // 
            // listBoxOverriding
            // 
            listBoxOverriding.Dock = DockStyle.Bottom;
            listBoxOverriding.FormattingEnabled = true;
            listBoxOverriding.HorizontalScrollbar = true;
            listBoxOverriding.ItemHeight = 13;
            listBoxOverriding.Location = new Point(0, 18);
            listBoxOverriding.Name = "listBoxOverriding";
            listBoxOverriding.Size = new Size(152, 82);
            listBoxOverriding.TabIndex = 21;
            listBoxOverriding.SelectedIndexChanged += listBoxOverriding_SelectedIndexChanged;
            listBoxOverriding.MouseDoubleClick += listBoxOverriding_MouseDoubleClick;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(0, 2);
            label6.Name = "label6";
            label6.Size = new Size(62, 13);
            label6.TabIndex = 25;
            label6.Text = "Overriding";
            // 
            // listBoxOverriddenBy
            // 
            listBoxOverriddenBy.Dock = DockStyle.Bottom;
            listBoxOverriddenBy.FormattingEnabled = true;
            listBoxOverriddenBy.HorizontalScrollbar = true;
            listBoxOverriddenBy.ItemHeight = 13;
            listBoxOverriddenBy.Location = new Point(0, 18);
            listBoxOverriddenBy.Name = "listBoxOverriddenBy";
            listBoxOverriddenBy.Size = new Size(153, 82);
            listBoxOverriddenBy.TabIndex = 23;
            listBoxOverriddenBy.SelectedIndexChanged += listBox3_SelectedIndexChanged;
            listBoxOverriddenBy.MouseDoubleClick += listBoxOverriddenBy_MouseDoubleClick;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(0, 2);
            label5.Name = "label5";
            label5.Size = new Size(79, 13);
            label5.TabIndex = 24;
            label5.Text = "Overridden By";
            // 
            // labelModNameOverrides
            // 
            labelModNameOverrides.AutoSize = true;
            labelModNameOverrides.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModNameOverrides.Location = new Point(9, 12);
            labelModNameOverrides.Name = "labelModNameOverrides";
            labelModNameOverrides.Size = new Size(19, 13);
            labelModNameOverrides.TabIndex = 20;
            labelModNameOverrides.Text = "---";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(9, 148);
            label7.Name = "label7";
            label7.Size = new Size(74, 13);
            label7.TabIndex = 26;
            label7.Text = "Mod content";
            // 
            // tabPageModInfo
            // 
            tabPageModInfo.Controls.Add(pictureBoxModImage);
            tabPageModInfo.Controls.Add(panelModInfo);
            tabPageModInfo.Location = new Point(4, 22);
            tabPageModInfo.Name = "tabPageModInfo";
            tabPageModInfo.Padding = new Padding(3);
            tabPageModInfo.Size = new Size(326, 457);
            tabPageModInfo.TabIndex = 3;
            tabPageModInfo.Text = "Overview";
            tabPageModInfo.UseVisualStyleBackColor = true;
            // 
            // pictureBoxModImage
            // 
            pictureBoxModImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxModImage.Location = new Point(3, 6);
            pictureBoxModImage.Margin = new Padding(0);
            pictureBoxModImage.Name = "pictureBoxModImage";
            pictureBoxModImage.Size = new Size(318, 99);
            pictureBoxModImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxModImage.TabIndex = 2;
            pictureBoxModImage.TabStop = false;
            // 
            // panelModInfo
            // 
            panelModInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelModInfo.Controls.Add(pictureBoxNexusmodsIcon);
            panelModInfo.Controls.Add(labelNexusmods);
            panelModInfo.Controls.Add(linkLabelNexusmods);
            panelModInfo.Controls.Add(pictureBoxSteamIcon);
            panelModInfo.Controls.Add(label1);
            panelModInfo.Controls.Add(richTextBoxModDescription);
            panelModInfo.Controls.Add(labelSteamId);
            panelModInfo.Controls.Add(linkLabelSteamId);
            panelModInfo.Controls.Add(linkLabelModAuthorUrl);
            panelModInfo.Controls.Add(labelModBuildNumber);
            panelModInfo.Controls.Add(labelModVersion);
            panelModInfo.Controls.Add(labelModAuthor);
            panelModInfo.Controls.Add(labelModName);
            panelModInfo.Location = new Point(0, 105);
            panelModInfo.Margin = new Padding(0);
            panelModInfo.Name = "panelModInfo";
            panelModInfo.Size = new Size(343, 352);
            panelModInfo.TabIndex = 1;
            panelModInfo.Visible = false;
            // 
            // pictureBoxNexusmodsIcon
            // 
            pictureBoxNexusmodsIcon.Image = (Image)resources.GetObject("pictureBoxNexusmodsIcon.Image");
            pictureBoxNexusmodsIcon.Location = new Point(8, 123);
            pictureBoxNexusmodsIcon.Name = "pictureBoxNexusmodsIcon";
            pictureBoxNexusmodsIcon.Size = new Size(16, 16);
            pictureBoxNexusmodsIcon.TabIndex = 14;
            pictureBoxNexusmodsIcon.TabStop = false;
            // 
            // labelNexusmods
            // 
            labelNexusmods.AutoSize = true;
            labelNexusmods.Location = new Point(26, 125);
            labelNexusmods.Name = "labelNexusmods";
            labelNexusmods.Size = new Size(83, 13);
            labelNexusmods.TabIndex = 13;
            labelNexusmods.Text = "Nexusmods ID:";
            // 
            // linkLabelNexusmods
            // 
            linkLabelNexusmods.AutoSize = true;
            linkLabelNexusmods.Location = new Point(111, 126);
            linkLabelNexusmods.Name = "linkLabelNexusmods";
            linkLabelNexusmods.Size = new Size(112, 13);
            linkLabelNexusmods.TabIndex = 12;
            linkLabelNexusmods.TabStop = true;
            linkLabelNexusmods.Text = "linkLabelNexusmods";
            linkLabelNexusmods.LinkClicked += linkLabelNexusmods_LinkClicked;
            // 
            // pictureBoxSteamIcon
            // 
            pictureBoxSteamIcon.Image = (Image)resources.GetObject("pictureBoxSteamIcon.Image");
            pictureBoxSteamIcon.Location = new Point(8, 101);
            pictureBoxSteamIcon.Name = "pictureBoxSteamIcon";
            pictureBoxSteamIcon.Size = new Size(16, 16);
            pictureBoxSteamIcon.TabIndex = 11;
            pictureBoxSteamIcon.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 163);
            label1.Name = "label1";
            label1.Size = new Size(69, 13);
            label1.TabIndex = 10;
            label1.Text = "Description:";
            // 
            // richTextBoxModDescription
            // 
            richTextBoxModDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxModDescription.Location = new Point(8, 182);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new Size(313, 157);
            richTextBoxModDescription.TabIndex = 9;
            richTextBoxModDescription.Text = "";
            richTextBoxModDescription.LinkClicked += richTextBoxModDescription_LinkClicked;
            // 
            // labelSteamId
            // 
            labelSteamId.AutoSize = true;
            labelSteamId.Location = new Point(26, 103);
            labelSteamId.Name = "labelSteamId";
            labelSteamId.Size = new Size(55, 13);
            labelSteamId.TabIndex = 8;
            labelSteamId.Text = "Steam ID:";
            // 
            // linkLabelSteamId
            // 
            linkLabelSteamId.AutoSize = true;
            linkLabelSteamId.Location = new Point(111, 103);
            linkLabelSteamId.Name = "linkLabelSteamId";
            linkLabelSteamId.Size = new Size(94, 13);
            linkLabelSteamId.TabIndex = 7;
            linkLabelSteamId.TabStop = true;
            linkLabelSteamId.Text = "linkLabelSteamId";
            linkLabelSteamId.LinkClicked += linkLabelSteamId_LinkClicked;
            // 
            // linkLabelModAuthorUrl
            // 
            linkLabelModAuthorUrl.AutoEllipsis = true;
            linkLabelModAuthorUrl.Location = new Point(5, 46);
            linkLabelModAuthorUrl.Name = "linkLabelModAuthorUrl";
            linkLabelModAuthorUrl.Size = new Size(313, 13);
            linkLabelModAuthorUrl.TabIndex = 6;
            linkLabelModAuthorUrl.TabStop = true;
            linkLabelModAuthorUrl.Text = "linkLabel1";
            linkLabelModAuthorUrl.LinkClicked += linkLabelModAuthorUrl_LinkClicked;
            // 
            // labelModBuildNumber
            // 
            labelModBuildNumber.AutoSize = true;
            labelModBuildNumber.Location = new Point(147, 73);
            labelModBuildNumber.Name = "labelModBuildNumber";
            labelModBuildNumber.Size = new Size(123, 13);
            labelModBuildNumber.TabIndex = 4;
            labelModBuildNumber.Text = "labelModBuildNumber";
            // 
            // labelModVersion
            // 
            labelModVersion.AutoSize = true;
            labelModVersion.Location = new Point(5, 73);
            labelModVersion.Name = "labelModVersion";
            labelModVersion.Size = new Size(94, 13);
            labelModVersion.TabIndex = 3;
            labelModVersion.Text = "labelModVersion";
            // 
            // labelModAuthor
            // 
            labelModAuthor.AutoSize = true;
            labelModAuthor.Location = new Point(5, 28);
            labelModAuthor.Name = "labelModAuthor";
            labelModAuthor.Size = new Size(92, 13);
            labelModAuthor.TabIndex = 2;
            labelModAuthor.Text = "labelModAuthor";
            // 
            // labelModName
            // 
            labelModName.AutoSize = true;
            labelModName.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModName.Location = new Point(5, 8);
            labelModName.Name = "labelModName";
            labelModName.Size = new Size(88, 13);
            labelModName.TabIndex = 1;
            labelModName.Text = "labelModName";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageModInfo);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(334, 483);
            tabControl1.TabIndex = 11;
            // 
            // rotatingLabelHighPriority
            // 
            rotatingLabelHighPriority.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabelHighPriority.Location = new Point(5, 22);
            rotatingLabelHighPriority.Name = "rotatingLabelHighPriority";
            rotatingLabelHighPriority.NewText = "Highest priority »";
            rotatingLabelHighPriority.RotateAngle = -90;
            rotatingLabelHighPriority.Size = new Size(19, 118);
            rotatingLabelHighPriority.TabIndex = 38;
            toolTip1.SetToolTip(rotatingLabelHighPriority, "Mods are loaded later and override mods that were loaded earlier.");
            // 
            // rotatingLabelLowPriority
            // 
            rotatingLabelLowPriority.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rotatingLabelLowPriority.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabelLowPriority.Location = new Point(5, 340);
            rotatingLabelLowPriority.Name = "rotatingLabelLowPriority";
            rotatingLabelLowPriority.NewText = "« Lowest priority";
            rotatingLabelLowPriority.RotateAngle = -90;
            rotatingLabelLowPriority.Size = new Size(19, 113);
            rotatingLabelLowPriority.TabIndex = 39;
            toolTip1.SetToolTip(rotatingLabelLowPriority, "Mods are loaded earlier and may get overriden by mods loading after them.");
            // 
            // panelColorLegend
            // 
            panelColorLegend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panelColorLegend.Controls.Add(label8);
            panelColorLegend.Controls.Add(label4);
            panelColorLegend.Controls.Add(label2);
            panelColorLegend.Controls.Add(panelColorOverridingOverridden);
            panelColorLegend.Controls.Add(panelColorOverriding);
            panelColorLegend.Controls.Add(panelColorOverridden);
            panelColorLegend.Location = new Point(30, 460);
            panelColorLegend.Name = "panelColorLegend";
            panelColorLegend.Size = new Size(368, 21);
            panelColorLegend.TabIndex = 37;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(193, 2);
            label8.Margin = new Padding(0);
            label8.Name = "label8";
            label8.Size = new Size(135, 13);
            label8.TabIndex = 5;
            label8.Text = "Overriding && Overridden";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(105, 2);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(65, 13);
            label4.TabIndex = 4;
            label4.Text = "Overridden";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 2);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(62, 13);
            label2.TabIndex = 3;
            label2.Text = "Overriding";
            // 
            // panelColorOverridingOverridden
            // 
            panelColorOverridingOverridden.Location = new Point(178, 3);
            panelColorOverridingOverridden.Name = "panelColorOverridingOverridden";
            panelColorOverridingOverridden.Size = new Size(12, 12);
            panelColorOverridingOverridden.TabIndex = 2;
            // 
            // panelColorOverriding
            // 
            panelColorOverriding.Location = new Point(3, 3);
            panelColorOverriding.Name = "panelColorOverriding";
            panelColorOverriding.Size = new Size(12, 12);
            panelColorOverriding.TabIndex = 1;
            // 
            // panelColorOverridden
            // 
            panelColorOverridden.Location = new Point(90, 3);
            panelColorOverridden.Name = "panelColorOverridden";
            panelColorOverridden.Size = new Size(12, 12);
            panelColorOverridden.TabIndex = 0;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 72);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(rotatingLabelLowPriority);
            splitContainerMain.Panel1.Controls.Add(modsListView);
            splitContainerMain.Panel1.Controls.Add(rotatingLabelHighPriority);
            splitContainerMain.Panel1.Controls.Add(panelColorLegend);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(tabControl1);
            splitContainerMain.Panel2MinSize = 250;
            splitContainerMain.Size = new Size(1167, 483);
            splitContainerMain.SplitterDistance = 828;
            splitContainerMain.SplitterWidth = 5;
            splitContainerMain.TabIndex = 40;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Font = new Font("Segoe UI", 9F);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonApply, toolStripButtonStartGame, toolStripSeparator10, toolStripButtonReload, toolStripSeparator9, toolStripLabel1, toolStripTextFilterBox, toolStripButtonClearFilter, toolStripButtonFilterToggle, toolStripSeparator11 });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1167, 48);
            toolStrip1.TabIndex = 41;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonApply
            // 
            toolStripButtonApply.AutoSize = false;
            toolStripButtonApply.Image = (Image)resources.GetObject("toolStripButtonApply.Image");
            toolStripButtonApply.ImageTransparentColor = Color.Magenta;
            toolStripButtonApply.Name = "toolStripButtonApply";
            toolStripButtonApply.Size = new Size(70, 45);
            toolStripButtonApply.Text = "Apply";
            toolStripButtonApply.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonApply.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonApply.ToolTipText = "Apply load order to MechWarrior";
            toolStripButtonApply.Click += toolStripButtonApply_Click;
            // 
            // toolStripButtonStartGame
            // 
            toolStripButtonStartGame.AutoSize = false;
            toolStripButtonStartGame.Image = (Image)resources.GetObject("toolStripButtonStartGame.Image");
            toolStripButtonStartGame.ImageTransparentColor = Color.Magenta;
            toolStripButtonStartGame.Name = "toolStripButtonStartGame";
            toolStripButtonStartGame.Size = new Size(70, 45);
            toolStripButtonStartGame.Text = "Start MW5";
            toolStripButtonStartGame.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonStartGame.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonStartGame.ToolTipText = "Start MechWarrior";
            toolStripButtonStartGame.Click += toolStripButtonStart_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(6, 48);
            // 
            // toolStripButtonReload
            // 
            toolStripButtonReload.AutoSize = false;
            toolStripButtonReload.Image = (Image)resources.GetObject("toolStripButtonReload.Image");
            toolStripButtonReload.ImageTransparentColor = Color.Magenta;
            toolStripButtonReload.Name = "toolStripButtonReload";
            toolStripButtonReload.Size = new Size(70, 45);
            toolStripButtonReload.Text = "Reload";
            toolStripButtonReload.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonReload.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonReload.ToolTipText = "Reload settings from files, reverting unapplied changes";
            toolStripButtonReload.Click += toolStripButtonReload_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 48);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(45, 45);
            toolStripLabel1.Text = "Search:";
            // 
            // toolStripTextFilterBox
            // 
            toolStripTextFilterBox.Name = "toolStripTextFilterBox";
            toolStripTextFilterBox.Size = new Size(130, 48);
            toolStripTextFilterBox.TextChanged += toolStripTextFilterBox_TextChanged;
            // 
            // toolStripButtonClearFilter
            // 
            toolStripButtonClearFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonClearFilter.Enabled = false;
            toolStripButtonClearFilter.Image = (Image)resources.GetObject("toolStripButtonClearFilter.Image");
            toolStripButtonClearFilter.ImageTransparentColor = Color.Magenta;
            toolStripButtonClearFilter.Name = "toolStripButtonClearFilter";
            toolStripButtonClearFilter.Size = new Size(23, 45);
            toolStripButtonClearFilter.Text = "Clear";
            toolStripButtonClearFilter.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonClearFilter.ToolTipText = "Clear filter";
            toolStripButtonClearFilter.Click += toolStripButtonClearFilter_Click;
            // 
            // toolStripButtonFilterToggle
            // 
            toolStripButtonFilterToggle.CheckOnClick = true;
            toolStripButtonFilterToggle.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonFilterToggle.Image = (Image)resources.GetObject("toolStripButtonFilterToggle.Image");
            toolStripButtonFilterToggle.ImageTransparentColor = Color.Magenta;
            toolStripButtonFilterToggle.Name = "toolStripButtonFilterToggle";
            toolStripButtonFilterToggle.Size = new Size(23, 45);
            toolStripButtonFilterToggle.Text = "Filter";
            toolStripButtonFilterToggle.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonFilterToggle.ToolTipText = "Toggle filter mode";
            toolStripButtonFilterToggle.CheckedChanged += toolStripButtonFilterToggle_CheckedChanged;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(6, 48);
            // 
            // MainWindow
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1167, 579);
            Controls.Add(splitContainerMain);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(900, 550);
            Name = "MainWindow";
            Text = "MechWarrior 5 Load Order Configurator";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindow_Load;
            Shown += MainWindow_Shown;
            DragDrop += MainForm_DragDrop;
            DragEnter += Form1_DragEnter;
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStripMod.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabPageModInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).EndInit();
            panelModInfo.ResumeLayout(false);
            panelModInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).EndInit();
            tabControl1.ResumeLayout(false);
            panelColorLegend.ResumeLayout(false);
            panelColorLegend.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ListView modsListView;
        public System.Windows.Forms.ColumnHeader displayHeader;
        public System.Windows.Forms.ColumnHeader folderHeader;
        public System.Windows.Forms.ColumnHeader authorHeader;
        public System.Windows.Forms.ColumnHeader enabledHeader;
        public System.Windows.Forms.ColumnHeader versionHeader;
        public System.Windows.Forms.ToolStripLabel toolStripVendorLabeltoolStripLabel1;
        public System.ComponentModel.BackgroundWorker backgroundWorker2;
        private BindingSource textProgressBarBindingSource;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem exportLoadOrderToolStripMenuItem1;
        private ToolStripMenuItem importLoadOrderToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem exportmodsFolderToolStripMenuItem1;
        private ToolStripMenuItem shareModsViaTCPToolStripMenuItem;
        private ToolStripStatusLabel toolStripPlatformLabel;
        private ToolStripMenuItem modsToolStripMenuItem;
        private ToolStripMenuItem openModsFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem enableAllModsToolStripMenuItem;
        private ToolStripMenuItem disableAllModsToolStripMenuItem;
        public ToolStripStatusLabel toolStripStatusLabelMwVersion;
        private ContextMenuStrip contextMenuStripMod;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMenuItemSettings;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripMenuItem toolStripMenuItemOpenModFolderSteam;
        public ColumnHeader originalLoadOrderHeader;
        private TabPage tabPage1;
        private Label label6;
        private Label labelModNameOverrides;
        private ListBox listBoxOverriding;
        private Label label7;
        private ListBox listBoxOverriddenBy;
        private Label label5;
        private TabPage tabPageModInfo;
        private PictureBox pictureBoxModImage;
        private Panel panelModInfo;
        private Label label1;
        private RichTextBox richTextBoxModDescription;
        private Label labelSteamId;
        private LinkLabel linkLabelSteamId;
        private LinkLabel linkLabelModAuthorUrl;
        private Label labelModBuildNumber;
        private Label labelModVersion;
        private Label labelModAuthor;
        private Label labelModName;
        private TabControl tabControl1;
        public ToolStripMenuItem presetsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemLoadPresets;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem savePresetToolStripMenuItem;
        private ToolStripMenuItem deletePresetToolStripMenuItem;
        private ImageList imageListIcons;
        private PictureBox pictureBoxSteamIcon;
        private PictureBox pictureBoxNexusmodsIcon;
        private Label labelNexusmods;
        private LinkLabel linkLabelNexusmods;
        public ColumnHeader currentLoadOrderHeader;
        private ToolTip toolTip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabelModCountTotal;
        private ToolStripStatusLabel toolStripStatusLabelModsActive;
        private Panel panelColorLegend;
        private Label label8;
        private Label label4;
        private Label label2;
        private Panel panelColorOverridingOverridden;
        private Panel panelColorOverriding;
        private Panel panelColorOverridden;
        private ToolStripMenuItem contextMenuItemMoveToTop;
        private ToolStripMenuItem contextMenuItemMoveToBottom;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem moveupToolStripMenuItem;
        private ToolStripMenuItem movedownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private RotatingLabel rotatingLabelHighPriority;
        private RotatingLabel rotatingLabelLowPriority;
        private SplitContainer splitContainerMain;
        private ColumnHeader fileSizeHeader;
        private ToolStripMenuItem toolStripMenuItemSortDefaultLoadOrder;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem openUserModsFolderToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonReload;
        private ToolStripButton toolStripButtonApply;
        private ToolStripButton toolStripButtonStartGame;
        private ToolStripTextBox toolStripTextFilterBox;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton toolStripButtonFilterToggle;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton toolStripButtonClearFilter;
        private ToolStripSeparator toolStripSeparator10;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemImportArchive;
        private ToolStripMenuItem toolStripMenuItemImportFromFolder;
        private ToolStripSeparator toolStripSeparator12;
        private RichTextBox richTextBoxManifestOverridden;
        private ToolStripMenuItem toolStripMenuItemNexusmodsLink;
    }
}

