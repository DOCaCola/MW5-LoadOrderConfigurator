using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static MW5_Mod_Manager.MainForm;

namespace MW5.LoadOrder.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class SyntheticModOrderTests
    {
        private static readonly string[] SyntheticModIds = { "A", "B", "C", "D", "E" };

        [TestCleanup]
        public void Cleanup()
        {
            ModItemList.Instance.ModList = null;
            ModsManager.Instance.Mods.Clear();
            ModsManager.Instance.ModEnabledList.Clear();
        }

        [TestMethod]
        public void SingleItemMovesToEitherEdgeKeepViewModelAndLoadOrdersInSync()
        {
            var failures = new List<string>();

            foreach (bool reversed in new[] { false, true })
            {
                foreach (MovePosition position in Enum.GetValues<MovePosition>())
                {
                    foreach (string selectedId in SyntheticModIds)
                    {
                        ScenarioResult result = RunScenario(reversed, position, selectedId);
                        AddFailureIfOutOfSync(failures, result);
                    }
                }
            }

            AssertNoFailures(failures);
        }

        [TestMethod]
        public void MultiItemMovesToTopKeepViewModelAndLoadOrdersInSync()
        {
            List<string> failures = RunSelectionMatrix(MovePosition.Top);
            AssertNoFailures(failures);
        }

        [TestMethod]
        public void MultiItemMovesToBottomKeepViewModelAndLoadOrdersInSync()
        {
            List<string> failures = RunSelectionMatrix(MovePosition.Bottom);
            AssertNoFailures(failures);
        }

        [TestMethod]
        public void MoveBAndCToBottomLowToHighProducesDisplayedOrder()
        {
            ScenarioResult result = RunScenario(
                reversed: false,
                MovePosition.Bottom,
                "B",
                "C");

            AssertScenarioInSync(result);
        }

        [TestMethod]
        public void MoveDAndCToBottomHighToLowProducesDisplayedOrder()
        {
            ScenarioResult result = RunScenario(
                reversed: true,
                MovePosition.Bottom,
                "D",
                "C");

            AssertScenarioInSync(result);
        }

        [TestMethod]
        public void ReferenceMetadataMoveAcrossTiedAndSparseLoadOrdersProducesDisplayedOrder()
        {
            IReadOnlyList<ModDefinition> definitions = LoadReferenceDefinitions();
            var failures = new List<string>();
            string[] selectedIds =
            {
                "2549720490",
                "TTRulez_LanceMateOrderz",
                "Lore-based Mech Variants - YAML Edition"
            };

            foreach (bool reversed in new[] { false, true })
            {
                ScenarioResult result = RunScenario(
                    definitions,
                    reversed,
                    MovePosition.Bottom,
                    selectedIds);
                AddFailureIfOutOfSync(failures, result);
            }

            AssertNoFailures(failures);
        }

        [DataTestMethod]
        [DataRow(3)]
        [DataRow(4)]
        public void DisjointSelectionCanMoveToBoundaryOccupiedBySelectedItem(
            int insertionIndex)
        {
            List<ModItem> viewOrder = CreateViewOrder("A", "B", "C", "D", "E");

            ModOrderMutation.ViewReorderResult result =
                ModOrderMutation.CalculateViewReorder(
                    viewOrder,
                    new[] { viewOrder[1], viewOrder[3] },
                    insertionIndex);

            Assert.IsTrue(result.Changed);
            Assert.AreEqual(2, result.InsertionIndex);
            CollectionAssert.AreEqual(
                new[] { "A", "C", "B", "D", "E" },
                result.ViewOrder.Select(mod => mod.FolderName).ToArray());
            CollectionAssert.AreEqual(
                new[] { "B", "D" },
                result.DraggedItems.Select(mod => mod.FolderName).ToArray());
        }

        [TestMethod]
        public void ContiguousSelectionDroppedInsideOwnBlockIsNoOp()
        {
            List<ModItem> viewOrder = CreateViewOrder("A", "B", "C", "D", "E");

            ModOrderMutation.ViewReorderResult result =
                ModOrderMutation.CalculateViewReorder(
                    viewOrder,
                    new[] { viewOrder[1], viewOrder[2] },
                    insertionIndex: 2);

            Assert.IsFalse(result.Changed);
            CollectionAssert.AreEqual(
                new[] { "A", "B", "C", "D", "E" },
                result.ViewOrder.Select(mod => mod.FolderName).ToArray());
        }

        private static List<ModItem> CreateViewOrder(params string[] ids)
        {
            return ids
                .Select(id => new ModItem { FolderName = id })
                .ToList();
        }

        private static List<string> RunSelectionMatrix(MovePosition position)
        {
            var failures = new List<string>();

            foreach (bool reversed in new[] { false, true })
            {
                for (int selectionMask = 1; selectionMask < (1 << SyntheticModIds.Length) - 1; selectionMask++)
                {
                    string[] selectedIds = SyntheticModIds
                        .Where((_, index) => (selectionMask & (1 << index)) != 0)
                        .ToArray();

                    if (selectedIds.Length < 2)
                        continue;

                    ScenarioResult result = RunScenario(reversed, position, selectedIds);
                    AddFailureIfOutOfSync(failures, result);
                }
            }

            return failures;
        }

        private static ScenarioResult RunScenario(
            bool reversed,
            MovePosition position,
            params string[] selectedIds)
        {
            IReadOnlyList<ModDefinition> definitions = SyntheticModIds
                .Select((id, index) => new ModDefinition(
                    id,
                    "Synthetic Mod " + id,
                    index,
                    true,
                    "1.0",
                    index + 1))
                .ToList();

            return RunScenario(definitions, reversed, position, selectedIds);
        }

        private static ScenarioResult RunScenario(
            IReadOnlyList<ModDefinition> definitions,
            bool reversed,
            MovePosition position,
            params string[] selectedIds)
        {
            List<ModItem> model = CreateSyntheticModel(definitions);
            ModItemList.Instance.ModList = model;

            List<ModItem> initialView = reversed
                ? model.AsEnumerable().Reverse().ToList()
                : model.ToList();

            var selectedSet = new HashSet<string>(selectedIds, StringComparer.Ordinal);
            List<ModItem> selected = initialView
                .Where(mod => selectedSet.Contains(mod.FolderName))
                .ToList();
            List<ModItem> remaining = initialView
                .Where(mod => !selectedSet.Contains(mod.FolderName))
                .ToList();

            List<ModItem> finalView = position == MovePosition.Top
                ? selected.Concat(remaining).ToList()
                : remaining.Concat(selected).ToList();

            ModOrderMutation.UpdateModelOrderFromView(finalView, reversed);

            MW5_Mod_Manager.LoadOrder.RecomputeLoadOrders();

            List<string> expectedModel = (reversed
                    ? finalView.AsEnumerable().Reverse()
                    : finalView)
                .Select(mod => mod.FolderName)
                .ToList();

            List<ModItem> expectedModelItems = reversed
                ? finalView.AsEnumerable().Reverse().ToList()
                : finalView.ToList();
            bool expectedModelUsesOriginalLoadOrders = IsDefaultSorted(expectedModelItems);
            List<float> expectedViewLoadOrders;
            if (expectedModelUsesOriginalLoadOrders)
            {
                expectedViewLoadOrders = finalView
                    .Select(mod => mod.OriginalLoadOrder)
                    .ToList();
            }
            else
            {
                expectedViewLoadOrders = reversed
                    ? Enumerable.Range(1, finalView.Count).Reverse().Select(value => (float)value).ToList()
                    : Enumerable.Range(1, finalView.Count).Select(value => (float)value).ToList();
            }

            return new ScenarioResult(
                reversed,
                position,
                selected.Select(mod => mod.FolderName).ToArray(),
                finalView.Select(mod => mod.FolderName).ToArray(),
                expectedModel,
                ModItemList.Instance.ModList.Select(mod => mod.FolderName).ToList(),
                expectedViewLoadOrders,
                finalView.Select(mod => mod.CurrentLoadOrder).ToList());
        }

        private static List<ModItem> CreateSyntheticModel(IReadOnlyList<ModDefinition> definitions)
        {
            ModsManager.Instance.Mods.Clear();
            ModsManager.Instance.ModEnabledList.Clear();

            var model = new List<ModItem>(definitions.Count);
            foreach (ModDefinition definition in definitions)
            {
                string path = @"X:\SyntheticMods\" + definition.Folder;
                var item = new ModItem
                {
                    Enabled = definition.Enabled,
                    Path = path,
                    Name = definition.DisplayName,
                    FolderName = definition.Folder,
                    OriginalLoadOrder = definition.OriginalLoadOrder,
                    CurrentLoadOrder = definition.OriginalLoadOrder,
                    Version = definition.Version,
                    BuildNumber = definition.BuildNumber
                };

                model.Add(item);
                ModsManager.Instance.Mods[path] = new ModsManager.ModData
                {
                    OriginalLoadOrder = definition.OriginalLoadOrder,
                    NewLoadOrder = definition.OriginalLoadOrder
                };
            }

            return model;
        }

        private static bool IsDefaultSorted(IReadOnlyList<ModItem> model)
        {
            for (int index = 1; index < model.Count; index++)
            {
                ModItem previous = model[index - 1];
                ModItem current = model[index];

                if (current.OriginalLoadOrder < previous.OriginalLoadOrder)
                    return false;

                if (current.OriginalLoadOrder == previous.OriginalLoadOrder
                    && string.Compare(
                        previous.FolderName,
                        current.FolderName,
                        StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static IReadOnlyList<ModDefinition> LoadReferenceDefinitions()
        {
            string fixturePath = Path.Combine(
                AppContext.BaseDirectory,
                "Fixtures",
                "reference-mod-order.json");
            string json = File.ReadAllText(fixturePath);
            ReferenceFixture fixture = JsonSerializer.Deserialize<ReferenceFixture>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return fixture.Mods
                .OrderBy(mod => mod.OriginalLoadOrder)
                .ThenBy(mod => mod.Folder, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void AddFailureIfOutOfSync(List<string> failures, ScenarioResult result)
        {
            if (!result.IsInSync)
                failures.Add(result.ToString());
        }

        private static void AssertScenarioInSync(ScenarioResult result)
        {
            Assert.IsTrue(result.IsInSync, result.ToString());
        }

        private static void AssertNoFailures(List<string> failures)
        {
            Assert.AreEqual(
                0,
                failures.Count,
                failures.Count + " synthetic scenarios produced inconsistent output:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, failures.Take(12)));
        }

        private sealed record ScenarioResult(
            bool Reversed,
            MovePosition Position,
            IReadOnlyList<string> Selected,
            IReadOnlyList<string> ViewOrder,
            IReadOnlyList<string> ExpectedModelOrder,
            IReadOnlyList<string> ActualModelOrder,
            IReadOnlyList<float> ExpectedViewLoadOrders,
            IReadOnlyList<float> ActualViewLoadOrders)
        {
            public bool IsInSync =>
                ExpectedModelOrder.SequenceEqual(ActualModelOrder)
                && ExpectedViewLoadOrders.SequenceEqual(ActualViewLoadOrders);

            public override string ToString()
            {
                string direction = Reversed ? "HighToLow" : "LowToHigh";
                return $"{direction} {Position}, selected [{string.Join(", ", Selected)}]"
                    + $": view [{string.Join(", ", ViewOrder)}]"
                    + $", model expected [{string.Join(", ", ExpectedModelOrder)}]"
                    + $", model actual [{string.Join(", ", ActualModelOrder)}]"
                    + $", view load orders expected [{string.Join(", ", ExpectedViewLoadOrders)}]"
                    + $", view load orders actual [{string.Join(", ", ActualViewLoadOrders)}]";
            }
        }

        private sealed record ReferenceFixture(
            string GameVersion,
            IReadOnlyList<ModDefinition> Mods);

        private sealed record ModDefinition(
            string Folder,
            string DisplayName,
            float OriginalLoadOrder,
            bool Enabled,
            string Version,
            int BuildNumber);
    }
}
