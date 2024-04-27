namespace MW5_Mod_Manager
{
    partial class SettingsWindow
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            label2 = new System.Windows.Forms.Label();
            comboBoxPlatform = new System.Windows.Forms.ComboBox();
            buttonBrowse = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            textBoxMw5Path = new System.Windows.Forms.TextBox();
            buttonSave = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(comboBoxPlatform);
            groupBox1.Controls.Add(buttonBrowse);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBoxMw5Path);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(629, 90);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Path";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 22);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(56, 15);
            label2.TabIndex = 7;
            label2.Text = "Platform:";
            // 
            // comboBoxPlatform
            // 
            comboBoxPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxPlatform.FormattingEnabled = true;
            comboBoxPlatform.Items.AddRange(new object[] { "Other", "Epic Store", "GOG", "Steam", "Windows Store" });
            comboBoxPlatform.Location = new System.Drawing.Point(76, 22);
            comboBoxPlatform.Name = "comboBoxPlatform";
            comboBoxPlatform.Size = new System.Drawing.Size(121, 23);
            comboBoxPlatform.TabIndex = 1;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Location = new System.Drawing.Point(543, 51);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new System.Drawing.Size(75, 23);
            buttonBrowse.TabIndex = 3;
            buttonBrowse.Text = "&Browse";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(5, 54);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 4;
            label1.Text = "MW5 Path:";
            // 
            // textBoxMw5Path
            // 
            textBoxMw5Path.Location = new System.Drawing.Point(76, 51);
            textBoxMw5Path.Name = "textBoxMw5Path";
            textBoxMw5Path.Size = new System.Drawing.Size(461, 23);
            textBoxMw5Path.TabIndex = 2;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonSave.Location = new System.Drawing.Point(12, 114);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 4;
            buttonSave.Text = "&Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(93, 114);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // SettingsWindow
            // 
            AcceptButton = buttonSave;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(653, 149);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Settings";
            Load += SettingsWindow_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxPlatform;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMw5Path;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}