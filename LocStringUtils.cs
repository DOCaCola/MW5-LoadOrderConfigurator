using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MW5_Mod_Manager
{
    internal class LocStringUtils
    {
        /// Wraps a long path for tooltip display.
        /// Breaks the path into multiple lines at directory separators or after a certain length.
        public static string WrapPathForTooltip(string path, int maxLineLength = 40)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var sb = new StringBuilder();
            int lineStart = 0;

            for (int i = 0; i < path.Length; i++)
            {
                // Break at directory separator if line length exceeds maxLineLength
                if (i - lineStart >= maxLineLength && (path[i] == '\\' || path[i] == '/'))
                {
                    sb.AppendLine(path.Substring(lineStart, i - lineStart + 1));
                    lineStart = i + 1;
                }
            }

            // Append any remaining part of the path
            if (lineStart < path.Length)
            {
                sb.Append(path.Substring(lineStart));
            }

            return sb.ToString();
        }
    }
}
