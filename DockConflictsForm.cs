using MW5_Mod_Manager.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkModeForms;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5_Mod_Manager
{
    public partial class DockConflictsForm : DockContent
    {
        static public DockConflictsForm Instance;
        private readonly string _defaultManifestLabelText;

        public Panel noneSelectedPanel = new();
        public Label noneSelectedLabel = new();

        public DockConflictsForm()
        {
            InitializeComponent();
            _defaultManifestLabelText = labelManifestContentHeader.Text;

            noneSelectedPanel.Dock = DockStyle.Fill;
            SetNoneSelectedText();
            noneSelectedLabel.TextAlign = ContentAlignment.MiddleCenter;
            noneSelectedLabel.Enabled = false;
            noneSelectedLabel.Dock = DockStyle.Fill;
            noneSelectedPanel.Controls.Add(noneSelectedLabel);
            Controls.Add(noneSelectedPanel);
            noneSelectedPanel.SetDisableDarkMode(true);
            noneSelectedPanel.BringToFront();

            splitContainer2.SetDisableDarkMode(true);
        }

        public void SetNoneSelectedText()
        {
            noneSelectedLabel.Text = "(none selected)";
        }

        public void SetModNotEnabledText()
        {
            noneSelectedLabel.Text = "(mod is disabled)";
        }

        private string GetCurrentModDisplayLabel()
        {
            string modName = MainForm.GetSidebarSelectedModDetails()?.displayName;
            modName ??= "this mod";
            return modName;
        }

        private void SetManifestHeaderOverrideText(string sourceModDisplayName, string targetModDisplayName)
        {
            labelManifestContentHeader.Text = "Content from " + sourceModDisplayName + " overriding " + targetModDisplayName + ":";
        }

        public void ResetManifestHeaderOverrideText()
        {
            labelManifestContentHeader.Text = _defaultManifestLabelText;
        }

        private static bool IsPlaceholderItem(object item)
        {
            return item is ModListBoxItem modItem && modItem.IsPlaceholder;
        }

        public void ShowPlaceholder(ListBox targetListBox)
        {
            if (TargetListBoxHasItems(targetListBox))
                return;

            targetListBox.Items.Add(new ModListBoxItem
            {
                DisplayName = "(none)",
                ModDirName = string.Empty,
                ModKey = string.Empty,
                IsPlaceholder = true
            });
            targetListBox.SelectedIndex = -1;
            targetListBox.Enabled = false;
        }

        public void EnableListBox(ListBox targetListBox)
        {
            targetListBox.Enabled = true;

            for (int i = targetListBox.Items.Count - 1; i >= 0; i--)
            {
                if (IsPlaceholderItem(targetListBox.Items[i]))
                {
                    targetListBox.Items.RemoveAt(i);
                }
            }
        }

        private static bool TargetListBoxHasItems(ListBox targetListBox)
        {
            if (targetListBox.Items.Count == 0)
                return false;

            if (targetListBox.Items.Count == 1 && IsPlaceholderItem(targetListBox.Items[0]))
                return false;

            return true;
        }

        public void ClearModInfo()
        {
            SetNoneSelectedText();
            labelModNameOverrides.Text = string.Empty;
            richTextBoxManifestOverridden.Clear();
            listBoxOverriddenBy.Items.Clear();
            listBoxOverriding.Items.Clear();
            listBoxOverriddenBy.Enabled = true;
            listBoxOverriding.Enabled = true;
            noneSelectedPanel.Visible = true;
            labelManifestContentHeader.Text = _defaultManifestLabelText;
        }

        private void AppendContentPathToMainfestList(string contentPath, ref StringBuilder sb)
        {
            sb.Append(@"\b ");
            sb.Append(Path.GetFileName(contentPath));
            sb.Append(@" \b0 ");

            sb.Append(@" (" + Utils.RtfEscape(Path.GetDirectoryName(contentPath)) + @")");
            sb.Append(@" \line ");
        }
        private void listBoxOverriding_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool startedListUpdate = false;
            try
            {
                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    startedListUpdate = true;
                    DockModListForm.Instance.modObjectListView.BeginUpdate();
                    richTextBoxManifestOverridden.SuspendDrawing();
                    DockModListForm.Instance.RecolorObjectListViewRows();
                }

                labelManifestContentHeader.Text = _defaultManifestLabelText;
                richTextBoxManifestOverridden.Clear();

                if (listBoxOverriding.SelectedIndex == -1)
                    return;

                listBoxOverriddenBy.SelectedIndex = -1;
                if (listBoxOverriding.Items.Count == 0 || DockModListForm.Instance.modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriding.SelectedItem == null || IsPlaceholderItem(listBoxOverriding.SelectedItem))
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriding.SelectedItem;

                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    MainForm.Instance.HighlightModInList(selectedMod.ModKey);
                }

                ModConflictData modConflictData = MainForm.GetSidebarSelectedModConflictData();
                if (modConflictData == null)
                    return;

                if (!modConflictData.overrides.ContainsKey(selectedMod.ModDirName))
                    return;
					
                string currentModDisplayName = GetCurrentModDisplayLabel();
                string targetModDisplayName = selectedMod.DisplayName;

                SetManifestHeaderOverrideText(currentModDisplayName, targetModDisplayName);

                var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modConflictData.overrides[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry, ref sb);
                }
                sb.Append(@"}");
                richTextBoxManifestOverridden.Rtf = sb.ToString();
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.ResumeDrawing();
                    DockModListForm.Instance.modObjectListView.EndUpdate();
                }
            }
        }

        private void listBoxOverriding_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxOverriding.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                ModListBoxItem modListBoxItem = listBoxOverriding.Items[index] as ModListBoxItem;
                if (modListBoxItem == null || modListBoxItem.IsPlaceholder)
                    return;
                MainForm.Instance.SelectModInList(modListBoxItem.ModKey);
            }
        }

        private void listBoxOverriddenBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool startedListUpdate = false;
            try
            {
                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    startedListUpdate = true;
                    richTextBoxManifestOverridden.SuspendDrawing();
                    DockModListForm.Instance.modObjectListView.BeginUpdate();
                    DockModListForm.Instance.RecolorObjectListViewRows();
                }

                labelManifestContentHeader.Text = _defaultManifestLabelText;
                richTextBoxManifestOverridden.Clear();

                if (listBoxOverriddenBy.SelectedIndex == -1)
                    return;

                listBoxOverriding.SelectedIndex = -1;
                if (listBoxOverriddenBy.Items.Count == 0 || DockModListForm.Instance.modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriddenBy.SelectedItem == null || IsPlaceholderItem(listBoxOverriddenBy.SelectedItem))
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriddenBy.SelectedItem;

                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    MainForm.Instance.HighlightModInList(selectedMod.ModKey);
                }

                ModConflictData modConflictData = MainForm.GetSidebarSelectedModConflictData();
                if (modConflictData == null)
                    return;

                if (!modConflictData.overriddenBy.ContainsKey(selectedMod.ModDirName))
                    return;

                string targetModDisplayName = GetCurrentModDisplayLabel();
                string sourceModDisplayName = selectedMod.DisplayName;

                SetManifestHeaderOverrideText(sourceModDisplayName, targetModDisplayName);
				
				var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modConflictData.overriddenBy[selectedMod.ModDirName])
                {
                    AppendContentPathToMainfestList(entry, ref sb);
                }
                sb.Append(@"}");
                richTextBoxManifestOverridden.Rtf = sb.ToString();
            }
            finally
            {
                if (startedListUpdate)
                {
                    richTextBoxManifestOverridden.ResumeDrawing();
                    DockModListForm.Instance.modObjectListView.EndUpdate();
                }
            }
        }

        private void listBoxOverriddenBy_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxOverriddenBy.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                ModListBoxItem modListBoxItem = listBoxOverriddenBy.Items[index] as ModListBoxItem;
                if (modListBoxItem == null || modListBoxItem.IsPlaceholder)
                    return;
                MainForm.Instance.SelectModInList(modListBoxItem.ModKey);
            }
        }

        private void contextMenuManifest_Opening(object sender, CancelEventArgs e)
        {
            bool hasContent = richTextBoxManifestOverridden.TextLength > 0 && richTextBoxManifestOverridden.Enabled;
            contextMenuManifestSelectAllMenuItem.Enabled = hasContent;
            contextMenuManifestCopyMenuItem.Enabled = hasContent;
        }

        private void contextMenuManifestSelectAllMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBoxManifestOverridden.TextLength == 0)
                return;

            richTextBoxManifestOverridden.Focus();
            richTextBoxManifestOverridden.SelectAll();
        }

        private void contextMenuManifestCopyMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBoxManifestOverridden.TextLength == 0)
                return;

            int selectionStart = richTextBoxManifestOverridden.SelectionStart;
            int selectionLength = richTextBoxManifestOverridden.SelectionLength;
            bool hadSelection = selectionLength > 0;

            if (!hadSelection)
            {
                richTextBoxManifestOverridden.SelectAll();
            }

            richTextBoxManifestOverridden.Focus();
            richTextBoxManifestOverridden.Copy();

            if (!hadSelection)
            {
                richTextBoxManifestOverridden.Select(selectionStart, selectionLength);
            }
        }
    }
}
