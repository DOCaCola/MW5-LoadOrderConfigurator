namespace MW5_Mod_Manager
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            buttonClose = new System.Windows.Forms.Button();
            linkLabelNexusmods = new System.Windows.Forms.LinkLabel();
            label3 = new System.Windows.Forms.Label();
            labelVersion = new System.Windows.Forms.Label();
            linkLabelGithub = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(19, 201);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(144, 15);
            label1.TabIndex = 0;
            label1.Text = "Maintained by DOCa Cola";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(19, 219);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(168, 15);
            label2.TabIndex = 1;
            label2.Text = "Based on rjtwins' original code";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(19, 12);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(248, 139);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // buttonClose
            // 
            buttonClose.Location = new System.Drawing.Point(19, 295);
            buttonClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new System.Drawing.Size(88, 26);
            buttonClose.TabIndex = 4;
            buttonClose.Text = "Nice";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += button1_Click;
            // 
            // linkLabelNexusmods
            // 
            linkLabelNexusmods.AutoSize = true;
            linkLabelNexusmods.Location = new System.Drawing.Point(19, 247);
            linkLabelNexusmods.Name = "linkLabelNexusmods";
            linkLabelNexusmods.Size = new System.Drawing.Size(70, 15);
            linkLabelNexusmods.TabIndex = 5;
            linkLabelNexusmods.TabStop = true;
            linkLabelNexusmods.Text = "Nexusmods";
            linkLabelNexusmods.LinkClicked += linkLabelNexusmods_LinkClicked;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label3.Location = new System.Drawing.Point(19, 154);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(230, 15);
            label3.TabIndex = 6;
            label3.Text = "MechWarrior 5 Load Order Configurator";
            // 
            // labelVersion
            // 
            labelVersion.Location = new System.Drawing.Point(19, 173);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(122, 18);
            labelVersion.TabIndex = 7;
            labelVersion.Text = "labelVersion";
            // 
            // linkLabelGithub
            // 
            linkLabelGithub.AutoSize = true;
            linkLabelGithub.Location = new System.Drawing.Point(19, 265);
            linkLabelGithub.Name = "linkLabelGithub";
            linkLabelGithub.Size = new System.Drawing.Size(43, 15);
            linkLabelGithub.TabIndex = 8;
            linkLabelGithub.TabStop = true;
            linkLabelGithub.Text = "Github";
            linkLabelGithub.LinkClicked += linkLabelGithub_LinkClicked;
            // 
            // AboutWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonClose;
            ClientSize = new System.Drawing.Size(287, 344);
            Controls.Add(linkLabelGithub);
            Controls.Add(labelVersion);
            Controls.Add(label3);
            Controls.Add(linkLabelNexusmods);
            Controls.Add(buttonClose);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "About";
            Load += AboutWindow_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.LinkLabel linkLabelNexusmods;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkLabelGithub;
    }
}