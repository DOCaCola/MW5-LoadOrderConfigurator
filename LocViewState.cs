using BrightIdeasSoftware;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    internal class LocViewState
    {

        public class ViewStateData
        {
            public bool WindowMaximized = false;
            public List<ListViewState> listState;
            public Rectangle WindowPosition { get; set; }
        }

        public class ListViewState
        {
            public string Name = string.Empty;
            public bool Visible;
            public int DisplayIndex;
            public int Width;
        }

        public static ViewStateData _defaultViewState = new ViewStateData();
        static ViewStateData _viewStateData = null;
        static string _dockPanelXml = null;
        static DeserializeDockContent _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

        static public bool LoadViewStateFromFile()
        {
            string viewFile = Path.Combine(ModsManager.GetSettingsDirectory(), "ViewState.json");

            if (!File.Exists(viewFile))
                return false;

            try
            {
                string jsonData = File.ReadAllText(viewFile);
                JObject settingsFile = JObject.Parse(jsonData);
                _viewStateData = settingsFile.ToObject<ViewStateData>();

                // Extract dockPanel JSON and convert back to XML string
                if (settingsFile["dockPanel"] != null)
                {
                    XDocument dockXmlDoc = JsonConvert.DeserializeXNode(settingsFile["dockPanel"].ToString());
                    _dockPanelXml = dockXmlDoc.ToString();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading view state: " + e.Message);
            }

            return false;
        }

        static public List<ListViewState> GetCurrentListViewState()
        {
            List<ListViewState> list = new List<ListViewState>();
            foreach (OLVColumn allColumn in DockModListForm.Instance.modObjectListView.AllColumns)
            {
                ListViewState newListViewState = new ListViewState();

                newListViewState.Name = allColumn.Text;
                newListViewState.Visible = allColumn.IsVisible;
                newListViewState.DisplayIndex = allColumn.LastDisplayIndex;
                newListViewState.Width = allColumn.Width;

                list.Add(newListViewState);
            }

            return list;
        }

        static public void SaveCurrentState()
        {
            ViewStateData viewStateData = new ViewStateData();

            viewStateData.WindowMaximized = MainForm.Instance.WindowState == FormWindowState.Maximized;
            viewStateData.WindowPosition = MainForm.Instance.DesktopBounds;
            viewStateData.listState = GetCurrentListViewState();

            // Save dockPanel layout as XML to memory
            using (MemoryStream ms = new MemoryStream())
            {
                MainForm.Instance.dockPanel1.SaveAsXml(ms, Encoding.UTF8, true);
                ms.Position = 0;

                XDocument xmlDoc = XDocument.Load(ms);

                // Remove comments
                xmlDoc.DescendantNodes()
                    .OfType<XComment>()
                    .ToList()
                    .ForEach(c => c.Remove());

                string dockPanelJson = JsonConvert.SerializeXNode(xmlDoc, Formatting.None, false);

                JObject settingsFile = JObject.FromObject(viewStateData);
                settingsFile["dockPanel"] = JObject.Parse(dockPanelJson);

                string viewFile = Path.Combine(ModsManager.GetSettingsDirectory(), "ViewState.json");
                Directory.CreateDirectory(ModsManager.GetSettingsDirectory());

                using (StreamWriter sw = new StreamWriter(viewFile))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, settingsFile);
                }
            }
        }

        static public void RestoreViewState()
        {
            if (_viewStateData.WindowMaximized)
            {
                MainForm.Instance.WindowState = FormWindowState.Maximized;
            }
            else if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(_viewStateData.WindowPosition)))
            {
                MainForm.Instance.StartPosition = FormStartPosition.Manual;
                MainForm.Instance.DesktopBounds = _viewStateData.WindowPosition;
            }

            RestoreListViewState(_viewStateData.listState);
            RestoreDockPanelLayout(_deserializeDockContent);
        }

        static public void RestoreListViewState(List<ListViewState> listState)
        {
            foreach (var state in listState)
            {
                foreach (OLVColumn curColumn in DockModListForm.Instance.modObjectListView.AllColumns)
                {
                    if (curColumn.Text == state.Name)
                    {
                        curColumn.Width = state.Width;
                        if (curColumn.CanBeHidden)
                            curColumn.IsVisible = state.Visible;
                        curColumn.LastDisplayIndex = state.DisplayIndex;
                    }
                }
            }
            DockModListForm.Instance.modObjectListView.RebuildColumns();
        }

        public static void RestoreDockPanelLayout(DeserializeDockContent deserializeContent)
        {
            if (string.IsNullOrEmpty(_dockPanelXml) || MainForm.Instance?.dockPanel1 == null)
                return;

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_dockPanelXml)))
            {
                while (MainForm.Instance.dockPanel1.Contents.Count > 0)
                {
                    MainForm.Instance.dockPanel1.Contents[0].DockHandler.Dispose();
                }
                MainForm.Instance.dockPanel1.LoadFromXml(ms, deserializeContent, true);

                DockModListForm.Instance.Show(MainForm.Instance.dockPanel1, DockState.Document);
            }
        }

        static private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DockOverviewForm).ToString())
                return DockOverviewForm.Instance;
            else if (persistString == typeof(DockConflictsForm).ToString())
                return DockConflictsForm.Instance;

            return null;
        }
    }
}
