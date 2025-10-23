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

        public DockModListForm()
        {
            InitializeComponent();
            this.AllowEndUserDocking = false;

            panelColorLegend.SetDisableDarkMode(true);
            toolStrip2.SetDisableDarkMode(true);
            toolStrip2.SetDisableDarkModeChildren(true);
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
                e.Handled = true;
                ApplyListSortOrderChange(e.SortOrder);
                return;
            }
            SyncLoadOrderSortIndicator();
            e.Canceled = true;
        }

        private void modObjectListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            e.Parameters.PrimarySortOrder = SortOrder.None;
        }

        private void modObjectListView_AboutToCreateGroups(object sender, CreateGroupsEventArgs e)
        {
            // With this trick we have group without header.
            // Point being that the list is smoothly scrollable when groups are used
            foreach (OLVGroup group in e.Groups)
            {
                group.State ^= GroupState.LVGS_NOHEADER;
                group.StateMask ^= GroupState.LVGS_NOHEADER;
            }
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

        private void modObjectListView_FormatCell(object sender, FormatCellEventArgs e)
        {
            ModItem modItem = (ModItem)e.Model;

            /*
			// Modified load orders in bold
			if (e.ColumnIndex == this.olvColumnModCurLoadOrder.Index)
            {
                if (modItem.CurrentLoadOrder != modItem.OriginalLoadOrder)
                {
                    e.SubItem.Font = new Font(e.SubItem.Font, FontStyle.Bold);
                }
                else
                {
                    e.SubItem.Font = new Font(e.SubItem.Font, FontStyle.Regular);
                }
            }*/

            if (!modItem.Enabled)
            {
                e.SubItem.ForeColor = Color.FromArgb(142, 140, 142);
                return;
            }

            if (e.ColumnIndex == olvColumnModName.Index)
            {
                if (ModsManager.Instance.ModConflictData.ContainsKey(modItem.FolderName))
                {
                    ModConflictData a = ModsManager.Instance.ModConflictData[modItem.FolderName];
                    Color newItemColor = LocWindowColors.WindowText;
                    if (a.isOverridden)
                    {
                        newItemColor = LocWindowColors.ModOverriddenColor;
                    }
                    if (a.isOverriding)
                    {
                        newItemColor = LocWindowColors.ModOverridingColor;
                    }
                    if (a.isOverriding && a.isOverridden)
                    {
                        newItemColor = LocWindowColors.ModOverriddenOveridingColor;
                    }

                    e.SubItem.ForeColor = newItemColor;
                }
            }
            else if (e.ColumnIndex == this.olvColumnModCurLoadOrder.Index)
            {
                e.SubItem.ForeColor = modItem.ProcessedCurLoForeColor;
            }
            else if (e.ColumnIndex == olvColumnModOrgLoadOrder.Index)
            {
                e.SubItem.ForeColor = modItem.ProcessedOrgLoForeColor;
            }
        }

        private void modObjectListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            ModItem curModItem = (ModItem)e.Item.RowObject;
            e.Item.BackColor = curModItem.ProcessedRowBackColor;
            e.UseCellFormatEvents = true;
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

                MainForm.Instance.ColorListViewNumbers(olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
                RecolorObjectListViewRows();
                modObjectListView.RefreshObjects(modObjectListView.Objects.Cast<object>().ToList());
                modObjectListView.Sort();
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
                headerSortOrder = LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }

            modObjectListView.PrimarySortColumn = olvColumnModCurLoadOrder;
            modObjectListView.PrimarySortOrder = headerSortOrder;
            modObjectListView.HeaderControl?.Invalidate();

            eSortOrder desiredOrder = headerSortOrder == SortOrder.Ascending
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

            MainForm.Instance.ColorListViewNumbers(olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            RecolorObjectListViewRows();
            modObjectListView.RefreshItems();
        }

        public void SyncLoadOrderSortIndicator()
        {
            SortOrder order = LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow
                ? SortOrder.Descending
                : SortOrder.Ascending;
            modObjectListView.PrimarySortColumn = olvColumnModCurLoadOrder;
            modObjectListView.PrimarySortOrder = order;
            //modObjectListView.HeaderControl?.Invalidate();
        }

        public void RecolorObjectListViewRows()
        {
            bool showModOverrides = modObjectListView.SelectedObjects.Count == 1 && MainForm.Instance._filterMode != eFilterMode.ItemFilter;

            bool anyUpdated = false;
            for (int i = 0; i <= modObjectListView.Items.Count - 1; ++i)
            {
                OLVListItem curItem = (OLVListItem)modObjectListView.Items[i];
                ModItem curModItem = (ModItem)curItem.RowObject;

                bool alternateColor = i % 2 == 1;
                Color newBackColor = LocWindowColors.Window;
                if (alternateColor)
                {
                    newBackColor = LocWindowColors.ListColorAlternate;
                }

                /*if (_filterMode == eFilterMode.ItemHighlight)
                {
                    string filtertext = toolStripTextFilterBox.Text.ToLower();
                    if (!string.IsNullOrWhiteSpace(filtertext) && MatchItemToText(filtertext, curItem))
                    {
                        if (!alternateColor)
                            newBackColor = _highlightColor;
                        else
                            newBackColor = _highlightColorAlternate;
                    }
                }*/

                // Color mod overrides following the currently selected mod
                if (showModOverrides)
                {
                    ModItem firstSelectedItem = (ModItem)modObjectListView.SelectedObjects[0];
                    string selectedModPath = firstSelectedItem.Path;
                    string selectedModFolder = ModsManager.Instance.PathToDirNameDict[selectedModPath];
                    if (ModsManager.Instance.ModConflictData.ContainsKey(selectedModFolder))
                    {
                        ModConflictData modData = ModsManager.Instance.ModConflictData[selectedModFolder];
                        bool foundMatch = false;
                        foreach (string overriding in modData.overriddenBy.Keys)
                        {
                            string modKey = ModsManager.Instance.DirNameToPathDict[overriding];
                            if (modKey == curModItem.Path)
                            {
                                if (!alternateColor)
                                    newBackColor = LocWindowColors.ListModOverridingBackColor;
                                else
                                    newBackColor = LocWindowColors.ListModOverridingBackColorAlternate;
                                foundMatch = true;
                                break;
                            }
                        }

                        if (!foundMatch)
                        {
                            foreach (string overrides in modData.overrides.Keys)
                            {
                                string modKey = ModsManager.Instance.DirNameToPathDict[overrides];
                                if (modKey == curModItem.Path)
                                {
                                    if (!alternateColor)
                                        newBackColor = LocWindowColors.ListModOverriddenBackColor;
                                    else
                                        newBackColor = LocWindowColors.ListModOverriddenBackColorAlternate;
                                    break;
                                }
                            }
                        }
                    }
                }

                curModItem.ProcessedRowBackColor = newBackColor;

                foreach (OLVListSubItem subItem in curItem.SubItems)
                {
                    if (subItem.BackColor != newBackColor)
                    {
                        if (!anyUpdated)
                        {
                            anyUpdated = true;
                            modObjectListView.BeginUpdate();
                        }
                        subItem.BackColor = newBackColor;
                    }

                }
            }

            if (anyUpdated)
                modObjectListView.EndUpdate();
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
