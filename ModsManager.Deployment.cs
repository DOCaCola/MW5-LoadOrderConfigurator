using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{
    public partial class ModsManager
    {
        public bool UsesCachedModList => GameVersionPolicy.UsesCachedModList(GameVersion);
        public bool DeploymentNeedsRefresh { get; private set; }
        internal ResolvedGameVersion ResolvedVersion { get; private set; }
        internal Dictionary<string, GameModStatus> LoadedStatuses { get; private set; } =
            new(StringComparer.OrdinalIgnoreCase);
        private JObject _loadedModList;
        private bool _modDiscoveryIncomplete;
        private FileSystemWatcherAsync<eModPathType> _gameInfoWatcher;

        internal void ResolveLoadedPriorities()
        {
            foreach (var entry in Mods)
            {
                float priority = ModDetails[entry.Key].defaultLoadOrder;
                if (UsesCachedModList && LoadedStatuses.TryGetValue(Path.GetFileName(entry.Key), out var status))
                    priority = status.ResolvePriority(priority);
                entry.Value.NewLoadOrder = priority;
                entry.Value.DeployedLoadOrder = priority;
            }
            RefreshDeploymentRequirement();
        }

        internal void RefreshDeploymentRequirement()
        {
            DeploymentNeedsRefresh = false;
            if (!UsesCachedModList)
                return;
            DeploymentNeedsRefresh = _loadedModList == null ||
                GameVersionPolicy.ReadHeader(_loadedModList) != GameVersion;
            string installPath = LocSettings.Instance.Data.InstallPath;
            // The stock Windows game's BaseDir is the shipping executable directory.
            string gameBaseDirectory = string.IsNullOrWhiteSpace(installPath) ? null
                : Path.Combine(installPath, "MW5Mercs", "Binaries", "Win64");
            foreach (var entry in Mods)
            {
                if (!LoadedStatuses.TryGetValue(Path.GetFileName(entry.Key), out var status) ||
                    !status.Priority.HasValue ||
                    !FloatUtils.IsEqual(ModDetails[entry.Key].defaultLoadOrder, status.Priority.Value))
                {
                    DeploymentNeedsRefresh = true;
                    continue;
                }
                try
                {
                    string[] installed = GameModDeployment.EnumeratePakPaths(entry.Key);
                    if ((status.Enabled && installed.Length == 0) ||
                        !GameModDeployment.PakPathsMatch(status.PakPaths, installed, gameBaseDirectory))
                        DeploymentNeedsRefresh = true;
                }
                catch (Exception ex) when (LocFileUtils.IsFileAccessException(ex))
                {
                    DeploymentNeedsRefresh = true;
                }
            }
            if (LoadedStatuses.Keys.Any(folder => !DirNameToPathDict.ContainsKey(folder)))
                DeploymentNeedsRefresh = true;
        }

        public void SynchronizeWorkingModList()
        {
            ModEnabledList = ModItemList.Instance.ModList.AsEnumerable().Reverse().Select(mod =>
            {
                Mods[mod.Path].NewLoadOrder = mod.CurrentLoadOrder;
                return new ModImportData
                {
                    ModPath = mod.Path, ModFolder = mod.FolderName, Enabled = mod.Enabled,
                    LoadOrder = mod.CurrentLoadOrder, Available = true
                };
            }).ToList();
        }

        internal List<string> GetExternallyChangedMods()
        {
            var changed = new List<string>();
            if (LastAppliedPreset?.mods == null)
                return changed;
            var saved = new Dictionary<string, LastAppliedPresetModData>(
                LastAppliedPreset.mods, StringComparer.OrdinalIgnoreCase);
            foreach (var entry in Mods)
            {
                string folder = Path.GetFileName(entry.Key);
                if (!saved.TryGetValue(folder, out var prior))
                    continue;
                bool enabled = LoadedStatuses.TryGetValue(folder, out var status) && status.Enabled;
                if (enabled != prior.state ||
                    !FloatUtils.IsEqual(entry.Value.DeployedLoadOrder, prior.lastLoadOrder) ||
                    !FloatUtils.IsEqual(ModDetails[entry.Key].defaultLoadOrder, prior.lastLoadOrder))
                    changed.Add(ModDetails[entry.Key].displayName);
            }
            return changed;
        }

        private JObject PrepareDeployment()
        {
            if (_modDiscoveryIncomplete)
                throw new InvalidDataException("Some installed mods could not be read. Resolve the reported errors and reload before applying.");
            string path = GetModListJsonFilePath();
            string current = File.Exists(path) ? File.ReadAllText(path) : null;
            if (!string.Equals(current, rawJson, StringComparison.Ordinal))
                throw new IOException("modlist.json changed outside MW5LOC. Reload to compare it with your last applied load order before applying.");
            JObject document = current == null ? null : JObject.Parse(current);
            GameModDeployment.ReadStatuses(document);
            var version = GameVersionPolicy.Resolve(GameVersionPolicy.ReadHeader(document),
                GameVersionPolicy.ReadGameInfoVersion(LocSettings.Instance.Data.InstallPath),
                ModDetails.Values.Select(mod => mod.gameVersion));
            if (version.Value != GameVersion)
                throw new IOException("The detected game version changed. Reload before applying.");
            foreach (var entry in Mods)
            {
                string metadataPath = Path.Combine(entry.Key, "mod.json");
                if (File.ReadAllText(metadataPath) != entry.Value.LoadedMetadata)
                    throw new IOException($"The mod's Mod.json file changed: {metadataPath}. Reload before applying.");
            }
            return GameModDeployment.BuildDocument(document, GameVersion,
                ModEnabledList.Select(mod => new ModDeploymentEntry(mod.ModFolder, mod.ModPath,
                    mod.Enabled, Mods[mod.ModPath].NewLoadOrder)), UsesCachedModList,
                ModsPaths[eModPathType.Program]?.FullPath);
        }

        private void CommitModList(JObject document)
        {
            string contents = document.ToString(Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(GetModListJsonFilePath()));
            LocFileWriter.WriteAllText(GetModListJsonFilePath(), contents);
            rawJson = contents;
            _loadedModList = document;
            KnownModListGameVersion = GameVersion;
            LoadedStatuses = GameModDeployment.ReadStatuses(document);
            foreach (var mod in Mods.Values)
                mod.DeployedLoadOrder = mod.NewLoadOrder;
            ModEnabledListLastState = ModEnabledList.Select(mod => new ModImportData
            {
                ModFolder = mod.ModFolder, ModPath = mod.ModPath, Enabled = mod.Enabled,
                LoadOrder = Mods[mod.ModPath].NewLoadOrder, Available = true
            }).ToList();
        }

        public IReadOnlyList<string> SaveToFiles()
        {
            // Validate and enumerate before touching any deployed file.
            JObject document = PrepareDeployment();
            // Keep Apply pending if any part of the multi-file deployment fails.
            DeploymentNeedsRefresh = true;
            SaveModDetails();
            CommitModList(document);
            var warnings = new List<string>();
            try
            {
                SaveLastAppliedModOrder();
            }
            catch (Exception ex) when (LocFileUtils.IsFileAccessException(ex) || ex is JsonException)
            {
                DeploymentNeedsRefresh = true;
                warnings.Add("The game load order was saved, but LOC could not save its list of last applied load orders."
                    + "\r\n\r\n" + ex.Message);
                return warnings;
            }
            RefreshDeploymentRequirement();
            return warnings;
        }

        internal bool IsDeploymentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (string.Equals(path, GetModListJsonFilePath(), StringComparison.OrdinalIgnoreCase))
                return true;
            string directory = Path.GetDirectoryName(path);
            if (string.Equals(Path.GetFileName(path), "Paks", StringComparison.OrdinalIgnoreCase))
                return Mods.ContainsKey(directory);
            return string.Equals(Path.GetExtension(path), ".pak", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Path.GetFileName(directory), "Paks", StringComparison.OrdinalIgnoreCase) &&
                Mods.ContainsKey(Path.GetDirectoryName(directory));
        }

        private void GameInfoChanged(object sender, FileSystemEventArgs e)
        {
            if (!string.Equals(Path.GetFileName(e.FullPath), "game_info.xml", StringComparison.OrdinalIgnoreCase) &&
                !(e is RenamedEventArgs rename && string.Equals(Path.GetFileName(rename.OldFullPath), "game_info.xml", StringComparison.OrdinalIgnoreCase)))
                return;
            DeploymentNeedsRefresh = true;
            ModFilesChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
