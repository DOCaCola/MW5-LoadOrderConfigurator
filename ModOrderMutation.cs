using System.Collections.Generic;

namespace MW5_Mod_Manager
{
    internal static class ModOrderMutation
    {
        internal static void UpdateModelOrderFromView(
            IReadOnlyList<ModItem> viewOrder,
            bool reversed)
        {
            List<ModItem> modelList = ModItemList.Instance.ModList;
            modelList.Clear();

            if (!reversed)
            {
                modelList.AddRange(viewOrder);
                return;
            }

            for (int index = viewOrder.Count - 1; index >= 0; index--)
            {
                modelList.Add(viewOrder[index]);
            }
        }

        internal static void UpdateModelIndexFromView(ModItem mod, int newViewIndex, bool reversed)
        {
            List<ModItem> modelList = ModItemList.Instance.ModList;
            if (modelList == null)
                return;

            int oldModelIndex = modelList.IndexOf(mod);
            if (oldModelIndex >= 0)
            {
                modelList.RemoveAt(oldModelIndex);
            }

            int insertIndex;
            if (!reversed)
            {
                insertIndex = Clamp(newViewIndex, 0, modelList.Count);
            }
            else
            {
                int finalCount = modelList.Count + 1;
                insertIndex = finalCount - 1 - newViewIndex;
                insertIndex = Clamp(insertIndex, 0, modelList.Count);
            }

            modelList.Insert(insertIndex, mod);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}
