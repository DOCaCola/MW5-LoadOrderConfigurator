using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MW5_Mod_Manager.ImportForm;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public partial class ImportForm : Form
    {
        public enum eResultDataType { DirNames, ModNames }

        public eResultDataType ResultDataType { get; set; } = eResultDataType.DirNames;
        public Dictionary<string, bool> ResultData;

        public ImportForm()
        {
            InitializeComponent();
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

            Dictionary<string, bool> resultData = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            if (jsonImportObject != null)
            {
                bool isLegacyJson = jsonImportObject.Count > 0;
                // Check for legacy json data
                foreach (var property in jsonImportObject.Properties())
                {
                    if (property.Value.Type == JTokenType.Boolean)
                    {
                        resultData[property.Name] = (bool)property.Value;
                    }
                    else
                    {
                        isLegacyJson = false;
                        break;
                    }
                }

                if (isLegacyJson)
                {
                    ResultDataType = eResultDataType.DirNames;
                    ResultData = resultData;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }

            // At this point, it is not JSON anymore

            // Is LOC Human readable string
            bool isLocHumanReadable = false;
            Dictionary<int, string> resultModNames = new Dictionary<int, string>();
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

                    resultModNames.Add(int.Parse(matchResult.Groups[1].ToString()), modName);

                    matchResult = matchResult.NextMatch();
                }
            }
            catch (ArgumentException ex)
            {

            }

            if (isLocHumanReadable)
            {
                var sortedDict = resultModNames.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                foreach (var kvp in sortedDict)
                {
                    resultData.Add(kvp.Value, true);
                }

                ResultDataType = eResultDataType.ModNames;
                ResultData = resultData;
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

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            textBoxData.Text = ClipboardUtils.ClipboardHelper.GetTextFromClipboard();
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            Font monospaceFont = Utils.CreateBestAvailableMonospacePlatformFont(textBoxData.Font.Size);
            if (monospaceFont != null)
            {
                textBoxData.Font = monospaceFont;
            }
        }
    }
}