using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MW5.LoadOrder.Tests
{
    [TestClass]
    public sealed class SyntheticSearchPerformanceTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SearchMatchesNameAuthorAndFolderCaseInsensitively()
        {
            var mod = new ModItem
            {
                Name = "Yet Another Weapon",
                Author = "Synthetic Author 23",
                FolderName = "Synthetic_Mod_0599"
            };

            Assert.IsTrue(ModSearch.Matches(mod, "weapon"));
            Assert.IsTrue(ModSearch.Matches(mod, "AUTHOR 23"));
            Assert.IsTrue(ModSearch.Matches(mod, "mod_0599"));
            Assert.IsFalse(ModSearch.Matches(mod, "no-such-mod"));
        }

        [TestMethod]
        public void AllMatchDetectsWhenFilteringWouldNotChangeTheView()
        {
            List<ModItem> mods = CreateSyntheticMods(600);

            Assert.IsTrue(ModSearch.AllMatch(mods, "Synthetic"));
            Assert.IsFalse(ModSearch.AllMatch(mods, "Weapons"));
        }

        [TestMethod]
        public void LargeSyntheticSearchSetStaysWithinTimingBudget()
        {
            const int modCount = 25_000;
            const int repetitions = 50;
            List<ModItem> mods = CreateSyntheticMods(modCount);
            string[] searches =
            {
                "Weapons",
                "Author 23",
                "Synthetic_Mod_24999",
                "no-such-mod",
                "Visuals"
            };

            int matchCount = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                string searchText = searches[repetition % searches.Length];
                matchCount += mods.Count(mod => ModSearch.Matches(mod, searchText));
            }
            stopwatch.Stop();

            TestContext.WriteLine(
                $"Scanned {modCount:N0} mods for {repetitions:N0} searches "
                + $"in {stopwatch.Elapsed.TotalMilliseconds:N1} ms; "
                + $"aggregate matches: {matchCount:N0}.");

            Assert.IsTrue(matchCount > 0);
            Assert.IsTrue(
                stopwatch.Elapsed < TimeSpan.FromSeconds(2),
                "Model-only search exceeded the two-second regression budget.");
        }

        [TestMethod]
        public void MissingDeployedFileReportsTheAffectedPath()
        {
            string missingFile = Path.Combine(
                Path.GetTempPath(),
                "mw5-loc-tests",
                Guid.NewGuid().ToString("N"),
                "missing.pak");

            ModFileAccessException exception = Assert.ThrowsException<ModFileAccessException>(
                () => LocFileUtils.GetFileSize(missingFile));

            Assert.AreEqual(missingFile, exception.FilePath);
            Assert.AreEqual("read the file size", exception.Operation);
            Assert.IsInstanceOfType<IOException>(exception.InnerException);
        }

        [TestMethod]
        public void BrokenDeploymentLinkIsReportedInsteadOfEscapingAsRawIoFailure()
        {
            string testDirectory = Path.Combine(
                Path.GetTempPath(),
                "mw5-loc-tests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testDirectory);
            string missingTarget = Path.Combine(testDirectory, "missing-target.pak");
            string deployedLink = Path.Combine(testDirectory, "deployed.pak");

            try
            {
                try
                {
                    File.CreateSymbolicLink(deployedLink, missingTarget);
                }
                catch (Exception linkCreationException) when (
                    linkCreationException is UnauthorizedAccessException
                    || linkCreationException is PlatformNotSupportedException)
                {
                    Assert.Inconclusive(
                        $"Symbolic links are unavailable in this test environment: {linkCreationException.Message}");
                }

                ModFileAccessException exception =
                    Assert.ThrowsException<ModFileAccessException>(
                        () => LocFileUtils.GetFileSize(deployedLink));

                Assert.IsTrue(
                    string.Equals(exception.FilePath, deployedLink, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(exception.FilePath, missingTarget, StringComparison.OrdinalIgnoreCase));
                Assert.IsInstanceOfType<IOException>(exception.InnerException);
            }
            finally
            {
                Directory.Delete(testDirectory, recursive: true);
            }
        }

        private static List<ModItem> CreateSyntheticMods(int count)
        {
            string[] categories =
            {
                "Weapons", "Visuals", "Career", "Missions",
                "Audio", "Mechs", "Balance", "Interface"
            };
            var mods = new List<ModItem>(count);
            for (int index = 0; index < count; index++)
            {
                mods.Add(new ModItem
                {
                    Name = $"Synthetic {categories[index % categories.Length]} Test Mod {index:00000}",
                    Author = $"Synthetic Author {index % 47:00}",
                    FolderName = $"Synthetic_Mod_{index:00000}"
                });
            }

            return mods;
        }
    }
}
