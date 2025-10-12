using System.Drawing;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    partial class DockModListForm
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
            toolStrip2 = new ToolStrip();
            toTopToolStripButton = new ToolStripButton();
            upToolStripButton = new ToolStripButton();
            toolStripLabel2 = new ToolStripLabel();
            downToolStripButton = new ToolStripButton();
            toBottomToolStripButton = new ToolStripButton();
            rotatingLabelBottom = new MW5_Mod_Manager.Controls.RotatingLabel();
            rotatingLabelTop = new MW5_Mod_Manager.Controls.RotatingLabel();
            modObjectListView = new MW5_Mod_Manager.Controls.ModsObjectsListView();
            olvColumnModName = new BrightIdeasSoftware.OLVColumn();
            olvColumnModAuthor = new BrightIdeasSoftware.OLVColumn();
            olvColumnModVersion = new BrightIdeasSoftware.OLVColumn();
            olvColumnModCurLoadOrder = new BrightIdeasSoftware.OLVColumn();
            olvColumnModOrgLoadOrder = new BrightIdeasSoftware.OLVColumn();
            olvColumnModFileSize = new BrightIdeasSoftware.OLVColumn();
            olvColumnModFolder = new BrightIdeasSoftware.OLVColumn();
            olvColumnModFileAge = new BrightIdeasSoftware.OLVColumn();
            olvColumnFreeSpaceDummy = new BrightIdeasSoftware.OLVColumn();
            imageListIcons = new ImageList(components);
            panelColorLegend = new Panel();
            label8 = new Label();
            label4 = new Label();
            label2 = new Label();
            panelColorOverridingOverridden = new Panel();
            panelColorOverriding = new Panel();
            panelColorOverridden = new Panel();
            toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)modObjectListView).BeginInit();
            panelColorLegend.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip2
            // 
            toolStrip2.Anchor = AnchorStyles.Left;
            toolStrip2.AutoSize = false;
            toolStrip2.BackColor = Color.Transparent;
            toolStrip2.CanOverflow = false;
            toolStrip2.Dock = DockStyle.None;
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.Items.AddRange(new ToolStripItem[] { toTopToolStripButton, upToolStripButton, toolStripLabel2, downToolStripButton, toBottomToolStripButton });
            toolStrip2.LayoutStyle = ToolStripLayoutStyle.Flow;
            toolStrip2.Location = new Point(3, 147);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(25, 112);
            toolStrip2.TabIndex = 43;
            toolStrip2.Text = "toolStrip2";
            // 
            // toTopToolStripButton
            // 
            toTopToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toTopToolStripButton.Image = UiIcons.Top;
            toTopToolStripButton.ImageTransparentColor = Color.Magenta;
            toTopToolStripButton.Margin = new Padding(1, 1, 0, 2);
            toTopToolStripButton.Name = "toTopToolStripButton";
            toTopToolStripButton.Size = new Size(23, 20);
            toTopToolStripButton.Text = "To top";
            toTopToolStripButton.ToolTipText = "Move selected mod(s) to top";
            toTopToolStripButton.Click += toTopToolStripButton_Click;
            // 
            // upToolStripButton
            // 
            upToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            upToolStripButton.Image = UiIcons.Up;
            upToolStripButton.ImageTransparentColor = Color.Magenta;
            upToolStripButton.Margin = new Padding(1, 1, 0, 2);
            upToolStripButton.Name = "upToolStripButton";
            upToolStripButton.Size = new Size(23, 20);
            upToolStripButton.Text = "Up";
            upToolStripButton.ToolTipText = "Move selected mod(s) up";
            upToolStripButton.Click += upToolStripButton_Click;
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.AutoSize = false;
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(15, 15);
            // 
            // downToolStripButton
            // 
            downToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            downToolStripButton.Image = UiIcons.Down;
            downToolStripButton.ImageTransparentColor = Color.Magenta;
            downToolStripButton.Margin = new Padding(1, 1, 0, 2);
            downToolStripButton.Name = "downToolStripButton";
            downToolStripButton.Size = new Size(23, 20);
            downToolStripButton.Text = "Down";
            downToolStripButton.ToolTipText = "Move selected mod(s) down";
            downToolStripButton.Click += downToolStripButton_Click;
            // 
            // toBottomToolStripButton
            // 
            toBottomToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toBottomToolStripButton.Image = UiIcons.Bottom;
            toBottomToolStripButton.ImageTransparentColor = Color.Magenta;
            toBottomToolStripButton.Margin = new Padding(1, 1, 0, 2);
            toBottomToolStripButton.Name = "toBottomToolStripButton";
            toBottomToolStripButton.Size = new Size(23, 20);
            toBottomToolStripButton.Text = "To bottom";
            toBottomToolStripButton.ToolTipText = "Move selected mod(s) to bottom";
            toBottomToolStripButton.Click += toBottomToolStripButton_Click;
            // 
            // rotatingLabelBottom
            // 
            rotatingLabelBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rotatingLabelBottom.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabelBottom.Location = new Point(4, 306);
            rotatingLabelBottom.Name = "rotatingLabelBottom";
            rotatingLabelBottom.NewText = "« Low priority";
            rotatingLabelBottom.RotateAngle = -90;
            rotatingLabelBottom.Size = new Size(19, 94);
            rotatingLabelBottom.TabIndex = 42;
            // 
            // rotatingLabelTop
            // 
            rotatingLabelTop.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rotatingLabelTop.Location = new Point(4, 9);
            rotatingLabelTop.Name = "rotatingLabelTop";
            rotatingLabelTop.NewText = "High priority »";
            rotatingLabelTop.RotateAngle = -90;
            rotatingLabelTop.Size = new Size(19, 99);
            rotatingLabelTop.TabIndex = 41;
            // 
            // modObjectListView
            // 
            modObjectListView.AllColumns.Add(olvColumnModName);
            modObjectListView.AllColumns.Add(olvColumnModAuthor);
            modObjectListView.AllColumns.Add(olvColumnModVersion);
            modObjectListView.AllColumns.Add(olvColumnModCurLoadOrder);
            modObjectListView.AllColumns.Add(olvColumnModOrgLoadOrder);
            modObjectListView.AllColumns.Add(olvColumnModFileSize);
            modObjectListView.AllColumns.Add(olvColumnModFolder);
            modObjectListView.AllColumns.Add(olvColumnModFileAge);
            modObjectListView.AllColumns.Add(olvColumnFreeSpaceDummy);
            modObjectListView.AllowColumnReorder = true;
            modObjectListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modObjectListView.CheckBoxes = true;
            modObjectListView.Columns.AddRange(new ColumnHeader[] { olvColumnModName, olvColumnModAuthor, olvColumnModVersion, olvColumnModFileAge, olvColumnModCurLoadOrder, olvColumnModOrgLoadOrder, olvColumnModFileSize, olvColumnModFolder, olvColumnFreeSpaceDummy });
            modObjectListView.FullRowSelect = true;
            modObjectListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            modObjectListView.Location = new Point(30, 0);
            modObjectListView.Name = "modObjectListView";
            modObjectListView.SelectColumnsOnRightClick = false;
            modObjectListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            modObjectListView.ShowFilterMenuOnRightClick = false;
            modObjectListView.ShowImagesOnSubItems = true;
            modObjectListView.ShowItemToolTips = true;
            modObjectListView.ShowSortIndicators = false;
            modObjectListView.Size = new Size(770, 424);
            modObjectListView.SmallImageList = imageListIcons;
            modObjectListView.TabIndex = 41;
            modObjectListView.UseHotControls = false;
            modObjectListView.UseHotItem = true;
            modObjectListView.UseOverlays = false;
            modObjectListView.UseTranslucentHotItem = true;
            modObjectListView.UseTranslucentSelection = true;
            modObjectListView.View = View.Details;
            modObjectListView.BeforeSearching += modObjectListView_BeforeSearching;
            modObjectListView.BeforeSorting += modObjectListView_BeforeSorting;
            modObjectListView.BeforeCreatingGroups += modObjectListView_BeforeCreatingGroups;
            modObjectListView.AboutToCreateGroups += modObjectListView_AboutToCreateGroups;
            modObjectListView.CellToolTipShowing += modObjectListView_CellToolTipShowing;
            modObjectListView.ColumnRightClick += modObjectListView_ColumnRightClick;
            modObjectListView.FormatCell += modObjectListView_FormatCell;
            modObjectListView.FormatRow += modObjectListView_FormatRow;
            modObjectListView.ModelDropped += modObjectListView_ModelDropped;
            modObjectListView.ColumnReordered += modObjectListView_ColumnReordered;
            modObjectListView.SelectedIndexChanged += modObjectListView_SelectedIndexChanged;
            modObjectListView.DragOver += modObjectListView_DragOver;
            modObjectListView.MouseClick += modObjectListView_MouseClick;
            // 
            // olvColumnModName
            // 
            olvColumnModName.Hideable = false;
            olvColumnModName.MinimumWidth = 40;
            olvColumnModName.Text = "Mod";
            olvColumnModName.Width = 300;
            // 
            // olvColumnModAuthor
            // 
            olvColumnModAuthor.Groupable = false;
            olvColumnModAuthor.MinimumWidth = 10;
            olvColumnModAuthor.Sortable = false;
            olvColumnModAuthor.Text = "Author";
            olvColumnModAuthor.ToolTipText = "";
            olvColumnModAuthor.Width = 90;
            // 
            // olvColumnModVersion
            // 
            olvColumnModVersion.Groupable = false;
            olvColumnModVersion.MinimumWidth = 10;
            olvColumnModVersion.Searchable = false;
            olvColumnModVersion.Sortable = false;
            olvColumnModVersion.Text = "Version";
            olvColumnModVersion.ToolTipText = "Version and build number";
            olvColumnModVersion.Width = 70;
            // 
            // olvColumnModCurLoadOrder
            // 
            olvColumnModCurLoadOrder.Groupable = false;
            olvColumnModCurLoadOrder.MinimumWidth = 10;
            olvColumnModCurLoadOrder.Searchable = false;
            olvColumnModCurLoadOrder.Sortable = false;
            olvColumnModCurLoadOrder.Text = "LO";
            olvColumnModCurLoadOrder.TextAlign = HorizontalAlignment.Right;
            olvColumnModCurLoadOrder.ToolTipText = "Current set load order";
            olvColumnModCurLoadOrder.Width = 40;
            // 
            // olvColumnModOrgLoadOrder
            // 
            olvColumnModOrgLoadOrder.Groupable = false;
            olvColumnModOrgLoadOrder.MinimumWidth = 10;
            olvColumnModOrgLoadOrder.Searchable = false;
            olvColumnModOrgLoadOrder.Sortable = false;
            olvColumnModOrgLoadOrder.Text = "oLO";
            olvColumnModOrgLoadOrder.TextAlign = HorizontalAlignment.Right;
            olvColumnModOrgLoadOrder.ToolTipText = "Stock/Original load order";
            olvColumnModOrgLoadOrder.Width = 40;
            // 
            // olvColumnModFileSize
            // 
            olvColumnModFileSize.Groupable = false;
            olvColumnModFileSize.MinimumWidth = 10;
            olvColumnModFileSize.Searchable = false;
            olvColumnModFileSize.Sortable = false;
            olvColumnModFileSize.Text = "File size";
            olvColumnModFileSize.ToolTipText = "Size of installed files";
            // 
            // olvColumnModFolder
            // 
            olvColumnModFolder.Groupable = false;
            olvColumnModFolder.MinimumWidth = 10;
            olvColumnModFolder.Sortable = false;
            olvColumnModFolder.Text = "Mod Folder";
            olvColumnModFolder.ToolTipText = "Mod directory name";
            olvColumnModFolder.Width = 100;
            // 
            // olvColumnModFileAge
            // 
            olvColumnModFileAge.MinimumWidth = 10;
            olvColumnModFileAge.Searchable = false;
            olvColumnModFileAge.Sortable = false;
            olvColumnModFileAge.Text = "File age";
            olvColumnModFileAge.ToolTipText = "Age of mod files";
            olvColumnModFileAge.Width = 70;
            // 
            // olvColumnFreeSpaceDummy
            // 
            olvColumnFreeSpaceDummy.FillsFreeSpace = true;
            olvColumnFreeSpaceDummy.Hideable = false;
            olvColumnFreeSpaceDummy.IsEditable = false;
            olvColumnFreeSpaceDummy.IsVisible = false;
            olvColumnFreeSpaceDummy.Searchable = false;
            olvColumnFreeSpaceDummy.ShowTextInHeader = false;
            olvColumnFreeSpaceDummy.Sortable = false;
            olvColumnFreeSpaceDummy.Text = "";
            // 
            // imageListIcons
            // 
            imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageListIcons.ImageSize = new Size(16, 16);
            imageListIcons.TransparentColor = Color.Transparent;
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
            panelColorLegend.Location = new Point(34, 427);
            panelColorLegend.Name = "panelColorLegend";
            panelColorLegend.Size = new Size(368, 21);
            panelColorLegend.TabIndex = 45;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(193, 2);
            label8.Margin = new Padding(0);
            label8.Name = "label8";
            label8.Size = new Size(138, 15);
            label8.TabIndex = 5;
            label8.Text = "Overriding && Overridden";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(105, 2);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(66, 15);
            label4.TabIndex = 4;
            label4.Text = "Overridden";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 2);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
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
            // DockModListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            CloseButton = false;
            CloseButtonVisible = false;
            Controls.Add(panelColorLegend);
            Controls.Add(modObjectListView);
            Controls.Add(toolStrip2);
            Controls.Add(rotatingLabelBottom);
            Controls.Add(rotatingLabelTop);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            Name = "DockModListForm";
            Text = "DockModListForm";
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)modObjectListView).EndInit();
            panelColorLegend.ResumeLayout(false);
            panelColorLegend.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.ToolStrip toolStrip2;
        public System.Windows.Forms.ToolStripButton toTopToolStripButton;
        public System.Windows.Forms.ToolStripButton upToolStripButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        public System.Windows.Forms.ToolStripButton downToolStripButton;
        public System.Windows.Forms.ToolStripButton toBottomToolStripButton;
        public Controls.RotatingLabel rotatingLabelBottom;
        public Controls.RotatingLabel rotatingLabelTop;
        public Controls.ModsObjectsListView modObjectListView;
        public BrightIdeasSoftware.OLVColumn olvColumnModName;
        public BrightIdeasSoftware.OLVColumn olvColumnModFolder;
        public BrightIdeasSoftware.OLVColumn olvColumnModAuthor;
        public BrightIdeasSoftware.OLVColumn olvColumnModFileSize;
        public BrightIdeasSoftware.OLVColumn olvColumnModVersion;
        public BrightIdeasSoftware.OLVColumn olvColumnModCurLoadOrder;
        public BrightIdeasSoftware.OLVColumn olvColumnModOrgLoadOrder;
        public BrightIdeasSoftware.OLVColumn olvColumnFreeSpaceDummy;
        public BrightIdeasSoftware.OLVColumn olvColumnModFileAge;
        private System.Windows.Forms.Panel panelColorLegend;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Panel panelColorOverridingOverridden;
        public System.Windows.Forms.Panel panelColorOverriding;
        public System.Windows.Forms.Panel panelColorOverridden;
        public ImageList imageListIcons;
    }
}