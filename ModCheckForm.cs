using DarkModeForms;
using MW5_Mod_Manager.Controls;
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
            if (LocWindowColors.DarkMode)
                _ = new DarkModeCS(this, false);
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
            foreach (ModsManager.ModPathInfo curModInfo in ModsManager.Instance.ModsPaths)
            {
                if (curModInfo == null || string.IsNullOrEmpty(curModInfo.FullPath))
                    continue;

                CheckModDirectory(curModInfo.FullPath);
            }
        }
    }
}
