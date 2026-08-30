using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MW5.LoadOrder.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class ModStartupOptimizationTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            ModItemList.Instance.ModList = null;
            ModsManager.Instance.Mods.Clear();
            ModsManager.Instance.ModDetails.Clear();
            ModsManager.Instance.ModEnabledList.Clear();
            ModsManager.Instance.ModDirectories.Clear();
            ModsManager.Instance.DirNameToPathDict.Clear();
            ModsManager.Instance.PathToDirNameDict.Clear();
            ModsManager.Instance.ModConflictData.Clear();
        }

        [TestMethod]
        public void IndexedConflictDataPreservesDirectionCasingDuplicatesAndSharedLists()
        {
            ModItem alpha = AddMod(
                "Alpha",
                10,
                true,
                new List<string> { "First.uasset", "SHARED.uasset", "shared.uasset" });
            ModItem beta = AddMod(
                "beta",
                10,
                true,
                new List<string> { "shared.uasset", "First.uasset" });
            ModItem gamma = AddMod(
                "Gamma",
                20,
                true,
                new List<string> { "FIRST.UASSET" });
            ModItemList.Instance.ModList = new List<ModItem> { alpha, beta, gamma };

            Dictionary<string, ModConflictData> result =
                ModsManager.Instance.BuildModConflictData(ModItemList.Instance.ModList);

            CollectionAssert.AreEqual(
                new[] { "Alpha", "beta", "Gamma" },
                result.Keys.ToArray());
            CollectionAssert.AreEqual(
                new[] { "First.uasset", "SHARED.uasset" },
                result["Alpha"].overriddenBy["beta"]);
            CollectionAssert.AreEqual(
                new[] { "First.uasset" },
                result["Alpha"].overriddenBy["Gamma"]);
            Assert.AreSame(
                result["Alpha"].overriddenBy["beta"],
                result["beta"].overrides["Alpha"]);
            CollectionAssert.AreEqual(
                new[] { "Alpha", "beta" },
                result["Gamma"].overrides.Keys.ToArray());
        }

        [TestMethod]
        public void IndexedConflictDataIncludesEnabledModsWithoutManifestsAndExcludesDisabledMods()
        {
            ModItem noManifest = AddMod("NoManifest", 0, true, null);
            ModItem emptyManifest = AddMod("EmptyManifest", 1, true, new List<string>());
            ModItem disabled = AddMod(
                "Disabled",
                2,
                false,
                new List<string> { "shared.uasset" });
            ModItem enabled = AddMod(
                "Enabled",
                3,
                true,
                new List<string> { "shared.uasset" });
            ModItemList.Instance.ModList =
                new List<ModItem> { noManifest, emptyManifest, disabled, enabled };

            Dictionary<string, ModConflictData> result =
                ModsManager.Instance.BuildModConflictData(ModItemList.Instance.ModList);

            CollectionAssert.AreEqual(
                new[] { "NoManifest", "EmptyManifest", "Enabled" },
                result.Keys.ToArray());
            Assert.IsFalse(result["NoManifest"].isOverridden);
            Assert.IsFalse(result["NoManifest"].isOverriding);
            Assert.IsFalse(result.ContainsKey("Disabled"));
        }

        [TestMethod]
        public void IndexedConflictDataDoesNotCompareDuplicateFolderNames()
        {
            ModItem first = AddMod(
                "Duplicate",
                1,
                true,
                new List<string> { "shared.uasset" });
            ModItem second = new ModItem
            {
                Enabled = true,
                FolderName = first.FolderName,
                Path = first.Path,
                CurrentLoadOrder = 2
            };

            Dictionary<string, ModConflictData> result =
                ModsManager.Instance.BuildModConflictData(
                    new List<ModItem> { first, second });

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result["Duplicate"].isOverridden);
            Assert.IsFalse(result["Duplicate"].isOverriding);
            Assert.AreEqual(0, result["Duplicate"].overrides.Count);
            Assert.AreEqual(0, result["Duplicate"].overriddenBy.Count);
        }

        [TestMethod]
        public void DuplicateFolderNamesPreserveFirstRecordedPeerRelationship()
        {
            ModItem firstDuplicate = AddModAtPath(
                "Duplicate",
                @"C:\SyntheticMods\First\Duplicate",
                1,
                true,
                new List<string> { "shared.uasset" });
            ModItem secondDuplicate = AddModAtPath(
                "Duplicate",
                @"C:\SyntheticMods\Second\Duplicate",
                10,
                true,
                new List<string> { "shared.uasset" });
            ModItem peer = AddModAtPath(
                "Peer",
                @"C:\SyntheticMods\Peer",
                5,
                true,
                new List<string> { "shared.uasset" });

            Dictionary<string, ModConflictData> result =
                ModsManager.Instance.BuildModConflictData(
                    new List<ModItem> { firstDuplicate, secondDuplicate, peer });

            Assert.IsTrue(result["Duplicate"].isOverridden);
            Assert.IsFalse(result["Duplicate"].isOverriding);
            CollectionAssert.AreEqual(
                new[] { "Peer" },
                result["Duplicate"].overriddenBy.Keys.ToArray());
            CollectionAssert.AreEqual(
                new[] { "Duplicate" },
                result["Peer"].overrides.Keys.ToArray());
        }

        [TestMethod]
        public void IndexedConflictDataMatchesPairwiseReferenceAcrossRandomizedManifests()
        {
            var random = new Random(70123);
            var mods = new List<ModItem>();
            for (int modIndex = 0; modIndex < 40; modIndex++)
            {
                List<string> manifest = modIndex % 11 == 0
                    ? null
                    : Enumerable.Range(0, random.Next(0, 45))
                        .Select(_ =>
                        {
                            int pathIndex = random.Next(0, 75);
                            string path = $"Content/Path{pathIndex}.uasset";
                            return random.Next(0, 4) == 0 ? path.ToUpperInvariant() : path;
                        })
                        .ToList();
                mods.Add(AddMod(
                    $"Mod{modIndex:D2}",
                    random.Next(0, 8),
                    random.Next(0, 5) != 0,
                    manifest));
            }

            Dictionary<string, ModConflictData> expected = BuildPairwiseReference(mods);
            Dictionary<string, ModConflictData> actual =
                ModsManager.Instance.BuildModConflictData(mods);

            AssertConflictDataEqual(expected, actual);
        }

        [TestMethod]
        public void DeferredFileMetadataExcludesManagerFilesAndUsesNewestContentTimestamp()
        {
            string modPath = Path.Combine(
                Path.GetTempPath(),
                "MW5-LOC-Metadata-" + Guid.NewGuid().ToString("N"));
            try
            {
                string paksPath = Path.Combine(modPath, "Paks");
                string resourcesPath = Path.Combine(modPath, "Resources");
                Directory.CreateDirectory(paksPath);
                Directory.CreateDirectory(resourcesPath);

                string pakPath = Path.Combine(paksPath, "content.pak");
                string resourcePath = Path.Combine(resourcesPath, "content.json");
                File.WriteAllBytes(pakPath, new byte[11]);
                File.WriteAllBytes(resourcePath, new byte[7]);
                File.WriteAllBytes(Path.Combine(modPath, "extra.bin"), new byte[5]);
                File.WriteAllBytes(Path.Combine(modPath, "mod.json"), new byte[101]);
                File.WriteAllBytes(Path.Combine(modPath, "backup.json"), new byte[103]);
                File.WriteAllBytes(
                    Path.Combine(modPath, "__folder_managed_by_vortex"),
                    new byte[107]);

                DateTime expectedAge = DateTime.UtcNow.AddMinutes(-5);
                File.SetLastWriteTimeUtc(pakPath, expectedAge.AddHours(-1));
                File.SetLastWriteTimeUtc(resourcePath, expectedAge);

                ModsManager.ModFileMetadataResult result =
                    ModsManager.LoadModFileMetadata(modPath);

                Assert.IsTrue(result.Success);
                Assert.AreEqual(23, result.FileSize);
                Assert.IsTrue(result.FileAge.HasValue);
                Assert.IsTrue(
                    (expectedAge - result.FileAge.Value.UtcDateTime).Duration()
                    <= TimeSpan.FromSeconds(2));
            }
            finally
            {
                if (Directory.Exists(modPath))
                    Directory.Delete(modPath, true);
            }
        }

        private static ModItem AddMod(
            string folderName,
            float loadOrder,
            bool enabled,
            List<string> manifest)
        {
            string fullPath = Path.Combine(@"C:\SyntheticMods", folderName);
            return AddModAtPath(
                folderName,
                fullPath,
                loadOrder,
                enabled,
                manifest);
        }

        private static ModItem AddModAtPath(
            string folderName,
            string fullPath,
            float loadOrder,
            bool enabled,
            List<string> manifest)
        {
            var item = new ModItem
            {
                Enabled = enabled,
                FolderName = folderName,
                Path = fullPath,
                CurrentLoadOrder = loadOrder
            };

            ModsManager.Instance.Mods[fullPath] = new ModsManager.ModData
            {
                NewLoadOrder = loadOrder
            };
            ModsManager.Instance.ModDetails[fullPath] = new ModObject
            {
                manifest = manifest
            };
            ModsManager.Instance.DirNameToPathDict[folderName] = fullPath;
            ModsManager.Instance.PathToDirNameDict[fullPath] = folderName;
            return item;
        }

        private static Dictionary<string, ModConflictData> BuildPairwiseReference(
            IReadOnlyList<ModItem> modItems)
        {
            List<ModItem> enabledMods = modItems.Where(item => item.Enabled).ToList();
            var result = new Dictionary<string, ModConflictData>(
                StringComparer.OrdinalIgnoreCase);
            foreach (ModItem itemA in enabledMods)
            {
                if (!result.TryGetValue(
                        itemA.FolderName,
                        out ModConflictData conflictDataA))
                {
                    conflictDataA = new ModConflictData
                    {
                        modPath = itemA.FolderName,
                        overrides = new Dictionary<string, List<string>>(),
                        overriddenBy = new Dictionary<string, List<string>>()
                    };
                    result[itemA.FolderName] = conflictDataA;
                }

                foreach (ModItem itemB in enabledMods)
                {
                    if (itemA.FolderName == itemB.FolderName
                        || conflictDataA.overriddenBy.ContainsKey(itemB.FolderName)
                        || conflictDataA.overrides.ContainsKey(itemB.FolderName))
                    {
                        continue;
                    }

                    if (!result.TryGetValue(
                            itemB.FolderName,
                            out ModConflictData conflictDataB))
                    {
                        conflictDataB = new ModConflictData
                        {
                            modPath = itemB.FolderName,
                            overrides = new Dictionary<string, List<string>>(),
                            overriddenBy = new Dictionary<string, List<string>>()
                        };
                        result[itemB.FolderName] = conflictDataB;
                    }
                    else if (conflictDataB.overriddenBy.ContainsKey(itemA.FolderName)
                             || conflictDataB.overrides.ContainsKey(itemA.FolderName))
                    {
                        continue;
                    }

                    List<string> manifestA =
                        ModsManager.Instance.ModDetails[
                            ModsManager.Instance.DirNameToPathDict[itemA.FolderName]]
                        .manifest;
                    List<string> manifestB =
                        ModsManager.Instance.ModDetails[
                            ModsManager.Instance.DirNameToPathDict[itemB.FolderName]]
                        .manifest;
                    if (manifestA == null || manifestB == null)
                        continue;

                    List<string> intersect = manifestA
                        .Intersect(manifestB, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                    if (intersect.Count == 0)
                        continue;

                    float loadOrderA =
                        ModsManager.Instance.Mods[itemA.Path].NewLoadOrder;
                    float loadOrderB =
                        ModsManager.Instance.Mods[itemB.Path].NewLoadOrder;
                    bool aOverridesB = loadOrderA > loadOrderB
                        || (loadOrderA == loadOrderB
                            && string.Compare(
                                itemA.FolderName,
                                itemB.FolderName,
                                StringComparison.OrdinalIgnoreCase) > 0);

                    if (aOverridesB)
                    {
                        conflictDataA.isOverriding = true;
                        conflictDataA.overrides[itemB.FolderName] = intersect;
                        conflictDataB.isOverridden = true;
                        conflictDataB.overriddenBy[itemA.FolderName] = intersect;
                    }
                    else
                    {
                        conflictDataA.isOverridden = true;
                        conflictDataA.overriddenBy[itemB.FolderName] = intersect;
                        conflictDataB.isOverriding = true;
                        conflictDataB.overrides[itemA.FolderName] = intersect;
                    }
                }
            }

            return result;
        }

        private static void AssertConflictDataEqual(
            Dictionary<string, ModConflictData> expected,
            Dictionary<string, ModConflictData> actual)
        {
            CollectionAssert.AreEqual(expected.Keys.ToArray(), actual.Keys.ToArray());
            foreach (string modPath in expected.Keys)
            {
                ModConflictData expectedData = expected[modPath];
                ModConflictData actualData = actual[modPath];
                Assert.AreEqual(expectedData.isOverridden, actualData.isOverridden);
                Assert.AreEqual(expectedData.isOverriding, actualData.isOverriding);
                AssertRelationshipsEqual(expectedData.overrides, actualData.overrides);
                AssertRelationshipsEqual(
                    expectedData.overriddenBy,
                    actualData.overriddenBy);
            }
        }

        private static void AssertRelationshipsEqual(
            Dictionary<string, List<string>> expected,
            Dictionary<string, List<string>> actual)
        {
            CollectionAssert.AreEqual(expected.Keys.ToArray(), actual.Keys.ToArray());
            foreach (string peer in expected.Keys)
                CollectionAssert.AreEqual(expected[peer], actual[peer]);
        }
    }
}
