using DarkModeForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5_Mod_Manager.Controls;

internal readonly record struct AppearanceSnapshot(
    bool DarkMode,
    OSThemeColors Colors);

internal interface ILocAppearanceAware
{
    void ApplyAppearance(AppearanceSnapshot appearance);

    void ApplyDpi(int oldDpi, int newDpi);
}

[SupportedOSPlatform("windows")]
internal static class AppearanceManager
{
    private static readonly Dictionary<Form, FormThemeController> Controllers = new();
    private static bool _initialized;
    private static int _refreshPending;
    private static int _applyingColorMode;
    private static Form _dispatcher;

    public static AppearanceSnapshot Current { get; private set; }

    public static void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;
        Current = CreateSnapshot();
        ApplyWinFormsColorMode(Current.DarkMode);
        LocWindowColors.Apply(Current);

        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        Application.ApplicationExit += (_, _) =>
        {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
            UiImageCache.Dispose();
        };
    }

    public static void Register(Form form)
    {
        Initialize();
        if (Controllers.ContainsKey(form))
            return;

        var controller = new FormThemeController(form);
        Controllers.Add(form, controller);
        form.Disposed += RegisteredForm_Disposed;
        if (_dispatcher == null
            || _dispatcher.IsDisposed
            || _dispatcher is LocDockContent && form is LocForm)
            _dispatcher = form;

        controller.Apply(Current);
        if (form is ILocAppearanceAware appearanceAware)
            appearanceAware.ApplyAppearance(Current);
    }

    public static void SetDarkModeAllowed(bool allowed)
    {
        LocSettings.Instance.Data.AllowDarkMode = allowed;
        RefreshFromSystem(force: true);
    }

    public static void NotifyDpiChanged(Form form, int oldDpi, int newDpi)
    {
        if (oldDpi == newDpi)
            return;

        if (form is ILocAppearanceAware appearanceAware)
            appearanceAware.ApplyDpi(oldDpi, newDpi);
    }

    internal static void RefreshFromSystem(bool force = false)
    {
        AppearanceSnapshot next = CreateSnapshot();
        if (!force && next.DarkMode == Current.DarkMode)
            return;

        Apply(next);
    }

    internal static void ApplyForTests(bool darkMode)
    {
        Initialize();
        Apply(CreateSnapshot(darkMode));
    }

    private static void Apply(AppearanceSnapshot next)
    {
        Current = next;
        ApplyWinFormsColorMode(next.DarkMode);
        LocWindowColors.Apply(next);

        Form[] forms = new Form[Controllers.Count];
        Controllers.Keys.CopyTo(forms, 0);
        foreach (Form form in forms)
        {
            if (form.IsDisposed)
                continue;

            FormThemeController controller = Controllers[form];
            controller.Apply(next);
            if (form is ILocAppearanceAware appearanceAware)
                appearanceAware.ApplyAppearance(next);
        }
    }

    private static AppearanceSnapshot CreateSnapshot()
    {
        bool darkMode = LocSettings.Instance.Data.AllowDarkMode
            && DarkModeCS.GetWindowsColorMode() <= 0;
        return CreateSnapshot(darkMode);
    }

    private static AppearanceSnapshot CreateSnapshot(bool darkMode)
    {
        OSThemeColors colors = new();
        if (darkMode)
        {
            colors.Background = Color.FromArgb(32, 32, 32);
            colors.BackgroundDark = Color.FromArgb(18, 18, 18);
            colors.BackgroundLight =
                ControlPaint.Light(colors.Background);
            colors.Surface = Color.FromArgb(43, 43, 43);
            colors.SurfaceLight = Color.FromArgb(50, 50, 50);
            colors.SurfaceDark = Color.FromArgb(29, 29, 29);
            colors.TextActive = Color.White;
            colors.TextInactive = Color.FromArgb(176, 176, 176);
            colors.Control = Color.FromArgb(55, 55, 55);
            colors.ControlDark = ControlPaint.Dark(colors.Control);
            colors.ControlLight = Color.FromArgb(67, 67, 67);
            colors.Primary = Color.FromArgb(3, 218, 198);
            colors.Secondary = Color.MediumSlateBlue;
        }
        return new AppearanceSnapshot(darkMode, colors);
    }

    private static void ApplyWinFormsColorMode(bool darkMode)
    {
        Interlocked.Increment(ref _applyingColorMode);
        try
        {
            Application.SetColorMode(
                darkMode
                    ? SystemColorMode.Dark
                    : SystemColorMode.Classic);
        }
        finally
        {
            Interlocked.Decrement(ref _applyingColorMode);
        }
    }

    private static void SystemEvents_UserPreferenceChanged(
        object sender,
        UserPreferenceChangedEventArgs e)
    {
        if (Volatile.Read(ref _applyingColorMode) != 0)
            return;

        if (Interlocked.Exchange(ref _refreshPending, 1) != 0)
            return;

        Form dispatcher = _dispatcher;

        if (dispatcher == null
            || dispatcher.IsDisposed
            || dispatcher.Disposing
            || !dispatcher.IsHandleCreated)
        {
            Interlocked.Exchange(ref _refreshPending, 0);
            return;
        }

        try
        {
            dispatcher.BeginInvoke((MethodInvoker)(() =>
            {
                Interlocked.Exchange(ref _refreshPending, 0);
                RefreshFromSystem(force: true);
            }));
        }
        catch (InvalidOperationException)
        {
            Interlocked.Exchange(ref _refreshPending, 0);
        }
    }

    private static void RegisteredForm_Disposed(object sender, EventArgs e)
    {
        var form = (Form)sender;
        form.Disposed -= RegisteredForm_Disposed;
        if (!Controllers.Remove(form, out FormThemeController controller))
            return;

        controller.Dispose();
        if (ReferenceEquals(_dispatcher, form))
        {
            _dispatcher = null;
            foreach (Form candidate in Controllers.Keys)
            {
                if (!candidate.IsDisposed)
                {
                    _dispatcher = candidate;
                    break;
                }
            }
        }
    }
}

[SupportedOSPlatform("windows")]
public class LocForm : Form
{
    private readonly DpiFontCoordinator _dpiFonts = new();

    protected void RegisterDpiFontRoot(Control control)
    {
        _dpiFonts.RegisterRoot(control);
    }

    protected virtual void OnDpiChangeCompleted(
        int oldDpi,
        int newDpi)
    {
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _dpiFonts.Capture(this);
        AppearanceManager.Register(this);
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        _dpiFonts.Capture(this);
        try
        {
            base.OnDpiChanged(e);
            _dpiFonts.Apply(this, e.DeviceDpiNew);
            AppearanceManager.NotifyDpiChanged(
                this,
                e.DeviceDpiOld,
                e.DeviceDpiNew);
        }
        finally
        {
            OnDpiChangeCompleted(e.DeviceDpiOld, e.DeviceDpiNew);
        }
    }

    protected void ReapplyDpiFonts(int targetDpi)
    {
        _dpiFonts.Capture(this);
        _dpiFonts.Apply(this, targetDpi);
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            base.Dispose(disposing);
        }
        finally
        {
            if (disposing)
                _dpiFonts.Dispose();
        }
    }
}

[SupportedOSPlatform("windows")]
public class LocDockContent : DockContent
{
    private readonly DpiFontCoordinator _dpiFonts = new();
    private int _lastAppliedDpi = 96;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _lastAppliedDpi = DeviceDpi;
        _dpiFonts.Capture(this);
        AppearanceManager.Register(this);
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        _dpiFonts.Capture(this);
        base.OnDpiChanged(e);
        NotifyDpiChanged(e.DeviceDpiOld, e.DeviceDpiNew);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        _dpiFonts.Capture(this);
        base.OnDpiChangedAfterParent(e);
        NotifyDpiChanged(_lastAppliedDpi, DeviceDpi);
    }

    private void NotifyDpiChanged(int oldDpi, int newDpi)
    {
        if (_lastAppliedDpi == newDpi)
            return;

        _lastAppliedDpi = newDpi;
        _dpiFonts.Apply(this, newDpi);
        AppearanceManager.NotifyDpiChanged(this, oldDpi, newDpi);
    }

    internal void ApplyHostDpiFonts(int targetDpi)
    {
        _dpiFonts.Capture(this);
        _dpiFonts.Apply(this, targetDpi);
        _lastAppliedDpi = targetDpi;
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            base.Dispose(disposing);
        }
        finally
        {
            if (disposing)
                _dpiFonts.Dispose();
        }
    }
}

[SupportedOSPlatform("windows")]
internal sealed class FormThemeController : IDisposable
{
    private readonly Form _form;
    private readonly Dictionary<Control, ControlThemeState> _states = new();
    private AppearanceSnapshot _appearance;

    public FormThemeController(Form form)
    {
        _form = form;
        Attach(form);
    }

    public void Apply(AppearanceSnapshot appearance)
    {
        _appearance = appearance;
        _form.SuspendLayout();
        try
        {
            ApplyRecursive(_form);
        }
        finally
        {
            _form.ResumeLayout(true);
        }
        _form.Invalidate(true);
    }

    public void Dispose()
    {
        foreach ((Control control, ControlThemeState state) in _states)
        {
            control.ControlAdded -= Control_ControlAdded;
            control.HandleCreated -= Control_HandleCreated;
            if (state.TabDrawHandler != null && control is TabControl tab)
                tab.DrawItem -= state.TabDrawHandler;
        }
        _states.Clear();
    }

    private void Attach(Control control)
    {
        if (_states.ContainsKey(control))
            return;

        var state = new ControlThemeState(control);
        _states.Add(control, state);
        control.ControlAdded += Control_ControlAdded;
        control.HandleCreated += Control_HandleCreated;

        if (control is TabControl tab)
        {
            state.TabDrawHandler = Tab_DrawItem;
            tab.DrawItem += state.TabDrawHandler;
        }

        if (control.ContextMenuStrip != null)
            Attach(control.ContextMenuStrip);
        foreach (Control child in control.Controls)
            Attach(child);
    }

    private void ApplyRecursive(Control control)
    {
        Attach(control);
        ApplyControl(control, _states[control]);
        if (control.ContextMenuStrip != null)
            ApplyRecursive(control.ContextMenuStrip);
        if (control.GetDisableDarkModeChildren())
            return;
        foreach (Control child in control.Controls)
            ApplyRecursive(child);
    }

    private void ApplyControl(Control control, ControlThemeState state)
    {
        if (control.GetDisableDarkMode())
        {
            if (!_appearance.DarkMode)
                state.Restore(control);
            return;
        }

        if (!_appearance.DarkMode)
        {
            state.Restore(control);
            NativeTheme.Apply(control, false);
            return;
        }

        OSThemeColors colors = _appearance.Colors;
        if (state.ThemeBackColor)
            control.BackColor = GetDarkBackColor(control, colors);
        if (state.ThemeForeColor)
            control.ForeColor = control is GroupBox
                ? colors.TextInactive
                : colors.TextActive;

        switch (control)
        {
            case TableLayoutPanel table:
                table.BorderStyle = BorderStyle.None;
                break;
            case Panel panel:
                panel.BorderStyle = BorderStyle.None;
                break;
            case TextBoxBase textBox:
                textBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ListBox listBox:
                listBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case TreeView tree:
                tree.BorderStyle = BorderStyle.None;
                break;
            case Button button:
                button.UseVisualStyleBackColor = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.CheckedBackColor = colors.Accent;
                button.FlatAppearance.BorderColor =
                    ReferenceEquals(_form.AcceptButton, button)
                        ? colors.Accent
                        : colors.Control;
                break;
            case CheckBox checkBox:
                checkBox.UseVisualStyleBackColor = false;
                checkBox.BackColor = checkBox.Parent?.BackColor ?? colors.Control;
                break;
            case RadioButton radioButton:
                radioButton.UseVisualStyleBackColor = false;
                radioButton.BackColor =
                    radioButton.Parent?.BackColor ?? colors.Control;
                break;
            case ComboBox comboBox:
                comboBox.FlatStyle = FlatStyle.Flat;
                break;
            case LinkLabel linkLabel:
                linkLabel.LinkColor = colors.AccentLight;
                linkLabel.ActiveLinkColor = colors.Accent;
                linkLabel.VisitedLinkColor = colors.Primary;
                break;
            case PictureBox pictureBox:
                pictureBox.BorderStyle = BorderStyle.None;
                pictureBox.BackColor =
                    pictureBox.Parent?.BackColor ?? colors.Surface;
                break;
            case TabControl tab:
                tab.Appearance = TabAppearance.Normal;
                tab.DrawMode = TabDrawMode.OwnerDrawFixed;
                break;
            case ToolStrip toolStrip:
                toolStrip.RenderMode = ToolStripRenderMode.Professional;
                toolStrip.Renderer = new MyRenderer(
                    new CustomColorTable(colors),
                    false)
                {
                    MyColors = colors
                };
                break;
        }

        NativeTheme.Apply(control, true);
    }

    private static Color GetDarkBackColor(
        Control control,
        OSThemeColors colors)
    {
        return control switch
        {
            Form => colors.Background,
            TextBoxBase => colors.Surface,
            ListBox => colors.Surface,
            ListView => colors.Surface,
            TreeView => colors.Surface,
            TableLayoutPanel => colors.Surface,
            Panel => colors.Surface,
            GroupBox => control.Parent?.BackColor ?? colors.Background,
            Label => control.Parent?.BackColor ?? colors.Background,
            CheckBox => control.Parent?.BackColor ?? colors.Background,
            RadioButton => control.Parent?.BackColor ?? colors.Background,
            PictureBox => control.Parent?.BackColor ?? colors.Background,
            _ => colors.Control
        };
    }

    private void Control_ControlAdded(object sender, ControlEventArgs e)
    {
        Attach(e.Control);
        ApplyRecursive(e.Control);
    }

    private void Control_HandleCreated(object sender, EventArgs e)
    {
        NativeTheme.Apply((Control)sender, _appearance.DarkMode);
    }

    private void Tab_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (!_appearance.DarkMode)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            return;
        }

        var tab = (TabControl)sender;
        TabPage page = tab.TabPages[e.Index];
        Color backColor = e.Index == tab.SelectedIndex
            ? _appearance.Colors.Surface
            : _appearance.Colors.Background;
        Color textColor = e.Index == tab.SelectedIndex
            ? _appearance.Colors.TextActive
            : _appearance.Colors.TextInactive;
        using var backBrush = new SolidBrush(backColor);
        e.Graphics.FillRectangle(backBrush, e.Bounds);
        TextRenderer.DrawText(
            e.Graphics,
            page.Text,
            page.Font,
            e.Bounds,
            textColor,
            TextFormatFlags.HorizontalCenter
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis);
    }

    private sealed class ControlThemeState
    {
        public ControlThemeState(Control control)
        {
            BackColor = control.BackColor;
            ForeColor = control.ForeColor;
            ThemeBackColor = BackColor.IsEmpty || BackColor.IsSystemColor;
            ThemeForeColor = ForeColor.IsEmpty || ForeColor.IsSystemColor;

            if (control is Panel panel)
                BorderStyle = panel.BorderStyle;
            else if (control is TableLayoutPanel table)
                BorderStyle = table.BorderStyle;
            else if (control is TextBoxBase textBox)
                BorderStyle = textBox.BorderStyle;
            else if (control is ListBox listBox)
                BorderStyle = listBox.BorderStyle;
            else if (control is TreeView tree)
                BorderStyle = tree.BorderStyle;
            else if (control is PictureBox pictureBox)
                BorderStyle = pictureBox.BorderStyle;

            if (control is Button button)
            {
                FlatStyle = button.FlatStyle;
                UseVisualStyleBackColor = button.UseVisualStyleBackColor;
                CheckedBackColor = button.FlatAppearance.CheckedBackColor;
                FlatBorderColor = button.FlatAppearance.BorderColor;
            }
            else if (control is CheckBox checkBox)
            {
                FlatStyle = checkBox.FlatStyle;
                UseVisualStyleBackColor = checkBox.UseVisualStyleBackColor;
            }
            else if (control is RadioButton radioButton)
            {
                FlatStyle = radioButton.FlatStyle;
                UseVisualStyleBackColor = radioButton.UseVisualStyleBackColor;
            }
            else if (control is ComboBox comboBox)
            {
                FlatStyle = comboBox.FlatStyle;
            }

            if (control is LinkLabel link)
            {
                LinkColor = link.LinkColor;
                ActiveLinkColor = link.ActiveLinkColor;
                VisitedLinkColor = link.VisitedLinkColor;
            }

            if (control is TabControl tab)
            {
                TabAppearance = tab.Appearance;
                TabDrawMode = tab.DrawMode;
            }

            if (control is ToolStrip toolStrip)
            {
                ToolStripRenderMode = toolStrip.RenderMode;
                ToolStripRenderer = toolStrip.Renderer;
            }
        }

        public Color BackColor { get; }
        public Color ForeColor { get; }
        public bool ThemeBackColor { get; }
        public bool ThemeForeColor { get; }
        public BorderStyle? BorderStyle { get; }
        public FlatStyle? FlatStyle { get; }
        public bool? UseVisualStyleBackColor { get; }
        public Color? CheckedBackColor { get; }
        public Color? FlatBorderColor { get; }
        public Color? LinkColor { get; }
        public Color? ActiveLinkColor { get; }
        public Color? VisitedLinkColor { get; }
        public TabAppearance? TabAppearance { get; }
        public TabDrawMode? TabDrawMode { get; }
        public ToolStripRenderMode? ToolStripRenderMode { get; }
        public ToolStripRenderer ToolStripRenderer { get; }
        public DrawItemEventHandler TabDrawHandler { get; set; }

        public void Restore(Control control)
        {
            control.BackColor = BackColor;
            control.ForeColor = ForeColor;

            if (BorderStyle.HasValue)
            {
                switch (control)
                {
                    case TableLayoutPanel table:
                        table.BorderStyle = BorderStyle.Value;
                        break;
                    case Panel panel:
                        panel.BorderStyle = BorderStyle.Value;
                        break;
                    case TextBoxBase textBox:
                        textBox.BorderStyle = BorderStyle.Value;
                        break;
                    case ListBox listBox:
                        listBox.BorderStyle = BorderStyle.Value;
                        break;
                    case TreeView tree:
                        tree.BorderStyle = BorderStyle.Value;
                        break;
                    case PictureBox pictureBox:
                        pictureBox.BorderStyle = BorderStyle.Value;
                        break;
                }
            }

            if (FlatStyle.HasValue)
            {
                switch (control)
                {
                    case Button button:
                        button.FlatStyle = FlatStyle.Value;
                        break;
                    case CheckBox checkBox:
                        checkBox.FlatStyle = FlatStyle.Value;
                        break;
                    case RadioButton radioButton:
                        radioButton.FlatStyle = FlatStyle.Value;
                        break;
                    case ComboBox comboBox:
                        comboBox.FlatStyle = FlatStyle.Value;
                        break;
                }
            }

            if (UseVisualStyleBackColor.HasValue)
            {
                switch (control)
                {
                    case Button button:
                        button.UseVisualStyleBackColor =
                            UseVisualStyleBackColor.Value;
                        button.FlatAppearance.CheckedBackColor =
                            CheckedBackColor.Value;
                        button.FlatAppearance.BorderColor =
                            FlatBorderColor.Value;
                        break;
                    case CheckBox checkBox:
                        checkBox.UseVisualStyleBackColor =
                            UseVisualStyleBackColor.Value;
                        break;
                    case RadioButton radioButton:
                        radioButton.UseVisualStyleBackColor =
                            UseVisualStyleBackColor.Value;
                        break;
                }
            }

            if (control is LinkLabel link)
            {
                link.LinkColor = LinkColor.Value;
                link.ActiveLinkColor = ActiveLinkColor.Value;
                link.VisitedLinkColor = VisitedLinkColor.Value;
            }

            if (control is TabControl tab)
            {
                tab.Appearance = TabAppearance.Value;
                tab.DrawMode = TabDrawMode.Value;
            }

            if (control is ToolStrip toolStrip)
            {
                if (ToolStripRenderMode == System.Windows.Forms
                        .ToolStripRenderMode.Custom)
                {
                    toolStrip.Renderer = ToolStripRenderer;
                }
                else
                {
                    toolStrip.RenderMode = ToolStripRenderMode.Value;
                }
            }
        }
    }
}

[SupportedOSPlatform("windows")]
internal static class NativeTheme
{
    private const int ImmersiveDarkModeBefore20H1 = 19;
    private const int ImmersiveDarkMode = 20;

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(
        IntPtr hWnd,
        string subAppName,
        string subIdList);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int attribute,
        ref int value,
        int valueSize);

    public static void Apply(Control control, bool darkMode)
    {
        if (!control.IsHandleCreated)
            return;

        SetWindowTheme(
            control.Handle,
            darkMode ? "DarkMode_Explorer" : null,
            null);

        if (control is Form)
        {
            int enabled = darkMode ? 1 : 0;
            if (DwmSetWindowAttribute(
                    control.Handle,
                    ImmersiveDarkMode,
                    ref enabled,
                    sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(
                    control.Handle,
                    ImmersiveDarkModeBefore20H1,
                    ref enabled,
                    sizeof(int));
            }
        }
    }
}

[SupportedOSPlatform("windows")]
internal static class ToolStripAppearance
{
    public static void Apply(
        ToolStrip toolStrip,
        AppearanceSnapshot appearance)
    {
        toolStrip.RenderMode = ToolStripRenderMode.Professional;
        toolStrip.Renderer = appearance.DarkMode
            ? new MyRenderer(
                new CustomColorTable(appearance.Colors),
                false)
            {
                MyColors = appearance.Colors
            }
            : new ToolStripTransparentRenderer();
    }

    public static void ApplyDpi(
        ToolStrip toolStrip,
        int dpi,
        params (ToolStripItem Item, Image Source)[] images)
    {
        int imageSize = UiImageCache.Scale(16, dpi);
        toolStrip.SuspendLayout();
        try
        {
            toolStrip.ImageScalingSize = new Size(imageSize, imageSize);
            foreach ((ToolStripItem item, Image source) in images)
                item.Image = UiImageCache.Get(source, 16, dpi);
        }
        finally
        {
            toolStrip.ResumeLayout(true);
        }

        // ToolStrip resets ImageScalingSize directly while processing
        // WM_DPICHANGED_BEFOREPARENT. If the value we assign above is already
        // equal, its property setter does not invalidate cached item layout.
        toolStrip.PerformLayout();
        toolStrip.Invalidate();
    }
}

[SupportedOSPlatform("windows")]
internal static class UiImageCache
{
    private static readonly Dictionary<ImageCacheKey, Bitmap> Images = new();

    public static Image Get(Image source, int logicalSize, int dpi)
    {
        int pixelSize = Scale(logicalSize, dpi);
        if (source.Width == pixelSize && source.Height == pixelSize)
            return source;

        var key = new ImageCacheKey(source, pixelSize);
        if (Images.TryGetValue(key, out Bitmap cached))
            return cached;

        var bitmap = new Bitmap(
            pixelSize,
            pixelSize,
            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CompositingQuality =
            System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode =
            System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode =
            System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        graphics.DrawImage(
            source,
            new Rectangle(0, 0, pixelSize, pixelSize));
        Images.Add(key, bitmap);
        return bitmap;
    }

    public static int Scale(int value, int dpi)
    {
        return Math.Max(1, (value * dpi + 48) / 96);
    }

    public static void Dispose()
    {
        foreach (Bitmap image in Images.Values)
            image.Dispose();
        Images.Clear();
    }

    private readonly record struct ImageCacheKey(Image Source, int PixelSize);
}
