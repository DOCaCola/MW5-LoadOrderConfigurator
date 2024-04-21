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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.toolStripVendorLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.modsListView = new System.Windows.Forms.ListView();
            this.enabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.display = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.author = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dependencies = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.filterBox = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button12 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.listBox4 = new System.Windows.Forms.ListBox();
            this.button11 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView2 = new System.Windows.Forms.ListView();
            this.label8 = new System.Windows.Forms.Label();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textProgressBarBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installDirectoryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setvendorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.epicStoreToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gOGToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.steamToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsStoreToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exportLoadOrderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importLoadOrderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.exportmodsFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.shareModsViaTCPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModsFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAllModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllModsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelMwVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.rotatingLabel1 = new MW5_Mod_Manager.RotatingLabel();
            this.contextMenuStripMod = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textProgressBarBindingSource)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripMod.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 38);
            this.button1.TabIndex = 1;
            this.button1.Text = "&UP";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 166);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 38);
            this.button2.TabIndex = 2;
            this.button2.Text = "&DOWN";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 279);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 38);
            this.button3.TabIndex = 3;
            this.button3.Text = "&Apply";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(260, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(506, 20);
            this.textBox1.TabIndex = 5;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(13, 46);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(128, 38);
            this.button6.TabIndex = 7;
            this.button6.Text = "Refresh";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // toolStripVendorLabel
            // 
            this.toolStripVendorLabel.Name = "toolStripVendorLabel";
            this.toolStripVendorLabel.Size = new System.Drawing.Size(22, 17);
            this.toolStripVendorLabel.Text = "---";
            // 
            // modsListView
            // 
            this.modsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modsListView.CheckBoxes = true;
            this.modsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.enabled,
            this.display,
            this.folder,
            this.author,
            this.version,
            this.dependencies});
            this.modsListView.FullRowSelect = true;
            this.modsListView.GridLines = true;
            this.modsListView.HideSelection = false;
            this.modsListView.LabelWrap = false;
            this.modsListView.Location = new System.Drawing.Point(124, 90);
            this.modsListView.MultiSelect = false;
            this.modsListView.Name = "modsListView";
            this.modsListView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.modsListView.Size = new System.Drawing.Size(708, 464);
            this.modsListView.TabIndex = 11;
            this.modsListView.UseCompatibleStateImageBehavior = false;
            this.modsListView.View = System.Windows.Forms.View.Details;
            this.modsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            this.modsListView.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.modsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.modsListView_MouseClick);
            // 
            // enabled
            // 
            this.enabled.Tag = "enabled";
            this.enabled.Text = "X";
            this.enabled.Width = 20;
            // 
            // display
            // 
            this.display.Tag = "display";
            this.display.Text = "Display Name";
            this.display.Width = 188;
            // 
            // folder
            // 
            this.folder.Tag = "folder";
            this.folder.Text = "Mod Folder";
            this.folder.Width = 196;
            // 
            // author
            // 
            this.author.Tag = "author";
            this.author.Text = "Author";
            this.author.Width = 72;
            // 
            // version
            // 
            this.version.Tag = "version";
            this.version.Text = "Version";
            this.version.Width = 54;
            // 
            // dependencies
            // 
            this.dependencies.Tag = "dependencies";
            this.dependencies.Text = "Dependencies";
            this.dependencies.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dependencies.Width = 88;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(13, 451);
            this.button4.Margin = new System.Windows.Forms.Padding(0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(70, 90);
            this.button4.TabIndex = 13;
            this.button4.Text = "Start MW5";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Mods Folder";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Filter Mods";
            // 
            // filterBox
            // 
            this.filterBox.Location = new System.Drawing.Point(333, 56);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(433, 20);
            this.filterBox.TabIndex = 17;
            this.filterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(260, 58);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(67, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Highlight";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(13, 327);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(70, 38);
            this.button5.TabIndex = 19;
            this.button5.Text = "Mark for Removal";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "---";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 70);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(160, 147);
            this.listBox1.TabIndex = 21;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.HorizontalScrollbar = true;
            this.listBox2.Location = new System.Drawing.Point(6, 256);
            this.listBox2.Name = "listBox2";
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox2.Size = new System.Drawing.Size(329, 225);
            this.listBox2.TabIndex = 22;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new System.Drawing.Point(175, 70);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(160, 147);
            this.listBox3.TabIndex = 23;
            this.listBox3.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(172, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Overridden By";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Overriding";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 240);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Manifest Entries";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(838, 26);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(346, 528);
            this.tabControl1.TabIndex = 30;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button12);
            this.tabPage3.Controls.Add(this.textBox2);
            this.tabPage3.Controls.Add(this.listBox4);
            this.tabPage3.Controls.Add(this.button11);
            this.tabPage3.Controls.Add(this.button7);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(338, 502);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Save/Load Presets";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(243, 13);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(86, 41);
            this.button12.TabIndex = 5;
            this.button12.Text = "Delete Preset";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 84);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(323, 20);
            this.textBox2.TabIndex = 4;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // listBox4
            // 
            this.listBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox4.FormattingEnabled = true;
            this.listBox4.Location = new System.Drawing.Point(7, 125);
            this.listBox4.Name = "listBox4";
            this.listBox4.Size = new System.Drawing.Size(323, 368);
            this.listBox4.TabIndex = 3;
            this.listBox4.SelectedIndexChanged += new System.EventHandler(this.listBox4_SelectedIndexChanged);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(125, 13);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(86, 41);
            this.button11.TabIndex = 2;
            this.button11.Text = "Load Preset";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(7, 13);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 41);
            this.button7.TabIndex = 1;
            this.button7.Text = "Save Preset";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.listBox1);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.listBox2);
            this.tabPage1.Controls.Add(this.listBox3);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(338, 502);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Overrding Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.listView2);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(338, 502);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Dependencies";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(6, 69);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(329, 409);
            this.listView2.TabIndex = 32;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.List;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "---";
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(513, 26);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(253, 20);
            this.textBox3.TabIndex = 34;
            this.textBox3.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.modsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installDirectoryToolStripMenuItem1,
            this.setvendorToolStripMenuItem,
            this.toolStripSeparator6,
            this.exportLoadOrderToolStripMenuItem1,
            this.importLoadOrderToolStripMenuItem1,
            this.toolStripSeparator7,
            this.exportmodsFolderToolStripMenuItem1,
            this.shareModsViaTCPToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // installDirectoryToolStripMenuItem1
            // 
            this.installDirectoryToolStripMenuItem1.Name = "installDirectoryToolStripMenuItem1";
            this.installDirectoryToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
            this.installDirectoryToolStripMenuItem1.Text = "Select &install directory";
            this.installDirectoryToolStripMenuItem1.Click += new System.EventHandler(this.installDirectoryToolStripMenuItem1_Click);
            // 
            // setvendorToolStripMenuItem
            // 
            this.setvendorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.epicStoreToolStripMenuItem1,
            this.gOGToolStripMenuItem1,
            this.steamToolStripMenuItem1,
            this.windowsStoreToolStripMenuItem1});
            this.setvendorToolStripMenuItem.Name = "setvendorToolStripMenuItem";
            this.setvendorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setvendorToolStripMenuItem.Text = "Set &vendor";
            // 
            // epicStoreToolStripMenuItem1
            // 
            this.epicStoreToolStripMenuItem1.Name = "epicStoreToolStripMenuItem1";
            this.epicStoreToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.epicStoreToolStripMenuItem1.Text = "E&pic Store";
            this.epicStoreToolStripMenuItem1.Click += new System.EventHandler(this.epicStoreToolStripMenuItem1_Click);
            // 
            // gOGToolStripMenuItem1
            // 
            this.gOGToolStripMenuItem1.Name = "gOGToolStripMenuItem1";
            this.gOGToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.gOGToolStripMenuItem1.Text = "&GOG";
            this.gOGToolStripMenuItem1.Click += new System.EventHandler(this.gOGToolStripMenuItem1_Click);
            // 
            // steamToolStripMenuItem1
            // 
            this.steamToolStripMenuItem1.Name = "steamToolStripMenuItem1";
            this.steamToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.steamToolStripMenuItem1.Text = "&Steam";
            this.steamToolStripMenuItem1.Click += new System.EventHandler(this.steamToolStripMenuItem1_Click);
            // 
            // windowsStoreToolStripMenuItem1
            // 
            this.windowsStoreToolStripMenuItem1.Name = "windowsStoreToolStripMenuItem1";
            this.windowsStoreToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.windowsStoreToolStripMenuItem1.Text = "&Windows Store";
            this.windowsStoreToolStripMenuItem1.Click += new System.EventHandler(this.windowsStoreToolStripMenuItem1_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(186, 6);
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            this.exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            this.exportLoadOrderToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
            this.exportLoadOrderToolStripMenuItem1.Text = "&Export load order";
            this.exportLoadOrderToolStripMenuItem1.Click += new System.EventHandler(this.exportLoadOrderToolStripMenuItem1_Click);
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            this.importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            this.importLoadOrderToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
            this.importLoadOrderToolStripMenuItem1.Text = "&Import load order";
            this.importLoadOrderToolStripMenuItem1.Click += new System.EventHandler(this.importLoadOrderToolStripMenuItem1_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(186, 6);
            // 
            // exportmodsFolderToolStripMenuItem1
            // 
            this.exportmodsFolderToolStripMenuItem1.Name = "exportmodsFolderToolStripMenuItem1";
            this.exportmodsFolderToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
            this.exportmodsFolderToolStripMenuItem1.Text = "Export &mods folder";
            this.exportmodsFolderToolStripMenuItem1.Click += new System.EventHandler(this.exportmodsFolderToolStripMenuItem1_Click);
            // 
            // shareModsViaTCPToolStripMenuItem
            // 
            this.shareModsViaTCPToolStripMenuItem.Name = "shareModsViaTCPToolStripMenuItem";
            this.shareModsViaTCPToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.shareModsViaTCPToolStripMenuItem.Text = "Share mods via &TCP";
            this.shareModsViaTCPToolStripMenuItem.Visible = false;
            this.shareModsViaTCPToolStripMenuItem.Click += new System.EventHandler(this.shareModsViaTCPToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // modsToolStripMenuItem
            // 
            this.modsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModsFolderToolStripMenuItem,
            this.enableAllModsToolStripMenuItem,
            this.disableAllModsToolStripMenuItem});
            this.modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            this.modsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.modsToolStripMenuItem.Text = "&Mods";
            // 
            // openModsFolderToolStripMenuItem
            // 
            this.openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            this.openModsFolderToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openModsFolderToolStripMenuItem.Text = "&Open Mods Folder";
            this.openModsFolderToolStripMenuItem.Click += new System.EventHandler(this.openModsFolderToolStripMenuItem_Click);
            // 
            // enableAllModsToolStripMenuItem
            // 
            this.enableAllModsToolStripMenuItem.Name = "enableAllModsToolStripMenuItem";
            this.enableAllModsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.enableAllModsToolStripMenuItem.Text = "&Enable all mods";
            this.enableAllModsToolStripMenuItem.Click += new System.EventHandler(this.enableAllModsToolStripMenuItem_Click);
            // 
            // disableAllModsToolStripMenuItem
            // 
            this.disableAllModsToolStripMenuItem.Name = "disableAllModsToolStripMenuItem";
            this.disableAllModsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.disableAllModsToolStripMenuItem.Text = "&Disable all mods";
            this.disableAllModsToolStripMenuItem.Click += new System.EventHandler(this.disableAllModsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "Ab&out";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripVendorLabel,
            this.toolStripStatusLabelMwVersion});
            this.statusStrip1.Location = new System.Drawing.Point(0, 557);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip1.TabIndex = 36;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelMwVersion
            // 
            this.toolStripStatusLabelMwVersion.Name = "toolStripStatusLabelMwVersion";
            this.toolStripStatusLabelMwVersion.Size = new System.Drawing.Size(22, 17);
            this.toolStripStatusLabelMwVersion.Text = "---";
            // 
            // rotatingLabel1
            // 
            this.rotatingLabel1.AutoSize = true;
            this.rotatingLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotatingLabel1.Location = new System.Drawing.Point(100, 118);
            this.rotatingLabel1.Name = "rotatingLabel1";
            this.rotatingLabel1.NewText = "";
            this.rotatingLabel1.RotateAngle = 0;
            this.rotatingLabel1.Size = new System.Drawing.Size(18, 17);
            this.rotatingLabel1.TabIndex = 12;
            this.rotatingLabel1.Text = "X";
            // 
            // contextMenuStripMod
            // 
            this.contextMenuStripMod.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem});
            this.contextMenuStripMod.Name = "contextMenuStripMod";
            this.contextMenuStripMod.Size = new System.Drawing.Size(140, 26);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openFolderToolStripMenuItem.Text = "Open &Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 579);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.rotatingLabel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.filterBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.modsListView);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(900, 300);
            this.Name = "MainWindow";
            this.Text = "MW5 LoadOrderManager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textProgressBarBindingSource)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripMod.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
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
        private System.Windows.Forms.Label label2;
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
        public TextBox textBox3;
        private BindingSource textProgressBarBindingSource;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem installDirectoryToolStripMenuItem1;
        private ToolStripMenuItem setvendorToolStripMenuItem;
        private ToolStripMenuItem epicStoreToolStripMenuItem1;
        private ToolStripMenuItem gOGToolStripMenuItem1;
        private ToolStripMenuItem steamToolStripMenuItem1;
        private ToolStripMenuItem windowsStoreToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator6;
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
    }
}

