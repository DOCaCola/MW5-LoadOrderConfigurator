using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class ObjectListViewCheckStateUpdateTests
{
    [STATestMethod]
    public void NestedCheckStateUpdatesRaiseOneOuterEventPair()
    {
        using var list = new ObjectListView();
        var events = new List<string>();
        list.CheckStateUpdateStarting += (_, _) =>
            events.Add($"starting:{list.IsCheckStateUpdateInProgress}");
        list.CheckStateUpdateFinished += (_, _) =>
            events.Add($"finished:{list.IsCheckStateUpdateInProgress}");

        IDisposable outer = list.BeginCheckStateUpdate();
        Assert.IsTrue(list.IsCheckStateUpdateInProgress);

        IDisposable inner = list.BeginCheckStateUpdate();
        Assert.IsTrue(list.IsCheckStateUpdateInProgress);
        inner.Dispose();
        inner.Dispose();

        CollectionAssert.AreEqual(
            new[] { "starting:True" },
            events);

        outer.Dispose();

        Assert.IsFalse(list.IsCheckStateUpdateInProgress);
        CollectionAssert.AreEqual(
            new[] { "starting:True", "finished:False" },
            events);
    }

    [STATestMethod]
    public void SelectedRowToggleBatchesPerItemNotifications()
    {
        Application.EnableVisualStyles();
        var models = Enumerable.Range(0, 4)
            .Select(index => new CheckableModel(index))
            .ToList();

        using var list = CreateList(models);
        using var host = CreateHost(list);
        host.Show();
        Application.DoEvents();

        int startingCount = 0;
        int finishedCount = 0;
        int putterCount = 0;
        int itemCheckCount = 0;
        int itemCheckedCount = 0;
        list.CheckStateUpdateStarting += (_, _) => startingCount++;
        list.CheckStateUpdateFinished += (_, _) =>
        {
            finishedCount++;
            Assert.IsFalse(list.IsCheckStateUpdateInProgress);
            Assert.IsTrue(models.All(model => model.Enabled));
        };
        list.BooleanCheckStatePutter = (model, value) =>
        {
            Assert.IsTrue(list.IsCheckStateUpdateInProgress);
            putterCount++;
            ((CheckableModel)model).Enabled = value;
            return value;
        };
        list.ItemCheck += (_, _) => itemCheckCount++;
        list.ItemChecked += (_, _) => itemCheckedCount++;

        foreach (OLVListItem item in list.Items)
            item.Selected = true;
        Application.DoEvents();

        list.ToggleSelectedRowCheckBoxes();

        Assert.AreEqual(1, startingCount);
        Assert.AreEqual(1, finishedCount);
        Assert.AreEqual(models.Count, putterCount);
        Assert.AreEqual(models.Count, itemCheckCount);
        Assert.AreEqual(models.Count, itemCheckedCount);
        Assert.IsTrue(models.All(model => model.Enabled));
    }

    [STATestMethod]
    public void CheckObjectsFinishesBatchAfterPutterException()
    {
        Application.EnableVisualStyles();
        var models = Enumerable.Range(0, 3)
            .Select(index => new CheckableModel(index))
            .ToList();

        using var list = CreateList(models);
        using var host = CreateHost(list);
        host.Show();
        Application.DoEvents();

        int startingCount = 0;
        int finishedCount = 0;
        int putterCount = 0;
        list.CheckStateUpdateStarting += (_, _) => startingCount++;
        list.CheckStateUpdateFinished += (_, _) => finishedCount++;
        list.BooleanCheckStatePutter = (model, value) =>
        {
            putterCount++;
            if (putterCount == 2)
                throw new InvalidOperationException("Synthetic putter failure");

            ((CheckableModel)model).Enabled = value;
            return value;
        };

        Assert.ThrowsException<InvalidOperationException>(
            () => list.CheckObjects(models));

        Assert.AreEqual(1, startingCount);
        Assert.AreEqual(1, finishedCount);
        Assert.IsFalse(list.IsCheckStateUpdateInProgress);

        list.BooleanCheckStatePutter = (model, value) =>
        {
            ((CheckableModel)model).Enabled = value;
            return value;
        };
        list.CheckObjects(models);

        Assert.AreEqual(2, startingCount);
        Assert.AreEqual(2, finishedCount);
        Assert.IsFalse(list.IsCheckStateUpdateInProgress);
        Assert.IsTrue(models.All(model => model.Enabled));
    }

    [STATestMethod]
    public void CheckStateChangesCanSkipRedundantRowRebuilds()
    {
        Application.EnableVisualStyles();
        var model = new CheckableModel(0);

        using var list = new RefreshTrackingObjectListView
        {
            CheckBoxes = true,
            RefreshItemOnCheckStateChange = false,
            View = View.Details,
        };
        var column = new OLVColumn("Value", null)
        {
            AspectGetter = value => ((CheckableModel)value).Index,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
        list.BooleanCheckStateGetter = value => ((CheckableModel)value).Enabled;
        list.BooleanCheckStatePutter = (value, enabled) =>
        {
            ((CheckableModel)value).Enabled = enabled;
            return enabled;
        };
        list.SetObjects(new[] { model });

        list.CheckObject(model);

        Assert.IsTrue(model.Enabled);
        Assert.AreEqual(CheckState.Checked, list.GetItem(0).CheckState);
        Assert.AreEqual(0, list.RefreshItemCallCount);
    }

    [STATestMethod]
    public void NativeBulkCheckStateUpdatesPreservePartialSelectionStates()
    {
        Application.EnableVisualStyles();
        var models = Enumerable.Range(0, 5)
            .Select(index => new CheckableModel(index)
            {
                Enabled = index == 3,
            })
            .ToList();

        using var list = CreateList(models);
        list.RefreshItemOnCheckStateChange = false;
        list.UseNativeCheckStateUpdates = true;
        using var host = CreateHost(list);
        host.Show();
        Application.DoEvents();

        int itemCheckCount = 0;
        int itemCheckedCount = 0;
        list.ItemCheck += (_, _) => itemCheckCount++;
        list.ItemChecked += (_, _) => itemCheckedCount++;

        for (int index = 0; index < 3; index++)
            list.Items[index].Selected = true;
        Application.DoEvents();

        list.ToggleSelectedRowCheckBoxes();
        AssertCheckStatesMatchModels(list, models);
        Assert.AreEqual(3, itemCheckCount);
        Assert.AreEqual(3, itemCheckedCount);

        itemCheckCount = 0;
        itemCheckedCount = 0;
        list.ToggleSelectedRowCheckBoxes();
        AssertCheckStatesMatchModels(list, models);
        Assert.AreEqual(3, itemCheckCount);
        Assert.AreEqual(3, itemCheckedCount);
    }

    private static ObjectListView CreateList(IReadOnlyCollection<CheckableModel> models)
    {
        var list = new ObjectListView
        {
            CheckBoxes = true,
            Dock = DockStyle.Fill,
            MultiSelect = true,
            View = View.Details,
        };
        var column = new OLVColumn("Value", null)
        {
            AspectGetter = model => ((CheckableModel)model).Index,
            Width = 200,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
        list.BooleanCheckStateGetter = model => ((CheckableModel)model).Enabled;
        list.BooleanCheckStatePutter = (model, value) =>
        {
            ((CheckableModel)model).Enabled = value;
            return value;
        };
        list.SetObjects(models);
        return list;
    }

    private static Form CreateHost(Control control)
    {
        var host = new Form
        {
            ShowInTaskbar = false,
        };
        host.Controls.Add(control);
        return host;
    }

    private static void AssertCheckStatesMatchModels(
        ObjectListView list,
        IReadOnlyList<CheckableModel> models)
    {
        for (int index = 0; index < models.Count; index++)
        {
            CheckState expected = models[index].Enabled
                ? CheckState.Checked
                : CheckState.Unchecked;
            Assert.AreEqual(expected, list.GetItem(index).CheckState);
            Assert.AreEqual(expected, GetNativeCheckState(list, index));
        }
    }

    private static CheckState GetNativeCheckState(ObjectListView list, int index)
    {
        const int LvmGetItemState = 0x102C;
        const int StateImageMask = 0xF000;
        int nativeState = SendMessage(
            list.Handle,
            LvmGetItemState,
            new IntPtr(index),
            new IntPtr(StateImageMask)).ToInt32();
        int stateImageIndex = ((nativeState & StateImageMask) >> 12) - 1;
        return (CheckState)stateImageIndex;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr window,
        int message,
        IntPtr wParam,
        IntPtr lParam);

    private sealed class CheckableModel
    {
        public CheckableModel(int index)
        {
            Index = index;
        }

        public int Index { get; }
        public bool Enabled { get; set; }
    }

    private sealed class RefreshTrackingObjectListView : ObjectListView
    {
        public int RefreshItemCallCount { get; private set; }

        public override void RefreshItem(OLVListItem item)
        {
            RefreshItemCallCount++;
            base.RefreshItem(item);
        }
    }
}
