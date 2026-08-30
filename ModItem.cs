using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using MW5_Mod_Manager;
using MW5_Mod_Manager.Controls;

// Current (unapplied) state of the mod list

namespace MW5_Mod_Manager
{

    [SupportedOSPlatform("windows")]
    public class ModItemList
    {
        public static ModItemList Instance = new ModItemList();

        public List<ModItem> ModList = null;

        public static void FillFromImportList(List<ModsManager.ModImportData> orderedModList)
        {
            if (orderedModList == null)
            {
                Instance.ModList = new List<ModItem>();
                return;
            }

            List<ModItem> targetList = Instance.ModList ?? new List<ModItem>(orderedModList.Count);
            targetList.Clear();

            foreach (var entry in orderedModList.AsEnumerable().Reverse())
            {
                ModItem newItem = ModItem.CreateFromImportData(entry);
                targetList.Add(newItem);
            }

            Instance.ModList = targetList;
        }

        public IEnumerable<ModItem> EnumerateLowToHigh()
        {
            return ModList ?? Enumerable.Empty<ModItem>();
        }

        public IEnumerable<ModItem> EnumerateForView()
        {
            if (ModList == null)
                yield break;

            if (LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow)
            {
                for (int i = ModList.Count - 1; i >= 0; i--)
                {
                    yield return ModList[i];
                }
            }
            else
            {
                for (int i = 0; i < ModList.Count; i++)
                {
                    yield return ModList[i];
                }
            }
        }

        public List<ModItem> GetViewOrderedItems()
        {
            return EnumerateForView().ToList();
        }

        public int ViewIndexToModelIndex(int viewIndex)
        {
            if (ModList == null)
                return -1;

            return LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow
                ? ModList.Count - 1 - viewIndex
                : viewIndex;
        }

        public int ModelIndexToViewIndex(int modelIndex)
        {
            if (ModList == null)
                return -1;

            return LocSettings.Instance.Data.ListSortOrder == eSortOrder.HighToLow
                ? ModList.Count - 1 - modelIndex
                : modelIndex;
        }

        public int GetModCount(bool enabledOnly)
        {
            int count = 0;
            if (enabledOnly)
            {
                foreach (var curMod in ModList)
                {
                    if (curMod.Enabled) { count++; }
                }
            }
            else
            {
                count = ModList.Count;
            }

            return count;
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(ModItem.DebugView))]
    public class ModItem
    {
        public bool Enabled;
        public string Path;
        public string Name;
        public string FolderName;
        public string Author;
        public string Version;
        public int BuildNumber;
        public string VersionCombined;
        public long FileSize;
        public DateTimeOffset? FileAge;
        public bool FileMetadataLoaded;
        public bool FileMetadataAvailable;
        public float CurrentLoadOrder;
        public float OriginalLoadOrder;
        public ModsManager.ModData.ModOrigin Origin;

        public Color ProcessedRowBackColor = LocWindowColors.Window;
        public Color ProcessedCurLoForeColor = LocWindowColors.WindowText;
        public Color ProcessedOrgLoForeColor = LocWindowColors.WindowText;

        public static ModItem CreateFromImportData(ModsManager.ModImportData entry)
        {
            var modPath = entry.ModPath;
            return new ModItem
            {
                Enabled = entry.Enabled,
                Path = modPath,
                Name = ModsManager.Instance.ModDetails[entry.ModPath].displayName,
                FolderName = ModsManager.Instance.PathToDirNameDict[entry.ModPath],
                FileSize = ModsManager.Instance.Mods[entry.ModPath].ModFileSize,
                FileAge = ModsManager.Instance.Mods[entry.ModPath].FileAge,
                FileMetadataLoaded = ModsManager.Instance.Mods[entry.ModPath].FileMetadataLoaded,
                FileMetadataAvailable = ModsManager.Instance.Mods[entry.ModPath].FileMetadataAvailable,
                Author = ModsManager.Instance.ModDetails[entry.ModPath].author,
                CurrentLoadOrder = ModsManager.Instance.Mods[entry.ModPath].NewLoadOrder,
                OriginalLoadOrder = ModsManager.Instance.Mods[entry.ModPath].OriginalLoadOrder,
                Origin = ModsManager.Instance.Mods[entry.ModPath].Origin,
                Version = ModsManager.Instance.ModDetails[entry.ModPath].version,
                BuildNumber = ModsManager.Instance.ModDetails[entry.ModPath].buildNumber,
                VersionCombined = (ModsManager.Instance.ModDetails[entry.ModPath].version + " (" + ModsManager.Instance.ModDetails[entry.ModPath].buildNumber.ToString() + ")").Trim()
            };
        }

        private string DebuggerDisplay => $"{Name} [{FolderName}] Enabled={Enabled} LO={CurrentLoadOrder}/{OriginalLoadOrder} Origin={Origin}";

        internal sealed class DebugView
        {
            private readonly ModItem m;
            public DebugView(ModItem m) { this.m = m; }

            public bool Enabled => m.Enabled;
            public string Name => m.Name;
            public string FolderName => m.FolderName;
            public string Path => m.Path;
            public string Author => m.Author;
            public string Version => m.Version;
            public int BuildNumber => m.BuildNumber;
            public string VersionCombined => m.VersionCombined;
            public long FileSize => m.FileSize;
            public DateTimeOffset? FileAge => m.FileAge;
            public bool FileMetadataLoaded => m.FileMetadataLoaded;
            public bool FileMetadataAvailable => m.FileMetadataAvailable;
            public float CurrentLoadOrder => m.CurrentLoadOrder;
            public float OriginalLoadOrder => m.OriginalLoadOrder;
            public ModsManager.ModData.ModOrigin Origin => m.Origin;
        }
    }
}

public static class ModListExtensions
{
    public static int ComputeModListHashCode(this List<ModItem> list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        var stringBuilder = new StringBuilder();
        foreach (var item in list)
        {
            stringBuilder.Append(item.Enabled);
            stringBuilder.Append(item.Path ?? string.Empty);
            stringBuilder.Append(item.CurrentLoadOrder);
        }
        return stringBuilder.ToString().GetHashCode();
    }
}
