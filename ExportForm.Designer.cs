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
            textBox1 = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            checkBoxEnabledOnly = new System.Windows.Forms.CheckBox();
            buttonCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBox1.Location = new System.Drawing.Point(12, 12);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBox1.Size = new System.Drawing.Size(351, 153);
            textBox1.TabIndex = 4;
            textBox1.WordWrap = false;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button1.Location = new System.Drawing.Point(12, 198);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 26);
            button1.TabIndex = 1;
            button1.Text = "Co&py";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBoxEnabledOnly
            // 
            checkBoxEnabledOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            checkBoxEnabledOnly.AutoSize = true;
            checkBoxEnabledOnly.Checked = true;
            checkBoxEnabledOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxEnabledOnly.Location = new System.Drawing.Point(12, 175);
            checkBoxEnabledOnly.Name = "checkBoxEnabledOnly";
            checkBoxEnabledOnly.Size = new System.Drawing.Size(143, 17);
            checkBoxEnabledOnly.TabIndex = 3;
            checkBoxEnabledOnly.Text = "Exclude disabled mods";
            checkBoxEnabledOnly.UseVisualStyleBackColor = true;
            checkBoxEnabledOnly.CheckedChanged += checkBoxEnabledOnly_CheckedChanged;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(108, 198);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "&Close";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // ExportWindow
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(375, 242);
            Controls.Add(buttonCancel);
            Controls.Add(checkBoxEnabledOnly);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(388, 259);
            Name = "ExportWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Export";
            Load += ExportWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxEnabledOnly;
        private System.Windows.Forms.Button buttonCancel;
    }
}