using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HardlinkUtils
{
    public static class HardLinkHelper
    {

        #region WinAPI P/Invoke declarations
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr FindFirstFileNameW(string lpFileName, uint dwFlags, ref uint StringLength, StringBuilder LinkName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool FindNextFileNameW(IntPtr hFindStream, ref uint StringLength, StringBuilder LinkName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool GetVolumePathName(string lpszFileName, [Out] StringBuilder lpszVolumePathName, uint cchBufferLength);

        public static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1); // 0xffffffff;
        public const int MAX_PATH = 65535; // Max. NTFS path length.
        #endregion

        /// <summary>
        //// Returns the enumeration of hardlinks for the given *file* as full file paths, which includes
        /// the input path itself.
        /// </summary>
        /// <remarks>
        /// If the file has only one hardlink (itself), or you specify a directory, only that
        /// file's / directory's full path is returned.
        /// If the path refers to a volume that doesn't support hardlinks, or the path
        /// doesn't exist, null is returned.
        /// </remarks>
        public static List<string> GetHardLinks(string filepath)
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            uint charCount = (uint)sbPath.Capacity; // in/out character-count variable for the WinAPI calls.
                                                    // Get the volume (drive) part of the target file's full path (e.g., @"C:\")
            GetVolumePathName(filepath, sbPath, (uint)sbPath.Capacity);
            string volume = sbPath.ToString();
            // Trim the trailing "\" from the volume path, to enable simple concatenation
            // with the volume-relative paths returned by the FindFirstFileNameW() and FindFirstFileNameW() functions,
            // which have a leading "\"
            volume = volume.Substring(0, volume.Length - 1);
            // Loop over and collect all hard links as their full paths.
            IntPtr findHandle;
            if (INVALID_HANDLE_VALUE == (findHandle = FindFirstFileNameW(filepath, 0, ref charCount, sbPath))) return null;
            List<string> links = new List<string>();
            do
            {
                links.Add(volume + sbPath.ToString()); // Add the full path to the result list.
                charCount = (uint)sbPath.Capacity; // Prepare for the next FindNextFileNameW() call.
            } while (FindNextFileNameW(findHandle, ref charCount, sbPath));
            FindClose(findHandle);
            return links;
        }

    }
}