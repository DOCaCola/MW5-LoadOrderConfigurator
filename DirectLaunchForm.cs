using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    public partial class DirectLaunchForm : Form
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
                            x.ModPath.Equals(curDesiredMod.ModPath, StringComparison.InvariantCultureIgnoreCase));

                        if (curTargetItem != null)
                        {
                            curTargetItem.Enabled = curDesiredMod.Enabled;
                        }
                    }
                }

                ModItemList.FillFromImportList(modlist);
                ModItemList.Instance.RecomputeLoadOrders();
                ModsManager.Instance.SaveToFiles();
            }
        }
    }
}
