using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ImportWindow : Form
    {
        public Dictionary<string, bool> ResultData;

        public ImportWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ResultData = JsonConvert.DeserializeObject<Dictionary<string, bool>>(textBoxData.Text);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(
                    "There was an error decoding the load order string.\r\nMake sure it is in the correct format.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void textBoxData_TextChanged(object sender, EventArgs e)
        {
            buttonImport.Enabled = textBoxData.Text.Length > 0;
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            textBoxData.Text = ClipboardUtils.ClipboardHelper.GetTextFromClipboard();
        }
    }
}