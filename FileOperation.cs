using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MW5_Mod_Manager
{
    internal class FileOperation
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public uint wFunc;
            public string pFrom;
            public string pTo;
            public ushort fFlags;
            public int fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        public static bool DeleteFile(string filePath, IntPtr hwnd = new IntPtr())
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.hwnd = hwnd;
            fileop.wFunc = 3; // FO_DELETE
            fileop.pFrom = filePath + "\0"; // Must be double null terminated
            fileop.fFlags = 0x0;

            int result = SHFileOperation(ref fileop);

            return result == 0;
        }

        public static bool CopyDirectory(string sourceDirPath, string destDirPath, IntPtr hwnd = new IntPtr())
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.hwnd = hwnd;
            fileop.wFunc = 2; // FO_COPY
            fileop.pFrom = sourceDirPath + "\0"; // Must be double null terminated
            fileop.pTo = destDirPath + "\0"; // Must be double null terminated
            fileop.fFlags = 0x0;

            int result = SHFileOperation(ref fileop);

            return result == 0;
        }
    }
}
