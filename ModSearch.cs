using BrightIdeasSoftware;
using System;
using System.Collections.Generic;

namespace MW5_Mod_Manager
{
    internal static class ModSearch
    {
        public static IModelFilter CreateFilter(string searchText)
        {
            return new ModelFilter(model =>
                model is ModItem mod && Matches(mod, searchText));
        }

        public static bool Matches(ModItem mod, string searchText)
        {
            if (mod == null || string.IsNullOrWhiteSpace(searchText))
                return true;

            return Contains(mod.Name, searchText)
                   || Contains(mod.Author, searchText)
                   || Contains(mod.FolderName, searchText);
        }

        public static ModItem FindFirst(
            IEnumerable<ModItem> mods,
            string searchText)
        {
            if (mods == null)
                return null;

            foreach (ModItem mod in mods)
            {
                if (Matches(mod, searchText))
                    return mod;
            }

            return null;
        }

        public static bool AllMatch(
            IEnumerable<ModItem> mods,
            string searchText)
        {
            if (mods == null)
                return true;

            foreach (ModItem mod in mods)
            {
                if (!Matches(mod, searchText))
                    return false;
            }

            return true;
        }

        private static bool Contains(string value, string searchText)
        {
            return !string.IsNullOrEmpty(value)
                   && value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
