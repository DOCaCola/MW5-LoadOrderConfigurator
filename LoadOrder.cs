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

            RecomputeLoadOrderAdaptive();
            return;
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

            List<ModsManager.ModImportData> newModList = new List<ModsManager.ModImportData>();

            foreach (ModItem curModItem in ModItemList.Instance.ModList.AsEnumerable().Reverse())
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
                ModItem prevModItem = ModItemList.Instance.ModList[i - 1];

                if (curModItem.OriginalLoadOrder < prevModItem.OriginalLoadOrder)
                {
                    return false;
                }

                if (curModItem.OriginalLoadOrder == prevModItem.OriginalLoadOrder &&
                    string.Compare(prevModItem.FolderName, curModItem.FolderName, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return false;
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
                int cmp = x.OriginalLoadOrder.CompareTo(y.OriginalLoadOrder);
                if (cmp != 0) return cmp;
                return string.Compare(x.FolderName, y.FolderName, StringComparison.OrdinalIgnoreCase);
            });

            // Comparator-as-predicate: does 'a' come before 'b' under default rules?
            bool IsOrderedBefore(ModItem a, ModItem b)
            {
                if (a.OriginalLoadOrder < b.OriginalLoadOrder) return true;
                if (a.OriginalLoadOrder > b.OriginalLoadOrder) return false;
                return string.Compare(a.FolderName, b.FolderName, StringComparison.OrdinalIgnoreCase) <= 0;
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

            HashSet<ModItem> finalKeepSet = new HashSet<ModItem>(modsKeepingOriginalOrder);
            HashSet<ModItem> reassignSet = new HashSet<ModItem>(modsNeedingReassignment);
            Dictionary<ModItem, int> assignedLoadOrders = new Dictionary<ModItem, int>(mods.Count);

            int RoundLoadOrder(float value)
            {
                return Convert.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));
            }

            bool IsPairLessOrEqual(int valueA, string folderA, int valueB, string folderB)
            {
                if (valueA < valueB) return true;
                if (valueA > valueB) return false;
                return string.Compare(folderA, folderB, StringComparison.OrdinalIgnoreCase) <= 0;
            }

            int FindNextKeepIndex(int startIndex)
            {
                for (int i = startIndex; i < adaptiveMarkedList.Count; i++)
                {
                    if (finalKeepSet.Contains(adaptiveMarkedList[i].Mod))
                    {
                        return i;
                    }
                }
                return -1;
            }

            bool TryAssignSegment(int segmentStart, int segmentEnd, bool hasLowerBound, int lowerValue, string lowerFolder, ModItem upperAnchor, out int lastValue, out string lastFolder)
            {
                int segmentLength = segmentEnd - segmentStart;
                if (segmentLength <= 0)
                {
                    lastValue = hasLowerBound ? lowerValue : -1;
                    lastFolder = hasLowerBound ? lowerFolder : string.Empty;
                    return true;
                }

                int anchorValue = RoundLoadOrder(upperAnchor.OriginalLoadOrder);
                string anchorFolder = upperAnchor.FolderName;

                int[] temporaryValues = new int[segmentLength];
                int nextValue = anchorValue;
                string nextFolder = anchorFolder;

                for (int offset = segmentLength - 1; offset >= 0; offset--)
                {
                    ModItem mod = adaptiveMarkedList[segmentStart + offset].Mod;
                    int candidate = nextValue;
                    bool assigned = false;
                    while (candidate >= 0)
                    {
                        if (IsPairLessOrEqual(candidate, mod.FolderName, nextValue, nextFolder))
                        {
                            temporaryValues[offset] = candidate;
                            nextValue = candidate;
                            nextFolder = mod.FolderName;
                            assigned = true;
                            break;
                        }
                        candidate--;
                    }

                    if (!assigned)
                    {
                        lastValue = -1;
                        lastFolder = string.Empty;
                        return false;
                    }
                }

                if (segmentLength > 0 && hasLowerBound)
                {
                    ModItem firstMod = adaptiveMarkedList[segmentStart].Mod;
                    int firstValue = temporaryValues[0];
                    if (!IsPairLessOrEqual(lowerValue, lowerFolder, firstValue, firstMod.FolderName))
                    {
                        lastValue = -1;
                        lastFolder = string.Empty;
                        return false;
                    }
                }

                for (int offset = 0; offset < segmentLength; offset++)
                {
                    ModItem mod = adaptiveMarkedList[segmentStart + offset].Mod;
                    int value = temporaryValues[offset];
                    assignedLoadOrders[mod] = value;
                    reassignSet.Add(mod);
                    finalKeepSet.Remove(mod);
                }

                ModItem lastMod = adaptiveMarkedList[segmentEnd - 1].Mod;
                lastValue = temporaryValues[segmentLength - 1];
                lastFolder = lastMod.FolderName;
                return true;
            }

            int currentIndex = 0;
            int prevValue = -1;
            string prevFolder = string.Empty;
            bool hasPrev = false;

            while (currentIndex < adaptiveMarkedList.Count)
            {
                int nextKeepIndex = FindNextKeepIndex(currentIndex);

                if (nextKeepIndex == -1)
                {
                    for (int i = currentIndex; i < adaptiveMarkedList.Count; i++)
                    {
                        ModItem mod = adaptiveMarkedList[i].Mod;
                        int candidate = Math.Max(RoundLoadOrder(mod.OriginalLoadOrder), hasPrev ? prevValue : 0);
                        while (hasPrev && !IsPairLessOrEqual(prevValue, prevFolder, candidate, mod.FolderName))
                        {
                            candidate++;
                        }

                        assignedLoadOrders[mod] = candidate;
                        reassignSet.Add(mod);
                        finalKeepSet.Remove(mod);

                        prevValue = candidate;
                        prevFolder = mod.FolderName;
                        hasPrev = true;
                    }
                    break;
                }

                if (nextKeepIndex > currentIndex)
                {
                    ModItem anchorMod = adaptiveMarkedList[nextKeepIndex].Mod;
                    if (!TryAssignSegment(currentIndex, nextKeepIndex, hasPrev, prevValue, prevFolder, anchorMod, out int lastSegmentValue, out string lastSegmentFolder))
                    {
                        finalKeepSet.Remove(anchorMod);
                        reassignSet.Add(anchorMod);
                        continue;
                    }

                    if (nextKeepIndex > currentIndex)
                    {
                        prevValue = lastSegmentValue;
                        prevFolder = lastSegmentFolder;
                        hasPrev = true;
                    }
                    currentIndex = nextKeepIndex;
                    continue;
                }

                ModItem currentAnchor = adaptiveMarkedList[currentIndex].Mod;
                int anchorValue = RoundLoadOrder(currentAnchor.OriginalLoadOrder);
                if (hasPrev && !IsPairLessOrEqual(prevValue, prevFolder, anchorValue, currentAnchor.FolderName))
                {
                    finalKeepSet.Remove(currentAnchor);
                    reassignSet.Add(currentAnchor);
                    continue;
                }

                assignedLoadOrders[currentAnchor] = anchorValue;
                prevValue = anchorValue;
                prevFolder = currentAnchor.FolderName;
                hasPrev = true;
                currentIndex++;
            }

            modsKeepingOriginalOrder = new List<ModItem>();
            modsNeedingReassignment = new List<ModItem>();
            foreach (var entry in adaptiveMarkedList)
            {
                ModItem mod = entry.Mod;
                if (finalKeepSet.Contains(mod))
                {
                    modsKeepingOriginalOrder.Add(mod);
                }
                else
                {
                    modsNeedingReassignment.Add(mod);
                }
            }

            foreach (var entry in adaptiveMarkedList)
            {
                ModItem mod = entry.Mod;
                if (!assignedLoadOrders.TryGetValue(mod, out int loadOrderValue))
                {
                    continue;
                }

                mod.CurrentLoadOrder = loadOrderValue;
                ModsManager.Instance.Mods[mod.Path].NewLoadOrder = loadOrderValue;
            }

            List<ModsManager.ModImportData> newModList = new List<ModsManager.ModImportData>(mods.Count);
            foreach (ModItem curModItem in mods.AsEnumerable().Reverse())
            {
                string modKey = curModItem.Path;
                ModsManager.ModImportData newImportData = new ModsManager.ModImportData
                {
                    ModPath = modKey,
                    ModFolder = curModItem.FolderName,
                    Enabled = curModItem.Enabled,
                    Available = true
                };
                newModList.Add(newImportData);
            }

            ModsManager.Instance.ModEnabledList = newModList;
        }

    }
}
