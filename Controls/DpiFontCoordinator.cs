using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MW5_Mod_Manager.Controls;

[SupportedOSPlatform("windows")]
internal sealed class DpiFontCoordinator : IDisposable
{
    private readonly List<ControlFontBinding> _bindings = new();
    private readonly List<ToolStripItemFontBinding> _itemBindings = new();
    private readonly HashSet<Control> _additionalRoots = new();
    private readonly Dictionary<FontCacheKey, Font> _fontCache = new();
    private readonly Dictionary<Control, ControlFontBinding> _controlBindings =
        new();
    private readonly HashSet<Control> _trackedControls = new();
    private readonly HashSet<ToolStripItem> _trackedItems = new();
    private readonly HashSet<ToolStrip> _trackedToolStrips = new();
    private int _currentDpi = 96;

    public void Capture(Control root)
    {
        int sourceDpi = Math.Max(root.DeviceDpi, 96);
        if (_bindings.Count == 0)
            _currentDpi = sourceDpi;
        CaptureControl(root, root, true, sourceDpi);
        foreach (Control additionalRoot in _additionalRoots)
            CaptureControl(additionalRoot, additionalRoot, true, sourceDpi);
    }

    public void RegisterRoot(Control root)
    {
        _additionalRoots.Add(root);
    }

    public void Apply(Control layoutRoot, int targetDpi)
    {
        _currentDpi = Math.Max(targetDpi, 96);
        layoutRoot.SuspendLayout();
        try
        {
            ApplyBindings(_currentDpi);
        }
        finally
        {
            layoutRoot.ResumeLayout(true);
        }

        RelayoutToolStrips();
        layoutRoot.Invalidate(true);
    }

    public void Dispose()
    {
        // DockPanelSuite and native WinForms layout code can retain a Font
        // reference beyond the WM_DPICHANGED callback. Keep one reusable
        // instance per logical font/DPI pair alive for the owning form's
        // lifetime instead of retiring fonts after each transition.
        foreach (Font font in _fontCache.Values)
            font.Dispose();

        _bindings.Clear();
        _itemBindings.Clear();
        _fontCache.Clear();
        _controlBindings.Clear();
        _additionalRoots.Clear();
        _trackedControls.Clear();
        _trackedItems.Clear();
        foreach (ToolStrip toolStrip in _trackedToolStrips)
        {
            if (toolStrip is ToolStripDropDown dropDown)
                dropDown.Opened -= ToolStripDropDown_Opened;
        }
        _trackedToolStrips.Clear();
    }

    private Font GetFont(DpiFontDescriptor descriptor, int targetDpi)
    {
        targetDpi = Math.Max(targetDpi, 96);
        var key = new FontCacheKey(descriptor, targetDpi);
        if (_fontCache.TryGetValue(key, out Font font))
            return font;

        font = descriptor.CreateFont(targetDpi);
        _fontCache.Add(key, font);
        return font;
    }

    public void SetFont(
        Control control,
        DpiFontDescriptor descriptor,
        int targetDpi)
    {
        if (!_controlBindings.TryGetValue(
                control,
                out ControlFontBinding binding))
        {
            binding = new ControlFontBinding(control, descriptor);
            _trackedControls.Add(control);
            _bindings.Add(binding);
            _controlBindings.Add(control, binding);
        }
        else
        {
            binding.UpdateDescriptor(descriptor);
        }

        targetDpi = Math.Max(targetDpi, 96);
        _currentDpi = targetDpi;
        binding.Apply(GetFont(descriptor, targetDpi), targetDpi);
        control.Invalidate();
    }

    private void CaptureControl(
        Control control,
        Control formRoot,
        bool forceCapture,
        int sourceDpi)
    {
        if (control.IsDisposed)
            return;

        bool shouldCapture = forceCapture
            || control is ToolStrip
            || control.Parent == null
            || HasExplicitFont(control);
        if (shouldCapture && _trackedControls.Add(control))
        {
            var binding = new ControlFontBinding(
                control,
                DpiFontDescriptor.Capture(control.Font, sourceDpi));
            _bindings.Add(binding);
            _controlBindings.Add(control, binding);
        }

        if (control is ToolStrip toolStrip)
            CaptureToolStrip(toolStrip, sourceDpi);

        if (control.ContextMenuStrip != null)
        {
            CaptureControl(
                control.ContextMenuStrip,
                control.ContextMenuStrip,
                true,
                sourceDpi);
        }

        foreach (Control child in control.Controls)
        {
            if (child is Form && !ReferenceEquals(child, formRoot))
                continue;

            CaptureControl(child, formRoot, false, sourceDpi);
        }
    }

    private void CaptureToolStrip(ToolStrip toolStrip, int sourceDpi)
    {
        if (_trackedToolStrips.Add(toolStrip))
        {
            if (toolStrip is ToolStripDropDown dropDown)
                dropDown.Opened += ToolStripDropDown_Opened;
        }
        foreach (ToolStripItem item in toolStrip.Items)
            CaptureToolStripItem(item, sourceDpi);
    }

    private void ToolStripDropDown_Opened(object sender, EventArgs e)
    {
        // A hidden ToolStripDropDown keeps the DPI of the monitor where its
        // popup handle was last shown. WinForms scales it when that handle is
        // recreated, which can double-scale fonts already updated with the
        // owning form. Its delayed native layout can run after Opened, so
        // reassert once now and once on the next message-loop turn.
        ApplyBindings(_currentDpi);
        RelayoutToolStrips();

        var dropDown = (ToolStripDropDown)sender;
        dropDown.BeginInvoke((Action)(() =>
        {
            if (dropDown.IsDisposed)
                return;
            ApplyBindings(_currentDpi);
            RelayoutToolStrips();
        }));
    }

    private void ApplyBindings(int targetDpi)
    {
        foreach (ControlFontBinding binding in _bindings)
            binding.Apply(GetFont(binding.Descriptor, targetDpi), targetDpi);

        foreach (ToolStripItemFontBinding binding in _itemBindings)
            binding.Apply(GetFont(binding.Descriptor, targetDpi), targetDpi);
    }

    private void CaptureToolStripItem(ToolStripItem item, int sourceDpi)
    {
        if (HasExplicitFont(item) && _trackedItems.Add(item))
        {
            _itemBindings.Add(new ToolStripItemFontBinding(
                item,
                DpiFontDescriptor.Capture(item.Font, sourceDpi)));
        }

        if (item is not ToolStripDropDownItem dropDownItem)
            return;

        ToolStripDropDown dropDown = dropDownItem.DropDown;
        CaptureControl(dropDown, dropDown, true, sourceDpi);
    }

    private static bool HasExplicitFont(object component)
    {
        PropertyDescriptor property =
            TypeDescriptor.GetProperties(component)[nameof(Control.Font)];
        return property?.ShouldSerializeValue(component) == true;
    }

    private void RelayoutToolStrips()
    {
        foreach (ToolStrip toolStrip in _trackedToolStrips)
        {
            if (toolStrip.IsDisposed)
                continue;

            toolStrip.PerformLayout();
            if (toolStrip is ToolStripDropDown dropDown
                && dropDown.AutoSize)
            {
                dropDown.Size = dropDown.GetPreferredSize(Size.Empty);
            }
            toolStrip.Invalidate();
        }
    }

    private sealed class ControlFontBinding
    {
        private readonly Control _control;
        public DpiFontDescriptor Descriptor { get; private set; }

        public ControlFontBinding(
            Control control,
            DpiFontDescriptor descriptor)
        {
            _control = control;
            Descriptor = descriptor;
            _appliedDpi = descriptor.SourceDpi;
        }

        public void UpdateDescriptor(DpiFontDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public void Apply(Font replacement, int targetDpi)
        {
            targetDpi = Math.Max(targetDpi, 96);
            if (_control.IsDisposed
                || _appliedDpi == targetDpi
                    && ReferenceEquals(_control.Font, replacement))
                return;

            _control.Font = replacement;
            _appliedDpi = targetDpi;
        }

        private int _appliedDpi;
    }

    private sealed class ToolStripItemFontBinding
    {
        private readonly ToolStripItem _item;
        public DpiFontDescriptor Descriptor { get; }
        private int _appliedDpi;

        public ToolStripItemFontBinding(
            ToolStripItem item,
            DpiFontDescriptor descriptor)
        {
            _item = item;
            Descriptor = descriptor;
            _appliedDpi = descriptor.SourceDpi;
        }

        public void Apply(Font replacement, int targetDpi)
        {
            targetDpi = Math.Max(targetDpi, 96);
            if (_item.IsDisposed
                || _appliedDpi == targetDpi
                    && ReferenceEquals(_item.Font, replacement))
                return;

            _item.Font = replacement;
            _appliedDpi = targetDpi;
        }
    }

    private readonly record struct FontCacheKey(
        DpiFontDescriptor Descriptor,
        int TargetDpi);

}

internal readonly record struct DpiFontDescriptor(
    string Name,
    float SizeInPoints,
    FontStyle Style,
    byte GdiCharSet,
    bool GdiVerticalFont,
    int SourceDpi)
{
    public static DpiFontDescriptor Capture(Font font, int sourceDpi)
    {
        return new DpiFontDescriptor(
            font.Name,
            font.SizeInPoints,
            font.Style,
            font.GdiCharSet,
            font.GdiVerticalFont,
            Math.Max(sourceDpi, 96));
    }

    public Font CreateFont(int targetDpi)
    {
        // System.Drawing binds Font.Height to the process startup DPI.
        // Scale the point size from the monitor where the descriptor was
        // captured so the native pixel height follows the target monitor.
        float targetSize = SizeInPoints
            * Math.Max(targetDpi, 96)
            / SourceDpi;
        return new Font(
            Name,
            targetSize,
            Style,
            GraphicsUnit.Point,
            GdiCharSet,
            GdiVerticalFont);
    }
}
