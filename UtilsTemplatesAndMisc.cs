using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    internal static class Utils
    {
        public static bool StringNullEmptyOrWhiteSpace(string txt)
        {
            return string.IsNullOrEmpty(txt) || string.IsNullOrWhiteSpace(txt);
        }

        public static bool IsUrlValid(string input)
        {
            Uri uriResult;
            bool isValidUri = Uri.TryCreate(input, UriKind.Absolute, out uriResult)
                              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return isValidUri;
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        public static bool IsSubdirectory(string candidate, string parent)
        {
            // Normalize the paths to ensure consistency
            candidate = Path.GetFullPath(candidate).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            parent = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Check if the candidate starts with the parent's path
            return candidate.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        public static String BytesToHumanReadableString(long byteCount)
        {
            string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        public static string ToTimeSinceString(this DateTime value)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - value.Ticks);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (seconds < 60 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (seconds < 120 * MINUTE)
                return "an hour ago";

            if (seconds < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (seconds < 48 * HOUR)
                return "yesterday";

            if (seconds < 30 * DAY)
                return ts.Days + " days ago";

            if (seconds < 12 * MONTH) {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        public static Color InterpolateColor(Color fromColor, Color toColor, double ratio)
        {
            int r = (int)(fromColor.R + (toColor.R - fromColor.R) * ratio);
            int g = (int)(fromColor.G + (toColor.G - fromColor.G) * ratio);
            int b = (int)(fromColor.B + (toColor.B - fromColor.B) * ratio);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Compare two version strings, e.g.  "3.2.1.0.b40" and "3.10.1.a".
        /// V1 and V2 can have different number of components.
        /// Components must be delimited by dot.
        /// </summary>
        /// <remarks>
        /// This doesn't do any null/empty checks so please don't pass dumb parameters
        /// </remarks>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>
        /// -1 if v1 is lower version number than v2,
        /// 0 if v1 == v2,
        /// 1 if v1 is higher version number than v2,
        /// -1000 if we couldn't figure it out (something went wrong)
        /// </returns>
        public static int CompareVersionStrings(string v1, string v2)
        {
            int rc = -1000;

            v1 = v1.ToLower();
            v2 = v2.ToLower();

            if (v1 == v2)
                return 0;

            string[] v1parts = v1.Split('.');
            string[] v2parts = v2.Split('.');

            for (int i = 0; i < v1parts.Length; i++)
            {
                if (v2parts.Length < i+1)
                    break; // we're done here
                
                string v1Token = v1parts[i];
                string v2Token = v2parts[i];
                
                int x;
                bool v1Numeric = int.TryParse(v1Token, out x);
                bool v2Numeric = int.TryParse(v2Token, out x);
                
                // handle scenario {"2" versus "20"} by prepending zeroes, e.g. it would become {"02" versus "20"}
                if (v1Numeric && v2Numeric) {
                    while (v1Token.Length < v2Token.Length)
                        v1Token = "0" + v1Token;
                    while (v2Token.Length < v1Token.Length)
                        v2Token = "0" + v2Token;
                }

                rc = String.Compare(v1Token, v2Token, StringComparison.Ordinal);
                //Console.WriteLine("v1Token=" + v1Token + " v2Token=" + v2Token + " rc=" + rc);
                if (rc != 0)
                    break;
            }

            if (rc == 0)
            {
                // catch this scenario: v1="1.0.1" v2="1.0"
                if (v1parts.Length > v2parts.Length)
                    rc = 1; // v1 is higher version than v2
                // catch this scenario: v1="1.0" v2="1.0.1"
                else if (v2parts.Length > v1parts.Length)
                    rc = -1; // v1 is lower version than v2
            }

            if (rc == 0 || rc == -1000)
                return rc;
            else
                return rc < 0 ? -1 : 1;
        }

        // Method to calculate the folder depth of a given path
        public static int GetFolderDepth(string path)
        {
            int depth = 0;
            foreach (char c in path)
            {
                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
                {
                    depth++;
                }
            }
            return depth;
        }

        public static Font CreateBestAvailableMonospacePlatformFont(float fontSize)
        {
            string[] preferredFonts = { "Cascadia Code", "Consolas", "Lucida Console", "Courier New" };

            Font selectedFont = preferredFonts
                .Select(fontName => new Font(fontName, fontSize))
                .FirstOrDefault(font => font.Name == preferredFonts.FirstOrDefault(f => f == font.Name));

            return selectedFont;
        }

        public static string RtfEscape(string s) {
            if (s == null)
                return s;

            var sb = new StringBuilder();
            char c;

            for (int i = 0; i < s.Length; i++) {
                c = s[i];

                // \r
                if (c == 13)
                    continue;

                // \n
                if (c == 10) {
                    sb.Append("\\line ");
                }
                else if (c >= 0x20 && c < 0x80) {
                    if (c == '\\' || c == '{' || c == '}')
                        sb.Append('\\');

                    sb.Append(c);
                }
                else if (c < 0x20 || (c >= 0x80 && c <= 0xFF)) {
                    sb
                        .Append('\\')
                        .Append(((byte) c).ToString("X"));
                }
                else {
                    sb
                        .Append("\\u")
                        .Append((uint) c)
                        .Append('?');
                }
            }

            return sb.ToString();
        }
    }

    public class ModObject
    {
        public string displayName { set; get; }
        public string version { set; get; }
        public int buildNumber { set; get; }
        public string description { set; get; }
        public string author { set; get; }
        public string authorURL { set; get; }
        public float defaultLoadOrder { set; get; }
        public string gameVersion { set; get; }
        public List<string> manifest { get; set; }
        public long steamPublishedFileId { set; get; }
        public long steamLastSubmittedBuildNumber { set; get; }
        public string steamModVisibility { set; get; }
        public float locOriginalLoadOrder { set; get; }
    }

    public class OverridingData
    {
        public string mod { set; get; }
        public bool isOverridden { set; get; }
        public bool isOverriding { set; get; }
        public Dictionary<string, List<string>> overrides { set; get; }
        public Dictionary<string, List<string>> overriddenBy { set; get; }
    }

    public class ModListBoxItem
    {
        public string DisplayName { get; set; }

        public string ModKey { get; set; }

        public string ModDirName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    static class FloatUtils {

        public const float DefaultEpsilon = (float)0.00001;

        public static bool IsEqual(float a, float b, float epsilon = DefaultEpsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }
    }

    /// <summary>An array indexed by an Enum</summary>
    /// <typeparam name="T">Type stored in array</typeparam>
    /// <typeparam name="U">Indexer Enum type</typeparam>
    public class ArrayByEnum<T,U> : IEnumerable where U : Enum // requires C# 7.3 or later
    {
        private readonly T[] _array;
        private readonly int _lower;

        public ArrayByEnum()
        {
            _lower = Convert.ToInt32(Enum.GetValues(typeof(U)).Cast<U>().Min());
            int upper = Convert.ToInt32(Enum.GetValues(typeof(U)).Cast<U>().Max());
            _array = new T[1 + upper - _lower];
        }

        public T this[U key]
        {
            get { return _array[Convert.ToInt32(key) - _lower]; }
            set { _array[Convert.ToInt32(key) - _lower] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return Enum.GetValues(typeof(U)).Cast<U>().Select(i => this[i]).GetEnumerator();
        }
    }
}


public static class EnumerableExtensions
{
    public static IEnumerable<TSource> ReverseIf<TSource>(this IEnumerable<TSource> source, bool reverse)
    {
        return reverse ? source.Reverse() : source;
    }

    public static IEnumerable<T> ReverseIterate<T>(this IList<T> items)
    {
        for (var i = items.Count - 1; i >= 0; i--)
            yield return items[i];
    }

    public static IEnumerable<T> ReverseIterateIf<T>(this IEnumerable<T> enumerable, bool reverse)
    {
        if (reverse)
        {
            var list = enumerable.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                yield return list[i];
            }
        }
        else
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }
        }
    }
}

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> ReverseIf<TKey, TValue>(this Dictionary<TKey, TValue> source, bool reverse)
    {
        if (reverse)
        {
            var reversedDict = source.Reverse().ToDictionary(kv => kv.Key, kv => kv.Value);
            return reversedDict;
        }
        else
        {
            return source;
        }
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> ReverseIterate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        var keys = new List<TKey>(dictionary.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var key = keys[i];
            yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
        }
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> ReverseIterateIf<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool reverse)
    {
        if (reverse)
        {
            var keys = new List<TKey>(dictionary.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var key = keys[i];
                yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
            }
        }
        else
        {
            foreach (var pair in dictionary)
            {
                yield return pair;
            }
        }
    }
}

[SupportedOSPlatform("windows")]
public static class ListViewExtensions
{
    /// <summary>
    /// Sets the double buffered property of a list view to the specified value
    /// </summary>
    /// <param name="listView">The List view</param>
    /// <param name="doubleBuffered">Double Buffered or not</param>
    public static void SetDoubleBuffered(this System.Windows.Forms.ListView listView, bool doubleBuffered = true)
    {
        listView
            .GetType()
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(listView, doubleBuffered, null);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam);

    private const int WM_SETREDRAW = 0x000B;

    // Force redraw of the list, even if in BeginUpdate/EndUpdate state
    public static void ForceRedraw(this ListView listView)
    {
        SendMessage(listView.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
    }
}

namespace System.Windows.Forms
{
    [SupportedOSPlatform("windows")]
    public static class ControlExtensions
    {
        private const int WM_SETREDRAW = 0x000B;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam);

        public static void SuspendDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public static void ResumeDrawing(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            control.Refresh();
        }

        public static void ForceRedraw(this Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
        }
    }
}

public static class Int32Extensions
{
    public static int Digits(this int n)
    {
        if (n >= 0)
        {
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            if (n < 100000) return 5;
            if (n < 1000000) return 6;
            if (n < 10000000) return 7;
            if (n < 100000000) return 8;
            if (n < 1000000000) return 9;
            return 10;
        }
        else
        {
            if (n > -10) return 2;
            if (n > -100) return 3;
            if (n > -1000) return 4;
            if (n > -10000) return 5;
            if (n > -100000) return 6;
            if (n > -1000000) return 7;
            if (n > -10000000) return 8;
            if (n > -100000000) return 9;
            if (n > -1000000000) return 10;
            return 11;
        }
    }
}