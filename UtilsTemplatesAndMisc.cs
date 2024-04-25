using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    internal static class Utils
    {
        public static bool StringNullEmptyOrWhiteSpace(string txt)
        {
            return string.IsNullOrEmpty(txt) || string.IsNullOrWhiteSpace(txt);
        }

        public static bool IsUrlValid(string input)
        {
            Uri uriResult;
            bool isValidUri = Uri.TryCreate(input, UriKind.Absolute, out uriResult)
                              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return isValidUri;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        public static String BytesToHumanReadableString(long byteCount)
        {
            string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

                /// <summary>
        /// Compare two version strings, e.g.  "3.2.1.0.b40" and "3.10.1.a".
        /// V1 and V2 can have different number of components.
        /// Components must be delimited by dot.
        /// </summary>
        /// <remarks>
        /// This doesn't do any null/empty checks so please don't pass dumb parameters
        /// </remarks>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>
        /// -1 if v1 is lower version number than v2,
        /// 0 if v1 == v2,
        /// 1 if v1 is higher version number than v2,
        /// -1000 if we couldn't figure it out (something went wrong)
        /// </returns>
        public static int CompareVersionStrings(string v1, string v2)
        {
            int rc = -1000;

            v1 = v1.ToLower();
            v2 = v2.ToLower();

            if (v1 == v2)
                return 0;

            string[] v1parts = v1.Split('.');
            string[] v2parts = v2.Split('.');

            for (int i = 0; i < v1parts.Length; i++)
            {
                if (v2parts.Length < i+1)
                    break; // we're done here
                
                string v1Token = v1parts[i];
                string v2Token = v2parts[i];
                
                int x;
                bool v1Numeric = int.TryParse(v1Token, out x);
                bool v2Numeric = int.TryParse(v2Token, out x);
                
                // handle scenario {"2" versus "20"} by prepending zeroes, e.g. it would become {"02" versus "20"}
                if (v1Numeric && v2Numeric) {
                    while (v1Token.Length < v2Token.Length)
                        v1Token = "0" + v1Token;
                    while (v2Token.Length < v1Token.Length)
                        v2Token = "0" + v2Token;
                }

                rc = String.Compare(v1Token, v2Token, StringComparison.Ordinal);
                //Console.WriteLine("v1Token=" + v1Token + " v2Token=" + v2Token + " rc=" + rc);
                if (rc != 0)
                    break;
            }

            if (rc == 0)
            {
                // catch this scenario: v1="1.0.1" v2="1.0"
                if (v1parts.Length > v2parts.Length)
                    rc = 1; // v1 is higher version than v2
                // catch this scenario: v1="1.0" v2="1.0.1"
                else if (v2parts.Length > v1parts.Length)
                    rc = -1; // v1 is lower version than v2
            }

            if (rc == 0 || rc == -1000)
                return rc;
            else
                return rc < 0 ? -1 : 1;
        }
    }

    #region extra designer items

    //The rotating label for priority indication.
    [SupportedOSPlatform("windows")]
    public class RotatingLabel : System.Windows.Forms.Label
    {
        private int m_RotateAngle = 0;
        private string m_NewText = string.Empty;

        public int RotateAngle { get { return m_RotateAngle; } set { m_RotateAngle = value; Invalidate(); } }
        public string NewText { get { return m_NewText; } set { m_NewText = value; Invalidate(); } }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Func<double, double> DegToRad = (angle) => Math.PI * angle / 180.0;

            Brush b = new SolidBrush(this.ForeColor);
            SizeF size = e.Graphics.MeasureString(this.NewText, this.Font, this.Parent.Width);

            int normalAngle = ((RotateAngle % 360) + 360) % 360;
            double normaleRads = DegToRad(normalAngle);

            int hSinTheta = (int)Math.Ceiling((size.Height * Math.Sin(normaleRads)));
            int wCosTheta = (int)Math.Ceiling((size.Width * Math.Cos(normaleRads)));
            int wSinTheta = (int)Math.Ceiling((size.Width * Math.Sin(normaleRads)));
            int hCosTheta = (int)Math.Ceiling((size.Height * Math.Cos(normaleRads)));

            int rotatedWidth = Math.Abs(hSinTheta) + Math.Abs(wCosTheta);
            int rotatedHeight = Math.Abs(wSinTheta) + Math.Abs(hCosTheta);

            this.Width = rotatedWidth;
            this.Height = rotatedHeight;

            int numQuadrants =
                (normalAngle >= 0 && normalAngle < 90) ? 1 :
                (normalAngle >= 90 && normalAngle < 180) ? 2 :
                (normalAngle >= 180 && normalAngle < 270) ? 3 :
                (normalAngle >= 270 && normalAngle < 360) ? 4 :
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
            e.Graphics.RotateTransform(this.RotateAngle);

            e.Graphics.DrawString(this.NewText, this.Font, b, 0f, 0f);
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
                    case (ProgressBarDisplayMode.Percentage):
                        text = _percentageStr;
                        break;

                    case (ProgressBarDisplayMode.CurrProgress):
                        text = _currProgressStr;
                        break;

                    case (ProgressBarDisplayMode.TextAndCurrProgress):
                        text = $"{CustomText}: {_currProgressStr}";
                        break;

                    case (ProgressBarDisplayMode.TextAndPercentage):
                        text = $"{CustomText}: {_percentageStr}";
                        break;
                }

                return text;
            }
            set { }
        }

        private string _percentageStr { get { return $"{(int)((float)Value - Minimum) / ((float)Maximum - Minimum) * 100 } %"; } }

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
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);

                g.FillRectangle(_progressColourBrush, clip);
            }
        }

        private void DrawStringIfNeeded(Graphics g)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = _textToDraw;

                SizeF len = g.MeasureString(text, TextFont);

                Point location = new Point(((Width / 2) - (int)len.Width / 2), ((Height / 2) - (int)len.Height / 2));

                g.DrawString(text, TextFont, (Brush)_textColourBrush, location);
            }
        }

        public new void Dispose()
        {
            _textColourBrush.Dispose();
            _progressColourBrush.Dispose();
            base.Dispose();
        }
    }

    #endregion extra designer items

    public class ModObject
    {
        public string displayName { set; get; }
        public string version { set; get; }
        public int buildNumber { set; get; }
        public string description { set; get; }
        public string author { set; get; }
        public string authorURL { set; get; }
        public float defaultLoadOrder { set; get; }
        public string gameVersion { set; get; }
        public List<string> manifest { get; set; }
        public long steamPublishedFileId { set; get; }
        public long steamLastSubmittedBuildNumber { set; get; }
        public string steamModVisibility { set; get; }
        public List<string> Requires { set; get; }
        public float locOriginalLoadOrder { set; get; }
    }

    public class ProgramData
    {
        public string platform { set; get; }
        public float version { set; get; }
        public string InstallPath { set; get; }
        public string[] ModPaths { set; get; }
    }

    public class OverridingData
    {
        public string mod { set; get; }
        public bool isOverridden { set; get; }
        public bool isOverriding { set; get; }
        public Dictionary<string, List<string>> overrides { set; get; }
        public Dictionary<string, List<string>> overriddenBy { set; get; }
    }

    [SupportedOSPlatform("windows")]
    public class ModListViewItem : ListViewItem
    {
        public ModListViewItem() : base("", 0)
        {
            //other stuff here
        }
    }

    public class ModListBoxItem
    {
        public string DisplayName { get; set; }

        public string ModKey { get; set; }

        public string ModDirName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }


}