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

        public static void SwapModsToMatchFilter(ref List<KeyValuePair<string, bool>> targetList, List<KeyValuePair<string, bool>> filterList)
        {
            int filterIndex = 0;
            for (int i = 0; i < targetList.Count && filterIndex < filterList.Count; i++)
            {
                KeyValuePair<string, bool> currentItem = targetList[i];
                if (filterList.FindIndex(t => t.Key == currentItem.Key) != -1)
                {
                    if (currentItem.Key != filterList[filterIndex].Key)
                    {
                        // Swap the items
                        int filterItemIndex = targetList.FindIndex(t => t.Key == filterList[filterIndex].Key);
                        targetList[filterItemIndex] = currentItem;
                        targetList[i] = filterList[filterIndex];
                    }
                    filterIndex++;
                }
            }
        }

        public static void SwapModsToMatchFilter(List<string> targetList, List<string> filterList)
        {
            int filterIndex = 0;
            for (int i = 0; i < targetList.Count && filterIndex < filterList.Count; i++)
            {
                string currentItem = targetList[i];
                if (filterList.Contains(currentItem))
                {
                    if (currentItem != filterList[filterIndex])
                    {
                        // Swap the items
                        int filterItemIndex = targetList.FindIndex(t => t == filterList[filterIndex]);
                        targetList[filterItemIndex] = currentItem;
                        targetList[i] = filterList[filterIndex];
                    }
                    filterIndex++;
                }
            }
        }

        public static bool IsModOrderMatching(List<KeyValuePair<string, bool>> targetList, List<KeyValuePair<string, bool>> filterList)
        {
            int filterIndex = 0;
            foreach (KeyValuePair<string, bool> item in targetList)
            {
                if (filterIndex < filterList.Count && item.Key == filterList[filterIndex].Key)
                {
                    filterIndex++;
                }
            }
            return filterIndex == filterList.Count;
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

        public static List<string> GetNonMatchingMods(List<string> targetList, List<string> filterList)
        {
            List<string> nonMatchingItems = new List<string>();
            int filterIndex = 0;
            foreach (string item in targetList)
            {
                if (filterIndex < filterList.Count && item == filterList[filterIndex])
                {
                    filterIndex++;
                }
                else
                {
                    nonMatchingItems.Add(item);
                }
            }
            return nonMatchingItems;
        }
    }
}
