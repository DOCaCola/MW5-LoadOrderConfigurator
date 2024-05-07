namespace MW5_Mod_Manager
{
    partial class ImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
            textBoxData = new System.Windows.Forms.TextBox();
            buttonImport = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripButtonLoadFromFile = new System.Windows.Forms.ToolStripButton();
            toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxData
            // 
            textBoxData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxData.Location = new System.Drawing.Point(12, 37);
            textBoxData.Multiline = true;
            textBoxData.Name = "textBoxData";
            textBoxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxData.Size = new System.Drawing.Size(570, 196);
            textBoxData.TabIndex = 0;
            textBoxData.TextChanged += textBoxData_TextChanged;
            // 
            // buttonImport
            // 
            buttonImport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonImport.Enabled = false;
            buttonImport.Location = new System.Drawing.Point(12, 243);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new System.Drawing.Size(88, 26);
            buttonImport.TabIndex = 1;
            buttonImport.Text = "&Import";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.Click += buttonImport_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(106, 243);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "&Close";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = System.Drawing.Color.Transparent;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonLoadFromFile, toolStripButtonPaste });
            toolStrip1.Location = new System.Drawing.Point(12, 9);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(199, 25);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonLoadFromFile
            // 
            toolStripButtonLoadFromFile.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonLoadFromFile.Image");
            toolStripButtonLoadFromFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonLoadFromFile.Name = "toolStripButtonLoadFromFile";
            toolStripButtonLoadFromFile.Size = new System.Drawing.Size(110, 22);
            toolStripButtonLoadFromFile.Text = "&Load from file...";
            toolStripButtonLoadFromFile.ToolTipText = "Open load order from file";
            toolStripButtonLoadFromFile.Click += toolStripButtonLoadFromFile_Click;
            // 
            // toolStripButtonPaste
            // 
            toolStripButtonPaste.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonPaste.Image");
            toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonPaste.Name = "toolStripButtonPaste";
            toolStripButtonPaste.Size = new System.Drawing.Size(55, 22);
            toolStripButtonPaste.Text = "&Paste";
            toolStripButtonPaste.ToolTipText = "Paste load order from clipboard";
            toolStripButtonPaste.Click += toolStripButtonPaste_Click;
            // 
            // ImportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(594, 281);
            Controls.Add(toolStrip1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonImport);
            Controls.Add(textBoxData);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(388, 259);
            Name = "ImportForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = " Import";
            Load += ImportForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
    }
}