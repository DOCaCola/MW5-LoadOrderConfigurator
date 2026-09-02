using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using MW5_Mod_Manager.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class DpiViewStateTests
{
    [TestMethod]
    public void LogicalViewStateMeasurementsScaleToTargetDpi()
    {
        Assert.AreEqual(150, LocViewState.ScaleForDpi(100, 96, 144));
        Assert.AreEqual(
            new Rectangle(15, 30, 450, 300),
            LocViewState.ScaleForDpi(
                new Rectangle(10, 20, 300, 200),
                96,
                144));
        Assert.AreEqual(
            new Rectangle(-1200, 80, 450, 300),
            LocViewState.RestoreWindowBounds(
                new Rectangle(-1200, 80, 300, 200),
                96,
                144));
        Assert.AreEqual(
            new Rectangle(-1200, 80, 300, 200),
            LocViewState.NormalizeWindowBounds(
                new Rectangle(-1200, 80, 450, 300),
                144));
    }

    [TestMethod]
    public void LegacyViewStateDoesNotClaimLogicalDpiUnits()
    {
        LocViewState.ViewStateData legacyState =
            JObject.Parse(
                    """
                    {
                      "WindowMaximized": false,
                      "WindowPosition": "10, 20, 800, 600"
                    }
                    """)
                .ToObject<LocViewState.ViewStateData>();

        Assert.AreEqual(0, legacyState.SchemaVersion);
        Assert.AreEqual(0, legacyState.SavedDpi);
    }

    [STATestMethod]
    public void RotatingLabelDoesNotChangeLayoutDuringPaint()
    {
        using var host = new Form();
        using var label = new RotatingLabel
        {
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            NewText = "High priority",
            RotateAngle = -90
        };
        host.Controls.Add(label);
        _ = host.Handle;
        Size sizeBeforePaint = label.Size;
        Point locationBeforePaint = label.Location;

        using var bitmap = new Bitmap(
            Math.Max(1, label.Width),
            Math.Max(1, label.Height));
        label.DrawToBitmap(bitmap, label.ClientRectangle);

        Assert.AreEqual(sizeBeforePaint, label.Size);
        Assert.AreEqual(locationBeforePaint, label.Location);
    }

    [STATestMethod]
    public void AllApplicationFormsUseDpiAutoscaling()
    {
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();

        var forms = new List<Form>
        {
            new AboutForm(),
            new DirectLaunchForm(),
            new DockConflictsForm(),
            new DockModListForm(),
            new DockOverviewForm(),
            new ExportForm(),
            new ExtractForm(),
            new ImportForm(),
            new ModCheckForm(),
            new PresetDeleteForm(),
            new PresetSaveForm(),
            new SettingsForm()
        };

        try
        {
            foreach (Form form in forms)
            {
                Assert.AreEqual(
                    AutoScaleMode.Dpi,
                    form.AutoScaleMode,
                    form.GetType().Name);
            }
        }
        finally
        {
            foreach (Form form in forms)
                form.Dispose();
        }
    }

    [STATestMethod]
    public void DialogToolStripImagesScaleForMonitorDpi()
    {
        Application.EnableVisualStyles();
        AppearanceManager.Initialize();

        using var form = new ImportForm();
        var appearanceAware = (ILocAppearanceAware)form;
        appearanceAware.ApplyDpi(96, 144);

        ToolStrip toolStrip = form.Controls
            .OfType<ToolStrip>()
            .Single();
        Assert.AreEqual(new Size(24, 24), toolStrip.ImageScalingSize);
        foreach (ToolStripItem item in toolStrip.Items)
            Assert.AreEqual(new Size(24, 24), item.Image.Size);
    }
}
