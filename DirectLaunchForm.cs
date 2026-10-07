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
            ModsManager.Instance.ClearAll();
            if (LocSettings.Instance.TryLoadProgramSettings())
            {
                ModsManager.Instance.ParseDirectories();
                ModsManager.Instance.ReloadModData();
                ModsManager.Instance.RenewModEnabledList();

                List<ModsManager.ModImportData> modlist = ModsManager.Instance.LoadMw5ModListFileData();
                if (modlist != null)
                {
                    ModsManager.Instance.ProcessModImportList(ref modlist, false);
                    ModsManager.Instance.ModEnabledListLastState = modlist;
                }
                ModsManager.Instance.DetermineBestAvailableGameVersion();
                ModsManager.Instance.ResolveLoadedPriorities();
                ModsManager.Instance.LoadLastAppliedPresetData();
                if (ModsManager.Instance.GetExternallyChangedMods().Count > 0 &&
                    MessageBox.Show(this, "Mod settings differ from your last applied load order."
                        + "\r\n\r\nA game update may have disabled mods, or settings may have been changed in the game or by another mod tool."
                        + "\r\n\r\nRestore the priorities and enabled states you applied "
                        + DateTime.UnixEpoch.AddSeconds(ModsManager.Instance.LastAppliedPreset.timeStamp).ToTimeSinceString() + "?",
                        "Mod settings changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    modlist = ModsManager.Instance.LastAppliedPresetModList;
                    foreach (var mod in modlist)
                        ModsManager.Instance.Mods[mod.ModPath].NewLoadOrder = mod.LoadOrder;
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

                var ordered = ModsManager.Instance.ModEnabledList
                    .OrderByDescending(mod => ModsManager.Instance.Mods[mod.ModPath].NewLoadOrder)
                    .ThenByDescending(mod => mod.ModFolder, StringComparer.OrdinalIgnoreCase).ToList();
                ModItemList.FillFromImportList(ordered);
                ModsManager.Instance.SynchronizeWorkingModList();
                ModsManager.Instance.StopModFileWatches();
                try
                {
                    var warnings = ModsManager.Instance.SaveToFiles();
                    if (warnings.Count > 0)
                        MessageBox.Show(this, string.Join(Environment.NewLine, warnings), "Deployment needs attention",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex) when (LocFileUtils.IsFileAccessException(ex) || ex is Newtonsoft.Json.JsonException)
                {
                    MessageBox.Show(this, ex.Message, "Could not apply mod settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ModsManager.Instance.StartModFileWatches();
                }
            }
        }
    }
}
