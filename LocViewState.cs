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
        internal const int CurrentSchemaVersion = 2;

        public class ViewStateData
        {
            public int SchemaVersion;
            public int SavedDpi;
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
        static public bool HasDockPanelLayout => !string.IsNullOrEmpty(_dockPanelXml);

        static public bool LoadViewStateFromFile()
        {
            _viewStateData = null;
            _dockPanelXml = null;
            string viewFile = Path.Combine(LocSettings.GetSettingsDirectory(), "ViewState.json");

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
            int dpi = MainForm.Instance?.DeviceDpi ?? 96;
            return GetCurrentListViewState(dpi);
        }

        internal static List<ListViewState> GetCurrentListViewState(
            int measurementDpi)
        {
            List<ListViewState> list = new List<ListViewState>();
            foreach (OLVColumn allColumn in DockModListForm.Instance.modObjectListView.AllColumns)
            {
                ListViewState newListViewState = new ListViewState();

                newListViewState.Name = allColumn.Text;
                newListViewState.Visible = allColumn.IsVisible;
                newListViewState.DisplayIndex = allColumn.LastDisplayIndex;
                newListViewState.Width = ScaleForDpi(
                    allColumn.Width,
                    measurementDpi,
                    96);

                list.Add(newListViewState);
            }

            return list;
        }

        static public void SaveCurrentState()
        {
            int dpi = MainForm.Instance.DeviceDpi;
            ViewStateData viewStateData = new ViewStateData
            {
                SchemaVersion = CurrentSchemaVersion,
                SavedDpi = dpi
            };

            viewStateData.WindowMaximized = MainForm.Instance.WindowState == FormWindowState.Maximized;
            viewStateData.WindowPosition = NormalizeWindowBounds(
                MainForm.Instance.DesktopBounds,
                dpi);
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

                string viewFile = Path.Combine(LocSettings.GetSettingsDirectory(), "ViewState.json");
                Directory.CreateDirectory(LocSettings.GetSettingsDirectory());

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
            int targetDpi = MainForm.Instance.DeviceDpi;
            int sourceDpi = GetStoredMeasurementDpi(targetDpi);
            Rectangle windowPosition = RestoreWindowBounds(
                _viewStateData.WindowPosition,
                sourceDpi,
                targetDpi);

            if (_viewStateData.WindowMaximized)
            {
                MainForm.Instance.WindowState = FormWindowState.Maximized;
            }
            else if (Screen.AllScreens.Any(
                         screen => screen.WorkingArea.IntersectsWith(
                             windowPosition)))
            {
                MainForm.Instance.StartPosition = FormStartPosition.Manual;
                MainForm.Instance.DesktopBounds = windowPosition;
            }

            RestoreListViewState(
                _viewStateData.listState,
                sourceDpi,
                targetDpi);
            RestoreDockPanelLayout(_deserializeDockContent);
        }

        static public void RestoreListViewState(List<ListViewState> listState)
        {
            int targetDpi = MainForm.Instance?.DeviceDpi ?? 96;
            RestoreListViewState(
                listState,
                GetStoredMeasurementDpi(targetDpi),
                targetDpi);
        }

        internal static void RestoreListViewState(
            List<ListViewState> listState,
            int sourceDpi,
            int targetDpi)
        {
            foreach (var state in listState)
            {
                foreach (OLVColumn curColumn in DockModListForm.Instance.modObjectListView.AllColumns)
                {
                    if (curColumn.Text == state.Name)
                    {
                        curColumn.Width = ScaleForDpi(
                            state.Width,
                            sourceDpi,
                            targetDpi);
                        if (curColumn.CanBeHidden)
                            curColumn.IsVisible = state.Visible;
                        curColumn.LastDisplayIndex = state.DisplayIndex;
                    }
                }
            }
            DockModListForm.Instance.modObjectListView.RebuildColumns();
        }

        private static int GetStoredMeasurementDpi(int legacyDpi)
        {
            if (_viewStateData?.SchemaVersion >= CurrentSchemaVersion
                && _viewStateData.SavedDpi > 0)
            {
                return 96;
            }

            return legacyDpi;
        }

        internal static int ScaleForDpi(
            int value,
            int sourceDpi,
            int targetDpi)
        {
            return (int)Math.Round(
                value * (double)targetDpi / sourceDpi,
                MidpointRounding.AwayFromZero);
        }

        internal static Rectangle ScaleForDpi(
            Rectangle value,
            int sourceDpi,
            int targetDpi)
        {
            return new Rectangle(
                ScaleForDpi(value.X, sourceDpi, targetDpi),
                ScaleForDpi(value.Y, sourceDpi, targetDpi),
                ScaleForDpi(value.Width, sourceDpi, targetDpi),
                ScaleForDpi(value.Height, sourceDpi, targetDpi));
        }

        internal static Rectangle NormalizeWindowBounds(
            Rectangle bounds,
            int dpi)
        {
            return new Rectangle(
                bounds.Location,
                new Size(
                    ScaleForDpi(bounds.Width, dpi, 96),
                    ScaleForDpi(bounds.Height, dpi, 96)));
        }

        internal static Rectangle RestoreWindowBounds(
            Rectangle bounds,
            int sourceDpi,
            int targetDpi)
        {
            return new Rectangle(
                bounds.Location,
                new Size(
                    ScaleForDpi(bounds.Width, sourceDpi, targetDpi),
                    ScaleForDpi(bounds.Height, sourceDpi, targetDpi)));
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
