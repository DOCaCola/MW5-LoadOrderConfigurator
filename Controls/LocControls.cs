﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using BrightIdeasSoftware;
using SharpCompress;

namespace MW5_Mod_Manager.Controls
{
    [SupportedOSPlatform("windows")]
    public class ModsObjectsListView : ObjectListView
    {

        public ModsObjectsListView()
        {
            // Hide selection dotted line
            SendMessage(Handle, 0x127, 0x10001, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern nint SendMessage(nint hWnd, int Msg, int wParam, int lParam);

        protected override bool ProcessLButtonDown(OlvListViewHitTestInfo hti)
        {
            //return true;
            return base.ProcessLButtonDown(hti);
        }
    }

    //The rotating label for priority indication.
    [SupportedOSPlatform("windows")]
    public class RotatingLabel : Label
    {
        private int m_RotateAngle = 0;
        private string m_NewText = string.Empty;

        public int RotateAngle { get { return m_RotateAngle; } set { m_RotateAngle = value; Invalidate(); } }
        public string NewText { get { return m_NewText; } set { m_NewText = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            Func<double, double> DegToRad = (angle) => Math.PI * angle / 180.0;

            Brush b = new SolidBrush(ForeColor);
            SizeF size = e.Graphics.MeasureString(NewText, Font, Parent.Width);

            int normalAngle = (RotateAngle % 360 + 360) % 360;
            double normaleRads = DegToRad(normalAngle);

            int hSinTheta = (int)Math.Ceiling(size.Height * Math.Sin(normaleRads));
            int wCosTheta = (int)Math.Ceiling(size.Width * Math.Cos(normaleRads));
            int wSinTheta = (int)Math.Ceiling(size.Width * Math.Sin(normaleRads));
            int hCosTheta = (int)Math.Ceiling(size.Height * Math.Cos(normaleRads));

            int rotatedWidth = Math.Abs(hSinTheta) + Math.Abs(wCosTheta);
            int rotatedHeight = Math.Abs(wSinTheta) + Math.Abs(hCosTheta);

            Width = rotatedWidth;
            // This is incomplete and only checks if bottom anchor is set
            int oldHeight = Height;
            if (Anchor.HasFlag(AnchorStyles.Bottom))
            {
                if (Height != rotatedHeight)
                {
                    int offset = rotatedHeight - Height;
                    Top -= offset;
                }
            }
            Height = rotatedHeight;

            int numQuadrants =
                normalAngle >= 0 && normalAngle < 90 ? 1 :
                normalAngle >= 90 && normalAngle < 180 ? 2 :
                normalAngle >= 180 && normalAngle < 270 ? 3 :
                normalAngle >= 270 && normalAngle < 360 ? 4 :
                0;

            int horizShift = 0;
            int vertShift = 0;

            if (numQuadrants == 1)
            {
                horizShift = Math.Abs(hSinTheta);
            }
            else if (numQuadrants == 2)
            {
                horizShift = rotatedWidth;
                vertShift = Math.Abs(hCosTheta);
            }
            else if (numQuadrants == 3)
            {
                horizShift = Math.Abs(wCosTheta);
                vertShift = rotatedHeight;
            }
            else if (numQuadrants == 4)
            {
                vertShift = Math.Abs(wSinTheta);
            }

            e.Graphics.TranslateTransform(horizShift, vertShift);
            e.Graphics.RotateTransform(RotateAngle);

            e.Graphics.DrawString(NewText, Font, b, 0f, 0f);
            base.OnPaint(e);
        }
    }

    public enum ProgressBarDisplayMode
    {
        NoText,
        Percentage,
        CurrProgress,
        CustomText,
        TextAndPercentage,
        TextAndCurrProgress
    }

    [SupportedOSPlatform("windows")]
    public class TextProgressBar : ProgressBar
    {
        [Description("Font of the text on ProgressBar"), Category("Additional Options")]
        public Font TextFont { get; set; } = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold | FontStyle.Italic);

        private SolidBrush _textColourBrush = (SolidBrush)Brushes.Black;

        [Category("Additional Options")]
        public Color TextColor
        {
            get
            {
                return _textColourBrush.Color;
            }
            set
            {
                _textColourBrush.Dispose();
                _textColourBrush = new SolidBrush(value);
            }
        }

        private SolidBrush _progressColourBrush = (SolidBrush)Brushes.LightGreen;

        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color ProgressColor
        {
            get
            {
                return _progressColourBrush.Color;
            }
            set
            {
                _progressColourBrush.Dispose();
                _progressColourBrush = new SolidBrush(value);
            }
        }

        private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;

        [Category("Additional Options"), Browsable(true)]
        public ProgressBarDisplayMode VisualMode
        {
            get
            {
                return _visualMode;
            }
            set
            {
                _visualMode = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }

        private string _text = string.Empty;

        [Description("If it's empty, % will be shown"), Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string CustomText
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }

        private string _textToDraw
        {
            get
            {
                string text = CustomText;

                switch (VisualMode)
                {
                    case ProgressBarDisplayMode.Percentage:
                        text = _percentageStr;
                        break;

                    case ProgressBarDisplayMode.CurrProgress:
                        text = _currProgressStr;
                        break;

                    case ProgressBarDisplayMode.TextAndCurrProgress:
                        text = $"{CustomText}: {_currProgressStr}";
                        break;

                    case ProgressBarDisplayMode.TextAndPercentage:
                        text = $"{CustomText}: {_percentageStr}";
                        break;
                }

                return text;
            }
            set { }
        }

        private string _percentageStr { get { return $"{(int)((float)Value - Minimum) / ((float)Maximum - Minimum) * 100} %"; } }

        private string _currProgressStr
        {
            get
            {
                return $"{Value}/{Maximum}";
            }
        }

        public TextProgressBar()
        {
            Value = Minimum;
            FixComponentBlinking();
        }

        private void FixComponentBlinking()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            DrawProgressBar(g);

            DrawStringIfNeeded(g);
        }

        private void DrawProgressBar(Graphics g)
        {
            Rectangle rect = ClientRectangle;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);

            rect.Inflate(-3, -3);

            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round((float)Value / Maximum * rect.Width), rect.Height);

                g.FillRectangle(_progressColourBrush, clip);
            }
        }

        private void DrawStringIfNeeded(Graphics g)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = _textToDraw;

                SizeF len = g.MeasureString(text, TextFont);

                Point location = new Point(Width / 2 - (int)len.Width / 2, Height / 2 - (int)len.Height / 2);

                g.DrawString(text, TextFont, _textColourBrush, location);
            }
        }

        public new void Dispose()
        {
            _textColourBrush.Dispose();
            _progressColourBrush.Dispose();
            base.Dispose();
        }
    }

    [SupportedOSPlatform("windows")]
    public class ToolStripTransparentRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
        }
    }

    [SupportedOSPlatform("windows")]
    [ToolboxBitmap(typeof(ToolStripTextBox))]
    public class LocToolStripTextBox : ToolStripTextBox
    {
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(nint hWnd, int msg,
            int wParam, string lParam);
        public LocToolStripTextBox()
        {
            Control.HandleCreated += Control_HandleCreated;
        }
        private void Control_HandleCreated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cueBanner))
                UpdateCueBanner();
        }
        string cueBanner;
        public string CueBanner
        {
            get { return cueBanner; }
            set
            {
                cueBanner = value;
                UpdateCueBanner();
            }
        }
        private void UpdateCueBanner()
        {
            // Otherwise causes the component to resize in design mode on save
            if (DesignMode)
                return;

            SendMessage(Control.Handle, EM_SETCUEBANNER, 0, cueBanner);
        }
    }
}
