using BrightIdeasSoftware;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static class Program
{
    private const uint RdwInvalidate = 0x0001;
    private const uint RdwAllChildren = 0x0080;
    private const uint RdwUpdateNow = 0x0100;
    private const uint LvmScroll = 0x1014;
    private const uint WmMouseWheel = 0x020A;

    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Size hostSize = args.Length >= 2
            ? new Size(int.Parse(args[0]), int.Parse(args[1]))
            : new Size(1000, 500);
        List<ProbeItem> items = Enumerable.Range(0, 67)
            .Select(index => new ProbeItem(
                $"Mod {index:D2} Weapons and Equipment",
                $"Author {index % 9}",
                $"1.{index % 10}.{index % 4}",
                $"{index + 1} days",
                67 - index,
                index,
                $"{(index + 1) * 17} MB",
                $"ModFolder{index:D2}"))
            .ToList();

        Console.WriteLine($"Viewport: {hostSize.Width}x{hostSize.Height}");
        Measure("ListView no group", CreateNativeListView(items, grouped: false), hostSize);
        Measure("ListView grouped", CreateNativeListView(items, grouped: true), hostSize);
        Measure("OLV native no group", CreateObjectListView(
            items, decorated: false, highlighted: false, grouped: false,
            bypassCustomDraw: false, images: false), hostSize);
        Measure("OLV bypass no group", CreateObjectListView(
            items, decorated: false, highlighted: false, grouped: false,
            bypassCustomDraw: true, images: false), hostSize);
        Measure("OLV native images", CreateObjectListView(
            items, decorated: false, highlighted: false, grouped: false,
            bypassCustomDraw: false, images: true), hostSize);
        Measure("OLV native bare", CreateObjectListView(
            items, decorated: false, highlighted: false, grouped: true,
            bypassCustomDraw: false, images: false), hostSize);
        Measure("OLV native bypass", CreateObjectListView(
            items, decorated: false, highlighted: false, grouped: true,
            bypassCustomDraw: true, images: false), hostSize);
        Measure("OLV native decorated", CreateObjectListView(
            items, decorated: true, highlighted: false, grouped: false,
            bypassCustomDraw: false, images: true), hostSize);
        Measure("OLV owner highlight", CreateObjectListView(
            items, decorated: true, highlighted: true, grouped: false,
            bypassCustomDraw: false, images: true), hostSize);
    }

    private static ListView CreateNativeListView(IReadOnlyList<ProbeItem> items, bool grouped)
    {
        var list = new BufferedListView
        {
            CheckBoxes = true,
            Dock = DockStyle.Fill,
            FullRowSelect = true,
            View = View.Details,
        };
        AddColumns(list.Columns);
        ListViewGroup? group = null;
        if (grouped)
        {
            group = new ListViewGroup(string.Empty);
            list.Groups.Add(group);
        }
        for (int index = 0; index < items.Count; index++)
        {
            ProbeItem model = items[index];
            var item = new ListViewItem(model.Name)
            {
                BackColor = index % 2 == 0 ? Color.FromArgb(32, 32, 32) : Color.FromArgb(38, 38, 38),
                Checked = index % 3 != 0,
                ForeColor = index % 3 == 0 ? Color.Gray : Color.Gainsboro,
            };
            item.Group = group;
            item.SubItems.Add(model.Author);
            item.SubItems.Add(model.Version);
            item.SubItems.Add(model.FileAge);
            item.SubItems.Add(model.LoadOrder.ToString());
            item.SubItems.Add(model.OriginalLoadOrder.ToString());
            item.SubItems.Add(model.FileSize);
            item.SubItems.Add(model.Folder);
            item.SubItems.Add(string.Empty);
            list.Items.Add(item);
        }

        return list;
    }

    private static ObjectListView CreateObjectListView(
        IReadOnlyList<ProbeItem> items,
        bool decorated,
        bool highlighted,
        bool grouped,
        bool bypassCustomDraw,
        bool images)
    {
        ObjectListView list = bypassCustomDraw
            ? new BypassCustomDrawObjectListView()
            : new ObjectListView();
        list.CheckBoxes = true;
        list.Dock = DockStyle.Fill;
        list.FullRowSelect = true;
        list.OwnerDraw = highlighted;
        list.UseHotItem = false;
        list.UseOverlays = false;
        list.View = View.Details;
        AddColumn(list, "Mod", item => item.Name, 280, searchable: true);
        AddColumn(list, "Author", item => item.Author, 100);
        AddColumn(list, "Version", item => item.Version, 80);
        AddColumn(list, "File age", item => item.FileAge, 90);
        AddColumn(list, "LO", item => item.LoadOrder, 50);
        AddColumn(list, "oLO", item => item.OriginalLoadOrder, 50);
        AddColumn(list, "File size", item => item.FileSize, 90);
        AddColumn(list, "Mod Folder", item => item.Folder, 150, searchable: true);
        AddColumn(list, string.Empty, _ => string.Empty, 80);
        if (decorated)
        {
            list.AllColumns[8].FillsFreeSpace = true;
            list.HeaderUsesThemes = false;
            var headerStyle = new HeaderFormatStyle();
            headerStyle.SetBackColor(Color.FromArgb(45, 45, 48));
            headerStyle.SetForeColor(Color.Gainsboro);
            list.HeaderFormatStyle = headerStyle;
        }
        if (images)
        {
            var imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16),
            };
            var bitmap = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(bitmap))
                graphics.FillRectangle(Brushes.CornflowerBlue, 0, 0, 16, 16);
            imageList.Images.Add(bitmap);
            list.SmallImageList = imageList;
            list.ShowImagesOnSubItems = true;
            list.AllColumns[0].ImageGetter = _ => 0;
        }

        if (grouped)
        {
            OLVColumn groupColumn = list.AllColumns[0];
            groupColumn.GroupKeyGetter = _ => 1;
            list.AlwaysGroupByColumn = groupColumn;
        }
        list.BooleanCheckStateGetter = model => ((ProbeItem)model).Enabled;
        list.FormatRow += (_, e) =>
        {
            ProbeItem model = (ProbeItem)e.Model;
            e.Item.BackColor = e.DisplayIndex % 2 == 0
                ? Color.FromArgb(32, 32, 32)
                : Color.FromArgb(38, 38, 38);
            e.Item.ForeColor = model.Enabled ? Color.Gainsboro : Color.Gray;
            e.UseCellFormatEvents = false;
        };

        if (decorated)
        {
            list.SelectedRowDecoration = new RowBorderDecoration
            {
                BorderPen = new Pen(Color.Transparent),
                FillBrush = new SolidBrush(Color.FromArgb(65, 91, 173, 255)),
                CornerRounding = 0,
            };
        }

        if (highlighted)
        {
            var renderer = (HighlightTextRenderer)list.DefaultRenderer;
            renderer.Filter = TextMatchFilter.Contains(list, "Weapons");
        }

        list.SetObjects(items);
        if (decorated)
        {
            for (int index = 0; index < list.Items.Count; index++)
            {
                var item = (OLVListItem)list.Items[index];
                item.UseItemStyleForSubItems = false;
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    subItem.BackColor = item.BackColor;
                    subItem.ForeColor = item.ForeColor;
                }
                item.SubItems[0].ForeColor = index % 2 == 0
                    ? Color.CornflowerBlue
                    : Color.Orange;
                item.SubItems[4].ForeColor = Color.LightGreen;
                item.SubItems[5].ForeColor = Color.LightSalmon;
            }
            list.SelectedObject = items[0];
        }
        return list;
    }

    private static void AddColumns(ListView.ColumnHeaderCollection columns)
    {
        columns.Add("Mod", 280);
        columns.Add("Author", 100);
        columns.Add("Version", 80);
        columns.Add("File age", 90);
        columns.Add("LO", 50);
        columns.Add("oLO", 50);
        columns.Add("File size", 90);
        columns.Add("Mod Folder", 150);
        columns.Add(string.Empty, 80);
    }

    private static void AddColumn(
        ObjectListView list,
        string title,
        Func<ProbeItem, object> getter,
        int width,
        bool searchable = false)
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

    private static void Measure(string label, Control list, Size hostSize)
    {
        using (list)
        using (var host = new Form
        {
            ClientSize = hostSize,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Location = new Point(40, 40),
        })
        {
            host.Controls.Add(list);
            host.Show();
            Application.DoEvents();
            Cursor.Position = new Point(0, 0);
            ProbePixelScroll(label, list);

            for (int index = 0; index < 10; index++)
                Redraw(list.Handle);

            var samples = new double[50];
            for (int index = 0; index < samples.Length; index++)
                samples[index] = Redraw(list.Handle);

            Array.Sort(samples);
            double median = (samples[24] + samples[25]) / 2;
            double mean = samples.Average();
            double p95 = samples[(int)Math.Ceiling(samples.Length * 0.95) - 1];
            Console.WriteLine(
                $"{label,-22}: median {median,7:N2} ms, mean {mean,7:N2} ms, p95 {p95,7:N2} ms");
        }
    }

    private static void ProbePixelScroll(string label, Control control)
    {
        if (control is not ListView list || list.Items.Count < 20)
            return;

        list.EnsureVisible(15);
        Application.DoEvents();
        int itemIndex = list.TopItem?.Index ?? 0;
        int before = list.Items[itemIndex].Bounds.Y;
        SendMessage(list.Handle, LvmScroll, IntPtr.Zero, new IntPtr(1));
        Application.DoEvents();
        int after = list.Items[itemIndex].Bounds.Y;
        int pixelScrollDistance = after - before;
        SendMessage(list.Handle, LvmScroll, IntPtr.Zero, new IntPtr(-1));
        Application.DoEvents();

        before = list.Items[itemIndex].Bounds.Y;
        SendMessage(
            list.Handle,
            WmMouseWheel,
            new IntPtr(unchecked((int)0xFF880000)),
            IntPtr.Zero);
        Application.DoEvents();
        after = list.Items[itemIndex].Bounds.Y;
        SendMessage(
            list.Handle,
            WmMouseWheel,
            new IntPtr(120 << 16),
            IntPtr.Zero);
        Console.WriteLine(
            $"{label,-22} 1px moved {pixelScrollDistance,3} px; wheel moved {after - before,4} px");
    }

    private static double Redraw(IntPtr window)
    {
        long start = Stopwatch.GetTimestamp();
        RedrawWindow(
            window,
            IntPtr.Zero,
            IntPtr.Zero,
            RdwInvalidate | RdwUpdateNow | RdwAllChildren);
        return Stopwatch.GetElapsedTime(start).TotalMilliseconds;
    }

    [DllImport("user32.dll")]
    private static extern bool RedrawWindow(
        IntPtr window,
        IntPtr updateRectangle,
        IntPtr updateRegion,
        uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam);

    private sealed class BufferedListView : ListView
    {
        public BufferedListView()
        {
            DoubleBuffered = true;
        }
    }

    private sealed class BypassCustomDrawObjectListView : ObjectListView
    {
        protected override bool HandleCustomDraw(ref Message message)
        {
            return false;
        }
    }

    private sealed record ProbeItem(
        string Name,
        string Author,
        string Version,
        string FileAge,
        int LoadOrder,
        int OriginalLoadOrder,
        string FileSize,
        string Folder)
    {
        public bool Enabled => LoadOrder % 3 != 0;
    }
}
