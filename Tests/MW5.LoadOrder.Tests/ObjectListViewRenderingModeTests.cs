using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static MW5_Mod_Manager.MainForm;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class ObjectListViewRenderingModeTests
{
    private const uint LvmScroll = 0x1014;
    private const uint LvmGetGroupCount = 0x1098;
    private const uint LvmIsGroupViewEnabled = 0x10AF;

    [STATestMethod]
    public void SearchHighlightUsesOwnerDrawOnlyWhileHighlightingIsActive()
    {
        Application.EnableVisualStyles();
        using var form = new DockModListForm();

        Assert.IsFalse(form.modObjectListView.OwnerDraw);
        Assert.IsNull(form.modObjectListView.AlwaysGroupByColumn);
        Assert.IsFalse(form.modObjectListView.ShowGroups);
        Assert.IsTrue(form.modObjectListView.UseSmoothPixelScrolling);

        form.SetSearchHighlightText("Yet");
        Assert.IsTrue(form.modObjectListView.OwnerDraw);

        form.SetSearchHighlightText(" ");
        Assert.IsFalse(form.modObjectListView.OwnerDraw);
    }

    [STATestMethod]
    public void SmoothPixelScrollingUsesOneHiddenNativeGroup()
    {
        Application.EnableVisualStyles();
        using var list = new ObjectListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            ShowGroups = false,
            UseSmoothPixelScrolling = true,
        };
        var column = new OLVColumn("Value", null)
        {
            AspectGetter = model => model,
            Width = 200,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
        list.SetObjects(Enumerable.Range(0, 60).Cast<object>().ToArray());

        using var host = new Form
        {
            ClientSize = new Size(400, 300),
            ShowInTaskbar = false,
        };
        host.Controls.Add(list);
        host.Show();
        Application.DoEvents();

        Assert.IsFalse(list.ShowGroups);
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmIsGroupViewEnabled, IntPtr.Zero, IntPtr.Zero).ToInt32());
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero).ToInt32());

        list.EnsureVisible(20);
        Application.DoEvents();
        int before = list.Items[20].Bounds.Y;
        SendMessage(list.Handle, LvmScroll, IntPtr.Zero, new IntPtr(1));
        Application.DoEvents();
        int after = list.Items[20].Bounds.Y;

        Assert.AreEqual(-1, after - before);

        list.Sort(column, SortOrder.Descending);
        Application.DoEvents();
        Assert.AreEqual(59, list.GetModelObject(0));
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero).ToInt32());

        list.AddObject(60);
        Application.DoEvents();
        Assert.AreEqual(60, list.GetModelObject(0));
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero).ToInt32());

        list.RemoveObject(60);
        Application.DoEvents();
        Assert.AreEqual(60, list.Items.Count);
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero).ToInt32());

        list.UseSmoothPixelScrolling = false;
        Application.DoEvents();
        Assert.IsFalse(list.ShowGroups);
        Assert.AreEqual(0, SendMessage(
            list.Handle, LvmIsGroupViewEnabled, IntPtr.Zero, IntPtr.Zero).ToInt32());
        Assert.AreEqual(0, list.Groups.Count);
        Assert.AreEqual(60, list.Items.Count);
    }

    [STATestMethod]
    public void ModListPreservesExplicitViewOrderForBothDisplayDirections()
    {
        Application.EnableVisualStyles();
        eSortOrder originalSortOrder = LocSettings.Instance.Data.ListSortOrder;
        List<ModItem> originalModList = ModItemList.Instance.ModList;

        try
        {
            using var form = new DockModListForm();
            form.olvColumnModCurLoadOrder.AspectGetter =
                model => ((ModItem)model).CurrentLoadOrder;

            List<ModItem> modelOrder = Enumerable.Range(1, 10)
                .Select(loadOrder => new ModItem
                {
                    Name = $"Mod {loadOrder}",
                    FolderName = $"Mod{loadOrder}",
                    Path = $@"X:\Mods\Mod{loadOrder}",
                    CurrentLoadOrder = loadOrder,
                    OriginalLoadOrder = loadOrder,
                })
                .ToList();
            ModItemList.Instance.ModList = modelOrder;

            AssertViewOrder(
                form,
                eSortOrder.HighToLow,
                modelOrder.AsEnumerable().Reverse());
            AssertViewOrder(
                form,
                eSortOrder.LowToHigh,
                modelOrder);
        }
        finally
        {
            LocSettings.Instance.Data.ListSortOrder = originalSortOrder;
            ModItemList.Instance.ModList = originalModList;
        }
    }

    [STATestMethod]
    public void SmoothPixelScrollingKeepsInsertedRowAtRequestedVisualPosition()
    {
        Application.EnableVisualStyles();
        using var list = new ObjectListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            ShowGroups = false,
            UseSmoothPixelScrolling = true,
        };
        var column = new OLVColumn("Value", null)
        {
            AspectGetter = model => model,
            Width = 200,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
        list.BeforeSorting += (_, e) => e.Handled = true;
        list.SetObjects(Enumerable.Range(0, 10).Cast<object>().ToArray());

        using var host = new Form
        {
            ClientSize = new Size(400, 300),
            ShowInTaskbar = false,
        };
        host.Controls.Add(list);
        host.Show();
        Application.DoEvents();

        list.RemoveObject(2);
        list.InsertObjects(5, new object[] { 2 });
        list.Sort();
        Application.DoEvents();

        CollectionAssert.AreEqual(
            new[] { 0, 1, 3, 4, 5, 2, 6, 7, 8, 9 },
            list.Items
                .Cast<OLVListItem>()
                .OrderBy(item => item.Bounds.Y)
                .Select(item => (int)item.RowObject)
                .ToArray());
        Assert.AreEqual(1, SendMessage(
            list.Handle, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero).ToInt32());
    }

    private static void AssertViewOrder(
        DockModListForm form,
        eSortOrder sortOrder,
        IEnumerable<ModItem> expectedOrder)
    {
        LocSettings.Instance.Data.ListSortOrder = sortOrder;
        form.SyncLoadOrderSortIndicator();
        form.modObjectListView.SetObjects(
            ModItemList.Instance.GetViewOrderedItems());
        form.modObjectListView.Sort();

        CollectionAssert.AreEqual(
            expectedOrder.Select(mod => mod.Name).ToList(),
            form.modObjectListView.Items
                .Cast<OLVListItem>()
                .Select(item => ((ModItem)item.RowObject).Name)
                .ToList());

        SortOrder expectedIndicator = sortOrder == eSortOrder.LowToHigh
            ? SortOrder.Descending
            : SortOrder.Ascending;
        Assert.AreEqual(
            expectedIndicator,
            form.modObjectListView.PrimarySortOrder);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam);
}
