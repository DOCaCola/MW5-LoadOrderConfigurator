using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ExportWindow : Form
    {
        public ExportWindow()
        {
            InitializeComponent();
        }

        //Copy txt to clipboard
        private void button1_Click(object sender, EventArgs e)
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

            ModsManager logic = MainWindow.MainForm.logic;

            //Get the folder names from the paths in modlist
            foreach (string key in logic.ModList.Keys)
            {
                bool isEnabled = logic.ModList[key];
                if (!isEnabled && enabledOnly)
                    continue;
                string folderName = logic.PathToDirectoryDict[key];
                FolderNameModList[folderName] = isEnabled;
            }

            string json = JsonConvert.SerializeObject(FolderNameModList, Formatting.Indented);
            ExportWindow exportDialog = new ExportWindow();

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