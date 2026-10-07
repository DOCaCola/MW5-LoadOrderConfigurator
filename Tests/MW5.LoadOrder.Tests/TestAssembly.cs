using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace MW5.LoadOrder.Tests;

[TestClass]
public static class TestAssembly
{
    private static string settingsDirectory;
    private static string originalSettingsDirectory;

    [AssemblyInitialize]
    public static void Initialize(TestContext context)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        // Shown MainForms run their normal startup. Keep that startup away from
        // the user's game, recovery prompts, and saved window layout.
        originalSettingsDirectory = Environment.GetEnvironmentVariable(
            LocSettings.SettingsDirectoryEnvironmentVariable);
        settingsDirectory = Path.Combine(Path.GetTempPath(),
            "MW5-LOC-TestSuite-" + Guid.NewGuid().ToString("N"));
        string game = Path.Combine(settingsDirectory, "Game");
        string mods = Path.Combine(game, "MW5Mercs", "Mods");
        Directory.CreateDirectory(mods);
        File.WriteAllText(Path.Combine(settingsDirectory, "Settings.json"),
            new JObject
            {
                ["platform"] = (int)eGamePlatform.Generic,
                ["InstallPath"] = game,
                ["EnableFileWatch"] = false
            }.ToString());
        File.WriteAllText(Path.Combine(mods, "modlist.json"),
            new JObject
            {
                ["gameVersion"] = "1.15.398",
                ["modStatus"] = new JObject()
            }.ToString());
        Environment.SetEnvironmentVariable(
            LocSettings.SettingsDirectoryEnvironmentVariable, settingsDirectory);
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        ModsManager.Instance.ClearGamePaths();
        Environment.SetEnvironmentVariable(
            LocSettings.SettingsDirectoryEnvironmentVariable, originalSettingsDirectory);
        Directory.Delete(settingsDirectory, recursive: true);
    }
}
