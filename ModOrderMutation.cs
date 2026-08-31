using System.Collections.Generic;
using System.Linq;

namespace MW5_Mod_Manager
{
    internal static class ModOrderMutation
    {
        internal static ViewReorderResult CalculateViewReorder(
            IReadOnlyList<ModItem> viewOrder,
            IEnumerable<ModItem> draggedItems,
            int insertionIndex)
        {
            var draggedSet = new HashSet<ModItem>(draggedItems);
            List<ModItem> orderedDraggedItems = viewOrder
                .Where(draggedSet.Contains)
                .ToList();

            int boundedInsertionIndex = Clamp(
                insertionIndex,
                0,
                viewOrder.Count);
            int removedBeforeInsertion = 0;
            for (int index = 0; index < boundedInsertionIndex; index++)
            {
                if (draggedSet.Contains(viewOrder[index]))
                    removedBeforeInsertion++;
            }

            var reorderedView = viewOrder
                .Where(mod => !draggedSet.Contains(mod))
                .ToList();
            int adjustedInsertionIndex = Clamp(
                boundedInsertionIndex - removedBeforeInsertion,
                0,
                reorderedView.Count);
            reorderedView.InsertRange(
                adjustedInsertionIndex,
                orderedDraggedItems);

            return new ViewReorderResult(
                reorderedView,
                orderedDraggedItems,
                adjustedInsertionIndex,
                !viewOrder.SequenceEqual(reorderedView));
        }

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

        internal sealed class ViewReorderResult
        {
            internal ViewReorderResult(
                List<ModItem> viewOrder,
                List<ModItem> draggedItems,
                int insertionIndex,
                bool changed)
            {
                ViewOrder = viewOrder;
                DraggedItems = draggedItems;
                InsertionIndex = insertionIndex;
                Changed = changed;
            }

            internal List<ModItem> ViewOrder { get; }
            internal List<ModItem> DraggedItems { get; }
            internal int InsertionIndex { get; }
            internal bool Changed { get; }
        }
    }
}
