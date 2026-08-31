using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpCompress.Archives;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class SharpCompressCompatibilityTests
{
    [TestMethod]
    public void OpensAndReadsZipArchive()
    {
        using var archiveStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(
                   archiveStream,
                   ZipArchiveMode.Create,
                   leaveOpen: true))
        {
            var entry = zipArchive.CreateEntry("ExampleMod/mod.json");
            using var entryStream = entry.Open();
            entryStream.Write(Encoding.UTF8.GetBytes("{}"));
        }

        archiveStream.Position = 0;
        using var archive = ArchiveFactory.OpenArchive(archiveStream);
        var modJsonEntry = archive.Entries.Single(
            entry => entry.Key == "ExampleMod/mod.json");

        using var extractedStream = new MemoryStream();
        modJsonEntry.WriteTo(extractedStream);

        Assert.AreEqual(
            "{}",
            Encoding.UTF8.GetString(extractedStream.ToArray()));
    }
}
