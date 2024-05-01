using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;
using Label = System.Windows.Forms.Label;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ExtractForm : Form
    {
        public string ArchiveFilePath { get; set; }
        public string OutputFolderPath { get; set; }

        private Task _extractionTask = null;

        enum eExtractionState
        {
            None,
            Extracting,
            Canceled,
            Done
        }

        private eExtractionState extractionState = eExtractionState.None;
        private CancellationTokenSource _cts;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd,
            uint Msg,
            uint wParam,
            uint lParam);

        public ExtractForm()
        {
            InitializeComponent();

            _cts = new CancellationTokenSource();
        }

        private void ExtractForm_Load(object sender, EventArgs e)
        {

        }

        private void UpdateButtonText()
        {
            switch (extractionState)
            {
                case eExtractionState.Extracting:
                    buttonAction.Text = "&Cancel";
                    break;
                default:
                    buttonAction.Text = "&Close";
                    break;
            }
        }

        private void SetCanceled()
        {
            SendMessage(progressBarExtract.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);

            SendMessage(progressBarFilePercentage.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0002, //PBST_ERROR
                0);

            this.extractionState = eExtractionState.Canceled;
            UpdateButtonText();
        }

        static bool IsFileInDirectory(string filePath, List<string> directoryPaths)
        {
            foreach (string directoryPath in directoryPaths)
            {
                if (filePath.StartsWith(directoryPath + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void DoDecompression()
        {
            try
            {
                using (var archive = ArchiveFactory.Open(ArchiveFilePath))
                {
                    int fileCount = 0;

                    // Check if the archive contains a mod.json
                    bool hasModJson = false;
                    // mod.json is in a subdirectory in root
                    bool modJsonCorrectDirLayout = false;
                    List<string> validModDirectories = new();

                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            fileCount++;

                            if (string.Equals(Path.GetFileName(entry.Key), "mod.json", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string modDirPath = Path.GetDirectoryName(entry.Key);

                                bool validPathLayout = Utils.GetFolderDepth(entry.Key) == 1;
                                if (validPathLayout)
                                {
                                    modJsonCorrectDirLayout = true;
                                    validModDirectories.Add(modDirPath);
                                }
                                hasModJson = true;
                            }
                        }
                    }

                    if (!hasModJson)
                    {
                        richTextBoxExtractLog.AppendText("\r\nThe archive doesn't contain a mod.json file. Aborting.\r\n");
                        SetCanceled();
                        return;
                    }

                    if (!modJsonCorrectDirLayout)
                    {
                        richTextBoxExtractLog.AppendText("\r\nThe archive contains a mod.json file but has an unexpected directory layout.\r\nPlease manually extract the mod following the mod author's instructions. Aborting.\r\n");
                        SetCanceled();
                        return;
                    }

                    // We want to remove any pre-existing directories before continuing
                    bool targetDirectoriesCleared = true;

                    this.Invoke(new Action(() =>
                    {
                        // Check if any Mods directories do already exist
                        foreach (string modDirName in validModDirectories)
                        {
                            string destinationPath = Path.GetFullPath(Path.Combine(OutputFolderPath, modDirName));
                            if (Directory.Exists(destinationPath))
                            {
                                targetDirectoriesCleared = false;

                                DialogResult dialogResult = MessageBox.Show("The target directory " + destinationPath
                                    + " already exists. It has to be deleted before extraction can begin."
                                    +"\r\n\r\nAre you sure you want to continue?",
                                    "Mod Directory already exists",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);

                                if (dialogResult == DialogResult.Yes)
                                {
                                    if (FileOperationUtils.DeleteFile(destinationPath, true, this.Handle))
                                    {
                                        targetDirectoriesCleared = true;
                                    }
                                }

                            }
                        }


                        progressBarExtract.Maximum = fileCount;
                        progressBarExtract.Step = 1;
                    }));

                    if (!targetDirectoriesCleared)
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBoxExtractLog.AppendText("\r\nAborted.\r\n");

                            SetCanceled();
                        }));
                        return;
                    }

                    int curFile = 1;
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            string normalizedPath = Path.Combine(entry.Key.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                            string destinationPath = Path.GetFullPath(Path.Combine(OutputFolderPath, normalizedPath));

                            bool isInModPath = IsFileInDirectory(normalizedPath, validModDirectories);
                            if (isInModPath)
                            {
                                string curFileName = Path.GetFileName(entry.Key);

                                richTextBoxExtractLog.Invoke(new Action(() =>
                                {
                                    labelCurrentFile.Visible = true;
                                    labelCurrentFile.Text = curFileName;
                                    richTextBoxExtractLog.AppendText(normalizedPath + "\r\n");
                                }));

                                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                                // Use custom ProgressFileStream for extraction.
                                using (var progressStream = new ProgressFileStream(destinationPath, FileMode.Create, progressBarFilePercentage, labelFileProgress, entry.Size, _cts.Token))
                                {
                                    try
                                    {
                                        entry.WriteTo(progressStream);
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        this.Invoke(new Action(() =>
                                        {
                                            richTextBoxExtractLog.AppendText("\r\nExtraction was canceled.\r\n");

                                            SetCanceled();
                                        }));
                                    }
                                }
                            }
                            else
                            {
                                // skip all files not in a mod path
                                this.Invoke(new Action(() =>
                                {
                                    labelCurrentFile.Visible = false;
                                    richTextBoxExtractLog.SelectionColor = Color.DarkSlateBlue;
                                    richTextBoxExtractLog.AppendText("File skipped: " + normalizedPath + "\r\n");
                                }));
                            }
                            if (_cts.IsCancellationRequested)
                                return;

                            // Advance progressbar to next file
                            this.Invoke(new Action(() =>
                            {
                                progressBarExtract.PerformStep();

                                double percentage = ((double)curFile / fileCount) * 100;
                                int progressPercentage = (int)double.Round(percentage);

                                // Update label with percentage
                                labelTotalProgress.Text = $@"{progressPercentage}%";
                            }));

                            curFile++;
                        }
                    }
                    this.Invoke(new Action(() =>
                    {
                        this.extractionState = eExtractionState.Done;
                    }));
                }
            }
            catch (Exception e)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBoxExtractLog.AppendText("\r\nError: " + e.Message + "\r\n");

                    SetCanceled();
                }));
                throw;
            }
            
            this.Invoke(new Action(() =>
            {
                labelCurrentFile.Visible = false;
                progressBarExtract.Maximum = 1;
                progressBarExtract.Value = 1;
                progressBarFilePercentage.Value = 100;
                richTextBoxExtractLog.AppendText("\r\nDone.\r\n");
                UpdateButtonText();
            }));
        }

        private Task StartDecompression()
        {
            richTextBoxExtractLog.AppendText("Extracting to: " + OutputFolderPath + "\r\n");
            extractionState = eExtractionState.Extracting;
            UpdateButtonText();

            return Task.Run(() =>
            {
                DoDecompression();
            });
        }

        private void ExtractForm_Shown(object sender, EventArgs e)
        {
            this.Text = "Extracting from " + Path.GetFileName(ArchiveFilePath);
            _extractionTask = StartDecompression();
        }

        private void AbortRunningExtraction()
        {
            if (extractionState == eExtractionState.Extracting)
            {
                _cts?.Cancel();
            }
        }

        private void buttonAction_Click(object sender, EventArgs e)
        {
            switch (extractionState)
            {
                case eExtractionState.Extracting:
                    AbortRunningExtraction();
                    break;
                default:
                    Close();
                    break;
            }

        }

        private void ExtractForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortRunningExtraction();
            buttonAction.Enabled = false;

            if (_cts != null)
            {
                while (_extractionTask != null && !_extractionTask.IsCompleted)
                {
                    Application.DoEvents();
                }
            }
        }
    }

    [SupportedOSPlatform("windows")]
    public class ProgressFileStream : FileStream
    {
        private long _totalBytes;
        private long _bytesWritten;
        private ProgressBar _progressBar;
        private Label _label;
        private CancellationToken _cancellationToken;

        public ProgressFileStream(string path, FileMode mode, ProgressBar progressBar, Label label, long totalBytes, CancellationToken cancellationToken)
            : base(path, mode)
        {
            _totalBytes = totalBytes;
            _bytesWritten = 0;
            _progressBar = progressBar;
            _label = label;
            _cancellationToken = cancellationToken;

            _progressBar.Invoke(new Action(() =>
            {
                _progressBar.Maximum = 100;
                _progressBar.Value = 0;
            }));
        }

        public override void Write(byte[] array, int offset, int count)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            base.Write(array, offset, count);
            _bytesWritten += count;

            int progressPercentage = 0;
            if (_totalBytes != 0)
            {
                progressPercentage = (int)((_bytesWritten * 100) / _totalBytes);
            }

            _progressBar.Invoke(new Action(() =>
            {
                _progressBar.Value = progressPercentage;
                _label.Text = $@"{progressPercentage}%";
            }));
        }
    }
}
