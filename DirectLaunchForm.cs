using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MW5_Mod_Manager.Controls;

namespace MW5_Mod_Manager
{
    public partial class DirectLaunchForm : LocForm
    {
        public DirectLaunchForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (LocSettings.Instance.TryLoadProgramSettings())
            {
                ModsManager.Instance.WarnIfNoModList();
                ModsManager.Instance.ParseDirectories();
                ModsManager.Instance.ReloadModData();
                ModsManager.Instance.DetermineBestAvailableGameVersion();
                ModsManager.Instance.RenewModEnabledList();

                List<ModsManager.ModImportData> modlist = ModsManager.Instance.LoadMw5ModListFileData();
                if (modlist != null)
                {
                    ModsManager.Instance.ProcessModImportList(ref modlist, false);
                    ModsManager.Instance.ModEnabledListLastState = modlist;
                }

                // set all mods to desired enabled states
                if (modlist != null)
                {
                    foreach (var curDesiredMod in modlist)
                    {
                        var curTargetItem = ModsManager.Instance.ModEnabledList.FirstOrDefault(x =>
                            x.ModPath.Equals(curDesiredMod.ModPath, StringComparison.OrdinalIgnoreCase));

                        if (curTargetItem != null)
                        {
                            curTargetItem.Enabled = curDesiredMod.Enabled;
                        }
                    }
                }

                ModItemList.FillFromImportList(modlist);
                LoadOrder.RecomputeLoadOrders();
                ModsManager.Instance.SaveToFiles();
            }
        }
    }
}
