using DarkModeForms;
using System.Drawing;

namespace MW5_Mod_Manager.Controls;

public static class LocWindowColors
{
    private static bool _DarkMode = DarkModeCS.GetWindowsColorMode() <= 0;
    public static OSThemeColors OScolors { get; set; } = DarkModeCS.GetSystemColors();

    public static bool DarkMode
    {
        get { return _DarkMode; }
        set { _DarkMode = value; }
    }

    public static Color ControlText
    {
        get
        {
            return _DarkMode ? OScolors.TextActive : SystemColors.ControlText;
        }
    }

    public static Color WindowText
    {
        get
        {
            return _DarkMode ? OScolors.TextActive : SystemColors.WindowText;
        }
    }

    public static Color Window
    {
        get
        {
            return _DarkMode ? OScolors.Control : SystemColors.Window;
        }
    }

    public static Color ButtonHighlight
    {
        get
        {
            return _DarkMode ? OScolors.ControlLight : SystemColors.ButtonHighlight;
        }
    }

    // List Drag drop feedback color
    public static Color ListFeedBackColor
    {
        get
        {
            return _DarkMode ? Color.White : Color.Black;
        }
    }

    public static Color ListColorAlternate
    {
        get
        {
            return _DarkMode ? Color.FromArgb(66, 66, 66) : Color.FromArgb(246, 245, 246);
        }
    }

    public static Color ListModHighlightColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(71, 118, 39) : Color.FromArgb(200, 253, 213);
        }
    }

    public static Color ListModHighlightColorAlternate
    {
        get
        {
            return _DarkMode ? Color.FromArgb(77, 120, 40) : Color.FromArgb(189, 240, 202);
        }
    }

    public static Color ListModOverriddenBackColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(78, 62, 8) : Color.FromArgb(255, 242, 203);
        }
    }

    public static Color ListModOverriddenBackColorAlternate
    {
        get
        {
            return _DarkMode ? Color.FromArgb(90, 72, 10) : Color.FromArgb(247, 234, 196);
        }
    }

    public static Color ListModOverridingBackColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(60, 36, 109) : Color.FromArgb(235, 225, 255);
        }
    }

    public static Color ListModOverridingBackColorAlternate
    {
        get
        {
            return _DarkMode ? Color.FromArgb(68, 41, 122) : Color.FromArgb(226, 217, 245);
        }
    }

    public static Color ModOverriddenColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(243, 209, 111) : Color.FromArgb(131, 101, 0);
        }
    }

    public static Color ModOverridingColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(184, 148, 255) : Color.FromArgb(80, 37, 192);
        }
    }

    public static Color ModOverriddenOveridingColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(223, 123, 147) : Color.FromArgb(170, 73, 97);
        }
    }


    public static Color ModHighPriorityColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(255, 114, 124) : Color.FromArgb(252, 54, 63);
        }
    }

    public static Color ModLowPriorityColor
    {
        get
        {
            return _DarkMode ? Color.FromArgb(33, 214, 39) : Color.FromArgb(17, 137, 21);
        }
    }
}