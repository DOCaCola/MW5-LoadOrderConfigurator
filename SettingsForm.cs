using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

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
            textBoxMw5Path.Text = LocSettings.Instance.Data.InstallPath;

            switch (LocSettings.Instance.Data.platform)
            {
                case eGamePlatform.Epic:
                    comboBoxPlatform.SelectedIndex = 0;
                    break;
                case eGamePlatform.Gog:
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
                case eGamePlatform.Steam:
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case eGamePlatform.WindowsStore:
                    comboBoxPlatform.SelectedIndex = 3;
                    break;
                case eGamePlatform.None:
                    UpdateInstallPathBoxState();
                    break;
            }

            radioButtonHighToLow.Checked = LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow;
            radioButtonLowToHigh.Checked = !radioButtonHighToLow.Checked;
        }


        // Look in defauls pathes for an valid installation
        private string CheckCommonPathsForMw()
        {
            switch (GetSelectedPlatform())
            {
                case eGamePlatform.Steam:
                    {
                        string steamDirectory = null;

                        try
                        {
                            steamDirectory = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;
                        }
                        catch (Exception ex)
                        {

                        }

                        if (steamDirectory != null)
                        {
                            string mw5Path = Path.GetFullPath(Path.Combine(steamDirectory, "SteamApps", "common", "MechWarrior 5 Mercenaries"));
                            if (File.Exists(Path.Combine(mw5Path, "MechWarrior.exe")))
                            {
                                return mw5Path;
                            }
                        }
                    }
                    break;
                case eGamePlatform.Gog:
                    {
                        string gogPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GOG Galaxy", "Games", "MW5Mercs");
                        if (File.Exists(Path.Combine(gogPath, "MechWarrior.exe")))
                        {
                            return gogPath;
                        }
                    }
                    break;
                case eGamePlatform.Epic:
                    {
                        string epicPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Epic Games", "MW5Mercs");
                        if (File.Exists(Path.Combine(epicPath, "MechWarrior.exe")))
                        {
                            return epicPath;
                        }
                    }
                    break;
            }

            return null;
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            string mw5InstallDir = CheckCommonPathsForMw();
            bool showFolderSelect = false;
            if (mw5InstallDir == null)
            {
                showFolderSelect = true;
            }
            else
            {
                DialogResult msgBoxResult = MessageBox.Show(
                    "MechWarrior 5 installation found in: " + mw5InstallDir +
                    "\r\n\r\nDo you want to use this path?", "MechWarrior found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                showFolderSelect = msgBoxResult != DialogResult.Yes;
            }

            if (showFolderSelect)
            {
                mw5InstallDir = null;
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select MechWarrior 5 executable";
                    ofd.Filter = "MechWarrior 5 Executable|*.exe";
                    DialogResult result = ofd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                    {
                        string selectedFilePath = ofd.FileName;
                        string fileName = Path.GetFileName(selectedFilePath);
                        string fileDirectory = Path.GetDirectoryName(selectedFilePath);


                        if (string.Equals(fileName, "MechWarrior-Win64-Shipping.exe",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            if (!fileDirectory.EndsWith(@"\MW5Mercs\Binaries\Win64",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show(
                                    @"The MechWarrior directory could not be determined using path: " +
                                    selectedFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            mw5InstallDir = Path.GetFullPath(Path.Combine(fileDirectory, @"..\..\.."));
                        }
                        else if (string.Equals(fileName, "MechWarrior.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            mw5InstallDir = fileDirectory;
                        }
                        else
                        {
                            MessageBox.Show(
                                @"The MechWarrior installation directory could not be located using path: " +
                                selectedFilePath +
                                "\r\n\r\nPlease select the MechWarrior.exe or MechWarrior-Win64-Shipping.exe in your MechWarrior 5 directory.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            if (mw5InstallDir != null)
            {
                textBoxMw5Path.Text = mw5InstallDir;
            }
        }

        private eGamePlatform GetSelectedPlatform()
        {
            switch (comboBoxPlatform.SelectedIndex)
            {
                case 0:
                    return eGamePlatform.Epic;
                case 1:
                    return eGamePlatform.Gog;
                case 2:
                    return eGamePlatform.Steam;
                case 3:
                    return eGamePlatform.WindowsStore;
                default:
                    return eGamePlatform.None;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Save trivial settings first
            LocSettings.Instance.Data.ListSortOrder =
                radioButtonHighToLow.Checked ? eSortOrder.HighToLow : eSortOrder.LowToHigh;
            
            if (comboBoxPlatform.SelectedIndex == -1)
            {
                MessageBox.Show(@"Please select your platform type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            eGamePlatform newPlatform = GetSelectedPlatform();

            string path = textBoxMw5Path.Text;

            bool settingsValid = false;

            if (newPlatform != eGamePlatform.WindowsStore)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (!File.Exists(path + "\\mechwarrior.exe"))
                    {
                        MessageBox.Show(@"The 'MechWarrior.exe' file could not be found in the selected directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (newPlatform == eGamePlatform.Steam)
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
                path = string.Empty;
                settingsValid = true;
            }

            if (settingsValid)
            {
                LocSettings.Instance.Data.platform = newPlatform;
                LocSettings.Instance.Data.InstallPath = path;
            }
            else
            {
                LocSettings.Instance.Data.platform = eGamePlatform.None;
                LocSettings.Instance.Data.InstallPath = string.Empty; 
            }

            MainForm.Instance.ClearAll();
            ModsManager.Instance.UpdateGamePaths();
            ModsManager.Instance.SaveSettings();
            MainForm.Instance.RefreshAll();

            MainForm.Instance.UpdatePriorityLabels();

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
