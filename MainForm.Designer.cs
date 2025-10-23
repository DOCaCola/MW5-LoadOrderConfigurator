using System;
using System.Drawing;
using System.Windows.Forms;
using MW5_Mod_Manager.Controls;

namespace MW5_Mod_Manager
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            openFileDialog1 = new OpenFileDialog();
            toolStripPlatformLabel = new ToolStripStatusLabel();
            textProgressBarBindingSource = new BindingSource(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            applyCurrentConfigToolStripMenuItem = new ToolStripMenuItem();
            runMechWarrior5ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator14 = new ToolStripSeparator();
            toolStripMenuItemInstallArchive = new ToolStripMenuItem();
            toolStripMenuItemInstallFromFolder = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            importLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
            exportLoadOrderToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripMenuItemSettings = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemColumns = new ToolStripMenuItem();
            toolStripSeparator16 = new ToolStripSeparator();
            toolStripMenuItemOverviewWindowToggle = new ToolStripMenuItem();
            toolStripMenuItemConflictWindowToggle = new ToolStripMenuItem();
            toolStripSeparator15 = new ToolStripSeparator();
            resetWindowLayoutToolStripMenuItem = new ToolStripMenuItem();
            presetsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemLoadPresets = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            savePresetToolStripMenuItem = new ToolStripMenuItem();
            deletePresetToolStripMenuItem = new ToolStripMenuItem();
            modsToolStripMenuItem = new ToolStripMenuItem();
            reloadModDataToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator13 = new ToolStripSeparator();
            enableAllModsToolStripMenuItem = new ToolStripMenuItem();
            disableAllModsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripMenuItemSortDefaultLoadOrder = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            openModsFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemOpenModFolderSteam = new ToolStripMenuItem();
            openUserModsFolderToolStripMenuItem = new ToolStripMenuItem();
            checkModFilesToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            reportBugToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator18 = new ToolStripSeparator();
            toolStripMenuItemNexusmodsLink = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelUpdate = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabelModsActive = new ToolStripStatusLabel();
            toolStripStatusLabelModCountTotal = new ToolStripStatusLabel();
            toolStripStatusLabelMwVersion = new ToolStripStatusLabel();
            contextMenuStripMod = new ContextMenuStrip(components);
            enableModsToolStripMenuItem = new ToolStripMenuItem();
            disableModsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            moveupToolStripMenuItem = new ToolStripMenuItem();
            movedownToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            contextMenuItemMoveToTop = new ToolStripMenuItem();
            contextMenuItemMoveToBottom = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            toolTip1 = new ToolTip(components);
            toolStrip1 = new ToolStrip();
            toolStripButtonApply = new ToolStripButton();
            toolStripButtonStartGame = new ToolStripButton();
            toolStripSeparator10 = new ToolStripSeparator();
            toolStripButtonReload = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            toolStripButtonNexusmods = new ToolStripButton();
            toolStripButtonSteamWorkshop = new ToolStripButton();
            toolStripSeparator17 = new ToolStripSeparator();
            toolStripTextFilterBox = new LocToolStripTextBox();
            toolStripSeparator11 = new ToolStripSeparator();
            toolStripButtonClearFilter = new ToolStripButton();
            toolStripButtonFilterToggle = new ToolStripButton();
            timerOverviewUpdateDelay = new Timer(components);
            timerDelayedListRecolor = new Timer(components);
            contextMenuStripColumnOptions = new ContextMenuStrip(components);
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(components);
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            contextMenuStripMod.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripPlatformLabel
            // 
            toolStripPlatformLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripPlatformLabel.Name = "toolStripPlatformLabel";
            toolStripPlatformLabel.Size = new Size(16, 19);
            toolStripPlatformLabel.Text = "-";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem, presetsToolStripMenuItem, modsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1167, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { applyCurrentConfigToolStripMenuItem, runMechWarrior5ToolStripMenuItem, toolStripSeparator14, toolStripMenuItemInstallArchive, toolStripMenuItemInstallFromFolder, toolStripSeparator12, importLoadOrderToolStripMenuItem1, exportLoadOrderToolStripMenuItem1, toolStripSeparator2, toolStripMenuItemSettings, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // applyCurrentConfigToolStripMenuItem
            // 
            applyCurrentConfigToolStripMenuItem.Name = "applyCurrentConfigToolStripMenuItem";
            applyCurrentConfigToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            applyCurrentConfigToolStripMenuItem.Size = new Size(223, 22);
            applyCurrentConfigToolStripMenuItem.Text = "&Apply current config";
            // 
            // runMechWarrior5ToolStripMenuItem
            // 
            runMechWarrior5ToolStripMenuItem.Name = "runMechWarrior5ToolStripMenuItem";
            runMechWarrior5ToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+M";
            runMechWarrior5ToolStripMenuItem.Size = new Size(223, 22);
            runMechWarrior5ToolStripMenuItem.Text = "Run &MechWarrior 5";
            runMechWarrior5ToolStripMenuItem.Click += runMechWarrior5ToolStripMenuItem_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new Size(220, 6);
            // 
            // toolStripMenuItemInstallArchive
            // 
            toolStripMenuItemInstallArchive.Name = "toolStripMenuItemInstallArchive";
            toolStripMenuItemInstallArchive.Size = new Size(223, 22);
            toolStripMenuItemInstallArchive.Text = "Install from &archive...";
            toolStripMenuItemInstallArchive.Click += toolStripMenuItemInstallArchive_Click;
            // 
            // toolStripMenuItemInstallFromFolder
            // 
            toolStripMenuItemInstallFromFolder.Name = "toolStripMenuItemInstallFromFolder";
            toolStripMenuItemInstallFromFolder.Size = new Size(223, 22);
            toolStripMenuItemInstallFromFolder.Text = "Install from &folder...";
            toolStripMenuItemInstallFromFolder.Click += toolStripMenuItemInstallFromFolder_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(220, 6);
            // 
            // importLoadOrderToolStripMenuItem1
            // 
            importLoadOrderToolStripMenuItem1.Name = "importLoadOrderToolStripMenuItem1";
            importLoadOrderToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+I";
            importLoadOrderToolStripMenuItem1.Size = new Size(223, 22);
            importLoadOrderToolStripMenuItem1.Text = "&Import load order...";
            importLoadOrderToolStripMenuItem1.Click += importLoadOrderToolStripMenuItem1_Click;
            // 
            // exportLoadOrderToolStripMenuItem1
            // 
            exportLoadOrderToolStripMenuItem1.Name = "exportLoadOrderToolStripMenuItem1";
            exportLoadOrderToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+E";
            exportLoadOrderToolStripMenuItem1.Size = new Size(223, 22);
            exportLoadOrderToolStripMenuItem1.Text = "&Export load order...";
            exportLoadOrderToolStripMenuItem1.Click += exportLoadOrderToolStripMenuItem1_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(220, 6);
            // 
            // toolStripMenuItemSettings
            // 
            toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            toolStripMenuItemSettings.Size = new Size(223, 22);
            toolStripMenuItemSettings.Text = "&Settings";
            toolStripMenuItemSettings.Click += toolStripMenuItemSettings_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(220, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(223, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemColumns, toolStripSeparator16, toolStripMenuItemOverviewWindowToggle, toolStripMenuItemConflictWindowToggle, toolStripSeparator15, resetWindowLayoutToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // toolStripMenuItemColumns
            // 
            toolStripMenuItemColumns.Name = "toolStripMenuItemColumns";
            toolStripMenuItemColumns.Size = new Size(183, 22);
            toolStripMenuItemColumns.Text = "&Columns";
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new Size(180, 6);
            // 
            // toolStripMenuItemOverviewWindowToggle
            // 
            toolStripMenuItemOverviewWindowToggle.Name = "toolStripMenuItemOverviewWindowToggle";
            toolStripMenuItemOverviewWindowToggle.Size = new Size(183, 22);
            toolStripMenuItemOverviewWindowToggle.Text = "Mod &Overview";
            toolStripMenuItemOverviewWindowToggle.Click += toolStripMenuItemOverviewWindowToggle_Click;
            // 
            // toolStripMenuItemConflictWindowToggle
            // 
            toolStripMenuItemConflictWindowToggle.Name = "toolStripMenuItemConflictWindowToggle";
            toolStripMenuItemConflictWindowToggle.Size = new Size(183, 22);
            toolStripMenuItemConflictWindowToggle.Text = "&Conflicts View";
            toolStripMenuItemConflictWindowToggle.Click += toolStripMenuItemConflictWindowToggle_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new Size(180, 6);
            // 
            // resetWindowLayoutToolStripMenuItem
            // 
            resetWindowLayoutToolStripMenuItem.Name = "resetWindowLayoutToolStripMenuItem";
            resetWindowLayoutToolStripMenuItem.Size = new Size(183, 22);
            resetWindowLayoutToolStripMenuItem.Text = "&Reset window layout";
            resetWindowLayoutToolStripMenuItem.Click += resetWindowLayoutToolStripMenuItem_Click;
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
            modsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { reloadModDataToolStripMenuItem, toolStripSeparator13, enableAllModsToolStripMenuItem, disableAllModsToolStripMenuItem, toolStripSeparator3, toolStripMenuItemSortDefaultLoadOrder, toolStripSeparator8, openModsFolderToolStripMenuItem, toolStripMenuItemOpenModFolderSteam, openUserModsFolderToolStripMenuItem, checkModFilesToolStripMenuItem });
            modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            modsToolStripMenuItem.Size = new Size(49, 20);
            modsToolStripMenuItem.Text = "&Mods";
            // 
            // reloadModDataToolStripMenuItem
            // 
            reloadModDataToolStripMenuItem.Name = "reloadModDataToolStripMenuItem";
            reloadModDataToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+R";
            reloadModDataToolStripMenuItem.Size = new Size(210, 22);
            reloadModDataToolStripMenuItem.Text = "&Reload mod data";
            reloadModDataToolStripMenuItem.Click += reloadModDataToolStripMenuItem_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new Size(207, 6);
            // 
            // enableAllModsToolStripMenuItem
            // 
            enableAllModsToolStripMenuItem.Name = "enableAllModsToolStripMenuItem";
            enableAllModsToolStripMenuItem.Size = new Size(210, 22);
            enableAllModsToolStripMenuItem.Text = "&Enable all";
            enableAllModsToolStripMenuItem.Click += enableAllModsToolStripMenuItem_Click;
            // 
            // disableAllModsToolStripMenuItem
            // 
            disableAllModsToolStripMenuItem.Name = "disableAllModsToolStripMenuItem";
            disableAllModsToolStripMenuItem.Size = new Size(210, 22);
            disableAllModsToolStripMenuItem.Text = "&Disable all";
            disableAllModsToolStripMenuItem.Click += disableAllModsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(207, 6);
            // 
            // toolStripMenuItemSortDefaultLoadOrder
            // 
            toolStripMenuItemSortDefaultLoadOrder.Name = "toolStripMenuItemSortDefaultLoadOrder";
            toolStripMenuItemSortDefaultLoadOrder.Size = new Size(210, 22);
            toolStripMenuItemSortDefaultLoadOrder.Text = "Re&store default load order";
            toolStripMenuItemSortDefaultLoadOrder.Click += toolStripMenuItemSortDefaultLoadOrder_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(207, 6);
            // 
            // openModsFolderToolStripMenuItem
            // 
            openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            openModsFolderToolStripMenuItem.Size = new Size(210, 22);
            openModsFolderToolStripMenuItem.Text = "&Open Mods Folder";
            openModsFolderToolStripMenuItem.Click += openModsFolderToolStripMenuItem_Click;
            // 
            // toolStripMenuItemOpenModFolderSteam
            // 
            toolStripMenuItemOpenModFolderSteam.Name = "toolStripMenuItemOpenModFolderSteam";
            toolStripMenuItemOpenModFolderSteam.Size = new Size(210, 22);
            toolStripMenuItemOpenModFolderSteam.Text = "Open S&team Mods folder";
            toolStripMenuItemOpenModFolderSteam.Visible = false;
            toolStripMenuItemOpenModFolderSteam.Click += toolStripMenuItemOpenModFolderSteam_Click;
            // 
            // openUserModsFolderToolStripMenuItem
            // 
            openUserModsFolderToolStripMenuItem.Name = "openUserModsFolderToolStripMenuItem";
            openUserModsFolderToolStripMenuItem.Size = new Size(210, 22);
            openUserModsFolderToolStripMenuItem.Text = "Open &User Mods folder";
            openUserModsFolderToolStripMenuItem.Click += openUserModsFolderToolStripMenuItem_Click;
            // 
            // checkModFilesToolStripMenuItem
            // 
            checkModFilesToolStripMenuItem.Name = "checkModFilesToolStripMenuItem";
            checkModFilesToolStripMenuItem.Size = new Size(210, 22);
            checkModFilesToolStripMenuItem.Text = "&Check Mod Files...";
            checkModFilesToolStripMenuItem.Visible = false;
            checkModFilesToolStripMenuItem.Click += checkModFilesToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { reportBugToolStripMenuItem, toolStripSeparator18, toolStripMenuItemNexusmodsLink, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(40, 20);
            helpToolStripMenuItem.Text = "&Info";
            // 
            // reportBugToolStripMenuItem
            // 
            reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
            reportBugToolStripMenuItem.Size = new Size(179, 22);
            reportBugToolStripMenuItem.Text = "&Report Bug";
            reportBugToolStripMenuItem.Click += reportBugToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            toolStripSeparator18.Size = new Size(176, 6);
            // 
            // toolStripMenuItemNexusmodsLink
            // 
            toolStripMenuItemNexusmodsLink.Name = "toolStripMenuItemNexusmodsLink";
            toolStripMenuItemNexusmodsLink.Size = new Size(179, 22);
            toolStripMenuItemNexusmodsLink.Text = "Visit on &Nexusmods";
            toolStripMenuItemNexusmodsLink.Click += toolStripMenuItemNexusmodsLink_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(179, 22);
            aboutToolStripMenuItem.Text = "Ab&out";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelUpdate, toolStripStatusLabel1, toolStripStatusLabelModsActive, toolStripStatusLabelModCountTotal, toolStripPlatformLabel, toolStripStatusLabelMwVersion });
            statusStrip1.Location = new Point(0, 555);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1167, 24);
            statusStrip1.TabIndex = 36;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelUpdate
            // 
            toolStripStatusLabelUpdate.IsLink = true;
            toolStripStatusLabelUpdate.Name = "toolStripStatusLabelUpdate";
            toolStripStatusLabelUpdate.Size = new Size(66, 19);
            toolStripStatusLabelUpdate.Text = "updateLink";
            toolStripStatusLabelUpdate.Visible = false;
            toolStripStatusLabelUpdate.Click += toolStripStatusLabelUpdate_Click;
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
            contextMenuStripMod.Items.AddRange(new ToolStripItem[] { enableModsToolStripMenuItem, disableModsToolStripMenuItem, toolStripSeparator7, moveupToolStripMenuItem, movedownToolStripMenuItem, toolStripSeparator6, contextMenuItemMoveToTop, contextMenuItemMoveToBottom, toolStripSeparator5, openFolderToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStripMod.Name = "contextMenuStripMod";
            contextMenuStripMod.Size = new Size(162, 198);
            // 
            // enableModsToolStripMenuItem
            // 
            enableModsToolStripMenuItem.Name = "enableModsToolStripMenuItem";
            enableModsToolStripMenuItem.Size = new Size(161, 22);
            enableModsToolStripMenuItem.Text = "Enable";
            enableModsToolStripMenuItem.Click += enableModsToolStripMenuItem_Click;
            // 
            // disableModsToolStripMenuItem
            // 
            disableModsToolStripMenuItem.Name = "disableModsToolStripMenuItem";
            disableModsToolStripMenuItem.Size = new Size(161, 22);
            disableModsToolStripMenuItem.Text = "Disable";
            disableModsToolStripMenuItem.Click += disableModsToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(158, 6);
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
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonApply, toolStripButtonStartGame, toolStripSeparator10, toolStripButtonReload, toolStripSeparator9, toolStripButtonNexusmods, toolStripButtonSteamWorkshop, toolStripSeparator17, toolStripTextFilterBox, toolStripSeparator11, toolStripButtonClearFilter, toolStripButtonFilterToggle });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1167, 45);
            toolStrip1.TabIndex = 41;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonApply
            // 
            toolStripButtonApply.Image = (Image)resources.GetObject("toolStripButtonApply.Image");
            toolStripButtonApply.ImageTransparentColor = Color.Magenta;
            toolStripButtonApply.Name = "toolStripButtonApply";
            toolStripButtonApply.Padding = new Padding(14, 0, 14, 0);
            toolStripButtonApply.Size = new Size(70, 42);
            toolStripButtonApply.Text = "Apply";
            toolStripButtonApply.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonApply.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonApply.ToolTipText = "Apply load order to MechWarrior";
            toolStripButtonApply.Click += toolStripButtonApply_Click;
            // 
            // toolStripButtonStartGame
            // 
            toolStripButtonStartGame.Image = UiIcons.MW5MercsLogo;
            toolStripButtonStartGame.ImageTransparentColor = Color.Magenta;
            toolStripButtonStartGame.Name = "toolStripButtonStartGame";
            toolStripButtonStartGame.Padding = new Padding(2, 0, 2, 0);
            toolStripButtonStartGame.Size = new Size(70, 42);
            toolStripButtonStartGame.Text = "Start MW5";
            toolStripButtonStartGame.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonStartGame.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonStartGame.ToolTipText = "Start MechWarrior";
            toolStripButtonStartGame.Click += toolStripButtonStart_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(6, 45);
            // 
            // toolStripButtonReload
            // 
            toolStripButtonReload.Image = UiIcons.Reload;
            toolStripButtonReload.ImageTransparentColor = Color.Magenta;
            toolStripButtonReload.Name = "toolStripButtonReload";
            toolStripButtonReload.Padding = new Padding(12, 0, 11, 0);
            toolStripButtonReload.Size = new Size(70, 42);
            toolStripButtonReload.Text = "Reload";
            toolStripButtonReload.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonReload.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonReload.Click += toolStripButtonReload_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 45);
            // 
            // toolStripButtonNexusmods
            // 
            toolStripButtonNexusmods.Image = UiIcons.Nexusmods;
            toolStripButtonNexusmods.ImageTransparentColor = Color.Magenta;
            toolStripButtonNexusmods.Name = "toolStripButtonNexusmods";
            toolStripButtonNexusmods.Overflow = ToolStripItemOverflow.Never;
            toolStripButtonNexusmods.Size = new Size(74, 42);
            toolStripButtonNexusmods.Text = "Nexusmods";
            toolStripButtonNexusmods.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonNexusmods.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonNexusmods.ToolTipText = "Open Nexusmods";
            toolStripButtonNexusmods.Click += toolStripButtonNexusmods_Click;
            // 
            // toolStripButtonSteamWorkshop
            // 
            toolStripButtonSteamWorkshop.Image = UiIcons.Steam;
            toolStripButtonSteamWorkshop.ImageTransparentColor = Color.Magenta;
            toolStripButtonSteamWorkshop.Name = "toolStripButtonSteamWorkshop";
            toolStripButtonSteamWorkshop.Padding = new Padding(3, 0, 2, 0);
            toolStripButtonSteamWorkshop.Size = new Size(70, 42);
            toolStripButtonSteamWorkshop.Text = "Workshop";
            toolStripButtonSteamWorkshop.TextAlign = ContentAlignment.BottomCenter;
            toolStripButtonSteamWorkshop.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonSteamWorkshop.ToolTipText = "Open Steam Workshop";
            toolStripButtonSteamWorkshop.Visible = false;
            toolStripButtonSteamWorkshop.Click += toolStripButtonSteamWorkshop_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new Size(6, 45);
            // 
            // toolStripTextFilterBox
            // 
            toolStripTextFilterBox.CueBanner = "Search (Ctrl+F)";
            toolStripTextFilterBox.Margin = new Padding(5, 0, 5, 0);
            toolStripTextFilterBox.Name = "toolStripTextFilterBox";
            toolStripTextFilterBox.Size = new Size(140, 45);
            toolStripTextFilterBox.KeyDown += toolStripTextFilterBox_KeyDown;
            toolStripTextFilterBox.KeyPress += toolStripTextFilterBox_KeyPress;
            toolStripTextFilterBox.KeyUp += toolStripTextFilterBox_KeyUp;
            toolStripTextFilterBox.TextChanged += toolStripTextFilterBox_TextChanged;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(6, 45);
            // 
            // toolStripButtonClearFilter
            // 
            toolStripButtonClearFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonClearFilter.Enabled = false;
            toolStripButtonClearFilter.Image = UiIcons.FilterClear;
            toolStripButtonClearFilter.ImageTransparentColor = Color.Magenta;
            toolStripButtonClearFilter.Name = "toolStripButtonClearFilter";
            toolStripButtonClearFilter.Size = new Size(23, 42);
            toolStripButtonClearFilter.Text = "Clear";
            toolStripButtonClearFilter.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonClearFilter.ToolTipText = "Clear filter";
            toolStripButtonClearFilter.Click += toolStripButtonClearFilter_Click;
            // 
            // toolStripButtonFilterToggle
            // 
            toolStripButtonFilterToggle.CheckOnClick = true;
            toolStripButtonFilterToggle.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonFilterToggle.Image = UiIcons.FilterToggle;
            toolStripButtonFilterToggle.ImageTransparentColor = Color.Magenta;
            toolStripButtonFilterToggle.Name = "toolStripButtonFilterToggle";
            toolStripButtonFilterToggle.Size = new Size(23, 42);
            toolStripButtonFilterToggle.Text = "Filter";
            toolStripButtonFilterToggle.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonFilterToggle.ToolTipText = "Toggle filter mode";
            toolStripButtonFilterToggle.CheckedChanged += toolStripButtonFilterToggle_CheckedChanged;
            // 
            // timerOverviewUpdateDelay
            // 
            timerOverviewUpdateDelay.Interval = 75;
            timerOverviewUpdateDelay.Tick += timerOverviewUpdateDelay_Tick;
            // 
            // timerDelayedListRecolor
            // 
            timerDelayedListRecolor.Interval = 10;
            timerDelayedListRecolor.Tick += timerDelayedListRecolor_Tick;
            // 
            // contextMenuStripColumnOptions
            // 
            contextMenuStripColumnOptions.Name = "contextMenuStrip1";
            contextMenuStripColumnOptions.Size = new Size(61, 4);
            // 
            // dockPanel1
            // 
            dockPanel1.Dock = DockStyle.Fill;
            dockPanel1.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            dockPanel1.Location = new Point(0, 69);
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Size = new Size(1167, 486);
            dockPanel1.TabIndex = 42;
            // 
            // visualStudioToolStripExtender1
            // 
            visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1167, 579);
            Controls.Add(dockPanel1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(900, 550);
            Name = "MainForm";
            Text = "MechWarrior 5 Load Order Configurator";
            FormClosing += MainWindow_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainWindow_Load;
            Shown += MainWindow_Shown;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            KeyDown += MainForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)textProgressBarBindingSource).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStripMod.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ToolStripLabel toolStripVendorLabeltoolStripLabel1;
        private BindingSource textProgressBarBindingSource;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem exportLoadOrderToolStripMenuItem1;
        private ToolStripMenuItem importLoadOrderToolStripMenuItem1;
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
        public ContextMenuStrip contextMenuStripMod;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMenuItemSettings;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripMenuItem toolStripMenuItemOpenModFolderSteam;
        public ToolStripMenuItem presetsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemLoadPresets;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem savePresetToolStripMenuItem;
        private ToolStripMenuItem deletePresetToolStripMenuItem;
        private ToolTip toolTip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabelModCountTotal;
        private ToolStripStatusLabel toolStripStatusLabelModsActive;
        private ToolStripMenuItem contextMenuItemMoveToTop;
        private ToolStripMenuItem contextMenuItemMoveToBottom;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem moveupToolStripMenuItem;
        private ToolStripMenuItem movedownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem toolStripMenuItemSortDefaultLoadOrder;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem openUserModsFolderToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonReload;
        private ToolStripButton toolStripButtonApply;
        private ToolStripButton toolStripButtonStartGame;
        private ToolStripButton toolStripButtonFilterToggle;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripButton toolStripButtonClearFilter;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemInstallArchive;
        private ToolStripMenuItem toolStripMenuItemInstallFromFolder;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem toolStripMenuItemNexusmodsLink;
        private ToolStripStatusLabel toolStripStatusLabelUpdate;
        private Timer timerOverviewUpdateDelay;
        private ToolStripMenuItem reloadModDataToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem applyCurrentConfigToolStripMenuItem;
        private ToolStripMenuItem runMechWarrior5ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private LocToolStripTextBox toolStripTextFilterBox;
        private ToolStripMenuItem enableModsToolStripMenuItem;
        private ToolStripMenuItem disableModsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private Timer timerDelayedListRecolor;
        public ContextMenuStrip contextMenuStripColumnOptions;
        private ToolStripMenuItem checkModFilesToolStripMenuItem;
        private ToolStripButton toolStripButtonSteamWorkshop;
        private ToolStripButton toolStripButtonNexusmods;
        private ToolStripSeparator toolStripSeparator17;
        public WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private ToolStripMenuItem viewToolStripMenuItem;
        public ToolStripMenuItem toolStripMenuItemOverviewWindowToggle;
        public ToolStripMenuItem toolStripMenuItemConflictWindowToggle;
        private ToolStripMenuItem toolStripMenuItemColumns;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem resetWindowLayoutToolStripMenuItem;
        private ToolStripMenuItem reportBugToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
    }
}

