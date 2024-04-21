using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ImportWindow : Form
    {
        public ImportWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}