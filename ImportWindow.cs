using System;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
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