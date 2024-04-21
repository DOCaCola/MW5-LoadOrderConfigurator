using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            switch (MainWindow.MainForm.logic.Platform)
            {
                case "EPIC":
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
                case "GOG":
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case "STEAM":
                    comboBoxPlatform.SelectedIndex = 3;
                    break;
                case "WINDOWS":
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
                    textBoxMw5Path.Text = fbd.SelectedPath;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

            switch (comboBoxPlatform.SelectedIndex)
            {
                case 1:
                    MainWindow.MainForm.logic.Platform = "EPIC";
                    break;
                case 2:
                    MainWindow.MainForm.logic.Platform = "GOG";
                    break;
                case 3:
                    MainWindow.MainForm.logic.Platform = "STEAM";
                    break;
                case 4:
                    MainWindow.MainForm.logic.Platform = "WINDOWS";
                    break;
                default:
                    MainWindow.MainForm.logic.Platform = "";
                    break;
            }

            string path = textBoxMw5Path.Text;

            MainWindow.MainForm.ClearAll();
            MainWindow.MainForm.SetInstallDirectory(path);
            MainWindow.MainForm.logic.SaveProgramData();
            MainWindow.MainForm.RefreshAll();

            Close();
        }
    }
}
