using Microsoft.VisualBasic.Logging;
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
    public partial class PresetSaveWindow : Form
    {
        public PresetSaveWindow()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBoxPresetName.Enabled = true;
                comboBoxPresets.Enabled = false;
            }
            else
            {
                textBoxPresetName.Enabled = false;
                comboBoxPresets.Enabled = true;
            }

            UpdateButtonEnabled();
        }

        private void UpdateButtonEnabled()
        {
            if (radioButton2.Checked)
            {
                buttonSave.Enabled = textBoxPresetName.Text.Length > 0;
            }
            else
            {
                buttonSave.Enabled = comboBoxPresets.SelectedIndex >= 0;
            }

        }

        private void comboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonEnabled();
        }

        private void textBoxPresetName_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnabled();
        }

        private void PresetSaveWindow_Load(object sender, EventArgs e)
        {
            ModsManager logic = MainWindow.MainForm.logic;
            foreach (string key in logic.Presets.Keys)
            {
                this.comboBoxPresets.Items.Add(key);
            }
        }

        private void PresetSaveWindow_Shown(object sender, EventArgs e)
        {
            textBoxPresetName.Focus();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string presetName;
            bool newPreset = false;
            if (radioButton2.Checked)
            {
                presetName = textBoxPresetName.Text;
                
                if (MainWindow.MainForm.logic.Presets.Keys.Contains(presetName))
                {
                    DialogResult result =
                        MessageBox.Show("A preset with the given name already exists. Overwrite preset?",
                            "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                }
                else
                {
                    newPreset = true;
                }
                
            }
            else
            {
                presetName = comboBoxPresets.Text;
            }

            MainWindow.MainForm.SavePreset(presetName);
            if (newPreset)
            {
                MainWindow.MainForm.RebuildPresetsMenu();
            }

            Close();
        }

        private void textBoxPresetName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSave_Click(this, new EventArgs());
            }
        }
    }
}
