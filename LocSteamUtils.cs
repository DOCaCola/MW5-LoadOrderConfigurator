using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MW5_Mod_Manager
{
    internal class SteamUtils
    {
        public static bool IsSteamRunning()
        {
            // Get Steam.exe path from registry
            string steamExePath;
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
            {
                steamExePath = key?.GetValue("SteamExe") as string;
            }

            if (string.IsNullOrEmpty(steamExePath) || !File.Exists(steamExePath))
                return false;

            string normalizedSteamExePath = Path.GetFullPath(steamExePath);
            string exeName = Path.GetFileNameWithoutExtension(normalizedSteamExePath);

            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try
                {
                    string processPath = Path.GetFullPath(process.MainModule.FileName);

                    if (string.Equals(normalizedSteamExePath, processPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch
                {
                    // Ignore processes we can't query
                }
            }

            return false;
        }

        public static string CreateRunGameCommand(int GameID, string Args = null)
        {
            string command = $"steam://run/{GameID}";
            if (!string.IsNullOrEmpty(Args))
            {
                command += $"//{Args}";
            }
            return command;
        }
    }
}
