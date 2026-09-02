using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class ObjectListViewDpiTests
{
    [STATestMethod]
    public void DpiAwareNativeImageListsScaleWithoutOwnerDrawing()
    {
        Application.EnableVisualStyles();
        using var sourceImages = new ImageList
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new Size(16, 16),
        };
        using var firstImage = CreateTestImage(Color.Gold);
        sourceImages.Images.Add("first", firstImage);

        using var list = new TestDpiObjectListView(96)
        {
            CheckBoxes = true,
            Dock = DockStyle.Fill,
            OwnerDraw = false,
            RowHeight = 18,
            SmallImageList = sourceImages,
            UseDpiAwareImageLists = true,
            View = View.Details,
        };
        var column = new OLVColumn("Value", null)
        {
            AspectGetter = model => model,
            Width = 200,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
        list.SetObjects(new object[] { "first" });

        using var host = new Form
        {
            ClientSize = new Size(400, 200),
            ShowInTaskbar = false,
        };
        host.Controls.Add(list);
        list.TestDpi = 144;
        list.ApplyParentDpiChangeForTest();
        host.Show();
        Application.DoEvents();

        Assert.AreEqual(18, list.RowHeight);
        Assert.AreEqual(new Size(16, 16), sourceImages.ImageSize);
        Assert.AreEqual(new Size(24, 24), list.BaseSmallImageList.ImageSize);
        Assert.AreEqual(1, list.BaseSmallImageList.Images.Count);
        Assert.AreEqual("first", list.BaseSmallImageList.Images.Keys[0]);
        Assert.AreEqual(new Size(24, 24), list.StateImageList.ImageSize);
        Assert.AreEqual(2, list.StateImageList.Images.Count);
        Assert.IsFalse(list.OwnerDraw);
        Size checkboxAt144Dpi = GetVisibleBounds(
            list.StateImageList.Images[0]).Size;

        using var secondImage = CreateTestImage(Color.MediumPurple);
        sourceImages.Images.Add("second", secondImage);
        list.RefreshDpiAwareImageLists();

        Assert.AreEqual(new Size(24, 24), list.BaseSmallImageList.ImageSize);
        Assert.AreEqual(2, list.BaseSmallImageList.Images.Count);
        Assert.AreEqual("second", list.BaseSmallImageList.Images.Keys[1]);

        list.TestDpi = 192;
        list.RefreshDpiAwareImageLists();

        Assert.AreEqual(new Size(16, 16), sourceImages.ImageSize);
        Assert.AreEqual(new Size(32, 32), list.BaseSmallImageList.ImageSize);
        Assert.AreEqual(2, list.BaseSmallImageList.Images.Count);
        Assert.AreEqual(new Size(32, 32), list.StateImageList.ImageSize);
        Assert.AreEqual(2, list.StateImageList.Images.Count);
        Assert.IsFalse(list.OwnerDraw);
        Size checkboxAt192Dpi = GetVisibleBounds(
            list.StateImageList.Images[0]).Size;
        // UxTheme only guarantees DPI-specific assets for DPI values used by
        // connected displays. Simulated DPI values can therefore share the same
        // glyph asset even though the containing native image list scales.
        Assert.IsTrue(checkboxAt144Dpi.Width > 0 && checkboxAt144Dpi.Width <= 24);
        Assert.IsTrue(checkboxAt144Dpi.Height > 0 && checkboxAt144Dpi.Height <= 24);
        Assert.IsTrue(checkboxAt192Dpi.Width > 0 && checkboxAt192Dpi.Width <= 32);
        Assert.IsTrue(checkboxAt192Dpi.Height > 0 && checkboxAt192Dpi.Height <= 32);
    }

    private static Bitmap CreateTestImage(Color color)
    {
        var bitmap = new Bitmap(16, 16);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(color);
        return bitmap;
    }

    private static Rectangle GetVisibleBounds(Image image)
    {
        using var bitmap = new Bitmap(image);
        int left = bitmap.Width;
        int top = bitmap.Height;
        int right = -1;
        int bottom = -1;

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A == 0)
                    continue;

                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }

        Assert.IsTrue(right >= left);
        Assert.IsTrue(bottom >= top);
        return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
    }

    private sealed class TestDpiObjectListView : ObjectListView
    {
        public TestDpiObjectListView(int dpi)
        {
            TestDpi = dpi;
        }

        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TestDpi { get; set; }

        protected override int GetImageListDpi()
        {
            return TestDpi;
        }

        public void ApplyParentDpiChangeForTest()
        {
            OnDpiChangedAfterParent(EventArgs.Empty);
        }
    }
}
