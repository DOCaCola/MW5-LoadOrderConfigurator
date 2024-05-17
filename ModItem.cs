using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5_Mod_Manager;

// Current (unapplied) state of the mod list

namespace MW5_Mod_Manager
{

    public class ModItemList
    {
        public static ModItemList Instance = new ModItemList();

        public List<ModItem> ModList = new List<ModItem>();

        public bool AreModsSortedByDefaultLoadOrder()
        {
            for (int i = 1; i < ModList.Count; i++)
            {
                if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
                {
                    if ((ModList[i].OriginalLoadOrder > ModList[i - 1].OriginalLoadOrder) ||
                        (ModList[i].OriginalLoadOrder == ModList[i - 1].OriginalLoadOrder) &&
                        (String.CompareOrdinal(ModList[i].FolderName, ModList[i - 1].FolderName) > 0))
                    {
                        return false;
                    }
                }
                else
                {
                    if ((ModList[i-1].OriginalLoadOrder > ModList[i].OriginalLoadOrder) ||
                        (ModList[i-1].OriginalLoadOrder == ModList[i].OriginalLoadOrder) &&
                        (String.CompareOrdinal(ModList[i-1].FolderName, ModList[i].FolderName) > 0))                   
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

            for (int i = 0; i < ModList.Count; i++)
            {
                if (isDefaultSorted)
                {
                    ModList[i].CurrentLoadOrder = ModList[i].OriginalLoadOrder;
                }
                else
                {
                    ModList[i].CurrentLoadOrder = curLoadOrder;
                    --curLoadOrder;
                }
            }
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
        public long FileSize;
        public int CurrentLoadOrder;
        public int OriginalLoadOrder;
        public int OriginalLoadOrderNormalized;
        public ModsManager.ModData.ModOrigin Origin;

        public Color ProcessedRowBackColor = SystemColors.Window;
    }
}
