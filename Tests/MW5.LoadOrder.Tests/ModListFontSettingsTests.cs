using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using Newtonsoft.Json.Linq;
using System;
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
}
