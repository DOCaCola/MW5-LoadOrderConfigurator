using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
        }

        //Copy txt to clipboard
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            ClipboardUtils.ClipboardHelper.CopyTextToClipboard(textBox1.Text);
        }

        private void ExportWindow_Load(object sender, EventArgs e)
        {
            RefreshList(checkBoxEnabledOnly.Checked);
        }

        private void RefreshList(bool enabledOnly)
        {
            Dictionary<string, bool> FolderNameModList = new Dictionary<string, bool>();

            //Get the folder names from the paths in modlist
            foreach (string key in ModsManager.Instance.ModList.Keys.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
            {
                bool isEnabled = ModsManager.Instance.ModList[key];
                if (!isEnabled && enabledOnly)
                    continue;
                string folderName = ModsManager.Instance.PathToDirectoryDict[key];
                FolderNameModList[folderName] = isEnabled;
            }

            string json = JsonConvert.SerializeObject(FolderNameModList, Formatting.Indented);
            ExportForm exportDialog = new ExportForm();

            textBox1.Text = json;
        }

        private void checkBoxEnabledOnly_CheckedChanged(object sender, EventArgs e)
        {
            RefreshList(checkBoxEnabledOnly.Checked);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}