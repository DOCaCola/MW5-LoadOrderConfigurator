using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = LocConstants.UrlNexusmods,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            labelVersion.Text = @"Version: " + MainForm.Instance.GetVersion();
        }

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = LocConstants.UrlGithub,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
