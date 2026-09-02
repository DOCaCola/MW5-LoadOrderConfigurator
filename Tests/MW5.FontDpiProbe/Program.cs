using BrightIdeasSoftware;
using MW5_Mod_Manager;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

internal static class Program
{
    private static string _outputPath = string.Empty;
    private static MainForm _mainForm = null!;
    private static int _lastDpi;
    private static int _snapshotIndex;
    private static DateTime _captureAfter;
    private static bool _usingSyntheticConflictData;
    private static System.Windows.Forms.Timer? _conflictsCaptureTimer;

    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length != 1)
            throw new ArgumentException("Expected an output JSON-lines path.");

        _outputPath = Path.GetFullPath(args[0]);
        File.Delete(_outputPath);

        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        PatchController.EnableHighDpi = true;
        PatchController.EnablePerScreenDpi = true;
        PatchController.EnableFontInheritanceFix = true;
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        _mainForm = new MainForm();
        _mainForm.Shown += (_, _) =>
        {
            _lastDpi = _mainForm.DeviceDpi;
            _mainForm.BeginInvoke((Action)(() =>
            {
                PopulateSidePanels();
                CaptureDockTabStates("initial");
            }));
        };

        var timer = new System.Windows.Forms.Timer { Interval = 100 };
        timer.Tick += (_, _) =>
        {
            int dpi = _mainForm.DeviceDpi;
            if (dpi != _lastDpi)
            {
                _lastDpi = dpi;
                _captureAfter = DateTime.UtcNow.AddSeconds(7);
            }

            if (_captureAfter != default
                && DateTime.UtcNow >= _captureAfter)
            {
                _captureAfter = default;
                CaptureDockTabStates("dpi-change");
            }
        };
        timer.Start();

        Application.Run(_mainForm);
    }

    private static void Capture(string reason)
    {
        var controls = new List<ControlFontSnapshot>();
        CaptureControlTree(_mainForm, "MainForm", controls);
        CaptureControlTree(
            DockModListForm.Instance,
            "DockModListForm",
            controls);
        CaptureControlTree(
            DockOverviewForm.Instance,
            "DockOverviewForm",
            controls);
        CaptureControlTree(
            DockConflictsForm.Instance,
            "DockConflictsForm",
            controls);
        CaptureControlTree(
            GetField<ContextMenuStrip>(
                _mainForm,
                "contextMenuStripMod"),
            "ContextMenuMod",
            controls);
        CaptureControlTree(
            GetField<ContextMenuStrip>(
                _mainForm,
                "contextMenuStripColumnOptions"),
            "ContextMenuColumns",
            controls);
        CaptureControlTree(
            GetField<ContextMenuStrip>(
                DockConflictsForm.Instance,
                "contextMenuManifest"),
            "ContextMenuManifest",
            controls);

        ObjectListView list =
            DockModListForm.Instance.modObjectListView;
        DockPanel dockPanel = _mainForm.Controls
            .OfType<DockPanel>()
            .Single();
        var rows = new List<RowFontSnapshot>();
        for (int rowIndex = 0;
             rowIndex < Math.Min(5, list.Items.Count);
             rowIndex++)
        {
            var item = (OLVListItem)list.Items[rowIndex];
            rows.Add(new RowFontSnapshot(
                rowIndex,
                item.Text,
                CaptureFont(item.Font),
                ReferenceEquals(item.Font, list.Font),
                item.UseItemStyleForSubItems,
                item.SubItems
                    .Cast<ListViewItem.ListViewSubItem>()
                    .Select((subItem, subItemIndex) =>
                        new SubItemFontSnapshot(
                            subItemIndex,
                            subItem.Text,
                            CaptureFont(subItem.Font),
                            ReferenceEquals(subItem.Font, list.Font),
                            ReferenceEquals(subItem.Font, item.Font)))
                    .ToArray()));
        }

        var snapshot = new FontSnapshot(
            ++_snapshotIndex,
            reason,
            DateTime.UtcNow,
            _mainForm.DeviceDpi,
            GetActiveDockContentName(dockPanel),
            _usingSyntheticConflictData,
            CaptureFont(
                dockPanel.Theme.Skin.DockPaneStripSkin.TextFont),
            CaptureFont(
                dockPanel.Theme.Skin.AutoHideStripSkin.TextFont),
            CaptureDockCaptions(dockPanel),
            CaptureSidePanelState(),
            controls,
            rows);
        File.AppendAllText(
            _outputPath,
            JsonSerializer.Serialize(snapshot)
                + Environment.NewLine);
    }

    private static void CaptureControlTree(
        Control root,
        string path,
        List<ControlFontSnapshot> snapshots)
    {
        snapshots.Add(new ControlFontSnapshot(
            path,
            root.GetType().FullName ?? root.GetType().Name,
            root.DeviceDpi,
            root.Text,
            CaptureFont(root.Font),
            root.Parent != null
                && ReferenceEquals(root.Font, root.Parent.Font),
            root is ListBox listBox
                ? listBox.ItemHeight
                : null,
            root.Bounds,
            root.ClientSize,
            root.Padding,
            root.Margin,
            root.GetPreferredSize(Size.Empty),
            root is ToolStrip rootToolStrip
                ? rootToolStrip.ImageScalingSize
                : null,
            null,
            null,
            root.Visible));

        if (root is ToolStrip toolStrip)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                snapshots.Add(new ControlFontSnapshot(
                    path + "/Items/" + item.Name,
                    item.GetType().FullName
                        ?? item.GetType().Name,
                    root.DeviceDpi,
                    item.Text ?? string.Empty,
                    CaptureFont(item.Font),
                    ReferenceEquals(item.Font, toolStrip.Font),
                    null,
                    item.Bounds,
                    item.ContentRectangle.Size,
                    item.Padding,
                    item.Margin,
                    item.GetPreferredSize(Size.Empty),
                    toolStrip.ImageScalingSize,
                    item.Image?.Size,
                    item.ImageScaling.ToString(),
                    item.Visible));

                if (item is ToolStripDropDownItem dropDownItem)
                {
                    CaptureControlTree(
                        dropDownItem.DropDown,
                        path + "/DropDown/" + item.Name,
                        snapshots);
                }
            }
        }

        foreach (Control child in root.Controls)
        {
            CaptureControlTree(
                child,
                path + "/" + (
                    string.IsNullOrEmpty(child.Name)
                        ? child.GetType().Name
                        : child.Name),
                snapshots);
        }
    }

    private static void PopulateSidePanels()
    {
        ObjectListView list =
            DockModListForm.Instance.modObjectListView;
        if (list.Items.Count == 0)
            return;

        object selected = list.Objects
            .Cast<object>()
            .FirstOrDefault(model =>
                model is ModItem mod
                && mod.Enabled
                && ModsManager.Instance.ModConflictData.TryGetValue(
                    mod.FolderName,
                    out ModConflictData? conflictData)
                && conflictData != null
                && (conflictData.overrides.Count > 0
                    || conflictData.overriddenBy.Count > 0))
            ?? SeedConflictFixture(list)
            ?? list.GetModelObject(0)
            ?? throw new InvalidOperationException(
                "The mod list has items but no model object.");
        list.SelectedObject = selected;
        list.FocusedObject = selected;
        MethodInfo update = typeof(MainForm).GetMethod(
            "UpdateSidePanelData",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMethodException(
                typeof(MainForm).FullName,
                "UpdateSidePanelData");
        update.Invoke(_mainForm, new object[] { true });
        Application.DoEvents();
    }

    private static object? SeedConflictFixture(ObjectListView list)
    {
        ModItem[] enabledMods = list.Objects
            .OfType<ModItem>()
            .Where(mod => mod.Enabled)
            .Take(2)
            .ToArray();
        if (enabledMods.Length < 2)
            return null;

        ModItem overriding = enabledMods[0];
        ModItem overridden = enabledMods[1];
        string[] manifest =
        {
            "MW5Mercs/Content/Paks/~mods/DpiProbeFixture.pak"
        };
        ModsManager.Instance.ModConflictData[overriding.FolderName] =
            new ModConflictData
            {
                modPath = overriding.FolderName,
                isOverriding = true,
                overrides = new Dictionary<string, List<string>>
                {
                    [overridden.FolderName] = manifest.ToList()
                },
                overriddenBy =
                    new Dictionary<string, List<string>>()
            };
        _usingSyntheticConflictData = true;
        return overriding;
    }

    private static void ActivateDockContent(DockContent content)
    {
        content.Activate();
        Application.DoEvents();
    }

    private static void CaptureDockTabStates(string reason)
    {
        PopulateSidePanels();
        ActivateDockContent(DockOverviewForm.Instance);
        Capture(reason + "-overview-active");

        PopulateConflictManifest();
        ActivateDockContent(DockConflictsForm.Instance);
        _conflictsCaptureTimer?.Stop();
        _conflictsCaptureTimer?.Dispose();
        _conflictsCaptureTimer = new System.Windows.Forms.Timer
        {
            Interval = 50
        };
        _conflictsCaptureTimer.Tick += (_, _) =>
        {
            _conflictsCaptureTimer.Stop();
            _conflictsCaptureTimer.Dispose();
            _conflictsCaptureTimer = null;
            Capture(reason + "-conflicts-active");
            HideManifestContextMenu();
        };
        _conflictsCaptureTimer.Start();
    }

    private static void PopulateConflictManifest()
    {
        DockConflictsForm conflicts = DockConflictsForm.Instance;
        ListBox? target = GetFirstPopulatedConflictList(conflicts);
        if (target == null)
            return;

        target.SelectedIndex = 0;
        Application.DoEvents();

        if (conflicts.richTextBoxManifestOverridden.TextLength == 0)
            return;

        ContextMenuStrip contextMenu = GetField<ContextMenuStrip>(
            conflicts,
            "contextMenuManifest");
        contextMenu.Show(
            conflicts.richTextBoxManifestOverridden,
            new Point(0, 0));
        for (int pass = 0; pass < 3; pass++)
            Application.DoEvents();
    }

    private static ListBox? GetFirstPopulatedConflictList(
        DockConflictsForm conflicts)
    {
        if (conflicts.listBoxOverriding.Items.Count > 0
            && conflicts.listBoxOverriding.Enabled)
        {
            return conflicts.listBoxOverriding;
        }

        if (conflicts.listBoxOverriddenBy.Items.Count > 0
            && conflicts.listBoxOverriddenBy.Enabled)
        {
            return conflicts.listBoxOverriddenBy;
        }

        return null;
    }

    private static void HideManifestContextMenu()
    {
        GetField<ContextMenuStrip>(
            DockConflictsForm.Instance,
            "contextMenuManifest").Hide();
    }

    private static string GetActiveDockContentName(DockPanel dockPanel)
    {
        object? activeContent = typeof(DockPanel)
            .GetProperty("ActiveContent")
            ?.GetValue(dockPanel);
        return activeContent is DockContent content
            ? content.Name
            : activeContent?.GetType().Name ?? string.Empty;
    }

    private static IReadOnlyList<DockCaptionSnapshot> CaptureDockCaptions(
        Control root)
    {
        return EnumerateControls(root)
            .Where(control => control.GetType().Name == "LocDockPaneCaption")
            .Select(control => new DockCaptionSnapshot(
                control.DeviceDpi,
                control.Bounds,
                control.ClientSize,
                CaptureFont(control.Font),
                control.Visible))
            .ToArray();
    }

    private static SidePanelSnapshot CaptureSidePanelState()
    {
        DockOverviewForm overview = DockOverviewForm.Instance;
        DockConflictsForm conflicts = DockConflictsForm.Instance;
        return new SidePanelSnapshot(
            overview.panelModInfo.Visible,
            overview.noneSelectedPanel.Visible,
            overview.labelModName.Text,
            conflicts.noneSelectedPanel.Visible,
            conflicts.labelModNameOverrides.Text,
            conflicts.listBoxOverriding.Items.Count,
            conflicts.listBoxOverriddenBy.Items.Count,
            conflicts.listBoxOverriding.SelectedIndex,
            conflicts.listBoxOverriddenBy.SelectedIndex,
            conflicts.richTextBoxManifestOverridden.TextLength,
            GetField<ContextMenuStrip>(
                conflicts,
                "contextMenuManifest").Visible);
    }

    private static IEnumerable<Control> EnumerateControls(Control root)
    {
        yield return root;
        foreach (Control child in root.Controls)
        {
            foreach (Control descendant in EnumerateControls(child))
                yield return descendant;
        }
    }

    private static T GetField<T>(object instance, string fieldName)
        where T : class
    {
        Type? type = instance.GetType();
        while (type != null)
        {
            FieldInfo? field = type.GetField(
                fieldName,
                BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public);
            if (field != null)
                return (T)field.GetValue(instance)!;
            type = type.BaseType;
        }

        throw new MissingFieldException(
            instance.GetType().FullName,
            fieldName);
    }

    private static FontInfo CaptureFont(System.Drawing.Font font)
    {
        return new FontInfo(
            font.Name,
            font.SizeInPoints,
            font.Height,
            font.Style.ToString(),
            font.Unit.ToString());
    }

    private sealed record FontSnapshot(
        int Index,
        string Reason,
        DateTime Timestamp,
        int MainDpi,
        string ActiveDockContent,
        bool UsingSyntheticConflictData,
        FontInfo DockPaneFont,
        FontInfo AutoHideFont,
        IReadOnlyList<DockCaptionSnapshot> DockCaptions,
        SidePanelSnapshot SidePanels,
        IReadOnlyList<ControlFontSnapshot> Controls,
        IReadOnlyList<RowFontSnapshot> Rows);

    private sealed record DockCaptionSnapshot(
        int Dpi,
        System.Drawing.Rectangle Bounds,
        System.Drawing.Size ClientSize,
        FontInfo Font,
        bool Visible);

    private sealed record SidePanelSnapshot(
        bool OverviewPanelVisible,
        bool OverviewNoneSelectedVisible,
        string OverviewModName,
        bool ConflictsNoneSelectedVisible,
        string ConflictsModName,
        int OverridingCount,
        int OverriddenByCount,
        int OverridingSelectedIndex,
        int OverriddenBySelectedIndex,
        int ManifestTextLength,
        bool ManifestContextMenuVisible);

    private sealed record ControlFontSnapshot(
        string Path,
        string Type,
        int Dpi,
        string Text,
        FontInfo Font,
        bool InheritsParentFont,
        int? ItemHeight,
        System.Drawing.Rectangle Bounds,
        System.Drawing.Size ClientSize,
        Padding Padding,
        Padding Margin,
        System.Drawing.Size PreferredSize,
        System.Drawing.Size? ImageScalingSize,
        System.Drawing.Size? ImageSize,
        string? ImageScaling,
        bool Visible);

    private sealed record RowFontSnapshot(
        int Index,
        string Text,
        FontInfo Font,
        bool UsesListFont,
        bool UseItemStyleForSubItems,
        IReadOnlyList<SubItemFontSnapshot> SubItems);

    private sealed record SubItemFontSnapshot(
        int Index,
        string Text,
        FontInfo Font,
        bool UsesListFont,
        bool UsesItemFont);

    private sealed record FontInfo(
        string Name,
        float SizeInPoints,
        int Height,
        string Style,
        string Unit);
}
