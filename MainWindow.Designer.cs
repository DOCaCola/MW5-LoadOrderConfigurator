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
            buttonMoveUp = new Button();
            buttonMoveDown = new Button();
            buttonApply = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            openFileDialog1 = new OpenFileDialog();
            buttonReload = new Button();
            toolStripPlatformLabel = new ToolStripStatusLabel();
            modsListView = new ListView();
            enabledHeader = new ColumnHeader();
            displayHeader = new ColumnHeader();
            folderHeader = new ColumnHeader();
            authorHeader = new ColumnHeader();
            versionHeader = new ColumnHeader();
            currentLoadOrderHeader = new ColumnHeader();
            originalLoadOrderHeader = new ColumnHeader();
            fileSizeHeader = new ColumnHeader();
            imageListIcons = new ImageList(components);
            buttonStartGame = new Button();
            label3 = new Label();
            filterBox = new TextBox();
            checkBoxFilter = new CheckBox();
            buttonRemove = new Button();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            textProgressBarBindingSource = new BindingSource(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
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
            openModsFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemOpenModFolderSteam = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
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
            tabPage1 = new TabPage();
            label6 = new Label();
            labelModNameOverrides = new Label();
            listBoxOverriding = new ListBox();
            label7 = new Label();
            listBoxManifestOverridden = new ListBox();
            listBoxOverriddenBy = new ListBox();
            label5 = new Label();
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
            buttonClearHighlight = new Button();
            toolTip1 = new ToolTip(components);
            rotatingLabel2 = new RotatingLabel();
            rotatingLabel1 = new RotatingLabel();
            panelColorLegend = new Panel();
            label8 = new Label();
            label4 = new Label();
            label2 = new Label();
            panelColorOverridingOverridden = new Panel();
            panelColorOverriding = new Panel();
            panelColorOverridden = new Panel();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            contextMenuStripMod.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPageModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).BeginInit();
            panelModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).BeginInit();
            tabControl1.SuspendLayout();
            panelColorLegend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonMoveUp
            // 
            buttonMoveUp.Location = new Point(13, 122);
            buttonMoveUp.Name = "buttonMoveUp";
            buttonMoveUp.Size = new Size(70, 38);
            buttonMoveUp.TabIndex = 5;
            buttonMoveUp.Text = "&UP";
            buttonMoveUp.UseVisualStyleBackColor = true;
            buttonMoveUp.Click += button1_Click;
            // 
            // buttonMoveDown
            // 
            buttonMoveDown.Location = new Point(13, 166);
            buttonMoveDown.Name = "buttonMoveDown";
            buttonMoveDown.Size = new Size(70, 38);
            buttonMoveDown.TabIndex = 6;
            buttonMoveDown.Text = "&DOWN";
            buttonMoveDown.UseVisualStyleBackColor = true;
            buttonMoveDown.Click += button2_Click;
            // 
            // buttonApply
            // 
            buttonApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonApply.Location = new Point(13, 399);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(70, 45);
            buttonApply.TabIndex = 7;
            buttonApply.Text = "&Apply";
            toolTip1.SetToolTip(buttonApply, "Save settings to game files.");
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // buttonReload
            // 
            buttonReload.Location = new Point(13, 46);
            buttonReload.Name = "buttonReload";
            buttonReload.Size = new Size(70, 38);
            buttonReload.TabIndex = 4;
            buttonReload.Text = "&Reload";
            toolTip1.SetToolTip(buttonReload, "Reload configuration from game files. Reverts current changes.");
            buttonReload.UseVisualStyleBackColor = true;
            buttonReload.Click += button6_Click;
            // 
            // toolStripPlatformLabel
            // 
            toolStripPlatformLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripPlatformLabel.Name = "toolStripPlatformLabel";
            toolStripPlatformLabel.Size = new Size(57, 19);
            toolStripPlatformLabel.Text = "platform";
            // 
            // modsListView
            // 
            modsListView.AllowDrop = true;
            modsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modsListView.CheckBoxes = true;
            modsListView.Columns.AddRange(new ColumnHeader[] { enabledHeader, displayHeader, folderHeader, authorHeader, versionHeader, currentLoadOrderHeader, originalLoadOrderHeader, fileSizeHeader });
            modsListView.FullRowSelect = true;
            modsListView.GridLines = true;
            modsListView.LabelWrap = false;
            modsListView.Location = new Point(30, 34);
            modsListView.MultiSelect = false;
            modsListView.Name = "modsListView";
            modsListView.RightToLeft = RightToLeft.No;
            modsListView.Size = new Size(734, 468);
            modsListView.SmallImageList = imageListIcons;
            modsListView.TabIndex = 10;
            modsListView.UseCompatibleStateImageBehavior = false;
            modsListView.View = View.Details;
            modsListView.ItemChecked += listView1_ItemChecked;
            modsListView.ItemDrag += modsListView_ItemDrag;
            modsListView.SelectedIndexChanged += listView1_SelectedIndexChanged;
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
            // folderHeader
            // 
            folderHeader.Tag = "";
            folderHeader.Text = "Mod Folder";
            folderHeader.Width = 110;
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
            // imageListIcons
            // 
            imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageListIcons.ImageStream = (ImageListStreamer)resources.GetObject("imageListIcons.ImageStream");
            imageListIcons.TransparentColor = Color.Transparent;
            imageListIcons.Images.SetKeyName(0, "Folder");
            imageListIcons.Images.SetKeyName(1, "Steam");
            imageListIcons.Images.SetKeyName(2, "Nexusmods");
            // 
            // buttonStartGame
            // 
            buttonStartGame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonStartGame.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonStartGame.Location = new Point(13, 451);
            buttonStartGame.Margin = new Padding(0);
            buttonStartGame.Name = "buttonStartGame";
            buttonStartGame.Size = new Size(70, 90);
            buttonStartGame.TabIndex = 9;
            buttonStartGame.Text = "&Start MW5";
            buttonStartGame.UseVisualStyleBackColor = true;
            buttonStartGame.Click += buttonStartGame_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 9);
            label3.Name = "label3";
            label3.Size = new Size(44, 13);
            label3.TabIndex = 16;
            label3.Text = "Search:";
            // 
            // filterBox
            // 
            filterBox.Location = new Point(77, 4);
            filterBox.Name = "filterBox";
            filterBox.Size = new Size(301, 22);
            filterBox.TabIndex = 1;
            filterBox.TextChanged += filterBox_TextChanged;
            // 
            // checkBoxFilter
            // 
            checkBoxFilter.AutoSize = true;
            checkBoxFilter.Location = new Point(411, 8);
            checkBoxFilter.Name = "checkBoxFilter";
            checkBoxFilter.Size = new Size(52, 17);
            checkBoxFilter.TabIndex = 3;
            checkBoxFilter.Text = "Filter";
            checkBoxFilter.UseVisualStyleBackColor = true;
            checkBoxFilter.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // buttonRemove
            // 
            buttonRemove.Location = new Point(13, 327);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new Size(70, 38);
            buttonRemove.TabIndex = 8;
            buttonRemove.Text = "Mark for Removal";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Visible = false;
            buttonRemove.Click += button5_Click;
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
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importLoadOrderToolStripMenuItem1, exportLoadOrderToolStripMenuItem1, toolStripSeparator7, exportmodsFolderToolStripMenuItem1, shareModsViaTCPToolStripMenuItem, toolStripSeparator2, toolStripMenuItemSettings, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            importLoadOrderToolStripMenuItem1.Size = new Size(177, 22);
            importLoadOrderToolStripMenuItem1.Text = "&Import load order...";
            importLoadOrderToolStripMenuItem1.Click += importLoadOrderToolStripMenuItem1_Click;
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            exportLoadOrderToolStripMenuItem1.Size = new Size(177, 22);
            exportLoadOrderToolStripMenuItem1.Text = "&Export load order...";
            exportLoadOrderToolStripMenuItem1.Click += exportLoadOrderToolStripMenuItem1_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(174, 6);
            toolStripSeparator7.Visible = false;
            // 
            // exportmodsFolderToolStripMenuItem1
            // 
            exportmodsFolderToolStripMenuItem1.Name = "exportmodsFolderToolStripMenuItem1";
            exportmodsFolderToolStripMenuItem1.Size = new Size(177, 22);
            exportmodsFolderToolStripMenuItem1.Text = "Export &mods folder";
            exportmodsFolderToolStripMenuItem1.Visible = false;
            exportmodsFolderToolStripMenuItem1.Click += exportmodsFolderToolStripMenuItem1_Click;
            // 
            // shareModsViaTCPToolStripMenuItem
            // 
            shareModsViaTCPToolStripMenuItem.Name = "shareModsViaTCPToolStripMenuItem";
            shareModsViaTCPToolStripMenuItem.Size = new Size(177, 22);
            shareModsViaTCPToolStripMenuItem.Text = "Share mods via &TCP";
            shareModsViaTCPToolStripMenuItem.Visible = false;
            shareModsViaTCPToolStripMenuItem.Click += shareModsViaTCPToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(174, 6);
            // 
            // toolStripMenuItemSettings
            // 
            toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            toolStripMenuItemSettings.Size = new Size(177, 22);
            toolStripMenuItemSettings.Text = "&Settings";
            toolStripMenuItemSettings.Click += toolStripMenuItemSettings_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(174, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(177, 22);
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
            modsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { enableAllModsToolStripMenuItem, disableAllModsToolStripMenuItem, toolStripSeparator3, openModsFolderToolStripMenuItem, toolStripMenuItemOpenModFolderSteam });
            modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            modsToolStripMenuItem.Size = new Size(49, 20);
            modsToolStripMenuItem.Text = "&Mods";
            // 
            // enableAllModsToolStripMenuItem
            // 
            enableAllModsToolStripMenuItem.Name = "enableAllModsToolStripMenuItem";
            enableAllModsToolStripMenuItem.Size = new Size(206, 22);
            enableAllModsToolStripMenuItem.Text = "&Enable all mods";
            enableAllModsToolStripMenuItem.Click += enableAllModsToolStripMenuItem_Click;
            // 
            // disableAllModsToolStripMenuItem
            // 
            disableAllModsToolStripMenuItem.Name = "disableAllModsToolStripMenuItem";
            disableAllModsToolStripMenuItem.Size = new Size(206, 22);
            disableAllModsToolStripMenuItem.Text = "&Disable all mods";
            disableAllModsToolStripMenuItem.Click += disableAllModsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(203, 6);
            // 
            // openModsFolderToolStripMenuItem
            // 
            openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            openModsFolderToolStripMenuItem.Size = new Size(206, 22);
            openModsFolderToolStripMenuItem.Text = "&Open Mods Folder";
            openModsFolderToolStripMenuItem.Click += openModsFolderToolStripMenuItem_Click;
            // 
            // toolStripMenuItemOpenModFolderSteam
            // 
            toolStripMenuItemOpenModFolderSteam.Name = "toolStripMenuItemOpenModFolderSteam";
            toolStripMenuItemOpenModFolderSteam.Size = new Size(206, 22);
            toolStripMenuItemOpenModFolderSteam.Text = "Open &Steam Mods folder";
            toolStripMenuItemOpenModFolderSteam.Visible = false;
            toolStripMenuItemOpenModFolderSteam.Click += toolStripMenuItemOpenModFolderSteam_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(40, 20);
            helpToolStripMenuItem.Text = "&Info";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
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
            toolStripStatusLabel1.Size = new Size(886, 19);
            toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabelModsActive
            // 
            toolStripStatusLabelModsActive.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelModsActive.Name = "toolStripStatusLabelModsActive";
            toolStripStatusLabelModsActive.Size = new Size(69, 19);
            toolStripStatusLabelModsActive.Text = "modActive";
            // 
            // toolStripStatusLabelModCountTotal
            // 
            toolStripStatusLabelModCountTotal.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelModCountTotal.Name = "toolStripStatusLabelModCountTotal";
            toolStripStatusLabelModCountTotal.Size = new Size(61, 19);
            toolStripStatusLabelModCountTotal.Text = "modTotal";
            // 
            // toolStripStatusLabelMwVersion
            // 
            toolStripStatusLabelMwVersion.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabelMwVersion.Margin = new Padding(0, 3, 30, 2);
            toolStripStatusLabelMwVersion.Name = "toolStripStatusLabelMwVersion";
            toolStripStatusLabelMwVersion.Size = new Size(49, 19);
            toolStripStatusLabelMwVersion.Text = "version";
            // 
            // contextMenuStripMod
            // 
            contextMenuStripMod.Items.AddRange(new ToolStripItem[] { moveupToolStripMenuItem, movedownToolStripMenuItem, toolStripSeparator6, contextMenuItemMoveToTop, contextMenuItemMoveToBottom, toolStripSeparator5, openFolderToolStripMenuItem });
            contextMenuStripMod.Name = "contextMenuStripMod";
            contextMenuStripMod.Size = new Size(162, 126);
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
            // tabPage1
            // 
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(labelModNameOverrides);
            tabPage1.Controls.Add(listBoxOverriding);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(listBoxManifestOverridden);
            tabPage1.Controls.Add(listBoxOverriddenBy);
            tabPage1.Controls.Add(label5);
            tabPage1.Location = new Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(287, 502);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Overrides";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(9, 36);
            label6.Name = "label6";
            label6.Size = new Size(62, 13);
            label6.TabIndex = 25;
            label6.Text = "Overriding";
            // 
            // labelModNameOverrides
            // 
            labelModNameOverrides.AutoSize = true;
            labelModNameOverrides.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModNameOverrides.Location = new Point(6, 11);
            labelModNameOverrides.Name = "labelModNameOverrides";
            labelModNameOverrides.Size = new Size(19, 13);
            labelModNameOverrides.TabIndex = 20;
            labelModNameOverrides.Text = "---";
            // 
            // listBoxOverriding
            // 
            listBoxOverriding.FormattingEnabled = true;
            listBoxOverriding.HorizontalScrollbar = true;
            listBoxOverriding.ItemHeight = 13;
            listBoxOverriding.Location = new Point(9, 52);
            listBoxOverriding.Name = "listBoxOverriding";
            listBoxOverriding.Size = new Size(138, 95);
            listBoxOverriding.TabIndex = 21;
            listBoxOverriding.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            listBoxOverriding.MouseDoubleClick += listBoxOverriding_MouseDoubleClick;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(9, 160);
            label7.Name = "label7";
            label7.Size = new Size(90, 13);
            label7.TabIndex = 26;
            label7.Text = "Manifest Entries";
            // 
            // listBoxManifestOverridden
            // 
            listBoxManifestOverridden.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxManifestOverridden.FormattingEnabled = true;
            listBoxManifestOverridden.HorizontalScrollbar = true;
            listBoxManifestOverridden.ItemHeight = 13;
            listBoxManifestOverridden.Location = new Point(9, 178);
            listBoxManifestOverridden.Name = "listBoxManifestOverridden";
            listBoxManifestOverridden.Size = new Size(275, 316);
            listBoxManifestOverridden.TabIndex = 22;
            // 
            // listBoxOverriddenBy
            // 
            listBoxOverriddenBy.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listBoxOverriddenBy.FormattingEnabled = true;
            listBoxOverriddenBy.HorizontalScrollbar = true;
            listBoxOverriddenBy.ItemHeight = 13;
            listBoxOverriddenBy.Location = new Point(153, 52);
            listBoxOverriddenBy.Name = "listBoxOverriddenBy";
            listBoxOverriddenBy.Size = new Size(131, 95);
            listBoxOverriddenBy.TabIndex = 23;
            listBoxOverriddenBy.SelectedIndexChanged += listBox3_SelectedIndexChanged;
            listBoxOverriddenBy.MouseDoubleClick += listBoxOverriddenBy_MouseDoubleClick;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(153, 36);
            label5.Name = "label5";
            label5.Size = new Size(79, 13);
            label5.TabIndex = 24;
            label5.Text = "Overridden By";
            // 
            // tabPageModInfo
            // 
            tabPageModInfo.Controls.Add(pictureBoxModImage);
            tabPageModInfo.Controls.Add(panelModInfo);
            tabPageModInfo.Location = new Point(4, 22);
            tabPageModInfo.Name = "tabPageModInfo";
            tabPageModInfo.Padding = new Padding(3);
            tabPageModInfo.Size = new Size(287, 502);
            tabPageModInfo.TabIndex = 3;
            tabPageModInfo.Text = "Overview";
            tabPageModInfo.UseVisualStyleBackColor = true;
            // 
            // pictureBoxModImage
            // 
            pictureBoxModImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxModImage.Location = new Point(5, 6);
            pictureBoxModImage.Name = "pictureBoxModImage";
            pictureBoxModImage.Size = new Size(287, 139);
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
            panelModInfo.Location = new Point(0, 150);
            panelModInfo.Name = "panelModInfo";
            panelModInfo.Size = new Size(307, 373);
            panelModInfo.TabIndex = 1;
            panelModInfo.Visible = false;
            // 
            // pictureBoxNexusmodsIcon
            // 
            pictureBoxNexusmodsIcon.Image = (Image)resources.GetObject("pictureBoxNexusmodsIcon.Image");
            pictureBoxNexusmodsIcon.Location = new Point(15, 125);
            pictureBoxNexusmodsIcon.Name = "pictureBoxNexusmodsIcon";
            pictureBoxNexusmodsIcon.Size = new Size(16, 16);
            pictureBoxNexusmodsIcon.TabIndex = 14;
            pictureBoxNexusmodsIcon.TabStop = false;
            // 
            // labelNexusmods
            // 
            labelNexusmods.AutoSize = true;
            labelNexusmods.Location = new Point(33, 127);
            labelNexusmods.Name = "labelNexusmods";
            labelNexusmods.Size = new Size(83, 13);
            labelNexusmods.TabIndex = 13;
            labelNexusmods.Text = "Nexusmods ID:";
            // 
            // linkLabelNexusmods
            // 
            linkLabelNexusmods.AutoSize = true;
            linkLabelNexusmods.Location = new Point(118, 128);
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
            pictureBoxSteamIcon.Location = new Point(15, 103);
            pictureBoxSteamIcon.Name = "pictureBoxSteamIcon";
            pictureBoxSteamIcon.Size = new Size(16, 16);
            pictureBoxSteamIcon.TabIndex = 11;
            pictureBoxSteamIcon.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 165);
            label1.Name = "label1";
            label1.Size = new Size(69, 13);
            label1.TabIndex = 10;
            label1.Text = "Description:";
            // 
            // richTextBoxModDescription
            // 
            richTextBoxModDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxModDescription.Location = new Point(15, 184);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new Size(266, 157);
            richTextBoxModDescription.TabIndex = 9;
            richTextBoxModDescription.Text = "";
            richTextBoxModDescription.LinkClicked += richTextBoxModDescription_LinkClicked;
            // 
            // labelSteamId
            // 
            labelSteamId.AutoSize = true;
            labelSteamId.Location = new Point(33, 105);
            labelSteamId.Name = "labelSteamId";
            labelSteamId.Size = new Size(55, 13);
            labelSteamId.TabIndex = 8;
            labelSteamId.Text = "Steam ID:";
            // 
            // linkLabelSteamId
            // 
            linkLabelSteamId.AutoSize = true;
            linkLabelSteamId.Location = new Point(118, 105);
            linkLabelSteamId.Name = "linkLabelSteamId";
            linkLabelSteamId.Size = new Size(94, 13);
            linkLabelSteamId.TabIndex = 7;
            linkLabelSteamId.TabStop = true;
            linkLabelSteamId.Text = "linkLabelSteamId";
            linkLabelSteamId.LinkClicked += linkLabelSteamId_LinkClicked;
            // 
            // linkLabelModAuthorUrl
            // 
            linkLabelModAuthorUrl.AutoSize = true;
            linkLabelModAuthorUrl.Location = new Point(12, 48);
            linkLabelModAuthorUrl.Name = "linkLabelModAuthorUrl";
            linkLabelModAuthorUrl.Size = new Size(59, 13);
            linkLabelModAuthorUrl.TabIndex = 6;
            linkLabelModAuthorUrl.TabStop = true;
            linkLabelModAuthorUrl.Text = "linkLabel1";
            linkLabelModAuthorUrl.LinkClicked += linkLabelModAuthorUrl_LinkClicked;
            // 
            // labelModBuildNumber
            // 
            labelModBuildNumber.AutoSize = true;
            labelModBuildNumber.Location = new Point(154, 75);
            labelModBuildNumber.Name = "labelModBuildNumber";
            labelModBuildNumber.Size = new Size(123, 13);
            labelModBuildNumber.TabIndex = 4;
            labelModBuildNumber.Text = "labelModBuildNumber";
            // 
            // labelModVersion
            // 
            labelModVersion.AutoSize = true;
            labelModVersion.Location = new Point(12, 75);
            labelModVersion.Name = "labelModVersion";
            labelModVersion.Size = new Size(94, 13);
            labelModVersion.TabIndex = 3;
            labelModVersion.Text = "labelModVersion";
            // 
            // labelModAuthor
            // 
            labelModAuthor.AutoSize = true;
            labelModAuthor.Location = new Point(12, 30);
            labelModAuthor.Name = "labelModAuthor";
            labelModAuthor.Size = new Size(92, 13);
            labelModAuthor.TabIndex = 2;
            labelModAuthor.Text = "labelModAuthor";
            // 
            // labelModName
            // 
            labelModName.AutoSize = true;
            labelModName.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModName.Location = new Point(12, 10);
            labelModName.Name = "labelModName";
            labelModName.Size = new Size(88, 13);
            labelModName.TabIndex = 1;
            labelModName.Text = "labelModName";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPageModInfo);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(3, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(295, 528);
            tabControl1.TabIndex = 11;
            // 
            // buttonClearHighlight
            // 
            buttonClearHighlight.Enabled = false;
            buttonClearHighlight.Location = new Point(384, 3);
            buttonClearHighlight.Name = "buttonClearHighlight";
            buttonClearHighlight.Size = new Size(21, 24);
            buttonClearHighlight.TabIndex = 2;
            buttonClearHighlight.Text = "X";
            buttonClearHighlight.UseVisualStyleBackColor = true;
            buttonClearHighlight.Click += buttonClearHighlight_Click;
            // 
            // rotatingLabel2
            // 
            rotatingLabel2.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabel2.Location = new Point(5, 46);
            rotatingLabel2.Name = "rotatingLabel2";
            rotatingLabel2.NewText = "Highest priority »";
            rotatingLabel2.RotateAngle = -90;
            rotatingLabel2.Size = new Size(19, 118);
            rotatingLabel2.TabIndex = 38;
            toolTip1.SetToolTip(rotatingLabel2, "Mods are loaded later and override mods that were loaded earlier.");
            // 
            // rotatingLabel1
            // 
            rotatingLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rotatingLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabel1.Location = new Point(5, 385);
            rotatingLabel1.Name = "rotatingLabel1";
            rotatingLabel1.NewText = "« Lowest priority";
            rotatingLabel1.RotateAngle = -90;
            rotatingLabel1.Size = new Size(19, 113);
            rotatingLabel1.TabIndex = 39;
            toolTip1.SetToolTip(rotatingLabel1, "Mods are loaded earlier and may get overriden by mods loading after them.");
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
            panelColorLegend.Location = new Point(30, 505);
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
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(86, 26);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1.Controls.Add(rotatingLabel1);
            splitContainer1.Panel1.Controls.Add(modsListView);
            splitContainer1.Panel1.Controls.Add(rotatingLabel2);
            splitContainer1.Panel1.Controls.Add(filterBox);
            splitContainer1.Panel1.Controls.Add(panelColorLegend);
            splitContainer1.Panel1.Controls.Add(checkBoxFilter);
            splitContainer1.Panel1.Controls.Add(buttonClearHighlight);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Panel2MinSize = 250;
            splitContainer1.Size = new Size(1081, 528);
            splitContainer1.SplitterDistance = 767;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 40;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1167, 579);
            Controls.Add(buttonStartGame);
            Controls.Add(statusStrip1);
            Controls.Add(buttonRemove);
            Controls.Add(menuStrip1);
            Controls.Add(buttonReload);
            Controls.Add(buttonApply);
            Controls.Add(buttonMoveDown);
            Controls.Add(buttonMoveUp);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(900, 550);
            Name = "MainWindow";
            Text = "MechWarrior 5 Load Order Configurator";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindow_Load;
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStripMod.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPageModInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).EndInit();
            panelModInfo.ResumeLayout(false);
            panelModInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).EndInit();
            tabControl1.ResumeLayout(false);
            panelColorLegend.ResumeLayout(false);
            panelColorLegend.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonApply;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonReload;
        public System.Windows.Forms.ListView modsListView;
        public System.Windows.Forms.ColumnHeader displayHeader;
        public System.Windows.Forms.ColumnHeader folderHeader;
        public System.Windows.Forms.ColumnHeader authorHeader;
        public System.Windows.Forms.ColumnHeader enabledHeader;
        public System.Windows.Forms.ColumnHeader versionHeader;
        public System.Windows.Forms.ToolStripLabel toolStripVendorLabeltoolStripLabel1;
        private System.Windows.Forms.Button buttonStartGame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox filterBox;
        private System.Windows.Forms.CheckBox checkBoxFilter;
        private System.Windows.Forms.Button buttonRemove;
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
        private ToolStripMenuItem toolStripMenuItemOpenModFolderSteam;
        private ColumnHeader originalLoadOrderHeader;
        private TabPage tabPage1;
        private Label label6;
        private Label labelModNameOverrides;
        private ListBox listBoxOverriding;
        private Label label7;
        private ListBox listBoxManifestOverridden;
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
        private Button buttonClearHighlight;
        private ColumnHeader currentLoadOrderHeader;
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
        private RotatingLabel rotatingLabel2;
        private RotatingLabel rotatingLabel1;
        private SplitContainer splitContainer1;
        private ColumnHeader fileSizeHeader;
    }
}

