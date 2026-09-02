using DarkModeForms;
using MW5_Mod_Manager.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class AboutForm : LocForm
    {
        private const int LogicalIconSize = 128;
        private static readonly byte[] MainIconData = LoadMainIconData();

        private Image _aboutIconImage;
        private Size _aboutIconCanvasSize;
        private int _aboutIconDpi;

        public AboutForm()
        {
            InitializeComponent();

            UpdateAboutIconImage();
            pictureBox1.SizeChanged += (_, _) => UpdateAboutIconImage();

        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            UpdateAboutIconImage();
        }

        private void UpdateAboutIconImage()
        {
            Size canvasSize = pictureBox1.ClientSize;
            int dpi = DeviceDpi;
            if (canvasSize.Width <= 0
                || canvasSize.Height <= 0
                || canvasSize == _aboutIconCanvasSize && dpi == _aboutIconDpi)
                return;

            int iconPixelSize = Math.Min(
                ScaleForDpi(LogicalIconSize, dpi),
                Math.Min(canvasSize.Width, canvasSize.Height));
            using Bitmap iconBitmap = CreateBestIconBitmap(iconPixelSize);
            Bitmap newImage = new(
                canvasSize.Width,
                canvasSize.Height,
                PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.Clear(Color.Transparent);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                int left = (canvasSize.Width - iconPixelSize) / 2;
                int top = (canvasSize.Height - iconPixelSize) / 2;
                graphics.DrawImage(
                    iconBitmap,
                    new Rectangle(left, top, iconPixelSize, iconPixelSize));
            }

            Image oldImage = _aboutIconImage;

            _aboutIconImage = newImage;
            _aboutIconCanvasSize = canvasSize;
            _aboutIconDpi = dpi;
            pictureBox1.Image = newImage;
            oldImage?.Dispose();
        }

        private void DisposeAboutIconImage()
        {
            pictureBox1.Image = null;
            _aboutIconImage?.Dispose();
            _aboutIconImage = null;
        }

        private static int ScaleForDpi(int value, int dpi)
        {
            return Math.Max(1, (value * dpi + 48) / 96);
        }

        private static byte[] LoadMainIconData()
        {
            using MemoryStream stream = new();
            Properties.Resources.MainIcon.Save(stream);
            return stream.ToArray();
        }

        internal static Bitmap CreateBestIconBitmap(int requestedSize)
        {
            int imageCount = BitConverter.ToUInt16(MainIconData, 4);
            int bestEntryOffset = 6;
            int bestSize = 0;
            int bestBitDepth = 0;
            bool bestFits = false;

            for (int index = 0; index < imageCount; index++)
            {
                int entryOffset = 6 + index * 16;
                int width = MainIconData[entryOffset] == 0
                    ? 256
                    : MainIconData[entryOffset];
                int height = MainIconData[entryOffset + 1] == 0
                    ? 256
                    : MainIconData[entryOffset + 1];
                int size = Math.Min(width, height);
                int bitDepth = BitConverter.ToUInt16(MainIconData, entryOffset + 6);
                bool fits = size >= requestedSize;

                bool isBetter = bestSize == 0
                    || fits && !bestFits
                    || fits == bestFits
                        && (fits
                            ? size < bestSize
                            : size > bestSize)
                    || fits == bestFits
                        && size == bestSize
                        && bitDepth > bestBitDepth;
                if (!isBetter)
                    continue;

                bestEntryOffset = entryOffset;
                bestSize = size;
                bestBitDepth = bitDepth;
                bestFits = fits;
            }

            int imageLength = (int)BitConverter.ToUInt32(
                MainIconData,
                bestEntryOffset + 8);
            int imageOffset = (int)BitConverter.ToUInt32(
                MainIconData,
                bestEntryOffset + 12);
            byte[] singleImageIcon = new byte[22 + imageLength];
            Buffer.BlockCopy(MainIconData, 0, singleImageIcon, 0, 6);
            singleImageIcon[4] = 1;
            singleImageIcon[5] = 0;
            Buffer.BlockCopy(
                MainIconData,
                bestEntryOffset,
                singleImageIcon,
                6,
                16);
            BitConverter.GetBytes(22u).CopyTo(singleImageIcon, 18);
            Buffer.BlockCopy(
                MainIconData,
                imageOffset,
                singleImageIcon,
                22,
                imageLength);

            using MemoryStream stream = new(singleImageIcon);
            using Icon icon = new(stream);
            return icon.ToBitmap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabelNexusmods_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = LocConstants.UrlNexusmods,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            labelVersion.Text = @"Version: " + MainForm.Instance.GetVersion();
        }

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = LocConstants.UrlGithub,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
