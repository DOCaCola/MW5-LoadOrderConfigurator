using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DarkModeForms;
using MW5_Mod_Manager.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ImportForm : LocForm, ILocAppearanceAware
    {
        private readonly Image _loadFromFileImageSource;
        private readonly Image _pasteImageSource;

        public enum eResultDataType { DirNames, ModNames }

        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public eResultDataType ResultDataType { get; set; } = eResultDataType.DirNames;
        public List<ModsManager.ModImportData> ResultData;

        public ImportForm()
        {
            InitializeComponent();
            _loadFromFileImageSource = toolStripButtonLoadFromFile.Image;
            _pasteImageSource = toolStripButtonPaste.Image;
        }

        void ILocAppearanceAware.ApplyAppearance(
            AppearanceSnapshot appearance)
        {
            ToolStripAppearance.Apply(toolStrip1, appearance);
            ApplyDpiAssets(DeviceDpi);
        }

        void ILocAppearanceAware.ApplyDpi(int oldDpi, int newDpi)
        {
            ApplyDpiAssets(newDpi);
        }

        private void ApplyDpiAssets(int dpi)
        {
            ToolStripAppearance.ApplyDpi(
                toolStrip1,
                dpi,
                (toolStripButtonLoadFromFile, _loadFromFileImageSource),
                (toolStripButtonPaste, _pasteImageSource));
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            JObject jsonImportObject = null;
            // Check if valid JSON
            try
            {
                jsonImportObject = JObject.Parse(textBoxData.Text);
            }
            catch (JsonReaderException)
            {

            }

            List<ModsManager.ModImportData> resultData = new List<ModsManager.ModImportData>();
            if (jsonImportObject != null)
            {
                bool isLegacyJson = jsonImportObject.Count > 0;
                // Check for legacy json data
                foreach (var property in jsonImportObject.Properties())
                {
                    if (property.Value.Type == JTokenType.Boolean)
                    {
                        ModsManager.ModImportData newImportData = new ModsManager.ModImportData();
                        newImportData.ModFolder = property.Name;
                        newImportData.Enabled = (bool)property.Value;

                        resultData.Add(newImportData);
                    }
                    else
                    {
                        isLegacyJson = false;
                        break;
                    }
                }

                if (isLegacyJson)
                {
                    // Create load order
                    for (int i = 0; i < resultData.Count; i++)
                    {
                        resultData[i].LoadOrder = resultData.Count - i;
                    }

                    ResultDataType = eResultDataType.DirNames;
                    ResultData = resultData;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }
            resultData.Clear();
            // At this point, it is not JSON anymore

            // Is LOC Human readable string
            bool isLocHumanReadable = false;
            try
            {
                // Check for strings in format
                // 58   420   "Super Test Mod" 3.1.2(357) by The Modder
                // 1    2      3               4     5       6
                Regex regexObj = new Regex(@"^ *([\d]+) +([\d]+) +""((?:""""|[^\""])*)"" ([^ ]*?)\(([ \d]+?)\) by (.*)$", RegexOptions.Multiline);
                Match matchResult = regexObj.Match(textBoxData.Text);
                while (matchResult.Success)
                {
                    isLocHumanReadable = true;

                    // unescape mod names
                    string modName = matchResult.Groups[3].ToString().Replace("\"\"", "\"");

                    ModsManager.ModImportData newImportData = new ModsManager.ModImportData();
                    newImportData.Enabled = true;
                    newImportData.ModName = modName;
                    newImportData.LoadOrder = float.Parse(matchResult.Groups[1].ToString());

                    resultData.Add(newImportData);

                    matchResult = matchResult.NextMatch();
                }
            }
            catch (ArgumentException ex)
            {
                isLocHumanReadable = false;
            }

            if (isLocHumanReadable)
            {
                ResultData = resultData.OrderByDescending(x => x.LoadOrder).ToList();
                ResultDataType = eResultDataType.ModNames;
                this.DialogResult = DialogResult.OK;
                return;
            }
            resultData.Clear();

            // We also can check for a MW5MO loadorder, though these are not particularly hardened for parsing
            bool isMw5MoLoadorder = false;
            try
            {
                // Check for strings in format
                // 0(13)	 | 	"Super Test Mod" 3.1.2 by The Modder
                // 1 2           3            4      5
                Regex regexObj = new Regex(@"^([\d]+)\(([\d]+)\)[ \t]+\|[ \t]+\""(.*)\"" (.*) by *(.*)$", RegexOptions.Multiline);
                Match matchResult = regexObj.Match(textBoxData.Text);
                while (matchResult.Success)
                {
                    isMw5MoLoadorder = true;

                    string modName = matchResult.Groups[3].ToString().Replace("\"\"", "\"");

                    ModsManager.ModImportData newImportData = new ModsManager.ModImportData();
                    newImportData.Enabled = true;
                    newImportData.ModName = modName;
                    newImportData.LoadOrder = float.Parse(matchResult.Groups[1].ToString());

                    resultData.Add(newImportData);

                    matchResult = matchResult.NextMatch();
                }
            }
            catch (ArgumentException ex)
            {
                isMw5MoLoadorder = false;
            }

            if (isMw5MoLoadorder)
            {
                ResultData = resultData.OrderByDescending(x => x.LoadOrder).ToList();

                ResultDataType = eResultDataType.ModNames;
                this.DialogResult = DialogResult.OK;
                return;
            }

            TaskDialog.ShowDialog(MainForm.Instance.Handle, new TaskDialogPage()
            {
                Text = "Make sure the entered load order is in a supported format.",
                Heading = "Error reading load order.",
                Caption = "Error",
                Buttons =
                {
                    TaskDialogButton.OK,
                },
                Icon = TaskDialogIcon.Error,
                DefaultButton = TaskDialogButton.OK,
                AllowCancel = true
            });
        }

        private void textBoxData_TextChanged(object sender, EventArgs e)
        {
            buttonImport.Enabled = textBoxData.Text.Length > 0;
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            Font monospaceFont = Utils.CreateBestAvailableMonospacePlatformFont(textBoxData.Font.Size);
            if (monospaceFont != null)
            {
                textBoxData.Font = monospaceFont;
            }
        }

        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
            textBoxData.Text = ClipboardUtils.ClipboardHelper.GetTextFromClipboard();
        }

        private void toolStripButtonLoadFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Load Order File";
            openFileDialog.Filter = "Text files|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileContents = File.ReadAllText(openFileDialog.FileName);
                // normalize (unix) line endings
                fileContents = fileContents.ReplaceLineEndings("\r\n");
                textBoxData.Text = fileContents;
            }
        }
    }

}
