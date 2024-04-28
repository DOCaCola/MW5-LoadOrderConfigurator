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
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            textBoxMw5Path.Text = MainWindow.MainForm.logic.InstallPath;

            switch (MainWindow.MainForm.logic.GamePlatform)
            {
                case MainLogic.eGamePlatform.Epic:
                    comboBoxPlatform.SelectedIndex = 0;
                    break;
                case MainLogic.eGamePlatform.Gog:
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
                case MainLogic.eGamePlatform.Steam:
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case MainLogic.eGamePlatform.WindowsStore:
                    comboBoxPlatform.SelectedIndex = 3;
                    break;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !Utils.StringNullEmptyOrWhiteSpace(fbd.SelectedPath))
                {
                    if (!File.Exists(fbd.SelectedPath + "\\mechwarrior.exe"))
                    {
                        MessageBox.Show(@"The 'MechWarrior.exe' file could not be found in the selected directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.eGamePlatform.Epic;
                    break;
                case 1:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.eGamePlatform.Gog;
                    break;
                case 2:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.eGamePlatform.Steam;
                    break;
                case 3:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.eGamePlatform.WindowsStore;
                    break;
                default:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.eGamePlatform.None;
                    break;
            }

            string path = textBoxMw5Path.Text;

            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path + "\\mechwarrior.exe"))
                {
                    MessageBox.Show(@"The 'MechWarrior.exe' file could not be found in the selected directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MainWindow.MainForm.logic.GamePlatform == MainLogic.eGamePlatform.Steam)
                {
                    if (MainLogic.FindSteamAppsParentDirectory(path) == null)
                    {
                        MessageBox.Show(@"The selected directory doesn't appear to be a valid Steam game installation.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                MainWindow.MainForm.ClearAll();
                MainWindow.MainForm.logic.SetGameInstallPath(path);
                MainWindow.MainForm.logic.SaveSettings();
                MainWindow.MainForm.RefreshAll();
            }

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
