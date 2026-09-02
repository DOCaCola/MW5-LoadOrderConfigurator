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
using DarkModeForms;
using MW5_Mod_Manager.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5_Mod_Manager
{
    public partial class DockOverviewForm : LocDockContent
    {
        static public DockOverviewForm Instance;

        public Panel noneSelectedPanel = new();
        private Label noneSelectedLabel = new();

        public DockOverviewForm()
        {
            InitializeComponent();

            noneSelectedPanel.Dock = DockStyle.Fill;
            noneSelectedLabel.Text = "(none selected)";
            noneSelectedLabel.TextAlign = ContentAlignment.MiddleCenter;
            noneSelectedLabel.Enabled = false;
            noneSelectedLabel.Dock = DockStyle.Fill;
            noneSelectedPanel.Controls.Add(noneSelectedLabel);
            Controls.Add(noneSelectedPanel);
            noneSelectedPanel.SetDisableDarkMode(true);
            noneSelectedPanel.BringToFront();

            splitContainerVersion.SetDisableDarkMode(true);
            splitContainerVersion.Panel1.SetDisableDarkMode(true);
            splitContainerVersion.Panel2.SetDisableDarkMode(true);

            splitContainer1.SetDisableDarkMode(true);
            splitContainer1.Panel1.SetDisableDarkMode(true);
            splitContainer1.Panel2.SetDisableDarkMode(true);

            panelModInfo.SetDisableDarkMode(true);
        }

        private void linkLabelModAuthorUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ModObject selectedMod = MainForm.GetSidebarSelectedModDetails();
            if (selectedMod == null)
                return;

            string modUrl = selectedMod.authorURL;
            bool isValidUrl = Utils.IsUrlValid(modUrl);
            if (!isValidUrl)
                return;
            var psi = new ProcessStartInfo()
            {
                FileName = modUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void linkLabelSteamId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                ModObject selectedMod = MainForm.GetSidebarSelectedModDetails();
                if (selectedMod == null)
                    return;

                string steamUrl = String.Empty;
                if (LocSettings.Instance.Data.platform == eGamePlatform.Steam && e.Button == MouseButtons.Left && SteamUtils.IsSteamRunning())
                    steamUrl = "steam://url/CommunityFilePage/";
                else
                    steamUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=";

                steamUrl += selectedMod.steamPublishedFileId;
                var psi = new ProcessStartInfo()
                {
                    FileName = steamUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ModsManager.ModData selectedModData = MainForm.GetSidebarSelectedModData();
                if (selectedModData == null)
                    return;

                string nexusUrl = "https://www.nexusmods.com/mechwarrior5mercenaries/mods/" +
                                  selectedModData.NexusModsId;

                var psi = new ProcessStartInfo()
                {
                    FileName = nexusUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void richTextBoxModDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            bool isValidUrl = Utils.IsUrlValid(e.LinkText);
            if (isValidUrl)
            {
                var psi = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }
    }
}
