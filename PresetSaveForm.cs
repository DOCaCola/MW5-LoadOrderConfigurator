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
    public partial class PresetSaveForm : Form
    {
        private static bool _lastWasNewPreset = true;
        static int _lastPresetIndex = -1;

        public PresetSaveForm()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePresetControlEnabled();

            UpdateButtonEnabled();
        }

        private void UpdatePresetControlEnabled()
        {
            if (radioButton2.Checked)
            {
                textBoxPresetName.Enabled = true;
                comboBoxPresets.Enabled = false;
                _lastWasNewPreset = true;
            }
            else
            {
                textBoxPresetName.Enabled = false;
                comboBoxPresets.Enabled = true;
                _lastWasNewPreset = false;
            }
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
            _lastPresetIndex = comboBoxPresets.SelectedIndex;
            UpdateButtonEnabled();
        }

        private void textBoxPresetName_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonEnabled();
        }

        private void PresetSaveWindow_Load(object sender, EventArgs e)
        {
            foreach (string key in ModsManager.Instance.Presets.Keys)
            {
                this.comboBoxPresets.Items.Add(key);
            }

            // Restore last selected preset
            if (_lastPresetIndex < this.comboBoxPresets.Items.Count)
            {
                comboBoxPresets.SelectedIndex = _lastPresetIndex;
            }

            if (_lastWasNewPreset)
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
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
                
                if (ModsManager.Instance.Presets.Keys.Contains(presetName))
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

            MainForm.Instance.SavePreset(presetName);
            if (newPreset)
            {
                MainForm.Instance.RebuildPresetsMenu();
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
