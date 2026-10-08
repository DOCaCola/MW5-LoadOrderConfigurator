using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MW5_Mod_Manager
{
    internal static class LocFileWriter
    {
        // Atomic replacement for ordinary files; in-place writing preserves hard links.
        public static void WriteAllText(string path, string contents)
        {
            var destination = new FileInfo(path);
            if (destination.LinkTarget != null)
            {
                var target = destination.ResolveLinkTarget(returnFinalTarget: true);
                if (!target.Exists)
                    throw new FileNotFoundException(
                        $"Cannot save '{path}': symbolic-link target '{target.FullName}' does not exist.",
                        target.FullName);
                destination = new FileInfo(target.FullName);
            }

            byte[] bytes = new UTF8Encoding(false).GetBytes(contents);
            if (destination.Exists)
            {
                // Open without truncating. Query and update the same file under an
                // exclusive handle so a hard-linked file keeps its identity.
                using var stream = new FileStream(destination.FullName, FileMode.Open, FileAccess.Write, FileShare.None);
                if (!GetFileInformationByHandle(stream.SafeFileHandle, out var information))
                    throw new IOException($"Could not inspect links for '{path}' (target '{destination.FullName}').",
                        new Win32Exception(Marshal.GetLastWin32Error()));
                if (information.NumberOfLinks > 1)
                {
                    stream.Write(bytes);
                    stream.SetLength(bytes.Length);
                    stream.Flush(true);
                    return;
                }
            }

            // A file link may cross volumes. Stage beside its resolved target, not
            // beside the link. Parent-directory symlinks and junctions are traversed
            // by Windows for both the temporary file and the destination.
            string temporary = Path.Combine(destination.DirectoryName, $".{destination.Name}.{Guid.NewGuid():N}.tmp");
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes);
                    stream.Flush(true);
                }
                if (destination.Exists)
                    File.Replace(temporary, destination.FullName, null);
                else
                    File.Move(temporary, destination.FullName);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ByHandleFileInformation
        {
            public uint FileAttributes;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetFileInformationByHandle(
            SafeFileHandle file, out ByHandleFileInformation information);
    }
}
