using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MW5_Mod_Manager.Controls;

/// <summary>
/// Restores ToolStrip layout metrics from one stable source-DPI snapshot.
/// WinForms rescales the ToolStrip control during a per-monitor transition,
/// but some ToolStrip and ToolStripItem padding, margins, and preferred-size
/// state can remain at the DPI where the objects were constructed.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class ToolStripDpiLayout : IDisposable
{
    private readonly Dictionary<ToolStrip, StripMetrics> _strips = new();
    private readonly Dictionary<ToolStripItem, ItemMetrics> _items = new();
    private readonly HashSet<ToolStripDropDown> _pendingDropDowns = new();
    private readonly System.Windows.Forms.Timer _dropDownCorrectionTimer;
    private readonly ToolStrip _root;
    private readonly bool _scaleStripSpacing;
    private readonly bool _scaleItemMetrics;
    private int _currentDpi;
    private bool _disposed;

    private ToolStripDpiLayout(
        ToolStrip root,
        int sourceDpi,
        bool scaleStripSpacing,
        bool scaleItemMetrics)
    {
        _root = root;
        _scaleStripSpacing = scaleStripSpacing;
        _scaleItemMetrics = scaleItemMetrics;
        _currentDpi = Math.Max(sourceDpi, 96);
        _dropDownCorrectionTimer = new System.Windows.Forms.Timer
        {
            Interval = 1
        };
        _dropDownCorrectionTimer.Tick +=
            DropDownCorrectionTimer_Tick;
        CaptureStrip(root, _currentDpi);
    }

    /// <summary>
    /// Captures the strip's current layout as its stable source-DPI layout.
    /// Capture this after InitializeComponent has assigned designer metrics.
    /// Later item additions are captured automatically.
    /// </summary>
    public static ToolStripDpiLayout Capture(
        ToolStrip root,
        int sourceDpi = 0,
        bool scaleStripSpacing = true,
        bool scaleItemMetrics = true)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (sourceDpi <= 0)
            sourceDpi = root.DeviceDpi;
        return new ToolStripDpiLayout(
            root,
            sourceDpi,
            scaleStripSpacing,
            scaleItemMetrics);
    }

    /// <summary>
    /// Applies deterministic padding, margin, image, and fixed-item metrics,
    /// then recursively lays out every owned dropdown. Fonts, images,
    /// renderers, item state, and command behavior are not changed.
    /// </summary>
    public void Apply(int targetDpi)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _currentDpi = Math.Max(targetDpi, 96);
        if (_root is LocContextMenuStrip contextMenu)
            contextMenu.ReleaseHandleForDpiChange();
        ApplyStrip(_root, _currentDpi);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _dropDownCorrectionTimer.Stop();
        _dropDownCorrectionTimer.Tick -=
            DropDownCorrectionTimer_Tick;
        _dropDownCorrectionTimer.Dispose();
        foreach (ToolStrip strip in _strips.Keys)
        {
            strip.ItemAdded -= Strip_ItemAdded;
            if (strip is ToolStripDropDown dropDown)
                dropDown.Opened -= Strip_Opened;
        }
        _strips.Clear();
        _items.Clear();
        _pendingDropDowns.Clear();
    }

    private void CaptureStrip(ToolStrip strip, int sourceDpi)
    {
        if (_strips.ContainsKey(strip))
            return;

        _strips.Add(strip, new StripMetrics(strip, sourceDpi));
        strip.ItemAdded += Strip_ItemAdded;
        if (strip is ToolStripDropDown dropDown)
            dropDown.Opened += Strip_Opened;
        for (int index = 0; index < strip.Items.Count; index++)
            CaptureItem(strip.Items[index], sourceDpi);
    }

    private void CaptureItem(ToolStripItem item, int sourceDpi)
    {
        if (!_items.ContainsKey(item))
        {
            _items.Add(
                item,
                new ItemMetrics(
                    item,
                    sourceDpi,
                    _scaleItemMetrics));
        }

        // Accessing DropDown also gives empty submenus an ItemAdded observer,
        // so menus populated after form creation receive the same treatment.
        if (item is ToolStripDropDownItem dropDownItem)
            CaptureStrip(dropDownItem.DropDown, sourceDpi);
    }

    private void Strip_ItemAdded(object sender, ToolStripItemEventArgs e)
    {
        CaptureItem(e.Item, _currentDpi);
    }

    private void Strip_Opened(object sender, EventArgs e)
    {
        // ToolStripDropDown performs a delayed scale when its popup handle is
        // created on a monitor with a different DPI. Restore metrics now,
        // then once more after popup creation and font relayout reach idle.
        var dropDown = (ToolStripDropDown)sender;
        ApplyStrip(dropDown, _currentDpi);
        _pendingDropDowns.Add(dropDown);
        _dropDownCorrectionTimer.Stop();
        _dropDownCorrectionTimer.Start();
    }

    private void DropDownCorrectionTimer_Tick(
        object sender,
        EventArgs e)
    {
        _dropDownCorrectionTimer.Stop();
        ToolStripDropDown[] pending = new ToolStripDropDown[
            _pendingDropDowns.Count];
        _pendingDropDowns.CopyTo(pending);
        _pendingDropDowns.Clear();
        foreach (ToolStripDropDown dropDown in pending)
        {
            if (!dropDown.IsDisposed)
                ApplyStrip(dropDown, _currentDpi);
        }
    }

    private void ApplyStrip(ToolStrip strip, int targetDpi)
    {
        // A caller can replace a ToolStripDropDown object after capture.
        // Capture that structural change once when it first becomes visible.
        if (!_strips.TryGetValue(strip, out StripMetrics stripMetrics))
        {
            CaptureStrip(strip, _currentDpi);
            stripMetrics = _strips[strip];
        }

        strip.SuspendLayout();
        try
        {
            stripMetrics.Apply(
                strip,
                targetDpi,
                _scaleStripSpacing);
            for (int index = 0; index < strip.Items.Count; index++)
            {
                ToolStripItem item = strip.Items[index];
                if (!_items.TryGetValue(item, out ItemMetrics itemMetrics))
                {
                    CaptureItem(item, _currentDpi);
                    itemMetrics = _items[item];
                }

                itemMetrics.Apply(item, targetDpi);
                if (item is ToolStripDropDownItem dropDownItem)
                    ApplyStrip(dropDownItem.DropDown, targetDpi);
            }
        }
        finally
        {
            strip.ResumeLayout(true);
        }

        // PerformLayout is intentional even when all assigned values already
        // match. WM_DPICHANGED can leave ToolStrip's preferred-size cache based
        // on the previous DPI without changing a public property value.
        strip.PerformLayout();
        if (strip is ToolStripDropDown dropDown && dropDown.AutoSize)
            dropDown.Size = dropDown.GetPreferredSize(Size.Empty);
        strip.Invalidate();
    }

    private readonly struct StripMetrics
    {
        private readonly DpiPadding _padding;
        private readonly DpiPadding _margin;
        private readonly DpiPadding _gripMargin;
        private readonly DpiSize _imageScalingSize;
        private readonly int _sourceDpi;

        public StripMetrics(ToolStrip strip, int sourceDpi)
        {
            _sourceDpi = sourceDpi;
            _padding = new DpiPadding(strip.Padding, sourceDpi);
            _margin = new DpiPadding(strip.Margin, sourceDpi);
            _gripMargin = new DpiPadding(strip.GripMargin, sourceDpi);
            _imageScalingSize =
                new DpiSize(strip.ImageScalingSize, sourceDpi);
        }

        public void Apply(
            ToolStrip strip,
            int targetDpi,
            bool scaleSpacing)
        {
            strip.Padding =
                _padding.At(targetDpi, _sourceDpi, scaleSpacing);
            strip.Margin =
                _margin.At(targetDpi, _sourceDpi, scaleSpacing);
            strip.GripMargin =
                _gripMargin.At(
                    targetDpi,
                    _sourceDpi,
                    scaleSpacing);
            strip.ImageScalingSize =
                _imageScalingSize.At(targetDpi, _sourceDpi);
        }
    }

    private readonly struct ItemMetrics
    {
        private readonly bool _autoSize;
        private readonly DpiPadding _padding;
        private readonly DpiPadding _margin;
        private readonly DpiSize _fixedSize;
        private readonly int _sourceDpi;
        private readonly bool _scaleMetrics;

        public ItemMetrics(
            ToolStripItem item,
            int sourceDpi,
            bool scaleMetrics)
        {
            _sourceDpi = sourceDpi;
            _scaleMetrics = scaleMetrics;
            _autoSize = item.AutoSize;
            _padding = new DpiPadding(item.Padding, sourceDpi);
            _margin = new DpiPadding(item.Margin, sourceDpi);
            _fixedSize = new DpiSize(item.Size, sourceDpi);
        }

        public void Apply(
            ToolStripItem item,
            int targetDpi)
        {
            item.Padding =
                _padding.At(targetDpi, _sourceDpi, _scaleMetrics);
            item.Margin =
                _margin.At(targetDpi, _sourceDpi, _scaleMetrics);
            if (!_autoSize)
            {
                item.Size =
                    _fixedSize.At(
                        targetDpi,
                        _sourceDpi,
                        _scaleMetrics);
            }
            item.Invalidate();
        }
    }

    private readonly struct DpiPadding
    {
        private readonly Padding _source;
        private readonly Padding _logical;

        public DpiPadding(Padding source, int sourceDpi)
        {
            _source = source;
            _logical = new Padding(
                ToLogical(source.Left, sourceDpi),
                ToLogical(source.Top, sourceDpi),
                ToLogical(source.Right, sourceDpi),
                ToLogical(source.Bottom, sourceDpi));
        }

        public Padding At(
            int targetDpi,
            int sourceDpi,
            bool scale = true)
        {
            if (!scale || targetDpi == sourceDpi)
                return _source;

            return new Padding(
                FromLogical(_logical.Left, targetDpi),
                FromLogical(_logical.Top, targetDpi),
                FromLogical(_logical.Right, targetDpi),
                FromLogical(_logical.Bottom, targetDpi));
        }
    }

    private readonly struct DpiSize
    {
        private readonly Size _source;
        private readonly Size _logical;

        public DpiSize(Size source, int sourceDpi)
        {
            _source = source;
            _logical = new Size(
                ToLogical(source.Width, sourceDpi),
                ToLogical(source.Height, sourceDpi));
        }

        public Size At(
            int targetDpi,
            int sourceDpi,
            bool scale = true)
        {
            if (!scale || targetDpi == sourceDpi)
                return _source;

            return new Size(
                FromLogical(_logical.Width, targetDpi),
                FromLogical(_logical.Height, targetDpi));
        }
    }

    private static int ToLogical(int value, int sourceDpi)
    {
        return Scale(value, 96, sourceDpi);
    }

    private static int FromLogical(int value, int targetDpi)
    {
        return Scale(value, targetDpi, 96);
    }

    private static int Scale(int value, int numerator, int denominator)
    {
        if (value == 0)
            return 0;

        long scaled = (long)value * numerator;
        long rounding = denominator / 2L;
        if (scaled < 0)
            return (int)((scaled - rounding) / denominator);
        return (int)((scaled + rounding) / denominator);
    }
}
