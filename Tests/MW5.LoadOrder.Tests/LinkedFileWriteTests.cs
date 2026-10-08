using Microsoft.VisualStudio.TestTools.UnitTesting;
using MW5_Mod_Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MW5.LoadOrder.Tests;

[TestClass]
public sealed class LinkedFileWriteTests
{
    private string root;
    private readonly List<(string path, bool directory)> links = new();
    private const string Original = "{\"description\":\"Original content longer than the replacement\"}";
    private const string Updated = "{\"name\":\"ü\"}";

    [TestInitialize]
    public void Initialize()
    {
        root = Path.Combine(Path.GetTempPath(), "MW5-LinkedWrites-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Remove links themselves before deleting the fixture's real directories.
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if (links[i].directory)
                Directory.Delete(links[i].path);
            else
                File.Delete(links[i].path);
        }
        Directory.Delete(root, recursive: true);
    }

    [DataTestMethod]
    [DataRow("Directory", "File")]
    [DataRow("Directory", "AbsoluteSymlink")]
    [DataRow("Directory", "RelativeSymlink")]
    [DataRow("Directory", "SymlinkChain")]
    [DataRow("Directory", "HardLink")]
    [DataRow("DirectorySymlink", "File")]
    [DataRow("DirectorySymlink", "AbsoluteSymlink")]
    [DataRow("DirectorySymlink", "RelativeSymlink")]
    [DataRow("DirectorySymlink", "SymlinkChain")]
    [DataRow("DirectorySymlink", "HardLink")]
    [DataRow("Junction", "File")]
    [DataRow("Junction", "AbsoluteSymlink")]
    [DataRow("Junction", "RelativeSymlink")]
    [DataRow("Junction", "SymlinkChain")]
    [DataRow("Junction", "HardLink")]
    public void SavingPreservesFileAndDirectoryLinks(string directoryKind, string fileKind)
    {
        string directory = Path.Combine(root, "Actual mods ü");
        Directory.CreateDirectory(directory);
        string deployedDirectory = directory;
        if (directoryKind != "Directory")
        {
            deployedDirectory = Path.Combine(root, "Deployed mods");
            if (directoryKind == "DirectorySymlink")
                CreateSymbolicLink(deployedDirectory, directory, isDirectory: true);
            else
                CreateJunction(deployedDirectory, directory);
        }

        string metadata = Path.Combine(directory, "mod.json");
        string target = fileKind == "File" ? metadata : Path.Combine(directory, "original.json");
        File.WriteAllText(target, Original);
        switch (fileKind)
        {
            case "AbsoluteSymlink":
                CreateSymbolicLink(metadata, target);
                break;
            case "RelativeSymlink":
                CreateSymbolicLink(metadata, "original.json");
                break;
            case "SymlinkChain":
                CreateSymbolicLink(Path.Combine(directory, "intermediate.json"), "original.json");
                CreateSymbolicLink(metadata, "intermediate.json");
                break;
            case "HardLink":
                CreateHardLink(metadata, target);
                break;
        }

        string originalFileLink = new FileInfo(metadata).LinkTarget;
        string originalDirectoryLink = new DirectoryInfo(deployedDirectory).LinkTarget;
        string deployedFile = Path.Combine(deployedDirectory, "mod.json");

        LocFileWriter.WriteAllText(deployedFile, Updated);

        Assert.AreEqual(Updated, File.ReadAllText(target), "The original file must receive the update.");
        Assert.AreEqual(Updated, File.ReadAllText(deployedFile));
        CollectionAssert.AreEqual(new UTF8Encoding(false).GetBytes(Updated), File.ReadAllBytes(target),
            "A shorter update must leave no old trailing bytes or UTF-8 BOM.");
        Assert.AreEqual(originalFileLink, new FileInfo(metadata).LinkTarget, "Preserve the file link.");
        Assert.AreEqual(originalDirectoryLink, new DirectoryInfo(deployedDirectory).LinkTarget,
            "Preserve the directory link.");
        Assert.AreEqual(0, Directory.GetFiles(directory, "*.tmp").Length);

        // Writing through the other name must remain visible after the save.
        // This detects hard-link replacement even when both files initially match.
        File.WriteAllText(target, "{\"updatedThroughOriginal\":true}");
        Assert.AreEqual(File.ReadAllText(target), File.ReadAllText(deployedFile));
    }

    [TestMethod]
    public void SymbolicLinkToHardLinkedFilePreservesBothKindsOfLink()
    {
        string target = Path.Combine(root, "original.json");
        string hardLink = Path.Combine(root, "hardlink.json");
        string symbolicLink = Path.Combine(root, "mod.json");
        File.WriteAllText(target, Original);
        CreateHardLink(hardLink, target);
        CreateSymbolicLink(symbolicLink, "hardlink.json");

        LocFileWriter.WriteAllText(symbolicLink, Updated);

        Assert.AreEqual(Updated, File.ReadAllText(target));
        Assert.AreEqual(Updated, File.ReadAllText(hardLink));
        Assert.AreEqual("hardlink.json", new FileInfo(symbolicLink).LinkTarget);
        File.WriteAllText(target, Original);
        Assert.AreEqual(Original, File.ReadAllText(symbolicLink));
    }

    [TestMethod]
    public void BrokenSymbolicLinkIsReportedAndPreserved()
    {
        string link = Path.Combine(root, "mod.json");
        string missing = Path.Combine(root, "missing.json");
        CreateSymbolicLink(link, "missing.json");

        var error = Assert.ThrowsException<FileNotFoundException>(() => LocFileWriter.WriteAllText(link, Updated));

        StringAssert.Contains(error.Message, link);
        StringAssert.Contains(error.Message, missing);
        Assert.AreEqual("missing.json", new FileInfo(link).LinkTarget);
        Assert.IsFalse(File.Exists(missing));
        Assert.AreEqual(0, Directory.GetFiles(root, "*.tmp").Length);
    }

    [DataTestMethod]
    [DataRow("File", false)]
    [DataRow("File", true)]
    [DataRow("Symlink", false)]
    [DataRow("Symlink", true)]
    [DataRow("HardLink", false)]
    [DataRow("HardLink", true)]
    public void LockedOrReadOnlyTargetPreservesContentAndLinks(string kind, bool readOnly)
    {
        string target = Path.Combine(root, "original.json");
        string path = kind == "File" ? target : Path.Combine(root, "mod.json");
        File.WriteAllText(target, Original);
        if (kind == "Symlink")
            CreateSymbolicLink(path, "original.json");
        else if (kind == "HardLink")
            CreateHardLink(path, target);

        if (readOnly)
        {
            File.SetAttributes(target, FileAttributes.ReadOnly);
            try
            {
                Assert.ThrowsException<UnauthorizedAccessException>(() => LocFileWriter.WriteAllText(path, Updated));
            }
            finally
            {
                File.SetAttributes(target, FileAttributes.Normal);
            }
        }
        else
        {
            using var locked = new FileStream(target, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Assert.ThrowsException<IOException>(() => LocFileWriter.WriteAllText(path, Updated));
        }

        Assert.AreEqual(Original, File.ReadAllText(target));
        Assert.AreEqual(Original, File.ReadAllText(path));
        if (kind == "Symlink")
            Assert.AreEqual("original.json", new FileInfo(path).LinkTarget);
        Assert.AreEqual(0, Directory.GetFiles(root, "*.tmp").Length);
        File.WriteAllText(target, Updated);
        Assert.AreEqual(Updated, File.ReadAllText(path), "A rejected write must preserve hard-link sharing.");
    }

    [DataTestMethod]
    [DataRow("Directory")]
    [DataRow("DirectorySymlink")]
    [DataRow("Junction")]
    public void NewFileCanBeCreatedInsideLinkedDirectory(string kind)
    {
        string directory = Path.Combine(root, "Actual");
        Directory.CreateDirectory(directory);
        string deployed = directory;
        if (kind != "Directory")
        {
            deployed = Path.Combine(root, "Deployed");
            if (kind == "DirectorySymlink")
                CreateSymbolicLink(deployed, directory, isDirectory: true);
            else
                CreateJunction(deployed, directory);
        }
        string originalLink = new DirectoryInfo(deployed).LinkTarget;

        LocFileWriter.WriteAllText(Path.Combine(deployed, "modlist.json"), Updated);

        Assert.AreEqual(Updated, File.ReadAllText(Path.Combine(directory, "modlist.json")));
        Assert.AreEqual(originalLink, new DirectoryInfo(deployed).LinkTarget);
        Assert.AreEqual(0, Directory.GetFiles(directory, "*.tmp").Length);
    }

    [TestMethod]
    public void SymbolicLinkCanTargetAnotherDirectory()
    {
        string targetDirectory = Path.Combine(root, "Elsewhere");
        Directory.CreateDirectory(targetDirectory);
        string target = Path.Combine(targetDirectory, "original.json");
        string link = Path.Combine(root, "mod.json");
        File.WriteAllText(target, Original);
        CreateSymbolicLink(link, Path.Combine("Elsewhere", "original.json"));
        string originalLink = new FileInfo(link).LinkTarget;

        LocFileWriter.WriteAllText(link, Updated);

        Assert.AreEqual(Updated, File.ReadAllText(target));
        Assert.AreEqual(originalLink, new FileInfo(link).LinkTarget);
        Assert.AreEqual(0, Directory.GetFiles(root, "*.tmp", SearchOption.AllDirectories).Length);
    }

    [TestMethod]
    public void SymbolicLinkLoopIsReportedWithoutReplacingLinks()
    {
        string first = Path.Combine(root, "first.json");
        string second = Path.Combine(root, "second.json");
        CreateSymbolicLink(first, "second.json");
        CreateSymbolicLink(second, "first.json");

        Assert.ThrowsException<IOException>(() => LocFileWriter.WriteAllText(first, Updated));

        Assert.AreEqual("second.json", new FileInfo(first).LinkTarget);
        Assert.AreEqual("first.json", new FileInfo(second).LinkTarget);
        Assert.AreEqual(0, Directory.GetFiles(root, "*.tmp").Length);
    }

    private void CreateSymbolicLink(string path, string target, bool isDirectory = false)
    {
        try
        {
            if (isDirectory)
                Directory.CreateSymbolicLink(path, target);
            else
                File.CreateSymbolicLink(path, target);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException ||
            ex is PlatformNotSupportedException || (ex is IOException && (ex.HResult & 0xffff) == 1314))
        {
            Assert.Inconclusive("Cannot create a symbolic-link fixture. Developer Mode or the "
                + "symbolic-link creation privilege may be required. " + ex.Message);
        }
        links.Add((path, isDirectory));
    }

    private static void CreateHardLink(string path, string target)
    {
        if (!CreateHardLinkW(path, target, IntPtr.Zero))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not create the hard-link fixture.");
    }

    private void CreateJunction(string path, string target)
    {
        // mklink is a cmd built-in; no elevation or shell-specific setup is needed.
        var start = new ProcessStartInfo("cmd.exe")
        {
            Arguments = $"/d /c mklink /J \"{path}\" \"{target}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(start);
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.AreEqual(0, process.ExitCode, $"Could not create the junction fixture: {output} {error}");
        links.Add((path, true));
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLinkW(string fileName, string existingFileName, IntPtr securityAttributes);
}
