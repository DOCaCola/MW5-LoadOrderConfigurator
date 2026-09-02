using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using MW5_Mod_Manager.Controls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class DockPaneCaptionTests
{
    [STATestMethod]
    public void RenderingPreservesSharedThemePensAndRtlLayoutRoundTrips()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();

        using var form = new MainForm
        {
            ShowInTaskbar = false,
        };
        form.Show();
        Application.DoEvents();

        LocDockPaneCaption caption = Descendants(form.dockPanel1)
            .OfType<LocDockPaneCaption>()
            .First(control => control.Width > 0 && control.Height > 0);
        ThemeBase theme = form.dockPanel1.Theme;
        ToolWindowCaptionPalette palette = caption.DockPane.IsActivePane
            ? theme.ColorPalette.ToolWindowCaptionActive
            : theme.ColorPalette.ToolWindowCaptionInactive;
        Pen sharedGripPen = theme.PaintingService.GetPen(palette.Grip, 1);
        var originalButtonBounds = caption.Controls
            .Cast<Control>()
            .ToDictionary(control => control, control => control.Bounds);

        using (var bitmap = new Bitmap(caption.Width, caption.Height))
            caption.DrawToBitmap(bitmap, caption.ClientRectangle);

        Assert.AreSame(
            sharedGripPen,
            theme.PaintingService.GetPen(palette.Grip, 1));
        Assert.AreEqual(
            System.Drawing.Drawing2D.DashStyle.Solid,
            sharedGripPen.DashStyle);

        caption.RightToLeft = RightToLeft.Yes;
        caption.PerformLayout();
        foreach ((Control button, Rectangle originalBounds)
            in originalButtonBounds)
        {
            Assert.AreEqual(
                caption.ClientRectangle.Right - originalBounds.Right,
                button.Left);
            Assert.AreEqual(originalBounds.Size, button.Size);
        }

        caption.RightToLeft = RightToLeft.No;
        caption.PerformLayout();
        foreach ((Control button, Rectangle originalBounds)
            in originalButtonBounds)
        {
            Assert.AreEqual(originalBounds, button.Bounds);
        }
    }

    private static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (Control descendant in Descendants(child))
                yield return descendant;
        }
    }
}
