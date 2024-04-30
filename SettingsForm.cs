using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            textBoxMw5Path.Text = MainForm.Instance.logic.InstallPath;

            switch (MainForm.Instance.logic.GamePlatform)
            {
                case ModsManager.eGamePlatform.Epic:
                    comboBoxPlatform.SelectedIndex = 0;
                    break;
                case ModsManager.eGamePlatform.Gog:
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
                case ModsManager.eGamePlatform.Steam:
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case ModsManager.eGamePlatform.WindowsStore:
                    comboBoxPlatform.SelectedIndex = 3;
                    break;
                case ModsManager.eGamePlatform.None:
                    UpdateInstallPathBoxState();
                    break;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select MechWarrior 5 install directory.";
                fbd.UseDescriptionForTitle = true;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !Utils.StringNullEmptyOrWhiteSpace(fbd.SelectedPath))
                {
                    if (!File.Exists(fbd.SelectedPath + "\\mechwarrior.exe"))
                    {
                        MessageBox.Show(@"The 'MechWarrior.exe' file could not be found in ." + fbd.SelectedPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    textBoxMw5Path.Text = fbd.SelectedPath;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxPlatform.SelectedIndex == -1)
            {
                MessageBox.Show(@"Please select your platform type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (comboBoxPlatform.SelectedIndex)
            {
                case 0:
                    MainForm.Instance.logic.GamePlatform = ModsManager.eGamePlatform.Epic;
                    break;
                case 1:
                    MainForm.Instance.logic.GamePlatform = ModsManager.eGamePlatform.Gog;
                    break;
                case 2:
                    MainForm.Instance.logic.GamePlatform = ModsManager.eGamePlatform.Steam;
                    break;
                case 3:
                    MainForm.Instance.logic.GamePlatform = ModsManager.eGamePlatform.WindowsStore;
                    break;
                default:
                    MainForm.Instance.logic.GamePlatform = ModsManager.eGamePlatform.None;
                    return;
            }

            string path = textBoxMw5Path.Text;

            bool settingsValid = false;

            if (MainForm.Instance.logic.GamePlatform != ModsManager.eGamePlatform.WindowsStore)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (!File.Exists(path + "\\mechwarrior.exe"))
                    {
                        MessageBox.Show(@"The 'MechWarrior.exe' file could not be found in the selected directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (MainForm.Instance.logic.GamePlatform == ModsManager.eGamePlatform.Steam)
                    {
                        if (ModsManager.FindSteamAppsParentDirectory(path) == null)
                        {
                            MessageBox.Show(@"The selected directory doesn't appear to be a valid Steam game installation.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    settingsValid = true;
                }
            }
            else
            {
                path = "";
                settingsValid = true;
            }

            if (settingsValid)
            {
                MainForm.Instance.ClearAll();
                MainForm.Instance.logic.InstallPath = path;
                MainForm.Instance.logic.UpdateGamePaths();
                MainForm.Instance.logic.SaveSettings();
                MainForm.Instance.RefreshAll();
            }

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateInstallPathBoxState()
        {
            bool enableInstallPathControls = !(comboBoxPlatform.SelectedIndex == 3 || comboBoxPlatform.SelectedIndex == -1);
            textBoxMw5Path.Enabled = enableInstallPathControls;
            buttonBrowse.Enabled = enableInstallPathControls;
        }

        private void comboBoxPlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInstallPathBoxState();
        }
    }
}
