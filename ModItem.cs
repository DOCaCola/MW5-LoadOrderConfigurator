using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using MW5_Mod_Manager;

// Current (unapplied) state of the mod list

namespace MW5_Mod_Manager
{

    [SupportedOSPlatform("windows")]
    public class ModItemList
    {
        public static ModItemList Instance = new ModItemList();

        public List<ModItem> ModList = new List<ModItem>();

        public bool AreModsSortedByDefaultLoadOrder()
        {
            for (int i = 1; i < ModList.Count; i++)
            {
                ModItem curModItem = ModList[i];
                ModItem prevModItem = ModList[i-1];

                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    if ((curModItem.OriginalLoadOrder > prevModItem.OriginalLoadOrder ||
                        (curModItem.OriginalLoadOrder == prevModItem.OriginalLoadOrder)) &&
                        string.Compare(curModItem.FolderName, prevModItem.FolderName, StringComparison.InvariantCulture) > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if ((prevModItem.OriginalLoadOrder > curModItem.OriginalLoadOrder ||
                        (prevModItem.OriginalLoadOrder == curModItem.OriginalLoadOrder)) &&
                        string.Compare(prevModItem.FolderName, curModItem.FolderName, StringComparison.InvariantCulture) > 0)                   
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private int GetModCount(bool enabledOnly)
        {
            int count = 0;
            if (enabledOnly)
            {
                foreach (var curMod in ModList)
                {
                    if (curMod.Enabled) { count++; }
                }
            }
            else
            {
                count = ModList.Count;
            }

            return count;
        }

        public void RecomputeLoadOrders(bool restoreLoadOrdersOfDisabled = false)
        {
            // If the list is sorted according to MW5's default load order,
            // we can reset everyting to the default load order
            bool isDefaultSorted = AreModsSortedByDefaultLoadOrder();

            /*List.Sort((x, y) =>
            {

                // Compare Original load order
                int priorityComparison = y.OriginalLoadOrder.CompareTo(x.OriginalLoadOrder);

                // If Priority is equal, compare Folder name
                if (priorityComparison == 0)
                {
                    return String.Compare(y.FolderName, x.FolderName, StringComparison.Ordinal);
                }

                return priorityComparison;
            });*/

            int curLoadOrder = GetModCount(restoreLoadOrdersOfDisabled);
            
            // Reorder modlist by recreating it...
            Dictionary<string, bool> newModList = new Dictionary<string, bool>();

            foreach (ModItem curModItem in ModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
            {
                string modKey = curModItem.Path;
                bool modEnabled = curModItem.Enabled;
                newModList[modKey] = modEnabled;
                if (!isDefaultSorted && (!restoreLoadOrdersOfDisabled || modEnabled))
                {
                    curModItem.CurrentLoadOrder = curLoadOrder;
                    ModsManager.Instance.Mods[modKey].NewLoadOrder = curLoadOrder;

                    --curLoadOrder;
                }
                else
                {
                    curModItem.CurrentLoadOrder = curModItem.OriginalLoadOrder;
                    ModsManager.Instance.Mods[modKey].NewLoadOrder = curModItem.OriginalLoadOrder;
                }
            }

            ModsManager.Instance.ModEnabledList = newModList;
        }

    }

    public class ModItem
    {
        public bool Enabled;
        public string Path;
        public string Name;
        public string FolderName;
        public string Author;
        public string Version;
        public int BuildNumber;
        public string VersionCombined;
        public long FileSize;
        public float CurrentLoadOrder;
        public float OriginalLoadOrder;
        public ModsManager.ModData.ModOrigin Origin;

        public Color ProcessedRowBackColor = SystemColors.Window;
        public Color ProcessedCurLoForeColor = SystemColors.WindowText;
        public Color ProcessedOrgLoForeColor = SystemColors.WindowText;
    }
}
