using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MW5_Mod_Manager;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class AboutFormDpiTests
{
    [STATestMethod]
    public void AboutIconUsesLargerIcoFrameAndFitsPictureBox()
    {
        using Bitmap normalDpiIcon = AboutForm.CreateBestIconBitmap(128);
        using Bitmap highDpiIcon = AboutForm.CreateBestIconBitmap(129);

        Assert.AreEqual(new Size(128, 128), normalDpiIcon.Size);
        Assert.AreEqual(new Size(256, 256), highDpiIcon.Size);

        using var form = new AboutForm();
        PictureBox pictureBox = form.Controls
            .OfType<PictureBox>()
            .Single();

        Assert.AreEqual(PictureBoxSizeMode.Zoom, pictureBox.SizeMode);
        Assert.IsNotNull(pictureBox.Image);
        Assert.AreEqual(pictureBox.ClientSize, pictureBox.Image.Size);
    }
}
