using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5_Mod_Manager
{
    public partial class DockOverviewForm : DockContent
    {
        static public DockOverviewForm Instance;

        public DockOverviewForm()
        {
            InitializeComponent();
        }

        private void linkLabelModAuthorUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string modKey = MainForm._sideBarSelectedModKey;
            string modUrl = ModsManager.Instance.ModDetails[modKey].authorURL;
            bool isValidUrl = Utils.IsUrlValid(modUrl);
            if (isValidUrl)
            {
                Process.Start(modUrl);
            }
        }

        private void linkLabelSteamId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                string modKey = MainForm._sideBarSelectedModKey;
                string steamUrl = String.Empty;
                if (LocSettings.Instance.Data.platform == eGamePlatform.Steam && e.Button == MouseButtons.Left && SteamUtils.IsSteamRunning())
                    steamUrl = "steam://url/CommunityFilePage/";
                else
                    steamUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=";

                steamUrl += ModsManager.Instance.ModDetails[modKey].steamPublishedFileId;
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = steamUrl,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string modKey = MainForm._sideBarSelectedModKey;
                string nexusUrl = "https://www.nexusmods.com/mechwarrior5mercenaries/mods/" +
                                  ModsManager.Instance.Mods[modKey].NexusModsId;

                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = nexusUrl,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }
    }
}
