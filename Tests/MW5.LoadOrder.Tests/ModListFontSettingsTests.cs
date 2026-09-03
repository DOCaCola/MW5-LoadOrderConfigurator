using BrightIdeasSoftware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ModListFontSettingsTests
{
    [TestMethod]
    public void CustomFontSizeIsPersistedAndLoaded()
    {
        LocSettings originalSettings = LocSettings.Instance;
        string settingsDirectory = Path.Combine(
            Path.GetTempPath(),
            "MW5-LOC-font-settings-" + Guid.NewGuid().ToString("N"));
        string settingsPath = Path.Combine(
            settingsDirectory,
            LocSettings.SettingsFileName);

        try
        {
            var settings = new LocSettings(settingsPath);
            settings.Data.ModListFontSize = 14;
            settings.SaveSettings();

            JObject savedSettings = JObject.Parse(
                File.ReadAllText(settingsPath));
            Assert.AreEqual(14, (int)savedSettings["ModListFontSize"]);

            var loadedSettings = new LocSettings(settingsPath);
            Assert.AreEqual(14, loadedSettings.Data.ModListFontSize);
        }
        finally
        {
            LocSettings.Instance = originalSettings;
            if (Directory.Exists(settingsDirectory))
                Directory.Delete(settingsDirectory, true);
        }
    }

    [STATestMethod]
    public void CustomFontSizeRemainsDpiAwareAndCanReturnToDefault()
    {
        Application.EnableVisualStyles();
        int originalFontSize =
            LocSettings.Instance.Data.ModListFontSize;

        try
        {
            LocSettings.Instance.Data.ModListFontSize = 14;
            using var form = new DockModListForm();

            Assert.AreEqual(
                14F,
                form.modObjectListView.Font.SizeInPoints,
                0.1F);
            int customRowHeight =
                form.modObjectListView.RowHeightEffective;

            form.ApplyHostDpiFonts(144);
            Assert.AreEqual(
                21F,
                form.modObjectListView.Font.SizeInPoints,
                0.1F);

            LocSettings.Instance.Data.ModListFontSize = 16;
            form.ApplyModListFontSetting(144);
            Assert.AreEqual(
                24F,
                form.modObjectListView.Font.SizeInPoints,
                0.1F);

            LocSettings.Instance.Data.ModListFontSize =
                LocSettings.DefaultModListFontSize;
            form.ApplyModListFontSetting(96);
            Assert.AreEqual(
                Control.DefaultFont.SizeInPoints,
                form.modObjectListView.Font.SizeInPoints,
                0.1F);
            Assert.IsTrue(
                customRowHeight
                    > form.modObjectListView.RowHeightEffective);
        }
        finally
        {
            LocSettings.Instance.Data.ModListFontSize =
                originalFontSize;
        }
    }

    [STATestMethod]
    public void SwitchingAnOpenListToAndFrom18PointKeepsCheckboxImages()
    {
        Application.EnableVisualStyles();
        int originalFontSize =
            LocSettings.Instance.Data.ModListFontSize;

        try
        {
            LocSettings.Instance.Data.ModListFontSize =
                LocSettings.DefaultModListFontSize;
            using var form = new DockModListForm
            {
                ClientSize = new Size(800, 450),
                ShowInTaskbar = false
            };
            using var icon = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(icon))
                graphics.Clear(Color.CornflowerBlue);
            form.imageListIcons.Images.Add("test-icon", icon);
            form.olvColumnModName.ImageGetter = _ => "test-icon";
            form.modObjectListView.BooleanCheckStateGetter = _ => true;
            form.modObjectListView.SetObjects(new object[]
            {
                new ModItem
                {
                    Enabled = true,
                    Name = "Checked row",
                    Path = "checked-row",
                    FolderName = "checked-row"
                }
            });
            form.Show();
            Application.DoEvents();

            Assert.IsNotNull(form.modObjectListView.StateImageList);
            var item = (OLVListItem)form.modObjectListView.Items[0];
            Assert.AreEqual("test-icon", item.ImageKey);
            Assert.AreEqual(
                CheckState.Checked,
                item.CheckState);
            ImageList defaultStateImages =
                form.modObjectListView.StateImageList;

            LocSettings.Instance.Data.ModListFontSize = 18;
            form.ApplyModListFontSetting();
            Application.DoEvents();

            Assert.IsTrue(form.modObjectListView.CheckBoxes);
            Assert.AreEqual(-1, form.modObjectListView.RowHeight);
            Assert.AreNotSame(
                defaultStateImages,
                form.modObjectListView.StateImageList);
            Assert.AreEqual(
                CheckState.Checked,
                item.CheckState);
            Assert.AreEqual(
                1,
                item.StateImageIndex);
            Assert.IsNotNull(form.modObjectListView.StateImageList);
            Assert.AreEqual(
                2,
                form.modObjectListView.StateImageList.Images.Count);
            Assert.AreSame(
                form.imageListIcons,
                form.modObjectListView.BaseSmallImageList);
            Assert.AreEqual("test-icon", item.ImageKey);
            Assert.AreEqual(
                1,
                form.modObjectListView.BaseSmallImageList.Images.Count);
            Assert.IsTrue(HasVisiblePixel(
                form.modObjectListView.StateImageList.Images[0]));
            Assert.IsTrue(HasVisiblePixel(
                form.modObjectListView.StateImageList.Images[1]));
            ImageList largeStateImages =
                form.modObjectListView.StateImageList;

            LocSettings.Instance.Data.ModListFontSize =
                LocSettings.DefaultModListFontSize;
            form.ApplyModListFontSetting();
            Application.DoEvents();

            Assert.AreEqual(-1, form.modObjectListView.RowHeight);
            Assert.AreNotSame(
                largeStateImages,
                form.modObjectListView.StateImageList);
            Assert.AreSame(
                form.imageListIcons,
                form.modObjectListView.BaseSmallImageList);
            Assert.AreEqual("test-icon", item.ImageKey);
            Assert.AreEqual(
                1,
                form.modObjectListView.BaseSmallImageList.Images.Count);
            Assert.AreEqual(
                "test-icon",
                ((OLVListItem)form.modObjectListView.Items[0]).ImageKey);
            Assert.AreEqual(
                CheckState.Checked,
                item.CheckState);
            Assert.AreEqual(1, item.StateImageIndex);
            Assert.AreEqual(
                2,
                form.modObjectListView.StateImageList.Images.Count);
            Assert.IsTrue(HasVisiblePixel(
                form.modObjectListView.StateImageList.Images[0]));
            Assert.IsTrue(HasVisiblePixel(
                form.modObjectListView.StateImageList.Images[1]));
        }
        finally
        {
            LocSettings.Instance.Data.ModListFontSize =
                originalFontSize;
        }
    }

    private static bool HasVisiblePixel(Image image)
    {
        using var bitmap = new Bitmap(image);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A != 0)
                    return true;
            }
        }

        return false;
    }
}
