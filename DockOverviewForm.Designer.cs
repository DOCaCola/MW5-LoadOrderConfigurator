using System.Drawing;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    partial class DockOverviewForm
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
            pictureBoxModImage = new PictureBox();
            panelModInfo = new Panel();
            splitContainerVersion = new SplitContainer();
            labelModVersion = new Label();
            labelModBuildNumber = new Label();
            pictureBoxNexusmodsIcon = new PictureBox();
            labelNexusmods = new Label();
            linkLabelNexusmods = new LinkLabel();
            pictureBoxSteamIcon = new PictureBox();
            label1 = new Label();
            richTextBoxModDescription = new RichTextBox();
            labelSteamId = new Label();
            linkLabelSteamId = new LinkLabel();
            linkLabelModAuthorUrl = new LinkLabel();
            labelModAuthor = new Label();
            labelModName = new Label();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).BeginInit();
            panelModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerVersion).BeginInit();
            splitContainerVersion.Panel1.SuspendLayout();
            splitContainerVersion.Panel2.SuspendLayout();
            splitContainerVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxModImage
            // 
            pictureBoxModImage.Dock = DockStyle.Fill;
            pictureBoxModImage.Location = new Point(0, 0);
            pictureBoxModImage.MaximumSize = new Size(10000, 500);
            pictureBoxModImage.Name = "pictureBoxModImage";
            pictureBoxModImage.Size = new Size(320, 130);
            pictureBoxModImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxModImage.TabIndex = 2;
            pictureBoxModImage.TabStop = false;
            // 
            // panelModInfo
            // 
            panelModInfo.Controls.Add(splitContainerVersion);
            panelModInfo.Controls.Add(pictureBoxNexusmodsIcon);
            panelModInfo.Controls.Add(labelNexusmods);
            panelModInfo.Controls.Add(linkLabelNexusmods);
            panelModInfo.Controls.Add(pictureBoxSteamIcon);
            panelModInfo.Controls.Add(label1);
            panelModInfo.Controls.Add(richTextBoxModDescription);
            panelModInfo.Controls.Add(labelSteamId);
            panelModInfo.Controls.Add(linkLabelSteamId);
            panelModInfo.Controls.Add(linkLabelModAuthorUrl);
            panelModInfo.Controls.Add(labelModAuthor);
            panelModInfo.Controls.Add(labelModName);
            panelModInfo.Dock = DockStyle.Fill;
            panelModInfo.Location = new Point(0, 0);
            panelModInfo.Margin = new Padding(0);
            panelModInfo.Name = "panelModInfo";
            panelModInfo.Size = new Size(320, 316);
            panelModInfo.TabIndex = 1;
            panelModInfo.Visible = false;
            // 
            // splitContainerVersion
            // 
            splitContainerVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerVersion.IsSplitterFixed = true;
            splitContainerVersion.Location = new Point(5, 62);
            splitContainerVersion.Name = "splitContainerVersion";
            // 
            // splitContainerVersion.Panel1
            // 
            splitContainerVersion.Panel1.Controls.Add(labelModVersion);
            // 
            // splitContainerVersion.Panel2
            // 
            splitContainerVersion.Panel2.Controls.Add(labelModBuildNumber);
            splitContainerVersion.Size = new Size(308, 23);
            splitContainerVersion.SplitterDistance = 149;
            splitContainerVersion.SplitterWidth = 1;
            splitContainerVersion.TabIndex = 15;
            splitContainerVersion.TabStop = false;
            // 
            // labelModVersion
            // 
            labelModVersion.AutoEllipsis = true;
            labelModVersion.Dock = DockStyle.Fill;
            labelModVersion.Location = new Point(0, 0);
            labelModVersion.Margin = new Padding(0);
            labelModVersion.Name = "labelModVersion";
            labelModVersion.Size = new Size(149, 23);
            labelModVersion.TabIndex = 3;
            labelModVersion.Text = "labelModVersion";
            // 
            // labelModBuildNumber
            // 
            labelModBuildNumber.AutoEllipsis = true;
            labelModBuildNumber.Dock = DockStyle.Fill;
            labelModBuildNumber.Location = new Point(0, 0);
            labelModBuildNumber.Name = "labelModBuildNumber";
            labelModBuildNumber.Size = new Size(158, 23);
            labelModBuildNumber.TabIndex = 4;
            labelModBuildNumber.Text = "labelModBuildNumber";
            // 
            // pictureBoxNexusmodsIcon
            // 
            pictureBoxNexusmodsIcon.Image = UiIcons.Nexusmods;
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
            labelNexusmods.Size = new Size(87, 15);
            labelNexusmods.TabIndex = 13;
            labelNexusmods.Text = "Nexusmods ID:";
            // 
            // linkLabelNexusmods
            // 
            linkLabelNexusmods.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkLabelNexusmods.AutoEllipsis = true;
            linkLabelNexusmods.Location = new Point(111, 126);
            linkLabelNexusmods.Name = "linkLabelNexusmods";
            linkLabelNexusmods.Size = new Size(205, 14);
            linkLabelNexusmods.TabIndex = 12;
            linkLabelNexusmods.TabStop = true;
            linkLabelNexusmods.Text = "linkLabelNexusmods";
            linkLabelNexusmods.LinkClicked += linkLabelNexusmods_LinkClicked;
            // 
            // pictureBoxSteamIcon
            // 
            pictureBoxSteamIcon.Image = UiIcons.Steam;
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
            label1.Size = new Size(70, 15);
            label1.TabIndex = 10;
            label1.Text = "Description:";
            // 
            // richTextBoxModDescription
            // 
            richTextBoxModDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxModDescription.Location = new Point(8, 182);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new Size(308, 127);
            richTextBoxModDescription.TabIndex = 9;
            richTextBoxModDescription.Text = "";
            richTextBoxModDescription.LinkClicked += richTextBoxModDescription_LinkClicked;
            // 
            // labelSteamId
            // 
            labelSteamId.AutoSize = true;
            labelSteamId.Location = new Point(26, 103);
            labelSteamId.Name = "labelSteamId";
            labelSteamId.Size = new Size(57, 15);
            labelSteamId.TabIndex = 8;
            labelSteamId.Text = "Steam ID:";
            // 
            // linkLabelSteamId
            // 
            linkLabelSteamId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkLabelSteamId.AutoEllipsis = true;
            linkLabelSteamId.Location = new Point(111, 103);
            linkLabelSteamId.Name = "linkLabelSteamId";
            linkLabelSteamId.Size = new Size(205, 15);
            linkLabelSteamId.TabIndex = 7;
            linkLabelSteamId.TabStop = true;
            linkLabelSteamId.Text = "linkLabelSteamId";
            linkLabelSteamId.LinkClicked += linkLabelSteamId_LinkClicked;
            // 
            // linkLabelModAuthorUrl
            // 
            linkLabelModAuthorUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkLabelModAuthorUrl.AutoEllipsis = true;
            linkLabelModAuthorUrl.Location = new Point(5, 43);
            linkLabelModAuthorUrl.Name = "linkLabelModAuthorUrl";
            linkLabelModAuthorUrl.Size = new Size(311, 16);
            linkLabelModAuthorUrl.TabIndex = 6;
            linkLabelModAuthorUrl.TabStop = true;
            linkLabelModAuthorUrl.Text = "linkLabel1";
            linkLabelModAuthorUrl.LinkClicked += linkLabelModAuthorUrl_LinkClicked;
            // 
            // labelModAuthor
            // 
            labelModAuthor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelModAuthor.AutoEllipsis = true;
            labelModAuthor.Location = new Point(5, 28);
            labelModAuthor.Name = "labelModAuthor";
            labelModAuthor.Size = new Size(311, 15);
            labelModAuthor.TabIndex = 2;
            labelModAuthor.Text = "labelModAuthor";
            // 
            // labelModName
            // 
            labelModName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelModName.AutoEllipsis = true;
            labelModName.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModName.Location = new Point(5, 5);
            labelModName.Name = "labelModName";
            labelModName.Size = new Size(311, 23);
            labelModName.TabIndex = 1;
            labelModName.Text = "labelModName";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(pictureBoxModImage);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panelModInfo);
            splitContainer1.Size = new Size(320, 450);
            splitContainer1.SplitterDistance = 130;
            splitContainer1.TabIndex = 3;
            splitContainer1.TabStop = false;
            // 
            // DockOverviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 450);
            Controls.Add(splitContainer1);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom;
            HideOnClose = true;
            Name = "DockOverviewForm";
            Text = "Overview";
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).EndInit();
            panelModInfo.ResumeLayout(false);
            panelModInfo.PerformLayout();
            splitContainerVersion.Panel1.ResumeLayout(false);
            splitContainerVersion.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerVersion).EndInit();
            splitContainerVersion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBoxModImage;
        public System.Windows.Forms.Panel panelModInfo;
        public System.Windows.Forms.PictureBox pictureBoxNexusmodsIcon;
        public System.Windows.Forms.Label labelNexusmods;
        public System.Windows.Forms.LinkLabel linkLabelNexusmods;
        public System.Windows.Forms.PictureBox pictureBoxSteamIcon;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.RichTextBox richTextBoxModDescription;
        public System.Windows.Forms.Label labelSteamId;
        public System.Windows.Forms.LinkLabel linkLabelSteamId;
        public System.Windows.Forms.LinkLabel linkLabelModAuthorUrl;
        public System.Windows.Forms.Label labelModBuildNumber;
        public System.Windows.Forms.Label labelModVersion;
        public System.Windows.Forms.Label labelModAuthor;
        public System.Windows.Forms.Label labelModName;
        public SplitContainer splitContainerVersion;
        private SplitContainer splitContainer1;
    }
}