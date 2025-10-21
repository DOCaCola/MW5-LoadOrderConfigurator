using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace MW5_Mod_Manager
{
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
                         string.Compare(curModItem.FolderName, prevModItem.FolderName, StringComparison.OrdinalIgnoreCase) > 0))
                    {
                        return false;
                    }
                }
                else
                {
                    if (prevModItem.OriginalLoadOrder > curModItem.OriginalLoadOrder ||
                        (prevModItem.OriginalLoadOrder == curModItem.OriginalLoadOrder &&
                         string.Compare(prevModItem.FolderName, curModItem.FolderName, StringComparison.OrdinalIgnoreCase) > 0))                 
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public static void RecomputeLoadOrderAdaptive()
        {
            // Temporary list sorted by default rules (OriginalLoadOrder, then FolderName)
            List<ModItem> defaultSorted = new List<ModItem>(ModItemList.Instance.ModList);
            defaultSorted.Sort((x, y) =>
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    int cmp = y.OriginalLoadOrder.CompareTo(x.OriginalLoadOrder);
                    if (cmp != 0) return cmp;
                    return string.Compare(y.FolderName, x.FolderName, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    int cmp = x.OriginalLoadOrder.CompareTo(y.OriginalLoadOrder);
                    if (cmp != 0) return cmp;
                    return string.Compare(x.FolderName, y.FolderName, StringComparison.OrdinalIgnoreCase);
                }
            });

            // Comparator-as-predicate: does 'a' come before 'b' under default rules?
            bool IsOrderedBefore(ModItem a, ModItem b)
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    if (a.OriginalLoadOrder > b.OriginalLoadOrder) return true; // higher first
                    if (a.OriginalLoadOrder < b.OriginalLoadOrder) return false;
                    return string.Compare(a.FolderName, b.FolderName, StringComparison.OrdinalIgnoreCase) >= 0; // tie: descending
                }
                else
                {
                    if (a.OriginalLoadOrder < b.OriginalLoadOrder) return true; // lower first
                    if (a.OriginalLoadOrder > b.OriginalLoadOrder) return false;
                    return string.Compare(a.FolderName, b.FolderName, StringComparison.OrdinalIgnoreCase) <= 0; // tie: ascending
                }
            }

            // Step 2: Find the largest subset (LIS) of current list that still follows default ordering
            List<ModItem> mods = ModItemList.Instance.ModList;
            int n = mods.Count;
            List<ModItem> modsKeepingOriginalOrder = new List<ModItem>();
            List<ModItem> modsNeedingReassignment = new List<ModItem>();

            if (n > 0)
            {
                int[] dp = new int[n];
                int[] prev = new int[n];
                for (int i = 0; i < n; i++) { dp[i] = 1; prev[i] = -1; }

                int bestLen = 1;
                int bestEnd = 0;

                for (int i = 1; i < n; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (IsOrderedBefore(mods[j], mods[i]) && dp[j] + 1 > dp[i])
                        {
                            dp[i] = dp[j] + 1;
                            prev[i] = j;
                        }
                    }
                    if (dp[i] > bestLen)
                    {
                        bestLen = dp[i];
                        bestEnd = i;
                    }
                }

                // Reconstruct LIS indices in order
                List<int> idxs = new List<int>();
                for (int k = bestEnd; k != -1; k = prev[k]) idxs.Add(k);
                idxs.Reverse();

                HashSet<ModItem> inKeeping = new HashSet<ModItem>();
                foreach (int idx in idxs)
                {
                    modsKeepingOriginalOrder.Add(mods[idx]);
                    inKeeping.Add(mods[idx]);
                }

                foreach (ModItem m in mods)
                {
                    if (!inKeeping.Contains(m)) modsNeedingReassignment.Add(m);
                }
            }

            // Create a list reflecting current order with a keep/reassign marker per mod
            HashSet<ModItem> keepSet = new HashSet<ModItem>(modsKeepingOriginalOrder);
            List<(ModItem Mod, bool KeepsOriginal)> adaptiveMarkedList = new List<(ModItem, bool)>(mods.Count);
            foreach (ModItem m in mods)
            {
                bool keeps = keepSet.Contains(m);
                adaptiveMarkedList.Add((m, keeps));
            }

            // TODO
        }

    }
    }
}
