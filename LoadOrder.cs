using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace MW5_Mod_Manager
{
    // Computes minimal-change load orders that match the current visual order.
    // - Uses InvariantCultureIgnoreCase for folder-name tie breaking
    // - No maximum on load orders
    // - Load orders are never negative (floor at 0); negative originals can't anchor
    internal static class LoadOrder
    {
        public static void RecomputeLoadOrders(bool restoreLoadOrdersOfDisabled = false)
        {
            // If the list is sorted according to MW5's default load order,
            // we can reset load orders to their default load order
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

            int curLoadOrder = ModItemList.Instance.GetModCount(restoreLoadOrdersOfDisabled);
            
            // Reorder modlist by recreating it...
            List<ModsManager.ModImportData> newModList = new List<ModsManager.ModImportData>();

            foreach (ModItem curModItem in ModItemList.Instance.ModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
            {
                string modKey = curModItem.Path;
                bool modEnabled = curModItem.Enabled;

                ModsManager.ModImportData newImportData = new ModsManager.ModImportData();
                newImportData.ModPath = modKey;
                newImportData.ModFolder = curModItem.FolderName;
                newImportData.Enabled = modEnabled;
                newImportData.Available = true;
                newModList.Add(newImportData);
                
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

        public static bool AreModsSortedByDefaultLoadOrder()
        {
            for (int i = 1; i < ModItemList.Instance.ModList.Count; i++)
            {
                ModItem curModItem = ModItemList.Instance.ModList[i];
                ModItem prevModItem = ModItemList.Instance.ModList[i-1];

                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    if (curModItem.OriginalLoadOrder > prevModItem.OriginalLoadOrder ||
                        (curModItem.OriginalLoadOrder == prevModItem.OriginalLoadOrder &&
                         string.Compare(curModItem.FolderName, prevModItem.FolderName, StringComparison.InvariantCultureIgnoreCase) > 0))
                    {
                        return false;
                    }
                }
                else
                {
                    if (prevModItem.OriginalLoadOrder > curModItem.OriginalLoadOrder ||
                        (prevModItem.OriginalLoadOrder == curModItem.OriginalLoadOrder &&
                         string.Compare(prevModItem.FolderName, curModItem.FolderName, StringComparison.InvariantCultureIgnoreCase) > 0))                 
                    {
                        return false;
                    }
                }

            }
            return true;
        }

    }
}
