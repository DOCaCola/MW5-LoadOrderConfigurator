namespace MW5_Mod_Manager
{
    partial class DockConflictsForm
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
            labelModNameOverrides = new System.Windows.Forms.Label();
            listBoxOverriding = new System.Windows.Forms.ListBox();
            label6 = new System.Windows.Forms.Label();
            listBoxOverriddenBy = new System.Windows.Forms.ListBox();
            label5 = new System.Windows.Forms.Label();
            richTextBoxManifestOverridden = new System.Windows.Forms.RichTextBox();
            label7 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // labelModNameOverrides
            // 
            labelModNameOverrides.AutoSize = true;
            labelModNameOverrides.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelModNameOverrides.Location = new System.Drawing.Point(9, 11);
            labelModNameOverrides.Name = "labelModNameOverrides";
            labelModNameOverrides.Size = new System.Drawing.Size(22, 15);
            labelModNameOverrides.TabIndex = 29;
            labelModNameOverrides.Text = "---";
            // 
            // listBoxOverriding
            // 
            listBoxOverriding.Dock = System.Windows.Forms.DockStyle.Bottom;
            listBoxOverriding.FormattingEnabled = true;
            listBoxOverriding.HorizontalScrollbar = true;
            listBoxOverriding.ItemHeight = 15;
            listBoxOverriding.Location = new System.Drawing.Point(0, 292);
            listBoxOverriding.Name = "listBoxOverriding";
            listBoxOverriding.Size = new System.Drawing.Size(800, 79);
            listBoxOverriding.TabIndex = 30;
            listBoxOverriding.SelectedIndexChanged += listBoxOverriding_SelectedIndexChanged;
            listBoxOverriding.MouseDoubleClick += listBoxOverriding_MouseDoubleClick;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(0, 1);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(75, 15);
            label6.TabIndex = 33;
            label6.Text = "Is overriding:";
            // 
            // listBoxOverriddenBy
            // 
            listBoxOverriddenBy.Dock = System.Windows.Forms.DockStyle.Bottom;
            listBoxOverriddenBy.FormattingEnabled = true;
            listBoxOverriddenBy.HorizontalScrollbar = true;
            listBoxOverriddenBy.ItemHeight = 15;
            listBoxOverriddenBy.Location = new System.Drawing.Point(0, 371);
            listBoxOverriddenBy.Name = "listBoxOverriddenBy";
            listBoxOverriddenBy.Size = new System.Drawing.Size(800, 79);
            listBoxOverriddenBy.TabIndex = 31;
            listBoxOverriddenBy.SelectedIndexChanged += listBoxOverriddenBy_SelectedIndexChanged;
            listBoxOverriddenBy.MouseDoubleClick += listBoxOverriddenBy_MouseDoubleClick;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(0, 1);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(94, 15);
            label5.TabIndex = 32;
            label5.Text = "Is overridden by:";
            // 
            // richTextBoxManifestOverridden
            // 
            richTextBoxManifestOverridden.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxManifestOverridden.Location = new System.Drawing.Point(9, 163);
            richTextBoxManifestOverridden.Name = "richTextBoxManifestOverridden";
            richTextBoxManifestOverridden.ReadOnly = true;
            richTextBoxManifestOverridden.Size = new System.Drawing.Size(225, 246);
            richTextBoxManifestOverridden.TabIndex = 35;
            richTextBoxManifestOverridden.Text = "";
            richTextBoxManifestOverridden.WordWrap = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(9, 147);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(127, 15);
            label7.TabIndex = 34;
            label7.Text = "Affected mod content:";
            // 
            // DockConflictsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(labelModNameOverrides);
            Controls.Add(listBoxOverriding);
            Controls.Add(label6);
            Controls.Add(listBoxOverriddenBy);
            Controls.Add(label5);
            Controls.Add(richTextBoxManifestOverridden);
            Controls.Add(label7);
            Name = "DockConflictsForm";
            Text = "DockConflictsForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public System.Windows.Forms.Label labelModNameOverrides;
        public System.Windows.Forms.ListBox listBoxOverriding;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ListBox listBoxOverriddenBy;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.RichTextBox richTextBoxManifestOverridden;
        private System.Windows.Forms.Label label7;
    }
}