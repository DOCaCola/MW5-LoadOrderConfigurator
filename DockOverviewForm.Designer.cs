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
            pictureBoxModImage = new System.Windows.Forms.PictureBox();
            panelModInfo = new System.Windows.Forms.Panel();
            pictureBoxNexusmodsIcon = new System.Windows.Forms.PictureBox();
            labelNexusmods = new System.Windows.Forms.Label();
            linkLabelNexusmods = new System.Windows.Forms.LinkLabel();
            pictureBoxSteamIcon = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            richTextBoxModDescription = new System.Windows.Forms.RichTextBox();
            labelSteamId = new System.Windows.Forms.Label();
            linkLabelSteamId = new System.Windows.Forms.LinkLabel();
            linkLabelModAuthorUrl = new System.Windows.Forms.LinkLabel();
            labelModBuildNumber = new System.Windows.Forms.Label();
            labelModVersion = new System.Windows.Forms.Label();
            labelModAuthor = new System.Windows.Forms.Label();
            labelModName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).BeginInit();
            panelModInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxModImage
            // 
            pictureBoxModImage.Dock = System.Windows.Forms.DockStyle.Top;
            pictureBoxModImage.Location = new System.Drawing.Point(0, 0);
            pictureBoxModImage.Margin = new System.Windows.Forms.Padding(0);
            pictureBoxModImage.Name = "pictureBoxModImage";
            pictureBoxModImage.Size = new System.Drawing.Size(320, 100);
            pictureBoxModImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBoxModImage.TabIndex = 3;
            pictureBoxModImage.TabStop = false;
            // 
            // panelModInfo
            // 
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
            panelModInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            panelModInfo.Location = new System.Drawing.Point(0, 100);
            panelModInfo.Margin = new System.Windows.Forms.Padding(0);
            panelModInfo.Name = "panelModInfo";
            panelModInfo.Size = new System.Drawing.Size(320, 350);
            panelModInfo.TabIndex = 4;
            panelModInfo.Visible = false;
            // 
            // pictureBoxNexusmodsIcon
            // 
            pictureBoxNexusmodsIcon.Image = UiIcons.Nexusmods;
            pictureBoxNexusmodsIcon.Location = new System.Drawing.Point(8, 123);
            pictureBoxNexusmodsIcon.Name = "pictureBoxNexusmodsIcon";
            pictureBoxNexusmodsIcon.Size = new System.Drawing.Size(16, 16);
            pictureBoxNexusmodsIcon.TabIndex = 14;
            pictureBoxNexusmodsIcon.TabStop = false;
            // 
            // labelNexusmods
            // 
            labelNexusmods.AutoSize = true;
            labelNexusmods.Location = new System.Drawing.Point(26, 125);
            labelNexusmods.Name = "labelNexusmods";
            labelNexusmods.Size = new System.Drawing.Size(87, 15);
            labelNexusmods.TabIndex = 13;
            labelNexusmods.Text = "Nexusmods ID:";
            // 
            // linkLabelNexusmods
            // 
            linkLabelNexusmods.AutoSize = true;
            linkLabelNexusmods.Location = new System.Drawing.Point(111, 126);
            linkLabelNexusmods.Name = "linkLabelNexusmods";
            linkLabelNexusmods.Size = new System.Drawing.Size(117, 15);
            linkLabelNexusmods.TabIndex = 12;
            linkLabelNexusmods.TabStop = true;
            linkLabelNexusmods.Text = "linkLabelNexusmods";
            linkLabelNexusmods.LinkClicked += linkLabelNexusmods_LinkClicked;
            // 
            // pictureBoxSteamIcon
            // 
            pictureBoxSteamIcon.Image = UiIcons.Steam;
            pictureBoxSteamIcon.Location = new System.Drawing.Point(8, 101);
            pictureBoxSteamIcon.Name = "pictureBoxSteamIcon";
            pictureBoxSteamIcon.Size = new System.Drawing.Size(16, 16);
            pictureBoxSteamIcon.TabIndex = 11;
            pictureBoxSteamIcon.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(5, 163);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 15);
            label1.TabIndex = 10;
            label1.Text = "Description:";
            // 
            // richTextBoxModDescription
            // 
            richTextBoxModDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxModDescription.Location = new System.Drawing.Point(8, 182);
            richTextBoxModDescription.Name = "richTextBoxModDescription";
            richTextBoxModDescription.ReadOnly = true;
            richTextBoxModDescription.Size = new System.Drawing.Size(309, 157);
            richTextBoxModDescription.TabIndex = 9;
            richTextBoxModDescription.Text = "";
            // 
            // labelSteamId
            // 
            labelSteamId.AutoSize = true;
            labelSteamId.Location = new System.Drawing.Point(26, 103);
            labelSteamId.Name = "labelSteamId";
            labelSteamId.Size = new System.Drawing.Size(57, 15);
            labelSteamId.TabIndex = 8;
            labelSteamId.Text = "Steam ID:";
            // 
            // linkLabelSteamId
            // 
            linkLabelSteamId.AutoSize = true;
            linkLabelSteamId.Location = new System.Drawing.Point(111, 103);
            linkLabelSteamId.Name = "linkLabelSteamId";
            linkLabelSteamId.Size = new System.Drawing.Size(97, 15);
            linkLabelSteamId.TabIndex = 7;
            linkLabelSteamId.TabStop = true;
            linkLabelSteamId.Text = "linkLabelSteamId";
            linkLabelSteamId.LinkClicked += linkLabelSteamId_LinkClicked;
            // 
            // linkLabelModAuthorUrl
            // 
            linkLabelModAuthorUrl.AutoEllipsis = true;
            linkLabelModAuthorUrl.Location = new System.Drawing.Point(5, 46);
            linkLabelModAuthorUrl.Name = "linkLabelModAuthorUrl";
            linkLabelModAuthorUrl.Size = new System.Drawing.Size(313, 13);
            linkLabelModAuthorUrl.TabIndex = 6;
            linkLabelModAuthorUrl.TabStop = true;
            linkLabelModAuthorUrl.Text = "linkLabel1";
            linkLabelModAuthorUrl.LinkClicked += linkLabelModAuthorUrl_LinkClicked;
            // 
            // labelModBuildNumber
            // 
            labelModBuildNumber.AutoSize = true;
            labelModBuildNumber.Location = new System.Drawing.Point(147, 73);
            labelModBuildNumber.Name = "labelModBuildNumber";
            labelModBuildNumber.Size = new System.Drawing.Size(128, 15);
            labelModBuildNumber.TabIndex = 4;
            labelModBuildNumber.Text = "labelModBuildNumber";
            // 
            // labelModVersion
            // 
            labelModVersion.AutoSize = true;
            labelModVersion.Location = new System.Drawing.Point(5, 73);
            labelModVersion.Name = "labelModVersion";
            labelModVersion.Size = new System.Drawing.Size(95, 15);
            labelModVersion.TabIndex = 3;
            labelModVersion.Text = "labelModVersion";
            // 
            // labelModAuthor
            // 
            labelModAuthor.AutoSize = true;
            labelModAuthor.Location = new System.Drawing.Point(5, 28);
            labelModAuthor.Name = "labelModAuthor";
            labelModAuthor.Size = new System.Drawing.Size(94, 15);
            labelModAuthor.TabIndex = 2;
            labelModAuthor.Text = "labelModAuthor";
            // 
            // labelModName
            // 
            labelModName.AutoSize = true;
            labelModName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelModName.Location = new System.Drawing.Point(5, 8);
            labelModName.Name = "labelModName";
            labelModName.Size = new System.Drawing.Size(91, 15);
            labelModName.TabIndex = 1;
            labelModName.Text = "labelModName";
            // 
            // DockOverviewForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(320, 450);
            Controls.Add(panelModInfo);
            Controls.Add(pictureBoxModImage);
            Name = "DockOverviewForm";
            Text = "DockOverviewForm";
            ((System.ComponentModel.ISupportInitialize)pictureBoxModImage).EndInit();
            panelModInfo.ResumeLayout(false);
            panelModInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxNexusmodsIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSteamIcon).EndInit();
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
    }
}