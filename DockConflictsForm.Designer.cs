using System.Drawing;
using System.Windows.Forms;

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
            labelModNameOverrides = new Label();
            listBoxOverriding = new ListBox();
            label6 = new Label();
            listBoxOverriddenBy = new ListBox();
            label5 = new Label();
            richTextBoxManifestOverridden = new RichTextBox();
            label7 = new Label();
            splitContainer2 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // labelModNameOverrides
            // 
            labelModNameOverrides.AutoSize = true;
            labelModNameOverrides.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelModNameOverrides.Location = new Point(9, 11);
            labelModNameOverrides.Name = "labelModNameOverrides";
            labelModNameOverrides.Size = new Size(22, 15);
            labelModNameOverrides.TabIndex = 29;
            labelModNameOverrides.Text = "---";
            // 
            // listBoxOverriding
            // 
            listBoxOverriding.Dock = DockStyle.Bottom;
            listBoxOverriding.FormattingEnabled = true;
            listBoxOverriding.HorizontalScrollbar = true;
            listBoxOverriding.ItemHeight = 15;
            listBoxOverriding.Location = new Point(0, 21);
            listBoxOverriding.Name = "listBoxOverriding";
            listBoxOverriding.Size = new Size(121, 79);
            listBoxOverriding.TabIndex = 0;
            listBoxOverriding.SelectedIndexChanged += listBoxOverriding_SelectedIndexChanged;
            listBoxOverriding.MouseDoubleClick += listBoxOverriding_MouseDoubleClick;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(0, 2);
            label6.Name = "label6";
            label6.Size = new Size(75, 15);
            label6.TabIndex = 33;
            label6.Text = "Is overriding:";
            // 
            // listBoxOverriddenBy
            // 
            listBoxOverriddenBy.Dock = DockStyle.Bottom;
            listBoxOverriddenBy.FormattingEnabled = true;
            listBoxOverriddenBy.HorizontalScrollbar = true;
            listBoxOverriddenBy.ItemHeight = 15;
            listBoxOverriddenBy.Location = new Point(0, 21);
            listBoxOverriddenBy.Name = "listBoxOverriddenBy";
            listBoxOverriddenBy.Size = new Size(121, 79);
            listBoxOverriddenBy.TabIndex = 1;
            listBoxOverriddenBy.SelectedIndexChanged += listBoxOverriddenBy_SelectedIndexChanged;
            listBoxOverriddenBy.MouseDoubleClick += listBoxOverriddenBy_MouseDoubleClick;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(0, 2);
            label5.Name = "label5";
            label5.Size = new Size(94, 15);
            label5.TabIndex = 32;
            label5.Text = "Is overridden by:";
            // 
            // richTextBoxManifestOverridden
            // 
            richTextBoxManifestOverridden.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxManifestOverridden.Location = new Point(9, 147);
            richTextBoxManifestOverridden.Name = "richTextBoxManifestOverridden";
            richTextBoxManifestOverridden.ReadOnly = true;
            richTextBoxManifestOverridden.Size = new Size(246, 221);
            richTextBoxManifestOverridden.TabIndex = 2;
            richTextBoxManifestOverridden.Text = "";
            richTextBoxManifestOverridden.WordWrap = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(9, 129);
            label7.Name = "label7";
            label7.Size = new Size(127, 15);
            label7.TabIndex = 34;
            label7.Text = "Affected mod content:";
            // 
            // splitContainer2
            // 
            splitContainer2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new Point(9, 26);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(listBoxOverriding);
            splitContainer2.Panel1.Controls.Add(label6);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(listBoxOverriddenBy);
            splitContainer2.Panel2.Controls.Add(label5);
            splitContainer2.Size = new Size(246, 100);
            splitContainer2.SplitterDistance = 121;
            splitContainer2.TabIndex = 27;
            // 
            // DockConflictsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(261, 380);
            Controls.Add(splitContainer2);
            Controls.Add(labelModNameOverrides);
            Controls.Add(richTextBoxManifestOverridden);
            Controls.Add(label7);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom;
            HideOnClose = true;
            Name = "DockConflictsForm";
            Text = "Conflicts";
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
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
        private SplitContainer splitContainer2;
    }
}