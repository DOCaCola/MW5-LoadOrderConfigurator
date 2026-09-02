using System;
using System.Runtime.Versioning;
using System.Windows.Forms;
using MW5_Mod_Manager.Controls;
using WeifenLuo.WinFormsUI.Docking;
using Application = System.Windows.Forms.Application;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            PatchController.EnableHighDpi = true;
            PatchController.EnablePerScreenDpi = true;
            PatchController.EnableFontInheritanceFix = true;
            AppearanceManager.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LocAppContext());
        }
    }

    public class LocAppContext : ApplicationContext
    {
        private MainForm mainForm;
        private DirectLaunchForm directLaunchForm;

        public LocAppContext()
        {
            bool directLaunch = false;

            if (directLaunch)
            {
                directLaunchForm = new DirectLaunchForm();
                directLaunchForm.Show();
            }
            else
            {
                mainForm = new MainForm();  
                mainForm.Show();
            }

        }
    }
}
