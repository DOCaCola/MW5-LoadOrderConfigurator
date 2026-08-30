using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

internal static class Program
{
    private const int ItemCount = 600;
    private const int MeasurementCount = 3;

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        List<ProbeItem> items = CreateItems();

        Console.WriteLine($"Synthetic items: {items.Count:N0}");
        Console.WriteLine();

        PrintResult(
            "Current-style ObjectListView (single hidden group + cell events)",
            MeasureList(items, useGrouping: true, consolidateFormatting: false));
        PrintResult(
            "ObjectListView without grouping",
            MeasureList(items, useGrouping: false, consolidateFormatting: false));
        PrintResult(
            "ObjectListView with consolidated row formatting",
            MeasureList(items, useGrouping: true, consolidateFormatting: true));

        ConflictColorResult conflictColors = MeasureConflictColoring(items);
        Console.WriteLine("Conflict color lookup CPU (100 passes):");
        Console.WriteLine($"  current nested scan : {conflictColors.NestedScan.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  cached hash lookup  : {conflictColors.HashLookup.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  speedup             : {conflictColors.Speedup,9:N1}x");
    }

    private static ProbeResult MeasureList(
        IReadOnlyList<ProbeItem> items,
        bool useGrouping,
        bool consolidateFormatting)
    {
        using var host = new Form
        {
            Location = new Point(-32000, -32000),
            ShowInTaskbar = false,
            Size = new Size(1000, 650),
            StartPosition = FormStartPosition.Manual,
        };
        using var list = new ObjectListView();
        ConfigureList(list, useGrouping, consolidateFormatting);
        list.Dock = DockStyle.Fill;
        host.Controls.Add(list);
        host.Show();
        Application.DoEvents();

        list.SetObjects(items);
        Application.DoEvents();
        list.ClearObjects();
        Application.DoEvents();

        TimeSpan build = Median(MeasurementCount, () =>
        {
            list.SetObjects(items);
            Application.DoEvents();
            list.ClearObjects();
            Application.DoEvents();
        });

        list.SetObjects(items);
        Application.DoEvents();
        list.UseFiltering = true;

        TimeSpan filter = Median(MeasurementCount, () =>
        {
            list.ModelFilter = TextMatchFilter.Contains(list, "Weapons");
            Application.DoEvents();
            list.ModelFilter = null;
            Application.DoEvents();
        });

        TimeSpan refresh = Median(MeasurementCount, () =>
        {
            list.RefreshObjects(items.ToList());
            Application.DoEvents();
        });

        TimeSpan invalidate = Median(MeasurementCount, () =>
        {
            list.Invalidate();
            Application.DoEvents();
        });

        list.UseFiltering = false;
        TimeSpan legacyHighlight = Median(MeasurementCount, () =>
        {
            list.ModelFilter = TextMatchFilter.Contains(list, "Weapons");
            list.Invalidate();
            list.RefreshObjects(items.ToList());
            Application.DoEvents();
            list.ModelFilter = null;
            list.Invalidate();
            list.RefreshObjects(items.ToList());
            Application.DoEvents();
        });

        var highlightRenderer = (HighlightTextRenderer)list.DefaultRenderer;
        TimeSpan rendererHighlight = Median(MeasurementCount, () =>
        {
            highlightRenderer.Filter = TextMatchFilter.Contains(list, "Weapons");
            list.Invalidate();
            Application.DoEvents();
            highlightRenderer.Filter = null;
            list.Invalidate();
            Application.DoEvents();
        });

        return new ProbeResult(
            build,
            filter,
            refresh,
            invalidate,
            legacyHighlight,
            rendererHighlight);
    }

    private static void ConfigureList(
        ObjectListView list,
        bool useGrouping,
        bool consolidateFormatting)
    {
        list.View = View.Details;
        list.FullRowSelect = true;
        list.CheckBoxes = true;
        list.UseHotItem = false;
        list.UseHotControls = false;
        list.UseOverlays = false;

        AddColumn(list, "Mod", item => item.Name, searchable: true, width: 280);
        AddColumn(list, "Author", item => item.Author, searchable: true, width: 100);
        AddColumn(list, "Version", item => item.Version, searchable: false, width: 80);
        AddColumn(list, "File age", item => item.FileAge, searchable: false, width: 90);
        AddColumn(list, "LO", item => item.LoadOrder, searchable: false, width: 50);
        AddColumn(list, "oLO", item => item.LoadOrder, searchable: false, width: 50);
        AddColumn(list, "File size", item => item.FileSize, searchable: false, width: 90);
        AddColumn(list, "Mod Folder", item => item.Folder, searchable: true, width: 150);
        AddColumn(list, "", _ => string.Empty, searchable: false, width: 80);

        if (useGrouping)
        {
            OLVColumn groupColumn = list.AllColumns[0];
            groupColumn.GroupKeyGetter = _ => 1;
            list.AlwaysGroupByColumn = groupColumn;
        }

        list.BooleanCheckStateGetter = model => ((ProbeItem)model).Enabled;
        list.FormatRow += (_, e) =>
        {
            ProbeItem item = (ProbeItem)e.Model;
            e.Item.BackColor = item.LoadOrder % 2 == 0
                ? Color.FromArgb(32, 32, 32)
                : Color.FromArgb(38, 38, 38);
            if (!consolidateFormatting)
            {
                e.UseCellFormatEvents = true;
                return;
            }

            e.Item.UseItemStyleForSubItems = false;
            if (!item.Enabled)
            {
                e.Item.ForeColor = Color.Gray;
                return;
            }

            if (e.Item.SubItems.Count > 0)
                e.Item.SubItems[0].ForeColor = item.ConflictGroup % 2 == 0
                    ? Color.Orange
                    : Color.CornflowerBlue;
            if (e.Item.SubItems.Count > 5)
            {
                Color loadOrderColor = InterpolateColor(
                    Color.Green,
                    Color.Red,
                    (double)item.LoadOrder / ItemCount);
                e.Item.SubItems[4].ForeColor = loadOrderColor;
                e.Item.SubItems[5].ForeColor = loadOrderColor;
            }
        };
        if (!consolidateFormatting)
        {
            list.FormatCell += (_, e) =>
            {
                ProbeItem item = (ProbeItem)e.Model;
                if (!item.Enabled)
                {
                    e.SubItem.ForeColor = Color.Gray;
                    return;
                }

                if (e.ColumnIndex == 0)
                    e.SubItem.ForeColor = item.ConflictGroup % 2 == 0 ? Color.Orange : Color.CornflowerBlue;
                else if (e.ColumnIndex is 4 or 5)
                    e.SubItem.ForeColor = InterpolateColor(
                        Color.Green,
                        Color.Red,
                        (double)item.LoadOrder / ItemCount);
            };
        }
    }

    private static Color InterpolateColor(Color from, Color to, double ratio)
    {
        return Color.FromArgb(
            (int)(from.R + (to.R - from.R) * ratio),
            (int)(from.G + (to.G - from.G) * ratio),
            (int)(from.B + (to.B - from.B) * ratio));
    }

    private static void AddColumn(
        ObjectListView list,
        string title,
        Func<ProbeItem, object> getter,
        bool searchable,
        int width)
    {
        var column = new OLVColumn(title, null)
        {
            AspectGetter = model => getter((ProbeItem)model),
            Searchable = searchable,
            Width = width,
        };
        list.AllColumns.Add(column);
        list.Columns.Add(column);
    }

    private static ConflictColorResult MeasureConflictColoring(IReadOnlyList<ProbeItem> items)
    {
        Dictionary<string, string> folderToPath = items.ToDictionary(
            item => item.Folder,
            item => item.Path,
            StringComparer.OrdinalIgnoreCase);
        Dictionary<string, byte> selectedConflicts = items.ToDictionary(
            item => item.Folder,
            _ => (byte)0,
            StringComparer.OrdinalIgnoreCase);
        var conflictPaths = new HashSet<string>(
            selectedConflicts.Keys.Select(folder => folderToPath[folder]),
            StringComparer.OrdinalIgnoreCase);

        TimeSpan nestedScan = Measure(() =>
        {
            int matches = 0;
            for (int pass = 0; pass < 100; pass++)
            {
                foreach (ProbeItem item in items)
                {
                    foreach (string folder in selectedConflicts.Keys)
                    {
                        if (string.Equals(folderToPath[folder], item.Path, StringComparison.OrdinalIgnoreCase))
                        {
                            matches++;
                            break;
                        }
                    }
                }
            }
            GC.KeepAlive(matches);
        });

        TimeSpan hashLookup = Measure(() =>
        {
            int matches = 0;
            for (int pass = 0; pass < 100; pass++)
            {
                foreach (ProbeItem item in items)
                {
                    if (conflictPaths.Contains(item.Path))
                        matches++;
                }
            }
            GC.KeepAlive(matches);
        });

        return new ConflictColorResult(nestedScan, hashLookup);
    }

    private static TimeSpan Median(int count, Action action)
    {
        var measurements = new List<long>(count);
        for (int index = 0; index < count; index++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            measurements.Add(stopwatch.ElapsedTicks);
        }
        measurements.Sort();
        return TimeSpan.FromTicks(measurements[count / 2]);
    }

    private static TimeSpan Measure(Action action)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static List<ProbeItem> CreateItems()
    {
        string[] categories =
        {
            "Weapons", "Visuals", "Career", "Missions",
            "Audio", "Mechs", "Balance", "Interface",
        };
        var items = new List<ProbeItem>(ItemCount);
        for (int index = 0; index < ItemCount; index++)
        {
            string folder = $"Synthetic_Mod_{index:0000}";
            items.Add(new ProbeItem(
                $"Synthetic {categories[index % categories.Length]} Test Mod {index:0000}",
                $"Synthetic Author {index % 47:00}",
                $"{1 + index % 5}.{index % 17}.{index % 23}",
                folder,
                $@"X:\SyntheticMods\{folder}",
                DateTime.UtcNow.AddDays(-index),
                index * 1024L,
                index,
                index % 20,
                true));
        }
        return items;
    }

    private static void PrintResult(string name, ProbeResult result)
    {
        Console.WriteLine(name + " median wall time:");
        Console.WriteLine($"  build 600 rows       : {result.Build.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  filter + clear       : {result.Filter.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  refresh all rows     : {result.Refresh.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  invalidate/redraw    : {result.Invalidate.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  3.x highlight cycle : {result.LegacyHighlight.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  renderer-only cycle : {result.RendererHighlight.TotalMilliseconds,9:N1} ms");
        Console.WriteLine($"  highlight speedup   : {result.HighlightSpeedup,9:N1}x");
        Console.WriteLine();
    }

    private sealed record ProbeItem(
        string Name,
        string Author,
        string Version,
        string Folder,
        string Path,
        DateTime FileAge,
        long FileSize,
        int LoadOrder,
        int ConflictGroup,
        bool Enabled);

    private sealed record ProbeResult(
        TimeSpan Build,
        TimeSpan Filter,
        TimeSpan Refresh,
        TimeSpan Invalidate,
        TimeSpan LegacyHighlight,
        TimeSpan RendererHighlight)
    {
        public double HighlightSpeedup =>
            LegacyHighlight.TotalMilliseconds / RendererHighlight.TotalMilliseconds;
    }

    private sealed record ConflictColorResult(TimeSpan NestedScan, TimeSpan HashLookup)
    {
        public double Speedup => NestedScan.TotalMilliseconds / HashLookup.TotalMilliseconds;
    }
}
