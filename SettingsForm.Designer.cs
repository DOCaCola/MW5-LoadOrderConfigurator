namespace MW5_Mod_Manager
{
    partial class SettingsForm
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
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            tabPage2 = new System.Windows.Forms.TabPage();
            groupBox3 = new System.Windows.Forms.GroupBox();
            checkBoxDarkMode = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            radioButtonLowToHigh = new System.Windows.Forms.RadioButton();
            radioButtonHighToLow = new System.Windows.Forms.RadioButton();
            groupBox1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(comboBoxPlatform);
            groupBox1.Controls.Add(buttonBrowse);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBoxMw5Path);
            groupBox1.Location = new System.Drawing.Point(6, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(550, 90);
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
            label2.TabIndex = 0;
            label2.Text = "&Platform:";
            // 
            // comboBoxPlatform
            // 
            comboBoxPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxPlatform.FormattingEnabled = true;
            comboBoxPlatform.Items.AddRange(new object[] { "Epic Games Store", "GOG.com", "Steam", "Microsoft Store/Xbox Game Pass", "Other" });
            comboBoxPlatform.Location = new System.Drawing.Point(110, 22);
            comboBoxPlatform.Name = "comboBoxPlatform";
            comboBoxPlatform.Size = new System.Drawing.Size(205, 23);
            comboBoxPlatform.TabIndex = 1;
            comboBoxPlatform.SelectedIndexChanged += comboBoxPlatform_SelectedIndexChanged;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Location = new System.Drawing.Point(464, 50);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new System.Drawing.Size(75, 25);
            buttonBrowse.TabIndex = 4;
            buttonBrowse.Text = "Se&lect...";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonSelect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(5, 54);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(99, 15);
            label1.TabIndex = 2;
            label1.Text = "MW5 &Install Path:";
            // 
            // textBoxMw5Path
            // 
            textBoxMw5Path.Location = new System.Drawing.Point(110, 51);
            textBoxMw5Path.Name = "textBoxMw5Path";
            textBoxMw5Path.Size = new System.Drawing.Size(348, 23);
            textBoxMw5Path.TabIndex = 3;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonSave.Location = new System.Drawing.Point(12, 197);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(88, 26);
            buttonSave.TabIndex = 4;
            buttonSave.Text = "&Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCancel.Location = new System.Drawing.Point(106, 197);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 26);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(571, 167);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(563, 139);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Main";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox3);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(563, 139);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Interface";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(checkBoxDarkMode);
            groupBox3.Location = new System.Drawing.Point(6, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(551, 64);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "General";
            // 
            // checkBoxDarkMode
            // 
            checkBoxDarkMode.AutoSize = true;
            checkBoxDarkMode.Location = new System.Drawing.Point(17, 22);
            checkBoxDarkMode.Name = "checkBoxDarkMode";
            checkBoxDarkMode.Size = new System.Drawing.Size(307, 19);
            checkBoxDarkMode.TabIndex = 0;
            checkBoxDarkMode.Text = "Enable Windows &Dark Mode support (requires restart)";
            checkBoxDarkMode.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(radioButtonLowToHigh);
            groupBox2.Controls.Add(radioButtonHighToLow);
            groupBox2.Location = new System.Drawing.Point(6, 76);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(551, 57);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Mod list";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(17, 25);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(157, 15);
            label3.TabIndex = 0;
            label3.Text = "Load &order display direction:";
            // 
            // radioButtonLowToHigh
            // 
            radioButtonLowToHigh.AutoSize = true;
            radioButtonLowToHigh.Location = new System.Drawing.Point(303, 23);
            radioButtonLowToHigh.Name = "radioButtonLowToHigh";
            radioButtonLowToHigh.Size = new System.Drawing.Size(118, 19);
            radioButtonLowToHigh.TabIndex = 2;
            radioButtonLowToHigh.Text = "Lowest to highest";
            radioButtonLowToHigh.UseVisualStyleBackColor = true;
            // 
            // radioButtonHighToLow
            // 
            radioButtonHighToLow.AutoSize = true;
            radioButtonHighToLow.Checked = true;
            radioButtonHighToLow.Location = new System.Drawing.Point(180, 23);
            radioButtonHighToLow.Name = "radioButtonHighToLow";
            radioButtonHighToLow.Size = new System.Drawing.Size(117, 19);
            radioButtonHighToLow.TabIndex = 1;
            radioButtonHighToLow.TabStop = true;
            radioButtonHighToLow.Text = "Highest to lowest";
            radioButtonHighToLow.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AcceptButton = buttonSave;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(592, 232);
            Controls.Add(tabControl1);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Settings";
            Load += SettingsWindow_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonLowToHigh;
        private System.Windows.Forms.RadioButton radioButtonHighToLow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxDarkMode;
    }
}