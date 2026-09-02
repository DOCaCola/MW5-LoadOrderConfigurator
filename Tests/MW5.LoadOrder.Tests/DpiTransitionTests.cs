using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using MW5_Mod_Manager.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class DpiTransitionTests
{
    private const int WmDpiChanged = 0x02E0;

    [STATestMethod]
    public void FontDescriptorScalesFromItsReferenceDpi()
    {
        using var source = new Font("Segoe UI", 9F);
        DpiFontDescriptor descriptor =
            DpiFontDescriptor.Capture(source, 168);

        using Font standardDpi = descriptor.CreateFont(96);
        using Font restored = descriptor.CreateFont(168);

        Assert.AreEqual(9F * 96F / 168F, standardDpi.SizeInPoints, 0.001F);
        Assert.AreEqual(9F, restored.SizeInPoints, 0.001F);
    }

    [STATestMethod]
    public void ToolStripLayoutReturnsToItsOriginalMetricsAfterDpiRoundTrip()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();

        using var source = new Bitmap(16, 16);
        using (Graphics graphics = Graphics.FromImage(source))
            graphics.Clear(Color.LimeGreen);

        using var form = new DpiToolStripForm(source);
        form.Show();
        Application.DoEvents();

        ToolStripMetrics original = form.GetMetrics();
        if (original.DeviceDpi != 96)
        {
            Assert.Inconclusive(
                "The synthetic toolstrip DPI round-trip requires a 100% "
                + "source monitor. Real monitor transitions are covered by "
                + "the command-line DPI probe.");
        }

        ApplyDpi(form, 144);
        ToolStripMetrics highDpi = form.GetMetrics();
        ApplyDpi(form, 96);
        ToolStripMetrics roundTrip = form.GetMetrics();

        Console.WriteLine($"96 original: {original}");
        Console.WriteLine($"144: {highDpi}");
        Console.WriteLine($"96 round trip: {roundTrip}");

        Assert.IsTrue(
            highDpi.ImageScalingSize.Width > original.ImageScalingSize.Width);
        Assert.IsTrue(
            highDpi.ToolStripFontHeight > original.ToolStripFontHeight);
        Assert.IsTrue(
            highDpi.ItemFontHeight > original.ItemFontHeight);
        Assert.AreEqual(original, roundTrip);
    }

    [STATestMethod]
    public void MainFormMetricsReturnAfterDpiRoundTrip()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();

        using var form = new MainForm
        {
            StartPosition = FormStartPosition.Manual,
            Location = new Point(0, 0),
            ShowInTaskbar = false,
        };
        form.Show();
        Application.DoEvents();
        CreateHandles(form);
        CreateHandles(DockModListForm.Instance);
        CreateHandles(DockOverviewForm.Instance);
        CreateHandles(DockConflictsForm.Instance);
        form.SetModConfigTainted(true);

        MainFormMetrics original = GetMainFormMetrics(form);
        if (original.DeviceDpi != 96)
        {
            Assert.Inconclusive(
                "The synthetic top-level WM_DPICHANGED round-trip requires "
                + "a 100% source monitor. Real monitor transitions are "
                + "covered by the command-line DPI probe.");
        }

        int alternateDpi = original.DeviceDpi == 96 ? 144 : 96;
        ApplyDpi(form, alternateDpi);
        MainFormMetrics transitioned = GetMainFormMetrics(form);
        ApplyDpi(form, original.DeviceDpi);
        MainFormMetrics roundTrip = GetMainFormMetrics(form);

        Console.WriteLine($"{original.DeviceDpi} original: {original}");
        Console.WriteLine($"{alternateDpi}: {transitioned}");
        Console.WriteLine(
            $"{original.DeviceDpi} round trip: {roundTrip}");

        Assert.AreNotEqual(
            original.MainToolbarImageScalingSize,
            transitioned.MainToolbarImageScalingSize);
        Size expectedImageSize = Scale(
            new Size(16, 16),
            alternateDpi,
            96);
        Assert.AreEqual(
            expectedImageSize,
            transitioned.SideToolbarImageScalingSize);
        Assert.AreEqual(
            Scale(new Size(25, 112), alternateDpi, 96),
            transitioned.SideToolbarSize);
        Assert.IsTrue(
            transitioned.DockContentSize.Width
                > original.DockContentSize.Width);
        Assert.IsTrue(
            transitioned.ObjectListViewSize.Width
                > original.ObjectListViewSize.Width);
        Assert.AreEqual(
            expectedImageSize,
            transitioned.ObjectListViewSmallImageSize);
        Assert.AreEqual(
            expectedImageSize,
            transitioned.ObjectListViewStateImageSize);
        Assert.IsTrue(
            transitioned.ApplyButtonFontHeight
                > original.ApplyButtonFontHeight);
        Assert.IsTrue(original.ApplyButtonFontIsBold);
        Assert.IsTrue(transitioned.ApplyButtonFontIsBold);
        Assert.AreEqual(
            Scale(new Size(1, 24), alternateDpi, 96).Height,
            transitioned.StatusSize.Height);
        Assert.AreEqual(original.DeviceDpi, roundTrip.DeviceDpi);
        Assert.AreEqual(original.FormSize, roundTrip.FormSize);
        Assert.AreEqual(
            original.MainToolbarSize,
            roundTrip.MainToolbarSize);
        Assert.AreEqual(
            original.MainToolbarImageScalingSize,
            roundTrip.MainToolbarImageScalingSize);
        Assert.AreEqual(
            original.ApplyButtonImageRectangle,
            roundTrip.ApplyButtonImageRectangle);
        Assert.AreEqual(
            original.ApplyButtonImageSize,
            roundTrip.ApplyButtonImageSize);
        Assert.AreEqual(original.DockContentDpi, roundTrip.DockContentDpi);
        Assert.AreEqual(
            original.ObjectListViewDpi,
            roundTrip.ObjectListViewDpi);
        Assert.AreEqual(
            original.SideToolbarImageScalingSize,
            roundTrip.SideToolbarImageScalingSize);
        Assert.AreEqual(
            original.SideToolbarSize,
            roundTrip.SideToolbarSize);
        Assert.AreEqual(
            original.ObjectListViewSize,
            roundTrip.ObjectListViewSize);
        Assert.AreEqual(
            original.ObjectListViewSmallImageSize,
            roundTrip.ObjectListViewSmallImageSize);
        Assert.AreEqual(
            original.ObjectListViewStateImageSize,
            roundTrip.ObjectListViewStateImageSize);
        Assert.AreEqual(original.StatusSize, roundTrip.StatusSize);
        Assert.AreEqual(
            original.MainFontHeight,
            roundTrip.MainFontHeight);
        Assert.AreEqual(
            original.MenuFontHeight,
            roundTrip.MenuFontHeight);
        Assert.AreEqual(
            original.MainToolbarFontHeight,
            roundTrip.MainToolbarFontHeight);
        Assert.AreEqual(
            original.ApplyButtonFontHeight,
            roundTrip.ApplyButtonFontHeight);
        Assert.IsTrue(roundTrip.ApplyButtonFontIsBold);
        Assert.AreEqual(
            original.StatusFontHeight,
            roundTrip.StatusFontHeight);
        Assert.AreEqual(
            original.SideToolbarFontHeight,
            roundTrip.SideToolbarFontHeight);
        Assert.AreEqual(
            original.ObjectListViewFontHeight,
            roundTrip.ObjectListViewFontHeight);
        Assert.AreEqual(
            original.OverviewHeadingFontHeight,
            roundTrip.OverviewHeadingFontHeight);
        Assert.AreEqual(
            original.DockPaneFontHeight,
            roundTrip.DockPaneFontHeight);
        Assert.AreEqual(
            original.AutoHideFontHeight,
            roundTrip.AutoHideFontHeight);
        Assert.IsTrue(transitioned.DockPaneFontUsesPanelFont);
        Assert.IsTrue(transitioned.AutoHideFontUsesPanelFont);
        Assert.IsTrue(roundTrip.DockPaneFontUsesPanelFont);
        Assert.IsTrue(roundTrip.AutoHideFontUsesPanelFont);
        AssertStripMetricsReadable(transitioned.MainMenu);
        AssertStripMetricsReadable(transitioned.FileMenu);
        AssertStripMetricsReadable(transitioned.ModContextMenu);
        AssertStripMetricsReadable(transitioned.ColumnContextMenu);
        AssertStripMetricsReadable(transitioned.ManifestContextMenu);
        Assert.IsTrue(
            transitioned.MainMenu.FontHeight > original.MainMenu.FontHeight);
        Assert.IsTrue(
            transitioned.ModContextMenu.FontHeight
                > original.ModContextMenu.FontHeight);
        Assert.IsTrue(
            transitioned.ColumnContextMenu.FontHeight
                > original.ColumnContextMenu.FontHeight);
        Assert.IsTrue(
            transitioned.ManifestContextMenu.FontHeight
                > original.ManifestContextMenu.FontHeight);
        Assert.AreEqual(original.MainMenu, roundTrip.MainMenu);
        Assert.AreEqual(original.FileMenu, roundTrip.FileMenu);
        Assert.AreEqual(original.ModContextMenu, roundTrip.ModContextMenu);
        Assert.AreEqual(
            original.ColumnContextMenu,
            roundTrip.ColumnContextMenu);
        Assert.AreEqual(
            original.ManifestContextMenu,
            roundTrip.ManifestContextMenu);
        Assert.IsTrue(transitioned.SideToolbarFitsDockContent);
        Assert.IsTrue(transitioned.LegendDoesNotOverlapList);
        Assert.IsTrue(transitioned.PriorityLabelsClearList);
        Assert.IsTrue(transitioned.ManifestHeaderUsesAvailableWidth);
        Assert.IsTrue(
            transitioned.PriorityTopFontHeight
                > original.PriorityTopFontHeight);
        Assert.IsTrue(
            transitioned.PriorityBottomFontHeight
                > original.PriorityBottomFontHeight);
        Assert.AreEqual(
            original.SideToolbarBounds,
            roundTrip.SideToolbarBounds);
        Assert.AreEqual(
            original.LegendBounds,
            roundTrip.LegendBounds);
        Assert.AreEqual(
            original.PriorityTopBounds,
            roundTrip.PriorityTopBounds);
        Assert.AreEqual(
            original.PriorityBottomBounds,
            roundTrip.PriorityBottomBounds);
        Assert.AreEqual(
            original.PriorityTopFontHeight,
            roundTrip.PriorityTopFontHeight);
        Assert.AreEqual(
            original.PriorityBottomFontHeight,
            roundTrip.PriorityBottomFontHeight);
        Assert.AreEqual(
            original.ManifestHeaderBounds,
            roundTrip.ManifestHeaderBounds);
        Assert.IsTrue(roundTrip.ManifestHeaderUsesAvailableWidth);
        AssertDockCaptionsAreReadable(transitioned);
        AssertDockCaptionsRoundTrip(original, roundTrip);
        Assert.IsTrue(
            Math.Abs(original.MenuSize.Height - roundTrip.MenuSize.Height)
                <= 1);
    }

    private static void ApplyDpi(DpiToolStripForm form, int dpi)
    {
        ApplyTopLevelDpi(form, dpi);
        form.Toolbar.PerformLayout();
        Application.DoEvents();
    }

    private static void ApplyDpi(MainForm form, int dpi)
    {
        ApplyTopLevelDpi(form, dpi);
        form.PerformLayout();
        Application.DoEvents();
    }

    private static void ApplyTopLevelDpi(Form form, int dpi)
    {
        int oldDpi = form.DeviceDpi;
        nint wParam = (nint)(dpi | dpi << 16);

        Rectangle bounds = form.Bounds;
        float factor = (float)dpi / oldDpi;
        var suggestedBounds = new NativeRect
        {
            Left = bounds.Left,
            Top = bounds.Top,
            Right = bounds.Left + (int)Math.Round(bounds.Width * factor),
            Bottom = bounds.Top + (int)Math.Round(bounds.Height * factor),
        };
        nint suggestedBoundsPointer =
            Marshal.AllocHGlobal(Marshal.SizeOf<NativeRect>());
        try
        {
            Marshal.StructureToPtr(
                suggestedBounds,
                suggestedBoundsPointer,
                false);
            SendMessage(
                form.Handle,
                WmDpiChanged,
                wParam,
                suggestedBoundsPointer);
        }
        finally
        {
            Marshal.FreeHGlobal(suggestedBoundsPointer);
        }
    }

    private static void CreateHandles(Control control)
    {
        _ = control.Handle;
        foreach (Control child in control.Controls)
            CreateHandles(child);
    }

    private static MainFormMetrics GetMainFormMetrics(MainForm form)
    {
        ToolStrip toolbar = GetField<ToolStrip>(form, "toolStrip1");
        MenuStrip menu = GetField<MenuStrip>(form, "menuStrip1");
        StatusStrip status = GetField<StatusStrip>(form, "statusStrip1");
        ToolStripButton applyButton =
            GetField<ToolStripButton>(form, "toolStripButtonApply");
        DockPanel dockPanel = GetField<DockPanel>(form, "dockPanel1");
        ToolStrip sideToolbar = DockModListForm.Instance.toolStrip2;
        DockModListForm modListForm = DockModListForm.Instance;
        ContextMenuStrip modContextMenu = GetField<ContextMenuStrip>(
            form,
            "contextMenuStripMod");
        ContextMenuStrip columnContextMenu = GetField<ContextMenuStrip>(
            form,
            "contextMenuStripColumnOptions");
        ContextMenuStrip manifestContextMenu = GetField<ContextMenuStrip>(
            DockConflictsForm.Instance,
            "contextMenuManifest");
        Panel legend = GetField<Panel>(modListForm, "panelColorLegend");
        Label overviewHeading = GetField<Label>(
            DockOverviewForm.Instance,
            "labelModName");
        Label manifestHeader = GetField<Label>(
            DockConflictsForm.Instance,
            "labelManifestContentHeader");
        RichTextBox manifestText = GetField<RichTextBox>(
            DockConflictsForm.Instance,
            "richTextBoxManifestOverridden");
        Font dockPaneFont =
            dockPanel.Theme.Skin.DockPaneStripSkin.TextFont;
        Font autoHideFont =
            dockPanel.Theme.Skin.AutoHideStripSkin.TextFont;
        DockCaptionMetrics[] captions = FindDockCaptions(dockPanel)
            .Select(caption => new DockCaptionMetrics(
                caption.Bounds,
                caption.ClientSize,
                caption.Font.Height,
                caption.Visible))
            .OrderBy(caption => caption.Bounds.X)
            .ThenBy(caption => caption.Bounds.Y)
            .ToArray();
        return new MainFormMetrics(
            form.DeviceDpi,
            form.Size,
            menu.Size,
            toolbar.Size,
            toolbar.ImageScalingSize,
            applyButton.Size,
            applyButton.ContentRectangle,
            GetImageRectangle(toolbar, applyButton),
            applyButton.Image.Size,
            status.Size,
            DockModListForm.Instance.DeviceDpi,
            DockModListForm.Instance.Size,
            sideToolbar.Size,
            sideToolbar.ImageScalingSize,
            DockModListForm.Instance.modObjectListView.DeviceDpi,
            DockModListForm.Instance.modObjectListView.Size,
            DockModListForm.Instance.modObjectListView
                .BaseSmallImageList.ImageSize,
            DockModListForm.Instance.modObjectListView
                .StateImageList.ImageSize,
            form.Font.Height,
            menu.Font.Height,
            toolbar.Font.Height,
            applyButton.Font.Height,
            applyButton.Font.Bold,
            status.Font.Height,
            sideToolbar.Font.Height,
            DockModListForm.Instance.modObjectListView.Font.Height,
            overviewHeading.Font.Height,
            dockPaneFont.Height,
            autoHideFont.Height,
            ReferenceEquals(dockPaneFont, dockPanel.Font),
            ReferenceEquals(autoHideFont, dockPanel.Font),
            CaptureStripMetrics(menu),
            CaptureStripMetrics(
                GetField<ToolStripMenuItem>(form, "fileToolStripMenuItem")
                    .DropDown),
            CaptureStripMetrics(modContextMenu),
            CaptureStripMetrics(columnContextMenu),
            CaptureStripMetrics(manifestContextMenu),
            sideToolbar.Bounds,
            modListForm.ClientRectangle.Contains(sideToolbar.Bounds),
            legend.Bounds,
            modListForm.modObjectListView.Bounds.Bottom <= legend.Bounds.Top,
            modListForm.rotatingLabelTop.Bounds,
            modListForm.rotatingLabelBottom.Bounds,
            modListForm.rotatingLabelTop.Font.Height,
            modListForm.rotatingLabelBottom.Font.Height,
            modListForm.rotatingLabelTop.Bounds.Right
                <= modListForm.modObjectListView.Bounds.Left
                && modListForm.rotatingLabelBottom.Bounds.Right
                    <= modListForm.modObjectListView.Bounds.Left,
            manifestHeader.Bounds,
            manifestHeader.Left == manifestText.Left
                && manifestHeader.Right >= manifestText.Right,
            captions);
    }

    private static T GetField<T>(object instance, string fieldName)
        where T : class
    {
        Type type = instance.GetType();
        while (type != null)
        {
            FieldInfo field = type.GetField(
                fieldName,
                BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public);
            if (field != null)
                return (T)field.GetValue(instance);
            type = type.BaseType;
        }

        throw new MissingFieldException(
            instance.GetType().FullName,
            fieldName);
    }

    private static StripMetrics CaptureStripMetrics(ToolStrip strip)
    {
        ToolStripItem item = strip.Items
            .Cast<ToolStripItem>()
            .FirstOrDefault(candidate =>
                candidate.Available
                && candidate is not ToolStripSeparator)
            ?? throw new InvalidOperationException(
                $"'{strip.Name}' has no visible non-separator item.");
        return new StripMetrics(
            strip.Font.Height,
            strip.Padding,
            strip.Size,
            strip.GetPreferredSize(Size.Empty),
            item.Font.Height,
            item.Padding,
            item.Bounds,
            item.ContentRectangle,
            item.GetPreferredSize(Size.Empty));
    }

    private static void AssertStripMetricsReadable(StripMetrics metrics)
    {
        Assert.IsTrue(
            metrics.ItemBounds.Height >= metrics.ItemFontHeight,
            $"Item bounds {metrics.ItemBounds} do not fit font height "
            + $"{metrics.ItemFontHeight}.");
        Assert.IsTrue(
            metrics.ItemContentRectangle.Height >= metrics.ItemFontHeight,
            $"Item content {metrics.ItemContentRectangle} does not fit "
            + $"font height {metrics.ItemFontHeight}.");
    }

    private static IEnumerable<Control> FindDockCaptions(Control root)
    {
        if (root.GetType().Name == "LocDockPaneCaption")
            yield return root;

        foreach (Control child in root.Controls)
        {
            foreach (Control caption in FindDockCaptions(child))
                yield return caption;
        }
    }

    private static void AssertDockCaptionsAreReadable(
        MainFormMetrics metrics)
    {
        DockCaptionMetrics[] renderedCaptions = metrics.DockCaptions
            .Where(caption =>
                caption.Visible
                && caption.Bounds.Width > 0
                && caption.Bounds.Height > 0)
            .ToArray();
        Assert.IsTrue(
            renderedCaptions.Length > 0,
            "Expected at least one custom dock-pane caption.");
        foreach (DockCaptionMetrics caption in renderedCaptions)
        {
            Assert.IsTrue(
                caption.Bounds.Height >= metrics.DockPaneFontHeight,
                $"Caption bounds {caption.Bounds} do not fit dock font "
                + $"height {metrics.DockPaneFontHeight}.");
        }
    }

    private static void AssertDockCaptionsRoundTrip(
        MainFormMetrics original,
        MainFormMetrics roundTrip)
    {
        Assert.AreEqual(
            original.DockCaptions.Length,
            roundTrip.DockCaptions.Length);
        for (int index = 0;
             index < original.DockCaptions.Length;
             index++)
        {
            Assert.AreEqual(
                original.DockCaptions[index],
                roundTrip.DockCaptions[index]);
        }
    }

    private static Rectangle GetImageRectangle(
        ToolStrip toolbar,
        ToolStripItem targetItem)
    {
        Rectangle imageRectangle = Rectangle.Empty;
        ToolStripItemImageRenderEventHandler handler = (_, e) =>
        {
            if (ReferenceEquals(e.Item, targetItem))
                imageRectangle = e.ImageRectangle;
        };
        toolbar.Renderer.RenderItemImage += handler;
        try
        {
            using var bitmap = new Bitmap(
                Math.Max(1, toolbar.Width),
                Math.Max(1, toolbar.Height));
            toolbar.DrawToBitmap(bitmap, toolbar.ClientRectangle);
        }
        finally
        {
            toolbar.Renderer.RenderItemImage -= handler;
        }
        return imageRectangle;
    }

    private static Size Scale(Size size, int newDpi, int oldDpi)
    {
        float factor = (float)newDpi / oldDpi;
        return new Size(
            (int)Math.Round(size.Width * factor),
            (int)Math.Round(size.Height * factor));
    }

    private static void AssertSizeWithin(
        Size expected,
        Size actual,
        int tolerance)
    {
        Assert.IsTrue(
            Math.Abs(expected.Width - actual.Width) <= tolerance,
            $"Expected width {expected.Width} ± {tolerance}, actual {actual.Width}.");
        Assert.IsTrue(
            Math.Abs(expected.Height - actual.Height) <= tolerance,
            $"Expected height {expected.Height} ± {tolerance}, actual {actual.Height}.");
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint SendMessage(
        nint hWnd,
        int msg,
        nint wParam,
        nint lParam);

    private readonly record struct ToolStripMetrics(
        int DeviceDpi,
        Size ToolStripSize,
        Size ImageScalingSize,
        Size ItemSize,
        Padding ItemPadding,
        Rectangle ItemContentRectangle,
        Size ImageSize,
        int ToolStripFontHeight,
        int ItemFontHeight);

    private readonly record struct MainFormMetrics(
        int DeviceDpi,
        Size FormSize,
        Size MenuSize,
        Size MainToolbarSize,
        Size MainToolbarImageScalingSize,
        Size ApplyButtonSize,
        Rectangle ApplyButtonContentRectangle,
        Rectangle ApplyButtonImageRectangle,
        Size ApplyButtonImageSize,
        Size StatusSize,
        int DockContentDpi,
        Size DockContentSize,
        Size SideToolbarSize,
        Size SideToolbarImageScalingSize,
        int ObjectListViewDpi,
        Size ObjectListViewSize,
        Size ObjectListViewSmallImageSize,
        Size ObjectListViewStateImageSize,
        int MainFontHeight,
        int MenuFontHeight,
        int MainToolbarFontHeight,
        int ApplyButtonFontHeight,
        bool ApplyButtonFontIsBold,
        int StatusFontHeight,
        int SideToolbarFontHeight,
        int ObjectListViewFontHeight,
        int OverviewHeadingFontHeight,
        int DockPaneFontHeight,
        int AutoHideFontHeight,
        bool DockPaneFontUsesPanelFont,
        bool AutoHideFontUsesPanelFont,
        StripMetrics MainMenu,
        StripMetrics FileMenu,
        StripMetrics ModContextMenu,
        StripMetrics ColumnContextMenu,
        StripMetrics ManifestContextMenu,
        Rectangle SideToolbarBounds,
        bool SideToolbarFitsDockContent,
        Rectangle LegendBounds,
        bool LegendDoesNotOverlapList,
        Rectangle PriorityTopBounds,
        Rectangle PriorityBottomBounds,
        int PriorityTopFontHeight,
        int PriorityBottomFontHeight,
        bool PriorityLabelsClearList,
        Rectangle ManifestHeaderBounds,
        bool ManifestHeaderUsesAvailableWidth,
        DockCaptionMetrics[] DockCaptions);

    private readonly record struct StripMetrics(
        int FontHeight,
        Padding Padding,
        Size Size,
        Size PreferredSize,
        int ItemFontHeight,
        Padding ItemPadding,
        Rectangle ItemBounds,
        Rectangle ItemContentRectangle,
        Size ItemPreferredSize);

    private readonly record struct DockCaptionMetrics(
        Rectangle Bounds,
        Size ClientSize,
        int FontHeight,
        bool Visible);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private sealed class DpiToolStripForm : LocForm, ILocAppearanceAware
    {
        private readonly Image _source;

        public DpiToolStripForm(Image source)
        {
            _source = source;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(500, 200);
            ShowInTaskbar = false;

            Button = new ToolStripButton
            {
                Image = source,
                Padding = new Padding(14, 0, 14, 0),
                Size = new Size(70, 42),
                Text = "Apply",
                TextAlign = ContentAlignment.BottomCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
            };
            Toolbar = new ToolStrip
            {
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(500, 45),
            };
            Toolbar.Items.Add(Button);
            Controls.Add(Toolbar);
        }

        public ToolStrip Toolbar { get; }

        public ToolStripButton Button { get; }

        public ToolStripMetrics GetMetrics()
        {
            return new ToolStripMetrics(
                Toolbar.DeviceDpi,
                Toolbar.Size,
                Toolbar.ImageScalingSize,
                Button.Size,
                Button.Padding,
                Button.ContentRectangle,
                Button.Image.Size,
                Toolbar.Font.Height,
                Button.Font.Height);
        }

        void ILocAppearanceAware.ApplyAppearance(
            AppearanceSnapshot appearance)
        {
        }

        void ILocAppearanceAware.ApplyDpi(int oldDpi, int newDpi)
        {
            ToolStripAppearance.ApplyDpi(
                Toolbar,
                newDpi,
                (Button, _source));
        }
    }

}
