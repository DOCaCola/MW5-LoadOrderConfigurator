using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{
    internal enum GameVersionSource { ModList, GameInfo, Mods }

    internal sealed record ResolvedGameVersion(string Value, GameVersionSource Source);

    internal static class GameVersionPolicy
    {
        // All consumers select their storage behavior through this one boundary.
        public static bool UsesCachedModList(string version) =>
            Utils.CompareVersionStrings(version, "1.15.398") >= 0;

        internal static string ValidVersion(string value)
        {
            value = value?.Trim();
            return !string.IsNullOrEmpty(value) && value.Split('.').All(part =>
                part.Length > 0 && part.All(char.IsAsciiDigit) &&
                int.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                ? value : null;
        }

        public static string ReadHeader(JObject document) =>
            document?["gameVersion"]?.Type == JTokenType.String
                ? ValidVersion((string)document["gameVersion"]) : null;

        public static string ReadGameInfoVersion(string installPath)
        {
            if (string.IsNullOrWhiteSpace(installPath))
                return null;
            try
            {
                using var reader = XmlReader.Create(Path.Combine(installPath, "game_info.xml"),
                    new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
                return ValidVersion((string)XDocument.Load(reader).Root?.Attribute("Version"));
            }
            catch (Exception ex) when (ex is XmlException || LocFileUtils.IsFileAccessException(ex))
            {
                // This optional source is absent on some installations.
                return null;
            }
        }

        public static ResolvedGameVersion Resolve(string header, string gameInfo, IEnumerable<string> modVersions)
        {
            header = ValidVersion(header);
            gameInfo = ValidVersion(gameInfo);
            if (gameInfo != null && (header == null || Utils.CompareVersionStrings(gameInfo, header) > 0))
                return new(gameInfo, GameVersionSource.GameInfo);
            if (header != null)
                return new(header, GameVersionSource.ModList);

            string highest = "0";
            foreach (string candidate in modVersions.Select(ValidVersion).Where(v => v != null))
                if (Utils.CompareVersionStrings(candidate, highest) > 0)
                    highest = candidate;
            return new(highest, GameVersionSource.Mods);
        }
    }

    internal sealed record GameModStatus(bool Enabled, float? Priority, string[] PakPaths)
    {
        public static GameModStatus Read(JObject json)
        {
            string[] paths = json["cachedPakPaths"] is JArray array &&
                array.All(p => p.Type == JTokenType.String && !string.IsNullOrWhiteSpace((string)p))
                ? array.Values<string>().ToArray() : null;
            return new(json["bEnabled"]?.Type == JTokenType.Boolean && (bool)json["bEnabled"],
                GameModDeployment.ReadPriority(json["defaultLoadOrder"]), paths);
        }

        public float ResolvePriority(float metadataPriority) =>
            Priority ?? (PakPaths?.Length > 0 ? 0f : metadataPriority);
    }

    internal sealed record ModDeploymentEntry(string Folder, string Path, bool Enabled, float Priority);

    internal static class GameModDeployment
    {
        public static float? ReadPriority(JToken token)
        {
            if (token?.Type != JTokenType.Integer && token?.Type != JTokenType.Float)
                return null;
            float value = token.Value<float>();
            return float.IsFinite(value) ? value : null;
        }

        public static Dictionary<string, GameModStatus> ReadStatuses(JObject document)
        {
            var result = new Dictionary<string, GameModStatus>(StringComparer.OrdinalIgnoreCase);
            if (document?["modStatus"] is JObject statuses)
                foreach (var property in statuses.Properties())
                {
                    if (property.Value is not JObject status || !result.TryAdd(property.Name, GameModStatus.Read(status)))
                        throw new InvalidDataException($"Invalid or duplicate mod status: {property.Name}");
                }
            else if (document?["modStatus"] != null)
                throw new InvalidDataException("modStatus must be a JSON object.");
            return result;
        }

        public static string[] EnumeratePakPaths(string modPath)
        {
            string path = Path.Combine(modPath, "Paks");
            try
            {
                return Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                    .Where(file => string.Equals(Path.GetExtension(file), ".pak", StringComparison.OrdinalIgnoreCase))
                    .Select(file => Path.GetFullPath(file).Replace('\\', '/'))
                    .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                return Array.Empty<string>();
            }
        }

        public static bool PakPathsMatch(string[] saved, string[] installed) =>
            saved != null && saved.Select(p => p.Replace('\\', '/'))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .SequenceEqual(installed, StringComparer.OrdinalIgnoreCase);

        public static JObject BuildDocument(JObject previous, string version,
            IEnumerable<ModDeploymentEntry> mods, bool cachedMode)
        {
            var result = previous == null ? new JObject() : (JObject)previous.DeepClone();
            var previousStatuses = result["modStatus"] as JObject;
            var statuses = new JObject();
            var folders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mod in mods)
            {
                if (!folders.Add(mod.Folder))
                    throw new InvalidDataException($"Multiple installed mods use the status name '{mod.Folder}'. Resolve the duplicate before applying.");
                if (!File.Exists(Path.Combine(mod.Path, "mod.json")))
                    throw new IOException($"The mod metadata is missing: {mod.Path}. Reload before applying.");
                if (!float.IsFinite(mod.Priority))
                    throw new InvalidDataException($"Invalid load order for {mod.Folder}.");

                var oldStatus = previousStatuses?.GetValue(mod.Folder, StringComparison.OrdinalIgnoreCase) as JObject;
                var status = oldStatus == null ? new JObject() : (JObject)oldStatus.DeepClone();
                status["bEnabled"] = mod.Enabled;
                if (cachedMode)
                {
                    string[] paths = EnumeratePakPaths(mod.Path);
                    if (mod.Enabled && paths.Length == 0)
                        throw new InvalidDataException($"Enabled mod '{mod.Folder}' has no pak files in '{Path.Combine(mod.Path, "Paks")}'.");
                    status["defaultLoadOrder"] = mod.Priority;
                    status["cachedPakPaths"] = JArray.FromObject(paths);
                }
                else
                {
                    // Do not leave stale cached priorities behind after a downgrade.
                    status.Remove("defaultLoadOrder");
                    status.Remove("cachedPakPaths");
                }
                statuses.Add(mod.Folder, status);
            }
            result["gameVersion"] = version;
            result["modStatus"] = statuses;
            return result;
        }

        public static void WriteAtomic(string path, string contents)
        {
            string temporary = Path.Combine(Path.GetDirectoryName(path), $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    byte[] bytes = new UTF8Encoding(false).GetBytes(contents);
                    stream.Write(bytes);
                    stream.Flush(true);
                }
                if (File.Exists(path))
                    File.Replace(temporary, path, null);
                else
                    File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }

        public static string RestoreMetadata(string path, string loadedContents)
        {
            string current = File.ReadAllText(path);
            var metadata = JObject.Parse(current);
            if (!metadata.ContainsKey("locOriginalLoadOrder"))
                return current;
            if (!string.Equals(current, loadedContents, StringComparison.Ordinal))
                throw new IOException($"The mod's Mod.json file changed since it was loaded: {path}. Reload before restoring its original load-order value.");
            float? original = ReadPriority(metadata["locOriginalLoadOrder"]);
            if (!original.HasValue)
                throw new InvalidDataException($"Invalid locOriginalLoadOrder in {path}.");
            metadata["defaultLoadOrder"] = metadata["locOriginalLoadOrder"].DeepClone();
            metadata.Remove("locOriginalLoadOrder");
            string restored = metadata.ToString(Newtonsoft.Json.Formatting.Indented);
            WriteAtomic(path, restored);
            return restored;
        }
    }
}
