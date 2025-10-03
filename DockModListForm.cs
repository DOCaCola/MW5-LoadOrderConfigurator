using BrightIdeasSoftware;
using MW5_Mod_Manager.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static MW5_Mod_Manager.MainForm;

namespace MW5_Mod_Manager
{
    public partial class DockModListForm : DockContent
    {
        static public DockModListForm Instance;

        public DockModListForm()
        {
            InitializeComponent();
            this.AllowEndUserDocking = false;
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
            // Disable sorting
            //e.Canceled = true;
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
            if (!modItem.Enabled)
            {
                e.SubItem.ForeColor = Color.FromArgb(142, 140, 142);
                return;
            }

            if (e.ColumnIndex == this.olvColumnModName.Index)
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
            else if (e.ColumnIndex == this.olvColumnModOrgLoadOrder.Index)
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
            foreach (ModItem curSourceModItem in e.SourceModels)
            {
                int sourceItemIndex = modObjectListView.IndexOf(curSourceModItem);
                if (e.DropTargetLocation == DropTargetLocation.BelowItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex)
                        return;

                    if (sourceItemIndex - 1 == e.DropTargetIndex)
                        return;
                }
                else if (e.DropTargetLocation == DropTargetLocation.AboveItem)
                {
                    if (sourceItemIndex == e.DropTargetIndex)
                        return;

                    // Next item after last selected
                    if (sourceItemIndex + 1 == e.DropTargetIndex)
                        return;
                }
                else
                {
                    return;
                }
            }

            int normalizedIndex = e.DropTargetIndex;
            if (e.DropTargetLocation == DropTargetLocation.BelowItem)
            {
                normalizedIndex++;
            }

            int adjustedTargetIndex = normalizedIndex;
            List<ModItem> sourceModItemList = e.SourceModels.Cast<ModItem>().ToList();

            foreach (ModItem curModItem in sourceModItemList)
            {
                int index = ModItemList.Instance.ModList.IndexOf(curModItem);
                if (index != -1)
                {
                    if (index < adjustedTargetIndex)
                    {
                        adjustedTargetIndex--;
                    }
                    ModItemList.Instance.ModList.RemoveAt(index);
                }
            }

            ModItemList.Instance.ModList.InsertRange(adjustedTargetIndex, sourceModItemList);

            modObjectListView.BeginUpdate();
            MainForm.Instance._movingItems = true;

            DragDropObjectRows(normalizedIndex, e.SourceModels);

            modObjectListView.SelectObjects(e.SourceModels);
            ModItemList.Instance.RecomputeLoadOrders();
            modObjectListView.RefreshObjects(ModItemList.Instance.ModList);
            MainForm.Instance.QueueSidePanelUpdate(true);
            MainForm.Instance._movingItems = false;
            RecolorObjectListViewRows();
            modObjectListView.EndUpdate();
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

        private void DragDropObjectRows(int insertIndex, IList draggedItems)
        {
            // More or less a copy of OLVs Move function with a fix when moving multiple item (originalInsertIndex comparison)
            modObjectListView.BeginUpdate();
            List<int> intList = new List<int>();
            int originalInsertIndex = insertIndex;
            foreach (object modelObject in draggedItems)
            {
                if (modelObject != null)
                {
                    int num = modObjectListView.IndexOf(modelObject);
                    if (num >= 0)
                    {
                        intList.Add(num);
                        if (num <= originalInsertIndex)
                            --insertIndex;
                    }
                }
            }
            intList.Sort();
            intList.Reverse();
            try
            {
                modObjectListView.BeginUpdate();
                foreach (int index1 in intList)
                    modObjectListView.Items.RemoveAt(index1);
                modObjectListView.InsertObjects(insertIndex, draggedItems);
            }
            finally
            {
                modObjectListView.EndUpdate();
            }

            ModItemList.Instance.RecomputeLoadOrders();

            ModsManager.Instance.RecomputeOverridingData();

            modObjectListView.UpdateObjects(ModItemList.Instance.ModList);
            RecolorObjectListViewRows();
            MainForm.Instance.ColorListViewNumbers(olvColumnModCurLoadOrder.Index, LocWindowColors.ModLowPriorityColor, LocWindowColors.ModHighPriorityColor);
            modObjectListView.EndUpdate();

            MainForm.Instance.CheckModConfigTainted();
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
    }
}
