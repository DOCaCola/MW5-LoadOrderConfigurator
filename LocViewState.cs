using BrightIdeasSoftware;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using System.Windows.Forms;

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

        static public void LoadViewStateFromFile()
        {
            string viewFile = Path.Combine(ModsManager.GetSettingsDirectory(), "ViewState.json");
            string listViewStateDataJson = File.ReadAllText(viewFile);
            _viewStateData = JObject.Parse(listViewStateDataJson).ToObject<ViewStateData>();
        }

        static public List<ListViewState> GetCurrentListViewState()
        {
            List<ListViewState> list = new List<ListViewState>();
            foreach (OLVColumn allColumn in MainForm.Instance.modObjectListView.AllColumns)
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

            JObject settingsFile = JObject.FromObject(viewStateData);
            string viewFile = Path.Combine(ModsManager.GetSettingsDirectory(), "ViewState.json");

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(viewFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, settingsFile);
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
        }

        static public void RestoreListViewState(List<ListViewState> listState)
        {
            foreach (var state in listState)
            {
                foreach (OLVColumn curColumn in MainForm.Instance.modObjectListView.AllColumns)
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
            MainForm.Instance.modObjectListView.RebuildColumns();
        }
    }
}
