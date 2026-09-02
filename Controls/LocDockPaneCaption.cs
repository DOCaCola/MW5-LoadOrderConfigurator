using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WeifenLuo.WinFormsUI.ThemeVS2012;

namespace MW5_Mod_Manager.Controls;

[SupportedOSPlatform("windows")]
internal sealed class LocDockPaneCaptionFactory
    : DockPanelExtender.IDockPaneCaptionFactory
{
    public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
    {
        return new LocDockPaneCaption(pane);
    }
}

[SupportedOSPlatform("windows")]
[ToolboxItem(false)]
internal sealed class LocDockPaneCaption : DockPaneCaptionBase
{
    private readonly ToolTip _toolTip = new();
    private VS2012DockPaneCaptionInertButton _closeButton;
    private VS2012DockPaneCaptionInertButton _autoHideButton;
    private VS2012DockPaneCaptionInertButton _optionsButton;

    private Font TextFont =>
        DockPane.DockPanel.Theme.Skin.DockPaneStripSkin.TextFont;

    private bool CloseButtonEnabled =>
        DockPane.ActiveContent?.DockHandler.CloseButton == true;

    private bool CloseButtonVisible =>
        DockPane.ActiveContent?.DockHandler.CloseButtonVisible == true;

    private bool ShouldShowAutoHideButton => !DockPane.IsFloat;

    protected override bool CanDragAutoHide => true;

    public LocDockPaneCaption(DockPane pane)
        : base(pane)
    {
        SuspendLayout();
        _ = CloseButton;
        _ = AutoHideButton;
        _ = OptionsButton;
        ResumeLayout();
    }

    private VS2012DockPaneCaptionInertButton CloseButton =>
        _closeButton ??= CreateButton(
            DockPane.DockPanel.Theme.ImageService.DockPaneHover_Close,
            DockPane.DockPanel.Theme.ImageService.DockPane_Close,
            DockPane.DockPanel.Theme.ImageService.DockPanePress_Close,
            DockPane.DockPanel.Theme.ImageService.DockPaneActiveHover_Close,
            DockPane.DockPanel.Theme.ImageService.DockPaneActive_Close,
            "Close",
            (_, _) => DockPane.CloseActiveContent());

    private VS2012DockPaneCaptionInertButton AutoHideButton =>
        _autoHideButton ??= CreateButton(
            DockPane.DockPanel.Theme.ImageService.DockPaneHover_Dock,
            DockPane.DockPanel.Theme.ImageService.DockPane_Dock,
            DockPane.DockPanel.Theme.ImageService.DockPanePress_Dock,
            DockPane.DockPanel.Theme.ImageService.DockPaneActiveHover_Dock,
            DockPane.DockPanel.Theme.ImageService.DockPaneActive_Dock,
            "Auto Hide",
            AutoHide_Click,
            DockPane.DockPanel.Theme.ImageService
                .DockPaneActiveHover_AutoHide,
            DockPane.DockPanel.Theme.ImageService.DockPaneActive_AutoHide,
            DockPane.DockPanel.Theme.ImageService
                .DockPanePress_AutoHide);

    private VS2012DockPaneCaptionInertButton OptionsButton =>
        _optionsButton ??= CreateButton(
            DockPane.DockPanel.Theme.ImageService.DockPaneHover_Option,
            DockPane.DockPanel.Theme.ImageService.DockPane_Option,
            DockPane.DockPanel.Theme.ImageService.DockPanePress_Option,
            DockPane.DockPanel.Theme.ImageService
                .DockPaneActiveHover_Option,
            DockPane.DockPanel.Theme.ImageService.DockPaneActive_Option,
            "Window Position",
            (_, _) => ShowTabPageContextMenu(
                PointToClient(Control.MousePosition)));

    protected override int MeasureHeight()
    {
        int dpi = GetTargetDpi();
        return Math.Max(
            TextFont.Height + Scale(5, dpi),
            Scale(23, dpi));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        DrawCaption(e.Graphics);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        SetButtons();
        base.OnLayout(e);
    }

    protected override void OnRefreshChanges()
    {
        SetButtons();
        Invalidate();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        PerformLayout();
        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        PerformLayout();
        Invalidate();
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _toolTip.Dispose();
        base.Dispose(disposing);
    }

    private VS2012DockPaneCaptionInertButton CreateButton(
        Bitmap hovered,
        Bitmap normal,
        Bitmap pressed,
        Bitmap hoveredActive,
        Bitmap active,
        string toolTip,
        EventHandler click,
        Bitmap hoveredAutoHide = null,
        Bitmap autoHide = null,
        Bitmap pressedAutoHide = null)
    {
        var button = new VS2012DockPaneCaptionInertButton(
            this,
            hovered,
            normal,
            pressed,
            hoveredActive,
            active,
            hoveredAutoHide,
            autoHide,
            pressedAutoHide);
        button.Click += click;
        Controls.Add(button);
        _toolTip.SetToolTip(button, toolTip);
        return button;
    }

    private void AutoHide_Click(object sender, EventArgs e)
    {
        DockPane.DockState = DockHelper.ToggleAutoHideState(
            DockPane.DockState);
        if (!DockHelper.IsDockStateAutoHide(DockPane.DockState))
            return;

        DockPane.DockPanel.ActiveAutoHideContent = null;
        DockPane.NestedDockingStatus.NestedPanes
            .SwitchPaneWithFirstChild(DockPane);
    }

    private void SetButtons()
    {
        CloseButton.Enabled = CloseButtonEnabled;
        CloseButton.Visible = CloseButtonVisible;
        AutoHideButton.Visible = ShouldShowAutoHideButton;
        OptionsButton.Visible = HasTabPageContextMenu;
        CloseButton.RefreshChanges();
        AutoHideButton.RefreshChanges();
        OptionsButton.RefreshChanges();
        SetButtonPositions();
    }

    private void SetButtonPositions()
    {
        int dpi = GetTargetDpi();
        int glyphSize = Scale(16, dpi);
        int rightGap = Scale(5, dpi);
        int topGap = Scale(4, dpi);
        int betweenGap = Scale(1, dpi);
        var bounds = new Rectangle(
            ClientRectangle.Right - rightGap - glyphSize,
            ClientRectangle.Top + topGap,
            glyphSize,
            glyphSize);

        CloseButton.Bounds = RtlTransform(bounds);
        if (CloseButtonVisible)
            bounds.X -= glyphSize + betweenGap;
        AutoHideButton.Bounds = RtlTransform(bounds);
        if (ShouldShowAutoHideButton)
            bounds.X -= glyphSize + betweenGap;
        OptionsButton.Bounds = RtlTransform(bounds);
    }

    private void DrawCaption(Graphics graphics)
    {
        if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
            return;

        ThemeBase theme = DockPane.DockPanel.Theme;
        ToolWindowCaptionPalette palette = DockPane.IsActivePane
            ? theme.ColorPalette.ToolWindowCaptionActive
            : theme.ColorPalette.ToolWindowCaptionInactive;
        graphics.FillRectangle(
            theme.PaintingService.GetBrush(palette.Background),
            ClientRectangle);

        Color border = theme.ColorPalette.ToolWindowBorder;
        Pen borderPen = theme.PaintingService.GetPen(border, 1);
        graphics.DrawLine(
            borderPen,
            ClientRectangle.Left,
            ClientRectangle.Top,
            ClientRectangle.Left,
            ClientRectangle.Bottom);
        graphics.DrawLine(
            borderPen,
            ClientRectangle.Left,
            ClientRectangle.Top,
            ClientRectangle.Right,
            ClientRectangle.Top);
        graphics.DrawLine(
            borderPen,
            ClientRectangle.Right - 1,
            ClientRectangle.Top,
            ClientRectangle.Right - 1,
            ClientRectangle.Bottom);

        int dpi = GetTargetDpi();
        Rectangle textBounds = ClientRectangle;
        textBounds.X += Scale(2, dpi);
        textBounds.Width -= Scale(5, dpi);
        textBounds.Width -= Scale(1, dpi)
            + CloseButton.Width
            + Scale(5, dpi);
        if (ShouldShowAutoHideButton)
            textBounds.Width -= AutoHideButton.Width + Scale(1, dpi);
        if (HasTabPageContextMenu)
            textBounds.Width -= OptionsButton.Width + Scale(1, dpi);
        textBounds.Y += Scale(3, dpi);
        textBounds.Height -= Scale(5, dpi);

        TextFormatFlags format =
            TextFormatFlags.EndEllipsis
            | TextFormatFlags.SingleLine
            | TextFormatFlags.VerticalCenter;
        if (RightToLeft == RightToLeft.Yes)
            format |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
        Rectangle renderedTextBounds = RtlTransform(textBounds);
        TextRenderer.DrawText(
            graphics,
            DockPane.CaptionText,
            TextFont,
            renderedTextBounds,
            palette.Text,
            format);

        int textWidth = TextRenderer.MeasureText(
            graphics,
            DockPane.CaptionText,
            TextFont,
            Size.Empty,
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
        Rectangle gripBounds = renderedTextBounds;
        int occupiedTextWidth = Math.Min(textWidth, gripBounds.Width);
        gripBounds.Width -= occupiedTextWidth;
        if (RightToLeft != RightToLeft.Yes)
            gripBounds.X += occupiedTextWidth;
        DrawGrip(graphics, gripBounds, palette.Grip, dpi);
    }

    private void DrawGrip(
        Graphics graphics,
        Rectangle bounds,
        Color color,
        int dpi)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        using var pen = new Pen(color);
        pen.DashStyle = DashStyle.Custom;
        float dot = Scale(1, dpi);
        pen.DashPattern = new[] { dot, Scale(3, dpi) };
        int center = bounds.Top + bounds.Height / 2;
        int inset = Scale(2, dpi);
        int spacing = Scale(2, dpi);
        graphics.DrawLine(
            pen,
            bounds.Left + inset,
            center,
            bounds.Right - inset,
            center);
        graphics.DrawLine(
            pen,
            bounds.Left,
            center - spacing,
            bounds.Right,
            center - spacing);
        graphics.DrawLine(
            pen,
            bounds.Left,
            center + spacing,
            bounds.Right,
            center + spacing);
    }

    private Rectangle RtlTransform(Rectangle bounds)
    {
        if (RightToLeft != RightToLeft.Yes)
            return bounds;
        return new Rectangle(
            ClientRectangle.Right - bounds.Right,
            bounds.Y,
            bounds.Width,
            bounds.Height);
    }

    private int GetTargetDpi()
    {
        return Math.Max(
            IsHandleCreated ? DeviceDpi : DockPane.DockPanel.DeviceDpi,
            96);
    }

    private static int Scale(int value, int dpi)
    {
        return UiImageCache.Scale(value, dpi);
    }
}
