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
            textBoxData = new System.Windows.Forms.TextBox();
            buttonCopy = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBoxData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxData.Location = new System.Drawing.Point(12, 12);
            textBoxData.Multiline = true;
            textBoxData.Name = "textBoxData";
            textBoxData.ReadOnly = true;
            textBoxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxData.Size = new System.Drawing.Size(570, 208);
            textBoxData.TabIndex = 4;
            textBoxData.WordWrap = false;
            // 
            // buttonCopy
            // 
            buttonCopy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCopy.Location = new System.Drawing.Point(12, 237);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new System.Drawing.Size(88, 26);
            buttonCopy.TabIndex = 1;
            buttonCopy.Text = "Co&py";
            buttonCopy.UseVisualStyleBackColor = true;
            buttonCopy.Click += buttonCopy_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(108, 237);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "&Close";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // ExportForm
            // 
            AcceptButton = buttonCopy;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(594, 281);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCopy);
            Controls.Add(textBoxData);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(388, 259);
            Name = "ExportForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Export";
            Load += ExportWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button buttonCopy;
        public System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.Button buttonCancel;
    }
}