using DarkModeForms;
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
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class PresetDeleteForm : Form
    {
        public PresetDeleteForm()
        {
            InitializeComponent();
            _ = new DarkModeCS(this, false);
        }

        private void PresetDeleteWindow_Load(object sender, EventArgs e)
        {
            foreach (string key in ModsManager.Instance.Presets.Keys)
            {
                this.listBoxPresets.Items.Add(key);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            foreach (var selectedItem in listBoxPresets.SelectedItems)
            {
                string presetName = selectedItem.ToString();
                ModsManager.Instance.Presets.Remove(presetName);

                foreach (ToolStripItem item in MainForm.Instance.presetsToolStripMenuItem.DropDownItems)
                {
                    if (item.Tag != null && item.Tag.ToString() == presetName)
                    {
                        MainForm.Instance.presetsToolStripMenuItem.DropDownItems.Remove(item);
                        break;
                    }
                }
            }

            ModsManager.Instance.SavePresets();
            Close();
        }

        private void listBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonDelete.Enabled = listBoxPresets.SelectedItems.Count > 0;
        }
    }
}
