namespace MW5_Mod_Manager
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            textBoxData = new System.Windows.Forms.TextBox();
            buttonCancel = new System.Windows.Forms.Button();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripButtonSaveToFile = new System.Windows.Forms.ToolStripButton();
            toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxData
            // 
            textBoxData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxData.Location = new System.Drawing.Point(12, 37);
            textBoxData.Multiline = true;
            textBoxData.Name = "textBoxData";
            textBoxData.ReadOnly = true;
            textBoxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxData.Size = new System.Drawing.Size(570, 196);
            textBoxData.TabIndex = 4;
            textBoxData.WordWrap = false;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(12, 243);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "C&lose";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = System.Drawing.Color.Transparent;
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonSaveToFile, toolStripButtonCopy });
            toolStrip1.Location = new System.Drawing.Point(12, 9);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(151, 25);
            toolStrip1.TabIndex = 6;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSaveToFile
            // 
            toolStripButtonSaveToFile.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonSaveToFile.Image");
            toolStripButtonSaveToFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonSaveToFile.Name = "toolStripButtonSaveToFile";
            toolStripButtonSaveToFile.Size = new System.Drawing.Size(93, 22);
            toolStripButtonSaveToFile.Text = "&Save to file...";
            toolStripButtonSaveToFile.ToolTipText = "Save load order to file";
            toolStripButtonSaveToFile.Click += ToolStripButtonSaveToFileClick;
            // 
            // toolStripButtonCopy
            // 
            toolStripButtonCopy.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonCopy.Image");
            toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonCopy.Name = "toolStripButtonCopy";
            toolStripButtonCopy.Size = new System.Drawing.Size(55, 22);
            toolStripButtonCopy.Text = "&Copy";
            toolStripButtonCopy.ToolTipText = "Copy to clipboard";
            toolStripButtonCopy.Click += toolStripButtonCopy_Click;
            // 
            // ExportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(594, 281);
            Controls.Add(toolStrip1);
            Controls.Add(buttonCancel);
            Controls.Add(textBoxData);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(388, 259);
            Name = "ExportForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Export load order";
            Load += ExportWindow_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveToFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
    }
}