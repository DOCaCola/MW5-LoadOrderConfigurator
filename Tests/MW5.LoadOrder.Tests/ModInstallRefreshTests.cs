using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.IO;
using System.Linq;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ModInstallRefreshTests
{
    [STATestMethod]
    public void ForcedPostInstallRefreshBuildsCurrentListBeforeRestoringProfile()
    {
        string testRoot = Path.Combine(
            Path.GetTempPath(),
            "MW5-LOC-InstallRefresh-" + Guid.NewGuid().ToString("N"));
        string settingsDirectory = Path.Combine(testRoot, "Settings");
        string installPath = Path.Combine(testRoot, "Game");
        string modsPath = Path.Combine(installPath, "MW5Mercs", "Mods");
        string modPath = Path.Combine(modsPath, "InstalledMod");

        LocSettings originalSettings = LocSettings.Instance;
        string originalSettingsDirectory =
            Environment.GetEnvironmentVariable(
                LocSettings.SettingsDirectoryEnvironmentVariable);
        MainForm originalMainForm = MainForm.Instance;
        DockModListForm originalModListForm = DockModListForm.Instance;
        DockOverviewForm originalOverviewForm = DockOverviewForm.Instance;
        DockConflictsForm originalConflictsForm = DockConflictsForm.Instance;
        ModsManager.LastAppliedPresetData originalLastAppliedPreset =
            ModsManager.Instance.LastAppliedPreset;
        var originalLastAppliedList =
            ModsManager.Instance.LastAppliedPresetModList;
        MainForm form = null;

        try
        {
            Directory.CreateDirectory(settingsDirectory);
            Directory.CreateDirectory(modPath);
            Environment.SetEnvironmentVariable(
                LocSettings.SettingsDirectoryEnvironmentVariable,
                settingsDirectory);

            File.WriteAllText(
                Path.Combine(modPath, "mod.json"),
                """
                {
                  "displayName": "Installed Mod",
                  "version": "1.0",
                  "buildNumber": 1,
                  "description": "",
                  "author": "",
                  "authorURL": "",
                  "defaultLoadOrder": 5,
                  "locOriginalLoadOrder": 5,
                  "gameVersion": "1.0",
                  "manifest": []
                }
                """);
            File.WriteAllText(
                Path.Combine(modsPath, "modlist.json"),
                """
                {
                  "gameVersion": "1.0",
                  "modStatus": {
                    "InstalledMod": {
                      "bEnabled": true
                    }
                  }
                }
                """);

            var settings = new LocSettings(
                Path.Combine(settingsDirectory, LocSettings.SettingsFileName));
            settings.Data.platform = eGamePlatform.Generic;
            settings.Data.InstallPath = installPath;
            ModsManager.Instance.LastAppliedPreset = null;
            ModsManager.Instance.LastAppliedPresetModList = null;

            form = new MainForm();
            form.RefreshAll(forceLoadLastApplied: true);

            Assert.IsNotNull(ModItemList.Instance.ModList);
            Assert.AreEqual(1, ModItemList.Instance.ModList.Count);
            ModItem installedMod = ModItemList.Instance.ModList.Single();
            Assert.AreEqual("InstalledMod", installedMod.FolderName);
            Assert.IsTrue(installedMod.Enabled);
            Assert.AreEqual(
                ModItemList.Instance.ModList.ComputeModListHashCode(),
                form._ActiveModListHash);
            Assert.IsFalse(ModsManager.Instance.ModSettingsTainted);
        }
        finally
        {
            form?.Dispose();
            ModsManager.Instance.ClearAll();
            ModsManager.Instance.LastAppliedPreset = originalLastAppliedPreset;
            ModsManager.Instance.LastAppliedPresetModList =
                originalLastAppliedList;
            ModItemList.Instance.ModList = null;
            LocSettings.Instance = originalSettings;
            Environment.SetEnvironmentVariable(
                LocSettings.SettingsDirectoryEnvironmentVariable,
                originalSettingsDirectory);
            MainForm.Instance = originalMainForm;
            DockModListForm.Instance = originalModListForm;
            DockOverviewForm.Instance = originalOverviewForm;
            DockConflictsForm.Instance = originalConflictsForm;

            if (Directory.Exists(testRoot))
                Directory.Delete(testRoot, recursive: true);
        }
    }
}
