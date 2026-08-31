using BrightIdeasSoftware;
using MW5_Mod_Manager.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkModeForms;
using WeifenLuo.WinFormsUI.Docking;
using static MW5_Mod_Manager.MainForm;

namespace MW5_Mod_Manager
{
    public partial class DockModListForm : DockContent
    {
        static public DockModListForm Instance;
        private int _listViewUpdateNesting;
        private readonly HashSet<string> _selectedOverridingModFolders =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _selectedOverriddenModFolders =
            new(StringComparer.OrdinalIgnoreCase);

        public DockModListForm()
        {
            InitializeComponent();
            AllowEndUserDocking = false;

            panelColorLegend.SetDisableDarkMode(true);
            toolStrip2.SetDisableDarkMode(true);
            toolStrip2.SetDisableDarkModeChildren(true);

            modObjectListView.OwnerDraw = false;
            modObjectListView.ShowGroups = false;
            modObjectListView.UseSmoothPixelScrolling = true;
        }

        public IDisposable BeginListViewUpdateScope()
        {
            _listViewUpdateNesting++;
            if (_listViewUpdateNesting == 1)
            {
                modObjectListView.BeginUpdate();
            }
            return new ListViewUpdateScope(this);
        }

        private void EndListViewUpdateScope()
        {
            if (_listViewUpdateNesting == 0)
                return;

            _listViewUpdateNesting--;
            if (_listViewUpdateNesting == 0)
            {
                modObjectListView.EndUpdate();
            }
        }

        private sealed class ListViewUpdateScope : IDisposable
        {
            private readonly DockModListForm _owner;
            private bool _disposed;

            public ListViewUpdateScope(DockModListForm owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _owner.EndListViewUpdateScope();
            }
        }


        private void modObjectListView_BeforeSearching(object sender, BeforeSearchingEventArgs e)
        {
            // Abort search if any control characters are in the search string
            foreach (var c in e.StringToFind)
            {
                if (char.IsControl(c))
                {
                    e.Canceled = true;
                    return;
                }
            }
        }

        private void modObjectListView_BeforeSorting(object sender, BeforeSortingEventArgs e)
        {
            if (ReferenceEquals(e.ColumnToSort, olvColumnModCurLoadOrder))
            {
                ApplyListSortOrderChange(e.SortOrder);
                return;
            }
            SyncLoadOrderSortIndicator();
            e.Canceled = true;
        }

        private void modObjectListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            ModItem curModItem = (ModItem)e.Model;
            if (e.Column == olvColumnModFolder)
            {
                e.Text = LocStringUtils.WrapPathForTooltip(curModItem.Path);
            }
            else if (e.Column == olvColumnModFileAge)
            {
                if (!curModItem.FileMetadataLoaded)
                {
                    e.Text = "Loading…";
                    return;
                }
                if (!curModItem.FileMetadataAvailable)
                {
                    e.Text = "Unavailable";
                    return;
                }

                if (curModItem.FileAge != null)
                {
                    CultureInfo culture = CultureInfo.CurrentCulture;

                    // Get the short date and long time patterns
                    string datePattern = culture.DateTimeFormat.ShortDatePattern; // e.g., "M/d/yyyy"
                    string timePattern = culture.DateTimeFormat.LongTimePattern;   // e.g., "h:mm:ss tt"

                    string format = $"{datePattern} {timePattern}";

                    e.Text = curModItem.FileAge.Value.ToString(format, culture);
                }
            }
        }

        private void modObjectListView_ColumnRightClick(object sender, ColumnClickEventArgs e)
        {
            MainForm.Instance.contextMenuStripColumnOptions.Show(Cursor.Position);
        }

        private Color GetModNameForeColor(ModItem modItem)
        {
            if (!modItem.Enabled)
                return Color.FromArgb(142, 140, 142);

            if (ModsManager.Instance.ModConflictData.TryGetValue(
                    modItem.FolderName,
                    out ModConflictData conflictData))
            {
                if (conflictData.isOverriding && conflictData.isOverridden)
                    return LocWindowColors.ModOverriddenOveridingColor;
                if (conflictData.isOverriding)
                    return LocWindowColors.ModOverridingColor;
                if (conflictData.isOverridden)
                    return LocWindowColors.ModOverriddenColor;
            }

            return LocWindowColors.WindowText;
        }

        private void ApplyRowForegroundColors(OLVListItem item, ModItem modItem)
        {
            Color defaultColor = modItem.Enabled
                ? LocWindowColors.WindowText
                : Color.FromArgb(142, 140, 142);

            item.ForeColor = defaultColor;
            if (!modItem.Enabled)
            {
                item.UseItemStyleForSubItems = true;
                return;
            }

            item.UseItemStyleForSubItems = false;
            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            {
                subItem.ForeColor = defaultColor;
            }

            if (olvColumnModName.Index >= 0)
                item.SubItems[olvColumnModName.Index].ForeColor = GetModNameForeColor(modItem);
            if (olvColumnModCurLoadOrder.Index >= 0)
                item.SubItems[olvColumnModCurLoadOrder.Index].ForeColor = modItem.ProcessedCurLoForeColor;
            if (olvColumnModOrgLoadOrder.Index >= 0)
                item.SubItems[olvColumnModOrgLoadOrder.Index].ForeColor = modItem.ProcessedOrgLoForeColor;
        }

        private Color GetRowBackColor(ModItem modItem, int displayIndex)
        {
            bool alternateColor = displayIndex % 2 == 1;

            if (_selectedOverridingModFolders.Contains(modItem.FolderName))
            {
                return alternateColor
                    ? LocWindowColors.ListModOverridingBackColorAlternate
                    : LocWindowColors.ListModOverridingBackColor;
            }

            if (_selectedOverriddenModFolders.Contains(modItem.FolderName))
            {
                return alternateColor
                    ? LocWindowColors.ListModOverriddenBackColorAlternate
                    : LocWindowColors.ListModOverriddenBackColor;
            }

            return alternateColor
                ? LocWindowColors.ListColorAlternate
                : LocWindowColors.Window;
        }

        private static void ApplyRowBackColor(OLVListItem item, ModItem modItem, Color color)
        {
            modItem.ProcessedRowBackColor = color;
            item.BackColor = color;
            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            {
                subItem.BackColor = color;
            }
        }

        private void UpdateSelectedConflictCache()
        {
            _selectedOverridingModFolders.Clear();
            _selectedOverriddenModFolders.Clear();

            if (MainForm.Instance._filterMode == eFilterMode.ItemFilter
                || modObjectListView.SelectedObjects.Count != 1)
            {
                return;
            }

            ModItem selectedMod = (ModItem)modObjectListView.SelectedObjects[0];
            if (!ModsManager.Instance.ModConflictData.TryGetValue(
                    selectedMod.FolderName,
                    out ModConflictData conflictData))
            {
                return;
            }

            _selectedOverridingModFolders.UnionWith(conflictData.overriddenBy.Keys);
            _selectedOverriddenModFolders.UnionWith(conflictData.overrides.Keys);
        }

        private void modObjectListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            ModItem modItem = (ModItem)e.Item.RowObject;
            ApplyRowBackColor(
                e.Item,
                modItem,
                GetRowBackColor(modItem, e.DisplayIndex));
            ApplyRowForegroundColors(e.Item, modItem);
            e.UseCellFormatEvents = false;
        }

        private void modObjectListView_ModelDropped(object sender, ModelDropEventArgs e)
        {
            if (e.DropTargetLocation != DropTargetLocation.AboveItem &&
                e.DropTargetLocation != DropTargetLocation.BelowItem)
            {
                return;
            }

            foreach (ModItem curSourceModItem in e.SourceModels)
            {
                int sourceItemIndex = modObjectListView.IndexOf(curSourceModItem);
                if (e.DropTargetLocation == DropTargetLocation.BelowItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex || sourceItemIndex - 1 == e.DropTargetIndex)
                        return;
                }
                else if (e.DropTargetLocation == DropTargetLocation.AboveItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex || sourceItemIndex + 1 == e.DropTargetIndex)
                        return;
                }
            }

            int normalizedIndex = e.DropTargetIndex;
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
            {
                normalizedIndex++;
            }

            bool reverseView = LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow;
            List<ModItem> draggedItems = e.SourceModels.Cast<ModItem>().ToList();
            if (draggedItems.Count == 0)
                return;

            int targetModelIndex = reverseView
                ? ModItemList.Instance.ModList.Count - normalizedIndex
                : normalizedIndex;

            foreach (var modItem in draggedItems)
            {
                int index = ModItemList.Instance.ModList.IndexOf(modItem);
                if (index >= 0)
                {
                    if (index < targetModelIndex)
                        targetModelIndex--;
                    ModItemList.Instance.ModList.RemoveAt(index);
                }
            }

            IEnumerable<ModItem> itemsToInsert = reverseView
                ? draggedItems.AsEnumerable().Reverse()
                : draggedItems;

            foreach (var modItem in itemsToInsert)
            {
                ModItemList.Instance.ModList.Insert(targetModelIndex++, modItem);
            }

            MainForm.Instance._movingItems = true;

            using (BeginListViewUpdateScope())
            {
                int insertionIndex = normalizedIndex;
                foreach (var mod in draggedItems)
                {
                    int currentViewIndex = modObjectListView.IndexOf(mod);
                    if (currentViewIndex >= 0 && currentViewIndex < insertionIndex)
                        insertionIndex--;
                }

                var draggedObjects = draggedItems.Cast<object>().ToList();
                modObjectListView.RemoveObjects(draggedObjects);
                modObjectListView.InsertObjects(insertionIndex, draggedObjects);
                modObjectListView.SelectedObjects = draggedObjects;

                LoadOrder.RecomputeLoadOrders();
                ModsManager.Instance.RecomputeOverridingData();

                RefreshCurrentLoadOrderCells();
                modObjectListView.Sort();
                RecolorObjectListViewRows();
            }

            MainForm.Instance.QueueSidePanelUpdate(true);
            MainForm.Instance.CheckModConfigTainted();

            MainForm.Instance._movingItems = false;
        }

        private void modObjectListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            if (e.NewDisplayIndex >= olvColumnFreeSpaceDummy.DisplayIndex)
            {
                e.Cancel = true;
            }
        }

        private void modObjectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.Instance.UpdateMoveControlEnabledState();

            if (MainForm.Instance._movingItems)
                return;

            MainForm.Instance.QueueSidePanelUpdate(false);

            RecolorObjectListViewRows();
        }

        private void modObjectListView_DragOver(object sender, DragEventArgs e)
        {
            // Simpledropsource sets this to false..
            if (!modObjectListView.FullRowSelect)
                modObjectListView.FullRowSelect = true;
        }

        private void modObjectListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = modObjectListView.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    MainForm.Instance.contextMenuStripMod.Show(Cursor.Position);
                }
            }
        }

        public void ApplyModelOrderToListView(HashSet<string> selectedPaths, string ensureVisiblePath, Point? scrollPosition, bool suppressUpdateScope = false)
        {
            Point scroll = scrollPosition ?? modObjectListView.LowLevelScrollPosition;
            List<ModItem> viewItems = ModItemList.Instance.GetViewOrderedItems();
            IDisposable updateScope = suppressUpdateScope ? null : BeginListViewUpdateScope();
            try
            {
                modObjectListView.SetObjects(viewItems);

                if (selectedPaths != null && selectedPaths.Count > 0)
                {
                    var selectedObjects = viewItems
                        .Where(mi => selectedPaths.Contains(mi.Path))
                        .Cast<object>()
                        .ToList();
                    modObjectListView.SelectedObjects = selectedObjects;

                    if (!string.IsNullOrEmpty(ensureVisiblePath))
                    {
                        var focus = viewItems.FirstOrDefault(mi => string.Equals(mi.Path, ensureVisiblePath, StringComparison.OrdinalIgnoreCase));
                        if (focus != null)
                        {
                            modObjectListView.EnsureModelVisible(focus);
                        }
                    }
                }
            }
            finally
            {
                updateScope?.Dispose();
            }
            modObjectListView.LowLevelScroll(scroll.X, scroll.Y);
        }

        private void ApplyListSortOrderChange(SortOrder headerSortOrder)
        {
            if (headerSortOrder == SortOrder.None)
            {
                headerSortOrder = LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }

            modObjectListView.PrimarySortColumn = olvColumnModCurLoadOrder;
            modObjectListView.PrimarySortOrder = headerSortOrder;
            modObjectListView.HeaderControl?.Invalidate();

            eSortOrder desiredOrder = headerSortOrder == SortOrder.Descending
                ? eSortOrder.LowToHigh
                : eSortOrder.HighToLow;
            if (LocSettings.Instance.Data.ListSortOrder == desiredOrder)
                return;

            var selectedMods = modObjectListView.SelectedObjects.Cast<ModItem>().ToList();
            HashSet<string> selectionSet = selectedMods.Count > 0
                ? new HashSet<string>(selectedMods.Select(mod => mod.Path), StringComparer.OrdinalIgnoreCase)
                : null;

            string ensureVisiblePath = null;
            if (modObjectListView.FocusedItem is OLVListItem focusedItem)
            {
                ensureVisiblePath = (focusedItem.RowObject as ModItem)?.Path;
            }
            if (ensureVisiblePath == null && selectedMods.Count > 0)
            {
                ensureVisiblePath = selectedMods[0].Path;
            }

            Point scrollPosition = modObjectListView.LowLevelScrollPosition;

            LocSettings.Instance.Data.ListSortOrder = desiredOrder;
            LocSettings.Instance.SaveSettings();
            MainForm.Instance.UpdatePriorityLabels();

            ApplyModelOrderToListView(selectionSet, ensureVisiblePath, scrollPosition);
        }

        public void SyncLoadOrderSortIndicator()
        {
            SortOrder order = LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh
                ? SortOrder.Descending
                : SortOrder.Ascending;
            modObjectListView.PrimarySortColumn = olvColumnModCurLoadOrder;
            modObjectListView.PrimarySortOrder = order;
        }

        public void SetSearchHighlightText(string searchText)
        {
            string normalizedText = searchText?.Trim() ?? string.Empty;
            IModelFilter textFilter = string.IsNullOrEmpty(normalizedText)
                ? null
                : TextMatchFilter.Contains(modObjectListView, normalizedText);

            SetRendererFilter(modObjectListView.DefaultRenderer, textFilter);
            foreach (OLVColumn column in modObjectListView.AllColumns)
            {
                SetRendererFilter(column.Renderer, textFilter);
            }

            bool requiresOwnerDraw = textFilter != null;
            if (modObjectListView.OwnerDraw != requiresOwnerDraw)
                modObjectListView.OwnerDraw = requiresOwnerDraw;

            modObjectListView.Invalidate();
        }

        private static void SetRendererFilter(IRenderer renderer, IModelFilter filter)
        {
            if (renderer is IFilterAwareRenderer filterAwareRenderer)
                filterAwareRenderer.Filter = filter;
        }

        public void RecolorObjectListViewRows()
        {
            UpdateSelectedConflictCache();
            if (MainForm.Instance._filterMode == eFilterMode.ItemFilter)
                return;

            using (BeginListViewUpdateScope())
            {
                for (int index = 0; index < modObjectListView.Items.Count; index++)
                {
                    OLVListItem item = (OLVListItem)modObjectListView.Items[index];
                    ModItem modItem = (ModItem)item.RowObject;
                    Color newBackColor = GetRowBackColor(modItem, index);
                    if (modItem.ProcessedRowBackColor == newBackColor)
                        continue;

                    ApplyRowBackColor(item, modItem, newBackColor);
                }
            }
        }

        public void RefreshRowForegroundColors()
        {
            using (BeginListViewUpdateScope())
            {
                foreach (OLVListItem item in modObjectListView.Items)
                {
                    ApplyRowForegroundColors(item, (ModItem)item.RowObject);
                }
            }
        }

        public void RefreshCurrentLoadOrderCells()
        {
            int columnIndex = olvColumnModCurLoadOrder.Index;
            if (columnIndex < 0)
                return;

            using (BeginListViewUpdateScope())
            {
                foreach (OLVListItem item in modObjectListView.Items)
                {
                    ModItem modItem = (ModItem)item.RowObject;
                    item.SubItems[columnIndex].Text =
                        olvColumnModCurLoadOrder.GetStringValue(modItem);
                    item.SubItems[columnIndex].ForeColor =
                        modItem.Enabled
                            ? modItem.ProcessedCurLoForeColor
                            : Color.FromArgb(142, 140, 142);
                }
            }
        }

        private void toTopToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MainForm.Instance.MoveListItems(modObjectListView.SelectedItems, MovePosition.Top);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[0]);
        }
        private void toBottomToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MainForm.Instance.MoveListItems(modObjectListView.SelectedItems, MovePosition.Bottom);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[^1]);
        }
        private void upToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MainForm.Instance.MoveListItems(modObjectListView.SelectedItems, MoveDirection.Up);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[0]);
        }
        private void downToolStripButton_Click(object sender, EventArgs e)
        {
            var selectedItems = modObjectListView.SelectedObjects;
            MainForm.Instance.MoveListItems(modObjectListView.SelectedItems, MoveDirection.Down);
            modObjectListView.SelectedObjects = selectedItems;
            modObjectListView.EnsureModelVisible(selectedItems[^1]);
        }
    }
}
