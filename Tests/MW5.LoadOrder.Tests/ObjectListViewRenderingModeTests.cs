using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam);
}
