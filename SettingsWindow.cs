using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
                case MainLogic.GamePlatformEnum.Epic:
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
                case MainLogic.GamePlatformEnum.Gog:
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case MainLogic.GamePlatformEnum.Steam:
                    comboBoxPlatform.SelectedIndex = 3;
                    break;
                case MainLogic.GamePlatformEnum.WindowsStore:
                    comboBoxPlatform.SelectedIndex = 4;
                    break;
                default:
                    comboBoxPlatform.SelectedIndex = 0;
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

            switch (comboBoxPlatform.SelectedIndex)
            {
                case 1:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.GamePlatformEnum.Epic;
                    break;
                case 2:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.GamePlatformEnum.Gog;
                    break;
                case 3:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.GamePlatformEnum.Steam;
                    break;
                case 4:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.GamePlatformEnum.WindowsStore;
                    break;
                default:
                    MainWindow.MainForm.logic.GamePlatform = MainLogic.GamePlatformEnum.None;
                    break;
            }

            string path = textBoxMw5Path.Text;

            if (!string.IsNullOrEmpty(path))
            {
                MainWindow.MainForm.ClearAll();
                MainWindow.MainForm.SetInstallDirectory(path);
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
