namespace MW5_Mod_Manager
{
    partial class PresetDeleteForm
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
            listBoxPresets = new System.Windows.Forms.ListBox();
            buttonDelete = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // listBoxPresets
            // 
            listBoxPresets.FormattingEnabled = true;
            listBoxPresets.ItemHeight = 15;
            listBoxPresets.Location = new System.Drawing.Point(12, 12);
            listBoxPresets.Name = "listBoxPresets";
            listBoxPresets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBoxPresets.Size = new System.Drawing.Size(196, 154);
            listBoxPresets.TabIndex = 0;
            listBoxPresets.SelectedIndexChanged += listBoxPresets_SelectedIndexChanged;
            // 
            // buttonDelete
            // 
            buttonDelete.Enabled = false;
            buttonDelete.Location = new System.Drawing.Point(12, 184);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new System.Drawing.Size(75, 23);
            buttonDelete.TabIndex = 1;
            buttonDelete.Text = "&Delete";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += buttonDelete_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(133, 184);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // PresetDeleteWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(221, 220);
            Controls.Add(buttonCancel);
            Controls.Add(buttonDelete);
            Controls.Add(listBoxPresets);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PresetDeleteWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Delete Preset";
            Load += PresetDeleteWindow_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox listBoxPresets;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
    }
}