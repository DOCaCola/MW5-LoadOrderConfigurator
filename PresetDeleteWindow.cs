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
    public partial class PresetDeleteWindow : Form
    {
        public PresetDeleteWindow()
        {
            InitializeComponent();
        }

        private void PresetDeleteWindow_Load(object sender, EventArgs e)
        {
            ModsManager logic = MainWindow.MainForm.logic;
            foreach (string key in logic.Presets.Keys)
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
            ModsManager logic = MainWindow.MainForm.logic;

            foreach (var selectedItem in listBoxPresets.SelectedItems)
            {
                string presetName = selectedItem.ToString();
                logic.Presets.Remove(presetName);

                foreach (ToolStripItem item in MainWindow.MainForm.presetsToolStripMenuItem.DropDownItems)
                {
                    if (item.Tag != null && item.Tag.ToString() == presetName)
                    {
                        MainWindow.MainForm.presetsToolStripMenuItem.DropDownItems.Remove(item);
                        break;
                    }
                }
            }

            logic.SavePresets();
            Close();
        }

        private void listBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonDelete.Enabled = listBoxPresets.SelectedItems.Count > 0;
        }
    }
}
