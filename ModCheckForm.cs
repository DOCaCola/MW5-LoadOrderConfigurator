using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ModCheckForm : Form
    {
        public ModCheckForm()
        {
            InitializeComponent();
        }

        private void AddLogMessage(string message)
        {
            richTextBox1.Text += message;
        }

        private void CheckMod(string modPath)
        {
            string dirName = Path.GetFileName(modPath);
            if (!File.Exists(Path.Combine(modPath, "mod.json")))
            {
                AddLogMessage("Warning: Empty directory: " + dirName + "\r\n");
                return;
            }

            AddLogMessage("Checking mod in directory: " + dirName + "\r\n");
        }

        private void CheckModDirectory(string path)
        {
            AddLogMessage("Running check on mod path: " + path + "\r\n");

            var directories = Directory.EnumerateDirectories(path);

            foreach (var curDir in directories)
            {
                CheckMod(curDir);
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            foreach (string curPath in ModsManager.Instance.ModsPaths)
            {
                if (string.IsNullOrEmpty(curPath))
                    continue;

                CheckModDirectory(curPath);
            }
        }
    }
}
