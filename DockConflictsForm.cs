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

        public Panel noneSelectedPanel = new();
        private Label noneSelectedLabel = new();

        public DockConflictsForm()
        {
            InitializeComponent();

            noneSelectedPanel.Dock = DockStyle.Fill;
            noneSelectedLabel.Text = "(none selected)";
            noneSelectedLabel.TextAlign = ContentAlignment.MiddleCenter;
            noneSelectedLabel.Enabled = false;
            noneSelectedLabel.Dock = DockStyle.Fill;
            noneSelectedPanel.Controls.Add(noneSelectedLabel);
            Controls.Add(noneSelectedPanel);
            noneSelectedPanel.SetDisableDarkMode(true);
            noneSelectedPanel.BringToFront();

            splitContainer2.SetDisableDarkMode(true);
        }

        public void ClearModInfo()
        {
            labelModNameOverrides.Text = string.Empty;
            richTextBoxManifestOverridden.Clear();
            listBoxOverriddenBy.Items.Clear();
            listBoxOverriding.Items.Clear();
            noneSelectedPanel.Visible = true;
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

                if (listBoxOverriding.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriddenBy.SelectedIndex = -1;
                if (listBoxOverriding.Items.Count == 0 || DockModListForm.Instance.modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriding.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriding.SelectedItem;

                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    MainForm.Instance.HighlightModInList(selectedMod.ModKey);
                }

                string superMod = ModsManager.Instance.PathToDirNameDict[MainForm._sideBarSelectedModKey];

                if (!ModsManager.Instance.ModConflictData.TryGetValue(superMod, out ModConflictData modData))
                    return;

                var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modData.overrides[selectedMod.ModDirName])
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

                if (listBoxOverriddenBy.SelectedIndex == -1)
                    return;

                richTextBoxManifestOverridden.Clear();
                listBoxOverriding.SelectedIndex = -1;
                if (listBoxOverriddenBy.Items.Count == 0 || DockModListForm.Instance.modObjectListView.Items.Count == 0)
                    return;

                if (listBoxOverriddenBy.SelectedItem == null)
                    return;

                ModListBoxItem selectedMod = (ModListBoxItem)listBoxOverriddenBy.SelectedItem;

                if (MainForm.Instance._filterMode == MainForm.eFilterMode.None)
                {
                    MainForm.Instance.HighlightModInList(selectedMod.ModKey);
                }

                string superMod = ModsManager.Instance.PathToDirNameDict[MainForm._sideBarSelectedModKey];

                if (!ModsManager.Instance.ModConflictData.TryGetValue(superMod, out ModConflictData modData))
                    return;

                if (!modData.overriddenBy.ContainsKey(selectedMod.ModDirName))
                    return;

                var sb = new StringBuilder();
                sb.Append(@"{\rtf1\ansi");
                foreach (string entry in modData.overriddenBy[selectedMod.ModDirName])
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
                MainForm.Instance.SelectModInList(modListBoxItem.ModKey);
            }
        }
    }
}
