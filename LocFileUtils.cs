using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW5_Mod_Manager
{
    internal class LocFileUtils
    {
        public static long GetFileSize(string filePath)
        {
            var fi = new FileInfo(filePath);
            long fileSize = fi.Length;

			// Symlinked files report their file size as zero
            if (fileSize == 0)
            {
                if (fi.LinkTarget != null && fi.DirectoryName != null)
                {
                    string linkTargetPath = Path.GetFullPath(Path.Combine(fi.DirectoryName, fi.LinkTarget));
                    var fiLinkTarget = new FileInfo(linkTargetPath);
                    fileSize = fiLinkTarget.Length;
                }
            }

            return fileSize;
        }
    }
}
