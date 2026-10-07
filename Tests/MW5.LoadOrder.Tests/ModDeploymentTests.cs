using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MW5.LoadOrder.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ModDeploymentTests
{
    private string root;
    private string install;
    private string mods;
    private string settings;
    private LocSettings originalSettings;
    private string originalSettingsDirectory;
    private ModsManager Manager => ModsManager.Instance;
    private string ModList => Path.Combine(mods, "modlist.json");

    [TestInitialize]
    public void Initialize()
    {
        root = Path.Combine(Path.GetTempPath(), "MW5-Deployment-" + Guid.NewGuid().ToString("N"));
        install = Path.Combine(root, "Game");
        mods = Path.Combine(install, "MW5Mercs", "Mods");
        settings = Path.Combine(root, "Settings");
        Directory.CreateDirectory(mods);
        Directory.CreateDirectory(settings);
        originalSettings = LocSettings.Instance;
        originalSettingsDirectory = Environment.GetEnvironmentVariable(LocSettings.SettingsDirectoryEnvironmentVariable);
        Environment.SetEnvironmentVariable(LocSettings.SettingsDirectoryEnvironmentVariable, settings);
        var configuration = new LocSettings(Path.Combine(settings, LocSettings.SettingsFileName));
        configuration.Data.platform = eGamePlatform.Generic;
        configuration.Data.InstallPath = install;
        configuration.Data.EnableFileWatch = false;
        Manager.ClearAll();
        Manager.LastAppliedPreset = null;
        Manager.LastAppliedPresetModList = null;
    }

    [TestCleanup]
    public void Cleanup()
    {
        Manager.ClearAll();
        Manager.LastAppliedPreset = null;
        Manager.LastAppliedPresetModList = null;
        ModItemList.Instance.ModList = null;
        LocSettings.Instance = originalSettings;
        Environment.SetEnvironmentVariable(LocSettings.SettingsDirectoryEnvironmentVariable, originalSettingsDirectory);
        foreach (string file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            File.SetAttributes(file, FileAttributes.Normal);
        Directory.Delete(root, true);
    }

    private string AddMod(string folder, float priority = 500, float? original = null, string version = "1.1.361")
    {
        string path = Path.Combine(mods, folder);
        Directory.CreateDirectory(Path.Combine(path, "Paks"));
        File.WriteAllText(Path.Combine(path, "Paks", folder + ".pak"), "test pak");
        var metadata = new JObject
        {
            ["displayName"] = folder, ["version"] = "1.0", ["buildNumber"] = 1,
            ["description"] = "", ["author"] = "", ["authorURL"] = "",
            ["defaultLoadOrder"] = priority, ["gameVersion"] = version, ["manifest"] = new JArray(),
            ["customMetadata"] = "preserve"
        };
        if (original.HasValue)
            metadata["locOriginalLoadOrder"] = original.Value;
        File.WriteAllText(Path.Combine(path, "mod.json"), metadata.ToString());
        return path;
    }

    private void WriteList(string version, params (string folder, bool enabled, float? priority, string[] paths)[] entries)
    {
        var statuses = new JObject();
        foreach (var entry in entries)
        {
            var status = new JObject { ["bEnabled"] = entry.enabled };
            if (entry.priority.HasValue) status["defaultLoadOrder"] = entry.priority.Value;
            if (entry.paths != null) status["cachedPakPaths"] = JArray.FromObject(entry.paths);
            statuses[entry.folder] = status;
        }
        var document = new JObject { ["modStatus"] = statuses };
        if (version != null) document["gameVersion"] = version;
        File.WriteAllText(ModList, document.ToString());
    }

    private void Load()
    {
        Manager.ClearAll();
        Manager.UpdateGamePaths();
        Manager.ParseDirectories();
        Manager.ReloadModData(false);
        var enabled = Manager.LoadMw5ModListFileData() ?? new List<ModsManager.ModImportData>();
        Manager.ProcessModImportList(ref enabled, false);
        Manager.ModEnabledListLastState = enabled;
        Manager.DetermineBestAvailableGameVersion();
        Manager.ResolveLoadedPriorities();
        Manager.RenewModEnabledList();
        foreach (var entry in Manager.ModEnabledList)
            entry.Enabled = enabled.Any(e => e.ModPath == entry.ModPath && e.Enabled);
        ModItemList.Instance.ModList = Manager.ModEnabledList.Select(ModItem.CreateFromImportData)
            .OrderBy(m => m.CurrentLoadOrder).ThenBy(m => m.FolderName, StringComparer.OrdinalIgnoreCase).ToList();
        Manager.SynchronizeWorkingModList();
        Manager.LoadLastAppliedPresetData();
    }

    [DataTestMethod]
    [DataRow("1.15.397", false)]
    [DataRow("1.15.398", true)]
    [DataRow("1.15.399", true)]
    [DataRow("1.16.0", true)]
    [DataRow("2.0.0", true)]
    [DataRow("1.9.999", false)]
    [DataRow("0", false)]
    public void StorageBoundaryUsesNumericVersionComparison(string version, bool expected) =>
        Assert.AreEqual(expected, GameVersionPolicy.UsesCachedModList(version));

    [DataTestMethod]
    [DataRow("1.1.361", "1.15.398", "1.15.398", "GameInfo")]
    [DataRow("1.15.399", "1.15.398", "1.15.399", "ModList")]
    [DataRow("1.15.398", "1.15.398", "1.15.398", "ModList")]
    [DataRow(null, "1.15.398", "1.15.398", "GameInfo")]
    [DataRow("1.1.361", null, "1.1.361", "ModList")]
    [DataRow("", "bad", "1.16.0", "Mods")]
    public void VersionResolutionUsesRequestedSourcePrecedence(string header, string xml, string expected, string source)
    {
        var result = GameVersionPolicy.Resolve(header, xml, new[] { "1.16.0", "", "invalid", "1.9.999" });
        Assert.AreEqual(expected, result.Value);
        Assert.AreEqual(source, result.Source.ToString());
    }

    [TestMethod]
    public void OptionalXmlHandlesMissingFileAttributeAndMalformedData()
    {
        Assert.IsNull(GameVersionPolicy.ReadGameInfoVersion(install));
        string file = Path.Combine(install, "game_info.xml");
        foreach (string content in new[] { "<Game />", "<Game Version=''/>", "<broken", "<Game Version='abc'/>" })
        {
            File.WriteAllText(file, content);
            Assert.IsNull(GameVersionPolicy.ReadGameInfoVersion(install));
        }
        File.WriteAllText(file, "<?xml version='1.0' encoding='utf-16'?><Game Version='1.15.398'/>", Encoding.Unicode);
        Assert.AreEqual("1.15.398", GameVersionPolicy.ReadGameInfoVersion(install));
        Assert.AreEqual("0", GameVersionPolicy.Resolve(null, null, new[] { "", "bad" }).Value);
    }

    [TestMethod]
    public void ReloadClearsOldHeaderAndUsesModFallback()
    {
        AddMod("A", version: "1.1.361");
        WriteList("1.15.398");
        Load();
        Assert.IsTrue(Manager.UsesCachedModList);
        File.Delete(ModList);
        Load();
        Assert.AreEqual("1.1.361", Manager.GameVersion);
        Assert.IsNull(Manager.KnownModListGameVersion);
        Assert.IsFalse(Manager.UsesCachedModList);
    }

    [TestMethod]
    public void NewGameWithoutModListUsesXmlAndCreatesCompleteDisabledEntries()
    {
        string path = AddMod("A");
        File.WriteAllText(Path.Combine(install, "game_info.xml"), "<Game Version='1.15.398'/>");
        Load();
        Assert.IsTrue(Manager.DeploymentNeedsRefresh);
        Assert.AreEqual(0, Manager.SaveToFiles().Count);
        var status = JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"];
        Assert.IsFalse((bool)status["bEnabled"]);
        Assert.AreEqual(500f, (float)status["defaultLoadOrder"]);
        CollectionAssert.AreEqual(GameModDeployment.EnumeratePakPaths(path), status["cachedPakPaths"].Values<string>().ToArray());
    }

    [TestMethod]
    public void ModernDeploymentRestoresLegacyMetadataOnceAndPreservesUserPriority()
    {
        string path = AddMod("A", 12, 500);
        string stock = AddMod("B", 30);
        byte[] stockBytes = File.ReadAllBytes(Path.Combine(stock, "mod.json"));
        WriteList("1.15.398", ("A", true, null, null), ("B", false, null, null));
        Load();
        Assert.AreEqual(12f, Manager.Mods[path].NewLoadOrder);
        Assert.AreEqual(500f, Manager.Mods[path].OriginalLoadOrder);
        Assert.AreEqual(0, Manager.SaveToFiles().Count);
        var metadata = JObject.Parse(File.ReadAllText(Path.Combine(path, "mod.json")));
        Assert.AreEqual(500f, (float)metadata["defaultLoadOrder"]);
        Assert.IsNull(metadata["locOriginalLoadOrder"]);
        Assert.AreEqual("1.1.361", (string)metadata["gameVersion"]);
        Assert.AreEqual("preserve", (string)metadata["customMetadata"]);
        CollectionAssert.AreEqual(stockBytes, File.ReadAllBytes(Path.Combine(stock, "mod.json")));
        byte[] migrated = File.ReadAllBytes(Path.Combine(path, "mod.json"));
        Load();
        Assert.AreEqual(12f, Manager.Mods[path].NewLoadOrder);
        Assert.IsFalse(Manager.DeploymentNeedsRefresh);
        Assert.AreEqual(0, Manager.GetExternallyChangedMods().Count);
        Manager.SaveToFiles();
        CollectionAssert.AreEqual(migrated, File.ReadAllBytes(Path.Combine(path, "mod.json")));
    }

    [TestMethod]
    public void LegacyModeWritesMetadataAndIgnoresLeftoverStatusPriority()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.1.361", ("A", true, 99, GameModDeployment.EnumeratePakPaths(path)));
        Load();
        Assert.AreEqual(12f, Manager.Mods[path].NewLoadOrder);
        ModItemList.Instance.ModList.Single().CurrentLoadOrder = 7;
        Manager.SynchronizeWorkingModList();
        Manager.SaveToFiles();
        var metadata = JObject.Parse(File.ReadAllText(Path.Combine(path, "mod.json")));
        Assert.AreEqual(7f, (float)metadata["defaultLoadOrder"]);
        Assert.AreEqual(500f, (float)metadata["locOriginalLoadOrder"]);
        var status = JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"];
        Assert.IsTrue((bool)status["bEnabled"]);
        Assert.IsNull(status["defaultLoadOrder"]);
        Assert.IsNull(status["cachedPakPaths"]);
        Load();
        Assert.AreEqual(7f, Manager.Mods[path].NewLoadOrder);
    }

    [TestMethod]
    public void StatusPriorityIncludingZeroAndFractionsOverridesMetadata()
    {
        string alpha = AddMod("alpha", 900, 500);
        string beta = AddMod("Beta", 200);
        string gamma = AddMod("Gamma", 800);
        WriteList("1.15.398", ("alpha", true, 0, GameModDeployment.EnumeratePakPaths(alpha)),
            ("Beta", false, 0, GameModDeployment.EnumeratePakPaths(beta)),
            ("Gamma", true, 1.25f, GameModDeployment.EnumeratePakPaths(gamma)));
        Load();
        CollectionAssert.AreEqual(new[] { "alpha", "Beta", "Gamma" }, ModItemList.Instance.ModList.Select(m => m.FolderName).ToArray());
        Manager.SaveToFiles();
        Load();
        Assert.AreEqual(0f, Manager.Mods[alpha].NewLoadOrder);
        Assert.AreEqual(1.25f, Manager.Mods[gamma].NewLoadOrder);
    }

    [TestMethod]
    public void MissingPriorityWithNonemptyCacheMatchesGameZeroAndRequiresRepair()
    {
        string path = AddMod("A");
        WriteList("1.15.398", ("A", true, null, GameModDeployment.EnumeratePakPaths(path)));
        Load();
        Assert.AreEqual(0f, Manager.Mods[path].NewLoadOrder);
        Assert.IsTrue(Manager.DeploymentNeedsRefresh);
    }

    [TestMethod]
    public void ApplyReenumeratesPaksWithoutReadingContentsAndPreservesUnknownFields()
    {
        string path = AddMod("A");
        WriteList("1.15.398", ("A", true, 12, GameModDeployment.EnumeratePakPaths(path)));
        var document = JObject.Parse(File.ReadAllText(ModList));
        document["customRoot"] = 42;
        document["modStatus"]["A"]["customStatus"] = "retained";
        File.WriteAllText(ModList, document.ToString());
        Load();
        Assert.IsFalse(Manager.DeploymentNeedsRefresh);
        string pak = Path.Combine(path, "Paks", "Additional.PAK");
        File.WriteAllText(pak, "additional");
        File.WriteAllText(Path.Combine(path, "Paks", "ignored.txt"), "ignored");
        Directory.CreateDirectory(Path.Combine(path, "Paks", "Nested"));
        File.WriteAllText(Path.Combine(path, "Paks", "Nested", "ignored.pak"), "ignored");
        Manager.RefreshDeploymentRequirement();
        Assert.IsTrue(Manager.DeploymentNeedsRefresh);
        // A locked pak is fine: deployment only enumerates names.
        using (var locked = new FileStream(pak, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            Manager.SaveToFiles();
        var saved = JObject.Parse(File.ReadAllText(ModList));
        Assert.AreEqual(2, saved["modStatus"]["A"]["cachedPakPaths"].Count());
        Assert.AreEqual(42, (int)saved["customRoot"]);
        Assert.AreEqual("retained", (string)saved["modStatus"]["A"]["customStatus"]);
        File.Move(pak, Path.Combine(path, "Paks", "Renamed.pak"));
        Manager.RefreshDeploymentRequirement();
        Assert.IsTrue(Manager.DeploymentNeedsRefresh);
        Manager.SaveToFiles();
        Assert.IsFalse(File.ReadAllText(ModList).Contains("Additional.PAK"));
    }

    [TestMethod]
    public void ExternalPriorityAndPartialDisableAreDetectedButAuthorUpdateIsNot()
    {
        string a = AddMod("A"), b = AddMod("B");
        WriteList("1.15.398", ("A", true, 10, GameModDeployment.EnumeratePakPaths(a)),
            ("B", true, 20, GameModDeployment.EnumeratePakPaths(b)));
        Load();
        Manager.SaveToFiles();
        var metadata = JObject.Parse(File.ReadAllText(Path.Combine(a, "mod.json")));
        metadata["defaultLoadOrder"] = 999;
        File.WriteAllText(Path.Combine(a, "mod.json"), metadata.ToString());
        Load();
        Assert.AreEqual(0, Manager.GetExternallyChangedMods().Count);
        var document = JObject.Parse(File.ReadAllText(ModList));
        document["modStatus"]["A"]["bEnabled"] = false;
        document["modStatus"]["B"]["defaultLoadOrder"] = 21;
        File.WriteAllText(ModList, document.ToString());
        Load();
        CollectionAssert.AreEquivalent(new[] { "A", "B" }, Manager.GetExternallyChangedMods());
        File.Delete(ModList);
        Load();
        Assert.AreEqual(2, Manager.GetExternallyChangedMods().Count);
    }

    [TestMethod]
    public void ExternalEditsPreventApplyFromOverwritingThem()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        string metadata = File.ReadAllText(Path.Combine(path, "mod.json"));
        string external = File.ReadAllText(ModList) + Environment.NewLine;
        File.WriteAllText(ModList, external);
        Assert.ThrowsException<IOException>(() => Manager.SaveToFiles());
        Assert.AreEqual(external, File.ReadAllText(ModList));
        Assert.AreEqual(metadata, File.ReadAllText(Path.Combine(path, "mod.json")));
    }

    [TestMethod]
    public void SnapshotFailureDefersRestorationAndCanBeRetried()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        string snapshot = Path.Combine(settings, ModsManager.LastAppliedOrderFileName);
        File.WriteAllText(snapshot, "{}");
        using (var locked = new FileStream(snapshot, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            Assert.AreEqual(1, Manager.SaveToFiles().Count);
            Assert.IsTrue(Manager.DeploymentNeedsRefresh);
            Assert.IsNotNull(JObject.Parse(File.ReadAllText(Path.Combine(path, "mod.json")))["locOriginalLoadOrder"]);
            Assert.AreEqual(12f, (float)JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"]["defaultLoadOrder"]);
        }
        Assert.AreEqual(0, Manager.SaveToFiles().Count);
        Assert.IsNull(JObject.Parse(File.ReadAllText(Path.Combine(path, "mod.json")))["locOriginalLoadOrder"]);
    }

    [TestMethod]
    public void FailedModListReplacementKeepsOriginalAndDoesNotMigrate()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        string originalList = File.ReadAllText(ModList);
        string metadata = File.ReadAllText(Path.Combine(path, "mod.json"));
        using (var locked = new FileStream(ModList, FileMode.Open, FileAccess.Read, FileShare.Read))
            Assert.ThrowsException<IOException>(() => Manager.SaveToFiles());
        Assert.AreEqual(originalList, File.ReadAllText(ModList));
        Assert.AreEqual(metadata, File.ReadAllText(Path.Combine(path, "mod.json")));
        Assert.IsFalse(File.Exists(Path.Combine(settings, ModsManager.LastAppliedOrderFileName)));
        Assert.AreEqual(0, Directory.GetFiles(mods, "*.tmp").Length);
    }

    [TestMethod]
    public void EnabledModWithoutPaksFailsBeforeAnyWrites()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        string before = File.ReadAllText(ModList);
        File.Delete(Path.Combine(path, "Paks", "A.pak"));
        Assert.ThrowsException<InvalidDataException>(() => Manager.SaveToFiles());
        Assert.AreEqual(before, File.ReadAllText(ModList));
    }

    [TestMethod]
    public void DuplicateStatusNamesFailBeforeDeployment()
    {
        string a = AddMod("A");
        Assert.ThrowsException<InvalidDataException>(() => GameModDeployment.BuildDocument(null, "1.15.398",
            new[] { new ModDeploymentEntry("A", a, true, 1), new ModDeploymentEntry("a", a, true, 2) }, true));
    }

    [TestMethod]
    public void RelevantWatcherPathsIncludePakChangesAndModList()
    {
        string a = AddMod("A");
        WriteList("1.15.398");
        Load();
        Assert.IsTrue(Manager.IsDeploymentPath(ModList));
        Assert.IsTrue(Manager.IsDeploymentPath(Path.Combine(a, "Paks")));
        Assert.IsTrue(Manager.IsDeploymentPath(Path.Combine(a, "Paks", "New.PAK")));
        Assert.IsFalse(Manager.IsDeploymentPath(Path.Combine(a, "Paks", "notes.txt")));
    }

    [TestMethod]
    public void FailedMetadataCleanupIsRetryableWithoutChangingUserPriority()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, 17, GameModDeployment.EnumeratePakPaths(path)));
        Load();
        string file = Path.Combine(path, "mod.json");
        File.SetAttributes(file, FileAttributes.ReadOnly);
        Assert.AreEqual(1, Manager.SaveToFiles().Count);
        Assert.IsTrue(Manager.DeploymentNeedsRefresh);
        Assert.AreEqual(17f, (float)JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"]["defaultLoadOrder"]);
        Assert.IsNotNull(JObject.Parse(File.ReadAllText(file))["locOriginalLoadOrder"]);
        File.SetAttributes(file, FileAttributes.Normal);
        Assert.AreEqual(0, Manager.SaveToFiles().Count);
        Assert.IsFalse(Manager.DeploymentNeedsRefresh);
        Assert.AreEqual(500f, (float)JObject.Parse(File.ReadAllText(file))["defaultLoadOrder"]);
    }

    [DataTestMethod]
    [DataRow("backup.json", true, 700f)]
    [DataRow("mod.json.bak", true, 700f)]
    [DataRow("backup.json", false, 12f)]
    public void OriginalPriorityRecoveryValidatesBackupIdentity(string filename, bool matching, float expected)
    {
        string path = AddMod("A", 12);
        var backup = JObject.Parse(File.ReadAllText(Path.Combine(path, "mod.json")));
        backup["defaultLoadOrder"] = 700;
        if (!matching) backup["buildNumber"] = 0;
        File.WriteAllText(Path.Combine(path, filename), backup.ToString());
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        Assert.AreEqual(expected, Manager.Mods[path].OriginalLoadOrder);
        Assert.AreEqual(12f, Manager.Mods[path].NewLoadOrder);
        string before = File.ReadAllText(Path.Combine(path, "mod.json"));
        Manager.SaveToFiles();
        Assert.AreEqual(before, File.ReadAllText(Path.Combine(path, "mod.json")));
    }

    [TestMethod]
    public void UpdatedMetadataCannotBeOverwrittenDuringMigration()
    {
        string path = AddMod("A", 12, 500);
        WriteList("1.15.398", ("A", true, null, null));
        Load();
        string file = Path.Combine(path, "mod.json");
        var updated = JObject.Parse(File.ReadAllText(file));
        updated["buildNumber"] = 2;
        File.WriteAllText(file, updated.ToString());
        string before = File.ReadAllText(ModList);
        Assert.ThrowsException<IOException>(() => Manager.SaveToFiles());
        Assert.AreEqual(before, File.ReadAllText(ModList));
        Assert.AreEqual(2, (int)JObject.Parse(File.ReadAllText(file))["buildNumber"]);
    }

    [TestMethod]
    public void WorkshopPathAndDisabledEmptyCacheAreSerializedCorrectly()
    {
        string path = AddMod("784080001", 30);
        string workshop = Path.Combine(root, "Workshop", "784080001");
        Directory.CreateDirectory(Path.GetDirectoryName(workshop));
        Directory.Move(path, workshop);
        string disabled = AddMod("Disabled", 40);
        Directory.Delete(Path.Combine(disabled, "Paks"), true);
        var document = GameModDeployment.BuildDocument(null, "1.15.398", new[]
        {
            new ModDeploymentEntry("784080001", workshop, true, 2),
            new ModDeploymentEntry("Disabled", disabled, false, 3)
        }, true);
        CollectionAssert.AreEqual(GameModDeployment.EnumeratePakPaths(workshop),
            document["modStatus"]["784080001"]["cachedPakPaths"].Values<string>().ToArray());
        Assert.AreEqual(0, document["modStatus"]["Disabled"]["cachedPakPaths"].Count());
    }

    [STATestMethod]
    public void UiReloadApplyAndSavedRestorePreserveNumbersAndDetectAddedPak()
    {
        var originalMain = MainForm.Instance;
        var originalList = DockModListForm.Instance;
        var originalOverview = DockOverviewForm.Instance;
        var originalConflicts = DockConflictsForm.Instance;
        MainForm form = null;
        try
        {
            string path = AddMod("A", 500);
            WriteList("1.15.398", ("A", true, 12.5f, GameModDeployment.EnumeratePakPaths(path)));
            form = new MainForm();
            form.RefreshAll(forceLoadLastApplied: true);
            Assert.AreEqual(12.5f, ModItemList.Instance.ModList.Single().CurrentLoadOrder);
            Assert.IsFalse(Manager.ModSettingsTainted);
            Assert.IsTrue(form.ApplyModSettings());
            var document = JObject.Parse(File.ReadAllText(ModList));
            document["modStatus"]["A"]["defaultLoadOrder"] = 99;
            document["modStatus"]["A"]["bEnabled"] = false;
            File.WriteAllText(ModList, document.ToString());
            File.WriteAllText(Path.Combine(path, "Paks", "Added.pak"), "additional");
            form.RefreshAll(forceLoadLastApplied: true);
            Assert.AreEqual(12.5f, ModItemList.Instance.ModList.Single().CurrentLoadOrder);
            Assert.IsTrue(ModItemList.Instance.ModList.Single().Enabled);
            Assert.IsTrue(Manager.ModSettingsTainted);
            Assert.IsTrue(form.ApplyModSettings());
            Assert.AreEqual(2, JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"]["cachedPakPaths"].Count());
            Assert.IsFalse(Manager.ModSettingsTainted);
            form.RefreshAll(forceLoadLastApplied: true);
            Assert.IsFalse(Manager.ModSettingsTainted);
            // One mod is already in author order, but its numeric priority still needs restoring.
            typeof(MainForm).GetMethod("toolStripMenuItemSortDefaultLoadOrder_Click",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(form, new object[] { null, EventArgs.Empty });
            Assert.AreEqual(500f, ModItemList.Instance.ModList.Single().CurrentLoadOrder);
            Assert.IsTrue(Manager.ModSettingsTainted);
            Assert.IsTrue(form.ApplyModSettings());
            Assert.AreEqual(500f, (float)JObject.Parse(File.ReadAllText(ModList))["modStatus"]["A"]["defaultLoadOrder"]);
        }
        finally
        {
            form?.Dispose();
            MainForm.Instance = originalMain;
            DockModListForm.Instance = originalList;
            DockOverviewForm.Instance = originalOverview;
            DockConflictsForm.Instance = originalConflicts;
        }
    }
}
