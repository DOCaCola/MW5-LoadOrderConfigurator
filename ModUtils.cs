using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW5_Mod_Manager
{
    internal class ModUtils
    {
        // Swaps items the targetList by the sequence of mods in filterList
        public static void SwapModsToMatchFilter(ref List<ModImportData> targetList, List<ModImportData> filterList)
        {
            int filterIndex = 0;
            for (int i = 0; i < targetList.Count && filterIndex < filterList.Count; i++)
            {
                ModImportData currentItem = targetList[i];

                var itemExists = filterList.FirstOrDefault(x => 
                    x.ModPath.Equals(currentItem.ModPath, StringComparison.InvariantCultureIgnoreCase));
                
                if (itemExists != null)
                {
                    if (currentItem.ModPath != filterList[filterIndex].ModPath)
                    {
                        // Swap the items
                        int filterItemIndex = targetList.FindIndex(t => t.ModPath == filterList[filterIndex].ModPath);
                        targetList[filterItemIndex] = currentItem;
                        targetList[i] = filterList[filterIndex];
                    }
                    filterIndex++;
                }
            }
        }

        public static bool IsModOrderMatching(List<string> targetList, List<string> filterList)
        {
            int filterIndex = 0;
            foreach (string item in targetList)
            {
                if (filterIndex < filterList.Count && item == filterList[filterIndex])
                {
                    filterIndex++;
                }
            }
            return filterIndex == filterList.Count;
        }
    }
}
