using MW5_Mod_Manager.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WeifenLuo.WinFormsUI.ThemeVS2012;
using WeifenLuo.WinFormsUI.ThemeVS2015;

namespace MW5_Mod_Manager.Controls
{

    public class LocLightTheme : VS2015LightTheme
    {
        public LocLightTheme()
        {
           Extender.DockPaneCaptionFactory =
               new LocDockPaneCaptionFactory();
           ToolStripRenderer = (ToolStripRenderer) new LocThemeToolStripRenderer(ColorPalette)
           {
               UseGlassOnMenuStrip = false
           };
        }

        [System.ComponentModel.Browsable(false),
         System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public new ToolStripRenderer ToolStripRenderer
        {
            get => base.ToolStripRenderer;
            set => base.ToolStripRenderer = value;
        }
    }

    public class LocDarkTheme : VS2015DarkTheme
    {
        public LocDarkTheme()
        {
            Extender.DockPaneCaptionFactory =
                new LocDockPaneCaptionFactory();
            ToolStripRenderer = (ToolStripRenderer) new LocThemeToolStripRenderer(ColorPalette)
            {
                UseGlassOnMenuStrip = false
            };
        }

        [System.ComponentModel.Browsable(false),
         System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public new ToolStripRenderer ToolStripRenderer
        {
            get => base.ToolStripRenderer;
            set => base.ToolStripRenderer = value;
        }
    }

    class LocThemeToolStripRenderer : VisualStudioToolStripRenderer
    {
        public LocThemeToolStripRenderer(DockPanelColorPalette palette) : base(palette)
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {

            if (e.TextColor == SystemColors.ControlText
                || e.TextColor == SystemColors.MenuText
                || e.TextColor == SystemColors.HighlightText)
            {
                base.OnRenderItemText(e);
                return;
            }

            //e.TextColor = Color.DarkRed;
            /*
            using (Font boldFont = new Font(e.TextFont, FontStyle.Bold))
            {
                e.TextFont = boldFont;
                base.OnRenderItemText(e);
            }*/

            TextRenderer.DrawText((IDeviceContext) e.Graphics, e.Text, e.TextFont, e.TextRectangle, e.TextColor, e.TextFormat);
        }
    }
}
