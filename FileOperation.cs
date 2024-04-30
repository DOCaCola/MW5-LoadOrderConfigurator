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
        private const int FO_COPY = 0x0002;
        private const int FO_DELETE = 0x0003;
        private const int FO_MOVE = 0x0001;
        private const int FO_RENAME = 0x0004;
        private const int FOF_ALLOWUNDO = 0x40;
        private const int FOF_NOCONFIRMATION = 0x10;
        private const int FOF_NOCONFIRMMKDIR = 0x0200;

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

        public static bool DeleteFile(string filePath, bool recycleBin, IntPtr hwnd = new IntPtr())
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.hwnd = hwnd;
            fileop.wFunc = 3; // FO_DELETE
            fileop.pFrom = filePath + "\0"; // Must be double null terminated
            fileop.fFlags = FOF_NOCONFIRMATION;
            if (recycleBin)
            {
                fileop.fFlags |= FOF_ALLOWUNDO;
            }

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
