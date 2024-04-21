using System;
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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            openFileDialog1 = new OpenFileDialog();
            button6 = new Button();
            toolStripVendorLabel = new ToolStripStatusLabel();
            modsListView = new ListView();
            enabled = new ColumnHeader();
            display = new ColumnHeader();
            folder = new ColumnHeader();
            author = new ColumnHeader();
            version = new ColumnHeader();
            dependencies = new ColumnHeader();
            button4 = new Button();
            label3 = new Label();
            filterBox = new TextBox();
            checkBox1 = new CheckBox();
            button5 = new Button();
            label4 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            listBox3 = new ListBox();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            tabControl1 = new TabControl();
            tabPage3 = new TabPage();
            button12 = new Button();
            textBox2 = new TextBox();
            listBox4 = new ListBox();
            button11 = new Button();
            button7 = new Button();
            tabPageModInfo = new TabPage();
            panelModInfo = new Panel();
            label1 = new Label();
            richTextBoxModDescription = new RichTextBox();
            labelSteamId = new Label();
            linkLabelSteamId = new LinkLabel();
            linkLabelModAuthorUrl = new LinkLabel();
            labelModBuildNumber = new Label();
            labelModVersion = new Label();
            labelModAuthor = new Label();
            labelModName = new Label();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            listView2 = new ListView();
            label8 = new Label();
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
            tabControl1.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPageModInfo.SuspendLayout();
            panelModInfo.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            contextMenuStripMod.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(13, 122);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(70, 38);
            button1.TabIndex = 1;
            button1.Text = "&UP";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(13, 166);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(70, 38);
            button2.TabIndex = 2;
            button2.Text = "&DOWN";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(13, 279);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(70, 38);
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
            button6.Location = new System.Drawing.Point(13, 46);
            button6.Name = "button6";
            button6.Size = new System.Drawing.Size(70, 38);
            button6.TabIndex = 7;
            button6.Text = "Refresh";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // toolStripVendorLabel
            // 
            toolStripVendorLabel.Name = "toolStripVendorLabel";
            toolStripVendorLabel.Size = new System.Drawing.Size(22, 17);
            toolStripVendorLabel.Text = "---";
            // 
            // modsListView
            // 
            modsListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modsListView.CheckBoxes = true;
            modsListView.Columns.AddRange(new ColumnHeader[] { enabled, display, folder, author, version, dependencies });
            modsListView.FullRowSelect = true;
            modsListView.GridLines = true;
            modsListView.LabelWrap = false;
            modsListView.Location = new System.Drawing.Point(124, 61);
            modsListView.MultiSelect = false;
            modsListView.Name = "modsListView";
            modsListView.RightToLeft = RightToLeft.No;
            modsListView.Size = new System.Drawing.Size(708, 493);
            modsListView.TabIndex = 11;
            modsListView.UseCompatibleStateImageBehavior = false;
            modsListView.View = View.Details;
            modsListView.ItemChecked += listView1_ItemChecked;
            modsListView.SelectedIndexChanged += listView1_SelectedIndexChanged;
            modsListView.MouseClick += modsListView_MouseClick;
            // 
            // enabled
            // 
            enabled.Tag = "enabled";
            enabled.Text = "X";
            enabled.Width = 20;
            // 
            // display
            // 
            display.Tag = "display";
            display.Text = "Display Name";
            display.Width = 188;
            // 
            // folder
            // 
            folder.Tag = "folder";
            folder.Text = "Mod Folder";
            folder.Width = 196;
            // 
            // author
            // 
            author.Tag = "author";
            author.Text = "Author";
            author.Width = 72;
            // 
            // version
            // 
            version.Tag = "version";
            version.Text = "Version";
            version.Width = 54;
            // 
            // dependencies
            // 
            dependencies.Tag = "dependencies";
            dependencies.Text = "Dependencies";
            dependencies.TextAlign = HorizontalAlignment.Center;
            dependencies.Width = 88;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            button4.Location = new System.Drawing.Point(13, 451);
            button4.Margin = new Padding(0);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(70, 90);
            button4.TabIndex = 13;
            button4.Text = "Start MW5";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(180, 33);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 13);
            label3.TabIndex = 16;
            label3.Text = "Filter Mods";
            // 
            // filterBox
            // 
            filterBox.Location = new System.Drawing.Point(339, 30);
            filterBox.Name = "filterBox";
            filterBox.Size = new System.Drawing.Size(425, 22);
            filterBox.TabIndex = 17;
            filterBox.TextChanged += filterBox_TextChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new System.Drawing.Point(258, 32);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(75, 17);
            checkBox1.TabIndex = 18;
            checkBox1.Text = "Highlight";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // button5
            // 
            button5.Location = new System.Drawing.Point(13, 327);
            button5.Name = "button5";
            button5.Size = new System.Drawing.Size(70, 38);
            button5.TabIndex = 19;
            button5.Text = "Mark for Removal";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 11);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(19, 13);
            label4.TabIndex = 20;
            label4.Text = "---";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 13;
            listBox1.Location = new System.Drawing.Point(6, 70);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(160, 147);
            listBox1.TabIndex = 21;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox2.FormattingEnabled = true;
            listBox2.HorizontalScrollbar = true;
            listBox2.ItemHeight = 13;
            listBox2.Location = new System.Drawing.Point(6, 256);
            listBox2.Name = "listBox2";
            listBox2.SelectionMode = SelectionMode.None;
            listBox2.Size = new System.Drawing.Size(329, 225);
            listBox2.TabIndex = 22;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            // 
            // listBox3
            // 
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 13;
            listBox3.Location = new System.Drawing.Point(175, 70);
            listBox3.Name = "listBox3";
            listBox3.Size = new System.Drawing.Size(160, 147);
            listBox3.TabIndex = 23;
            listBox3.SelectedIndexChanged += listBox3_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(172, 54);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(79, 13);
            label5.TabIndex = 24;
            label5.Text = "Overridden By";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(3, 54);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(62, 13);
            label6.TabIndex = 25;
            label6.Text = "Overriding";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(3, 240);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(90, 13);
            label7.TabIndex = 26;
            label7.Text = "Manifest Entries";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPageModInfo);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new System.Drawing.Point(838, 26);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(346, 528);
            tabControl1.TabIndex = 30;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(button12);
            tabPage3.Controls.Add(textBox2);
            tabPage3.Controls.Add(listBox4);
            tabPage3.Controls.Add(button11);
            tabPage3.Controls.Add(button7);
            tabPage3.Location = new System.Drawing.Point(4, 22);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new System.Drawing.Size(338, 502);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Save/Load Presets";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            button12.Location = new System.Drawing.Point(243, 13);
            button12.Name = "button12";
            button12.Size = new System.Drawing.Size(86, 41);
            button12.TabIndex = 5;
            button12.Text = "Delete Preset";
            button12.UseVisualStyleBackColor = true;
            button12.Click += button12_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(6, 84);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(323, 22);
            textBox2.TabIndex = 4;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // listBox4
            // 
            listBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox4.FormattingEnabled = true;
            listBox4.ItemHeight = 13;
            listBox4.Location = new System.Drawing.Point(7, 125);
            listBox4.Name = "listBox4";
            listBox4.Size = new System.Drawing.Size(323, 342);
            listBox4.TabIndex = 3;
            listBox4.SelectedIndexChanged += listBox4_SelectedIndexChanged;
            // 
            // button11
            // 
            button11.Location = new System.Drawing.Point(125, 13);
            button11.Name = "button11";
            button11.Size = new System.Drawing.Size(86, 41);
            button11.TabIndex = 2;
            button11.Text = "Load Preset";
            button11.UseVisualStyleBackColor = true;
            button11.Click += button11_Click;
            // 
            // button7
            // 
            button7.Location = new System.Drawing.Point(7, 13);
            button7.Name = "button7";
            button7.Size = new System.Drawing.Size(86, 41);
            button7.TabIndex = 1;
            button7.Text = "Save Preset";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // tabPageModInfo
            // 
            tabPageModInfo.Controls.Add(panelModInfo);
            tabPageModInfo.Location = new System.Drawing.Point(4, 24);
            tabPageModInfo.Name = "tabPageModInfo";
            tabPageModInfo.Padding = new Padding(3);
            tabPageModInfo.Size = new System.Drawing.Size(338, 500);
            tabPageModInfo.TabIndex = 3;
            tabPageModInfo.Text = "Overview";
            tabPageModInfo.UseVisualStyleBackColor = true;
            // 
            // panelModInfo
            // 
            panelModInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelModInfo.Controls.Add(label1);
            panelModInfo.Controls.Add(richTextBoxModDescription);
            panelModInfo.Controls.Add(labelSteamId);
            panelModInfo.Controls.Add(linkLabelSteamId);
            panelModInfo.Controls.Add(linkLabelModAuthorUrl);
            panelModInfo.Controls.Add(labelModBuildNumber);
            panelModInfo.Controls.Add(labelModVersion);
            panelModInfo.Controls.Add(labelModAuthor);
            panelModInfo.Controls.Add(labelModName);
            panelModInfo.Location = new System.Drawing.Point(6, 6);
            panelModInfo.Name = "panelModInfo";
            panelModInfo.Size = new System.Drawing.Size(324, 487);
            panelModInfo.TabIndex = 1;
            panelModInfo.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 154);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 13);
            label1.TabIndex = 10;
            label1.Text = "Description:";
            // 
            // richTextBoxModDescription
            // 
            richTextBoxModDescription.Location = new System.Drawing.Point(15, 173);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new System.Drawing.Size(295, 221);
            richTextBoxModDescription.TabIndex = 9;
            richTextBoxModDescription.Text = "";
            richTextBoxModDescription.LinkClicked += richTextBoxModDescription_LinkClicked;
            // 
            // labelSteamId
            // 
            labelSteamId.AutoSize = true;
            labelSteamId.Location = new System.Drawing.Point(12, 124);
            labelSteamId.Name = "labelSteamId";
            labelSteamId.Size = new System.Drawing.Size(55, 13);
            labelSteamId.TabIndex = 8;
            labelSteamId.Text = "Steam ID:";
            // 
            // linkLabelSteamId
            // 
            linkLabelSteamId.AutoSize = true;
            linkLabelSteamId.Location = new System.Drawing.Point(73, 124);
            linkLabelSteamId.Name = "linkLabelSteamId";
            linkLabelSteamId.Size = new System.Drawing.Size(94, 13);
            linkLabelSteamId.TabIndex = 7;
            linkLabelSteamId.TabStop = true;
            linkLabelSteamId.Text = "linkLabelSteamId";
            linkLabelSteamId.LinkClicked += linkLabelSteamId_LinkClicked;
            // 
            // linkLabelModAuthorUrl
            // 
            linkLabelModAuthorUrl.AutoSize = true;
            linkLabelModAuthorUrl.Location = new System.Drawing.Point(12, 54);
            linkLabelModAuthorUrl.Name = "linkLabelModAuthorUrl";
            linkLabelModAuthorUrl.Size = new System.Drawing.Size(59, 13);
            linkLabelModAuthorUrl.TabIndex = 6;
            linkLabelModAuthorUrl.TabStop = true;
            linkLabelModAuthorUrl.Text = "linkLabel1";
            linkLabelModAuthorUrl.LinkClicked += linkLabelModAuthorUrl_LinkClicked;
            // 
            // labelModBuildNumber
            // 
            labelModBuildNumber.AutoSize = true;
            labelModBuildNumber.Location = new System.Drawing.Point(12, 98);
            labelModBuildNumber.Name = "labelModBuildNumber";
            labelModBuildNumber.Size = new System.Drawing.Size(123, 13);
            labelModBuildNumber.TabIndex = 4;
            labelModBuildNumber.Text = "labelModBuildNumber";
            // 
            // labelModVersion
            // 
            labelModVersion.AutoSize = true;
            labelModVersion.Location = new System.Drawing.Point(12, 81);
            labelModVersion.Name = "labelModVersion";
            labelModVersion.Size = new System.Drawing.Size(94, 13);
            labelModVersion.TabIndex = 3;
            labelModVersion.Text = "labelModVersion";
            // 
            // labelModAuthor
            // 
            labelModAuthor.AutoSize = true;
            labelModAuthor.Location = new System.Drawing.Point(12, 36);
            labelModAuthor.Name = "labelModAuthor";
            labelModAuthor.Size = new System.Drawing.Size(92, 13);
            labelModAuthor.TabIndex = 2;
            labelModAuthor.Text = "labelModAuthor";
            // 
            // labelModName
            // 
            labelModName.AutoSize = true;
            labelModName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelModName.Location = new System.Drawing.Point(12, 10);
            labelModName.Name = "labelModName";
            labelModName.Size = new System.Drawing.Size(88, 13);
            labelModName.TabIndex = 1;
            labelModName.Text = "labelModName";
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
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new System.Drawing.Size(338, 500);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Overrding Data";
            tabPage1.UseVisualStyleBackColor = true;
            tabPage1.Click += tabPage1_Click;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = System.Drawing.Color.Transparent;
            tabPage2.Controls.Add(listView2);
            tabPage2.Controls.Add(label8);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new System.Drawing.Size(338, 500);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Dependencies";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            listView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listView2.Location = new System.Drawing.Point(6, 69);
            listView2.Name = "listView2";
            listView2.Size = new System.Drawing.Size(329, 410);
            listView2.TabIndex = 32;
            listView2.UseCompatibleStateImageBehavior = false;
            listView2.View = View.List;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(6, 11);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(19, 13);
            label8.TabIndex = 31;
            label8.Text = "---";
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, modsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1184, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportLoadOrderToolStripMenuItem1, importLoadOrderToolStripMenuItem1, toolStripSeparator7, exportmodsFolderToolStripMenuItem1, shareModsViaTCPToolStripMenuItem, toolStripSeparator2, toolStripMenuItemSettings, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            fileToolStripMenuItem.Click += fileToolStripMenuItem_Click;
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            exportLoadOrderToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            exportLoadOrderToolStripMenuItem1.Text = "&Export load order...";
            exportLoadOrderToolStripMenuItem1.Click += exportLoadOrderToolStripMenuItem1_Click;
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            importLoadOrderToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            importLoadOrderToolStripMenuItem1.Text = "&Import load order...";
            importLoadOrderToolStripMenuItem1.Click += importLoadOrderToolStripMenuItem1_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(174, 6);
            // 
            // exportmodsFolderToolStripMenuItem1
            // 
            exportmodsFolderToolStripMenuItem1.Name = "exportmodsFolderToolStripMenuItem1";
            exportmodsFolderToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            exportmodsFolderToolStripMenuItem1.Text = "Export &mods folder";
            exportmodsFolderToolStripMenuItem1.Click += exportmodsFolderToolStripMenuItem1_Click;
            // 
            // shareModsViaTCPToolStripMenuItem
            // 
            shareModsViaTCPToolStripMenuItem.Name = "shareModsViaTCPToolStripMenuItem";
            shareModsViaTCPToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            shareModsViaTCPToolStripMenuItem.Text = "Share mods via &TCP";
            shareModsViaTCPToolStripMenuItem.Visible = false;
            shareModsViaTCPToolStripMenuItem.Click += shareModsViaTCPToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // toolStripMenuItemSettings
            // 
            toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            toolStripMenuItemSettings.Size = new System.Drawing.Size(177, 22);
            toolStripMenuItemSettings.Text = "&Settings";
            toolStripMenuItemSettings.Click += toolStripMenuItemSettings_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // modsToolStripMenuItem
            // 
            modsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { enableAllModsToolStripMenuItem, disableAllModsToolStripMenuItem, toolStripSeparator3, openModsFolderToolStripMenuItem, toolStripMenuItemOpenModFolderSteam });
            modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            modsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            modsToolStripMenuItem.Text = "&Mods";
            // 
            // enableAllModsToolStripMenuItem
            // 
            enableAllModsToolStripMenuItem.Name = "enableAllModsToolStripMenuItem";
            enableAllModsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            enableAllModsToolStripMenuItem.Text = "&Enable all mods";
            enableAllModsToolStripMenuItem.Click += enableAllModsToolStripMenuItem_Click;
            // 
            // disableAllModsToolStripMenuItem
            // 
            disableAllModsToolStripMenuItem.Name = "disableAllModsToolStripMenuItem";
            disableAllModsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            disableAllModsToolStripMenuItem.Text = "&Disable all mods";
            disableAllModsToolStripMenuItem.Click += disableAllModsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // openModsFolderToolStripMenuItem
            // 
            openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            openModsFolderToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            openModsFolderToolStripMenuItem.Text = "&Open Mods Folder";
            openModsFolderToolStripMenuItem.Click += openModsFolderToolStripMenuItem_Click;
            // 
            // toolStripMenuItemOpenModFolderSteam
            // 
            toolStripMenuItemOpenModFolderSteam.Name = "toolStripMenuItemOpenModFolderSteam";
            toolStripMenuItemOpenModFolderSteam.Size = new System.Drawing.Size(206, 22);
            toolStripMenuItemOpenModFolderSteam.Text = "Open &Steam Mods folder";
            toolStripMenuItemOpenModFolderSteam.Visible = false;
            toolStripMenuItemOpenModFolderSteam.Click += toolStripMenuItemOpenModFolderSteam_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "Ab&out";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripVendorLabel, toolStripStatusLabelMwVersion });
            statusStrip1.Location = new System.Drawing.Point(0, 557);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1184, 22);
            statusStrip1.TabIndex = 36;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelMwVersion
            // 
            toolStripStatusLabelMwVersion.Name = "toolStripStatusLabelMwVersion";
            toolStripStatusLabelMwVersion.Size = new System.Drawing.Size(22, 17);
            toolStripStatusLabelMwVersion.Text = "---";
            // 
            // rotatingLabel1
            // 
            rotatingLabel1.AutoSize = true;
            rotatingLabel1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            rotatingLabel1.Location = new System.Drawing.Point(100, 118);
            rotatingLabel1.Name = "rotatingLabel1";
            rotatingLabel1.NewText = "";
            rotatingLabel1.RotateAngle = 0;
            rotatingLabel1.Size = new System.Drawing.Size(18, 19);
            rotatingLabel1.TabIndex = 12;
            rotatingLabel1.Text = "X";
            // 
            // contextMenuStripMod
            // 
            contextMenuStripMod.Items.AddRange(new ToolStripItem[] { openFolderToolStripMenuItem });
            contextMenuStripMod.Name = "contextMenuStripMod";
            contextMenuStripMod.Size = new System.Drawing.Size(140, 26);
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            openFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            openFolderToolStripMenuItem.Text = "Open &Folder";
            openFolderToolStripMenuItem.Click += openFolderToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1184, 579);
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
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            MinimumSize = new System.Drawing.Size(900, 300);
            Name = "MainWindow";
            Text = "MW5 LoadOrderManager";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPageModInfo.ResumeLayout(false);
            panelModInfo.ResumeLayout(false);
            panelModInfo.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStripMod.ResumeLayout(false);
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
        public System.Windows.Forms.ColumnHeader display;
        public System.Windows.Forms.ColumnHeader folder;
        public System.Windows.Forms.ColumnHeader author;
        private System.Windows.Forms.ColumnHeader enabled;
        private System.Windows.Forms.ColumnHeader version;
        public System.Windows.Forms.ToolStripLabel toolStripVendorLabeltoolStripLabel1;
        private RotatingLabel rotatingLabel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox filterBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ColumnHeader dependencies;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label8;
        private ListView listView2;
        public System.ComponentModel.BackgroundWorker backgroundWorker2;
        private TabPage tabPage3;
        public TextBox textBox2;
        public ListBox listBox4;
        public Button button11;
        public Button button7;
        public Button button12;
        private BindingSource textProgressBarBindingSource;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem exportLoadOrderToolStripMenuItem1;
        private ToolStripMenuItem importLoadOrderToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem exportmodsFolderToolStripMenuItem1;
        private ToolStripMenuItem shareModsViaTCPToolStripMenuItem;
        private ToolStripStatusLabel toolStripVendorLabel;
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
        private TabPage tabPageModInfo;
        private Panel panelModInfo;
        private Label labelModName;
        private Label labelModAuthor;
        private Label labelModVersion;
        private Label labelModBuildNumber;
        private LinkLabel linkLabelModAuthorUrl;
        private Label labelSteamId;
        private LinkLabel linkLabelSteamId;
        private RichTextBox richTextBoxModDescription;
        private Label label1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMenuItemSettings;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemOpenModFolderSteam;
    }
}

