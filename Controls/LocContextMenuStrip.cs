using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MW5_Mod_Manager.Controls;

/// <summary>
/// A context menu whose native popup window can be recreated at the current
/// monitor DPI without rebuilding its managed items or event handlers.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class LocContextMenuStrip : ContextMenuStrip
{
    public LocContextMenuStrip(IContainer components)
        : base(components)
    {
    }

    internal void ReleaseHandleForDpiChange()
    {
        if (IsHandleCreated && !Visible)
            DestroyHandle();
    }

    protected override void RescaleConstantsForDpi(
        int deviceDpiOld,
        int deviceDpiNew)
    {
        // ToolStripDropDownMenu's public constructor initializes its menu
        // gutter constants at 96 DPI. Scaling those constants later changes
        // an already displayed menu's shape after a DPI round trip. LOC
        // scales the visible font, images, and item metrics deterministically,
        // so preserving the original menu chrome keeps repeated transitions
        // reversible.
    }
}
