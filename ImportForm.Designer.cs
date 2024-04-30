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
            textBoxData = new System.Windows.Forms.TextBox();
            buttonImport = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            buttonCancel = new System.Windows.Forms.Button();
            buttonPaste = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // textBoxData
            // 
            textBoxData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxData.Location = new System.Drawing.Point(12, 47);
            textBoxData.Multiline = true;
            textBoxData.Name = "textBoxData";
            textBoxData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxData.Size = new System.Drawing.Size(348, 161);
            textBoxData.TabIndex = 0;
            textBoxData.TextChanged += textBoxData_TextChanged;
            // 
            // buttonImport
            // 
            buttonImport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonImport.Enabled = false;
            buttonImport.Location = new System.Drawing.Point(12, 224);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new System.Drawing.Size(88, 26);
            buttonImport.TabIndex = 1;
            buttonImport.Text = "&Import";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.Click += button1_Click;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(14, 11);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(346, 33);
            label1.TabIndex = 2;
            label1.Text = "Paste load order from clipboard into this text box. The data needs to be in valid JSON format.";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(200, 224);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "&Close";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonPaste
            // 
            buttonPaste.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonPaste.Location = new System.Drawing.Point(106, 224);
            buttonPaste.Name = "buttonPaste";
            buttonPaste.Size = new System.Drawing.Size(88, 26);
            buttonPaste.TabIndex = 2;
            buttonPaste.Text = "&Paste";
            buttonPaste.UseVisualStyleBackColor = true;
            buttonPaste.Click += buttonPaste_Click;
            // 
            // ImportWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(372, 262);
            Controls.Add(buttonPaste);
            Controls.Add(buttonCancel);
            Controls.Add(label1);
            Controls.Add(buttonImport);
            Controls.Add(textBoxData);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(388, 259);
            Name = "ImportWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = " Import";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonPaste;
    }
}