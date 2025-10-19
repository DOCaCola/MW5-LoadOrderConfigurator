using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            if (Instance.ModList != null)
            {
                Instance.ModList.Clear();
            }
            else
            {
                Instance.ModList = new List<ModItem>();
            }
            foreach (var entry in orderedModList.ReverseIterateIf(LocSettings.Instance.Data.ListSortOrder == eSortOrder.LowToHigh))
            {
                ModItem newItem = ModItem.CreateFromImportData(entry);

                Instance.ModList.Add(newItem);
            }
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
                Author = ModsManager.Instance.ModDetails[entry.ModPath].author,
                CurrentLoadOrder = ModsManager.Instance.Mods[entry.ModPath].NewLoadOrder,
                OriginalLoadOrder = ModsManager.Instance.Mods[entry.ModPath].OriginalLoadOrder,
                Origin = ModsManager.Instance.Mods[entry.ModPath].Origin,
                Version = ModsManager.Instance.ModDetails[entry.ModPath].version,
                BuildNumber = ModsManager.Instance.ModDetails[entry.ModPath].buildNumber,
                VersionCombined = (ModsManager.Instance.ModDetails[entry.ModPath].version + " (" + ModsManager.Instance.ModDetails[entry.ModPath].buildNumber.ToString() + ")").Trim()
            };
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