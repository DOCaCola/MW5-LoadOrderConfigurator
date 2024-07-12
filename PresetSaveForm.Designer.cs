namespace MW5_Mod_Manager
{
    partial class PresetSaveForm
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
            radioButton1 = new System.Windows.Forms.RadioButton();
            radioButton2 = new System.Windows.Forms.RadioButton();
            textBoxPresetName = new System.Windows.Forms.TextBox();
            comboBoxPresets = new System.Windows.Forms.ComboBox();
            buttonSave = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new System.Drawing.Point(34, 29);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(158, 19);
            radioButton1.TabIndex = 0;
            radioButton1.Text = "Overwrite existing preset:";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Checked = true;
            radioButton2.Location = new System.Drawing.Point(34, 82);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(122, 19);
            radioButton2.TabIndex = 1;
            radioButton2.TabStop = true;
            radioButton2.Text = "Create new preset:";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // textBoxPresetName
            // 
            textBoxPresetName.Location = new System.Drawing.Point(53, 107);
            textBoxPresetName.Name = "textBoxPresetName";
            textBoxPresetName.Size = new System.Drawing.Size(185, 23);
            textBoxPresetName.TabIndex = 2;
            textBoxPresetName.TextChanged += textBoxPresetName_TextChanged;
            textBoxPresetName.KeyDown += textBoxPresetName_KeyDown;
            // 
            // comboBoxPresets
            // 
            comboBoxPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxPresets.Enabled = false;
            comboBoxPresets.FormattingEnabled = true;
            comboBoxPresets.Location = new System.Drawing.Point(53, 53);
            comboBoxPresets.Name = "comboBoxPresets";
            comboBoxPresets.Size = new System.Drawing.Size(185, 23);
            comboBoxPresets.TabIndex = 3;
            comboBoxPresets.SelectedIndexChanged += comboBoxPresets_SelectedIndexChanged;
            // 
            // buttonSave
            // 
            buttonSave.Enabled = false;
            buttonSave.Location = new System.Drawing.Point(34, 160);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 4;
            buttonSave.Text = "&Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(163, 160);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // PresetSaveForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(280, 208);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(comboBoxPresets);
            Controls.Add(textBoxPresetName);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PresetSaveForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Save Preset";
            Load += PresetSaveWindow_Load;
            Shown += PresetSaveWindow_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox textBoxPresetName;
        private System.Windows.Forms.ComboBox comboBoxPresets;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}