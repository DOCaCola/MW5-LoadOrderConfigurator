using System;
using System.Runtime.Versioning;
using System.Windows.Forms;
using MW5_Mod_Manager.Controls;
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
           LocSettings.Instance.TryLoadProgramSettings();
            if (LocWindowColors.DarkMode)
                LocWindowColors.DarkMode = LocSettings.Instance.Data.AllowDarkMode;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.Run(new LocAppContext());
        }
    }

    public class LocAppContext : ApplicationContext
    {
        private MainForm mainForm;
        private DirectLaunchForm directLaunchForm;

        public LocAppContext()
        {
            bool directLaunch = true;

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