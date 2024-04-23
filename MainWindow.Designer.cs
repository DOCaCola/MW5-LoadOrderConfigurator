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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            openFileDialog1 = new OpenFileDialog();
            button6 = new Button();
            toolStripPlatformLabel = new ToolStripStatusLabel();
            modsListView = new ListView();
            enabledHeader = new ColumnHeader();
            displayHeader = new ColumnHeader();
            folderHeader = new ColumnHeader();
            authorHeader = new ColumnHeader();
            versionHeader = new ColumnHeader();
            buildHeader = new ColumnHeader();
            originalLoadOrderHeader = new ColumnHeader();
            imageListIcons = new ImageList(components);
            button4 = new Button();
            label3 = new Label();
            filterBox = new TextBox();
            checkBox1 = new CheckBox();
            button5 = new Button();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            textProgressBarBindingSource = new BindingSource(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exportLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
            importLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
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
            toolStripStatusLabelMwVersion = new ToolStripStatusLabel();
            rotatingLabel1 = new RotatingLabel();
            contextMenuStripMod = new ContextMenuStrip(components);
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            tabPage1 = new TabPage();
            label6 = new Label();
            label4 = new Label();
            listBox1 = new ListBox();
            label7 = new Label();
            listBox2 = new ListBox();
            listBox3 = new ListBox();
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
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(13, 122);
            button1.Name = "button1";
            button1.Size = new Size(70, 38);
            button1.TabIndex = 1;
            button1.Text = "&UP";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(13, 166);
            button2.Name = "button2";
            button2.Size = new Size(70, 38);
            button2.TabIndex = 2;
            button2.Text = "&DOWN";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(13, 279);
            button3.Name = "button3";
            button3.Size = new Size(70, 38);
            button3.TabIndex = 3;
            button3.Text = "&Apply";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // button6
            // 
            button6.Location = new Point(13, 46);
            button6.Name = "button6";
            button6.Size = new Size(70, 38);
            button6.TabIndex = 7;
            button6.Text = "Refresh";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // toolStripPlatformLabel
            // 
            toolStripPlatformLabel.Name = "toolStripPlatformLabel";
            toolStripPlatformLabel.Size = new Size(22, 17);
            toolStripPlatformLabel.Text = "---";
            // 
            // modsListView
            // 
            modsListView.AllowDrop = true;
            modsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modsListView.CheckBoxes = true;
            modsListView.Columns.AddRange(new ColumnHeader[] { enabledHeader, displayHeader, folderHeader, authorHeader, versionHeader, buildHeader, originalLoadOrderHeader });
            modsListView.FullRowSelect = true;
            modsListView.GridLines = true;
            modsListView.LabelWrap = false;
            modsListView.Location = new Point(124, 61);
            modsListView.MultiSelect = false;
            modsListView.Name = "modsListView";
            modsListView.RightToLeft = RightToLeft.No;
            modsListView.Size = new Size(708, 493);
            modsListView.SmallImageList = imageListIcons;
            modsListView.TabIndex = 11;
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
            authorHeader.Width = 72;
            // 
            // versionHeader
            // 
            versionHeader.Tag = "";
            versionHeader.Text = "Version";
            versionHeader.Width = 54;
            // 
            // buildHeader
            // 
            buildHeader.Text = "Build";
            // 
            // originalLoadOrderHeader
            // 
            originalLoadOrderHeader.Text = "Default Load Order";
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
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.Location = new Point(13, 451);
            button4.Margin = new Padding(0);
            button4.Name = "button4";
            button4.Size = new Size(70, 90);
            button4.TabIndex = 13;
            button4.Text = "Start MW5";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(180, 33);
            label3.Name = "label3";
            label3.Size = new Size(65, 13);
            label3.TabIndex = 16;
            label3.Text = "Filter Mods";
            // 
            // filterBox
            // 
            filterBox.Location = new Point(339, 30);
            filterBox.Name = "filterBox";
            filterBox.Size = new Size(425, 22);
            filterBox.TabIndex = 17;
            filterBox.TextChanged += filterBox_TextChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(258, 32);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(75, 17);
            checkBox1.TabIndex = 18;
            checkBox1.Text = "Highlight";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // button5
            // 
            button5.Location = new Point(13, 327);
            button5.Name = "button5";
            button5.Size = new Size(70, 38);
            button5.TabIndex = 19;
            button5.Text = "Mark for Removal";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
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
            menuStrip1.Size = new Size(1184, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportLoadOrderToolStripMenuItem1, importLoadOrderToolStripMenuItem1, toolStripSeparator7, exportmodsFolderToolStripMenuItem1, shareModsViaTCPToolStripMenuItem, toolStripSeparator2, toolStripMenuItemSettings, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            exportLoadOrderToolStripMenuItem1.Size = new Size(177, 22);
            exportLoadOrderToolStripMenuItem1.Text = "&Export load order...";
            exportLoadOrderToolStripMenuItem1.Click += exportLoadOrderToolStripMenuItem1_Click;
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            importLoadOrderToolStripMenuItem1.Size = new Size(177, 22);
            importLoadOrderToolStripMenuItem1.Text = "&Import load order...";
            importLoadOrderToolStripMenuItem1.Click += importLoadOrderToolStripMenuItem1_Click;
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
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
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
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripPlatformLabel, toolStripStatusLabelMwVersion });
            statusStrip1.Location = new Point(0, 557);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1184, 22);
            statusStrip1.TabIndex = 36;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelMwVersion
            // 
            toolStripStatusLabelMwVersion.Name = "toolStripStatusLabelMwVersion";
            toolStripStatusLabelMwVersion.Size = new Size(22, 17);
            toolStripStatusLabelMwVersion.Text = "---";
            // 
            // rotatingLabel1
            // 
            rotatingLabel1.AutoSize = true;
            rotatingLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabel1.Location = new Point(100, 118);
            rotatingLabel1.Name = "rotatingLabel1";
            rotatingLabel1.NewText = "";
            rotatingLabel1.RotateAngle = 0;
            rotatingLabel1.Size = new Size(18, 19);
            rotatingLabel1.TabIndex = 12;
            rotatingLabel1.Text = "X";
            // 
            // contextMenuStripMod
            // 
            contextMenuStripMod.Items.AddRange(new ToolStripItem[] { openFolderToolStripMenuItem });
            contextMenuStripMod.Name = "contextMenuStripMod";
            contextMenuStripMod.Size = new Size(140, 26);
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new Size(139, 22);
            openFolderToolStripMenuItem.Text = "Open &Folder";
            openFolderToolStripMenuItem.Click += openFolderToolStripMenuItem_Click;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(listBox1);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(listBox2);
            tabPage1.Controls.Add(listBox3);
            tabPage1.Controls.Add(label5);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(338, 500);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Overrides";
            tabPage1.UseVisualStyleBackColor = true;
            tabPage1.Click += tabPage1_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 54);
            label6.Name = "label6";
            label6.Size = new Size(62, 13);
            label6.TabIndex = 25;
            label6.Text = "Overriding";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 11);
            label4.Name = "label4";
            label4.Size = new Size(19, 13);
            label4.TabIndex = 20;
            label4.Text = "---";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 13;
            listBox1.Location = new Point(6, 70);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(160, 147);
            listBox1.TabIndex = 21;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 240);
            label7.Name = "label7";
            label7.Size = new Size(90, 13);
            label7.TabIndex = 26;
            label7.Text = "Manifest Entries";
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox2.FormattingEnabled = true;
            listBox2.HorizontalScrollbar = true;
            listBox2.ItemHeight = 13;
            listBox2.Location = new Point(6, 256);
            listBox2.Name = "listBox2";
            listBox2.SelectionMode = SelectionMode.None;
            listBox2.Size = new Size(329, 225);
            listBox2.TabIndex = 22;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            // 
            // listBox3
            // 
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 13;
            listBox3.Location = new Point(175, 70);
            listBox3.Name = "listBox3";
            listBox3.Size = new Size(160, 147);
            listBox3.TabIndex = 23;
            listBox3.SelectedIndexChanged += listBox3_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(172, 54);
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
            tabPageModInfo.Size = new Size(338, 502);
            tabPageModInfo.TabIndex = 3;
            tabPageModInfo.Text = "Overview";
            tabPageModInfo.UseVisualStyleBackColor = true;
            // 
            // pictureBoxModImage
            // 
            pictureBoxModImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxModImage.Location = new Point(12, 13);
            pictureBoxModImage.Name = "pictureBoxModImage";
            pictureBoxModImage.Size = new Size(318, 139);
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
            panelModInfo.Size = new Size(338, 373);
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
            richTextBoxModDescription.Location = new Point(15, 184);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new Size(295, 157);
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
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPageModInfo);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(838, 26);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(346, 528);
            tabControl1.TabIndex = 30;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 579);
            Controls.Add(button4);
            Controls.Add(statusStrip1);
            Controls.Add(rotatingLabel1);
            Controls.Add(tabControl1);
            Controls.Add(button5);
            Controls.Add(checkBox1);
            Controls.Add(filterBox);
            Controls.Add(label3);
            Controls.Add(modsListView);
            Controls.Add(menuStrip1);
            Controls.Add(button6);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(900, 300);
            Name = "MainWindow";
            Text = "MW5 LoadOrderManager";
            Load += Form1_Load;
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
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button6;
        public System.Windows.Forms.ListView modsListView;
        public System.Windows.Forms.ColumnHeader displayHeader;
        public System.Windows.Forms.ColumnHeader folderHeader;
        public System.Windows.Forms.ColumnHeader authorHeader;
        public System.Windows.Forms.ColumnHeader enabledHeader;
        public System.Windows.Forms.ColumnHeader versionHeader;
        public System.Windows.Forms.ToolStripLabel toolStripVendorLabeltoolStripLabel1;
        private RotatingLabel rotatingLabel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox filterBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button5;
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
        private ToolStripStatusLabel toolStripStatusLabelMwVersion;
        private ContextMenuStrip contextMenuStripMod;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMenuItemSettings;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemOpenModFolderSteam;
        private ColumnHeader buildHeader;
        private ColumnHeader originalLoadOrderHeader;
        private TabPage tabPage1;
        private Label label6;
        private Label label4;
        private ListBox listBox1;
        private Label label7;
        private ListBox listBox2;
        private ListBox listBox3;
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
    }
}

