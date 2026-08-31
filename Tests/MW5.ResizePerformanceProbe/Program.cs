using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

internal static class Program
{
    private const uint LvmFirst = 0x1000;
    private const uint LvmGetItemCount = LvmFirst + 4;
    private const uint LvmGetGroupCount = LvmFirst + 152;
    private const uint LvmIsGroupViewEnabled = LvmFirst + 175;
    private const uint RdwInvalidate = 0x0001;
    private const uint RdwAllChildren = 0x0080;
    private const uint RdwUpdateNow = 0x0100;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpNoZOrder = 0x0004;
    private const uint WmClose = 0x0010;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmMouseMove = 0x0200;
    private const uint WmEnterSizeMove = 0x0231;
    private const uint WmExitSizeMove = 0x0232;
    private const uint SmtoAbortIfHung = 0x0002;
    private const int SwRestore = 9;

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine(
                "Usage: MW5.ResizePerformanceProbe <app.exe> [minimum-item-count]"
                + " [screenshot-path]");
            return 2;
        }

        string executable = Path.GetFullPath(args[0]);
        int minimumItemCount = args.Length >= 2 ? int.Parse(args[1]) : 1;
        string settingsSource = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MW5LoadOrderConfigurator");
        string settingsCopy = Path.Combine(
            Path.GetTempPath(),
            "mw5-loc-resize-" + Guid.NewGuid().ToString("N"));

        CopyDirectory(settingsSource, settingsCopy);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo(executable)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(executable)!,
            },
        };
        process.StartInfo.Environment["MW5_LOC_SETTINGS_DIRECTORY"] = settingsCopy;
        process.StartInfo.Environment["MW5_LOC_TEST_LABEL"] = "resize-probe";

        try
        {
            process.Start();
            WaitForMainWindow(process, TimeSpan.FromSeconds(30));
            (IntPtr mainWindow, IntPtr listView) =
                WaitForListView(process, minimumItemCount, TimeSpan.FromSeconds(60));
            int itemCount = GetItemCount(listView);

            ShowWindow(mainWindow, SwRestore);
            SetForegroundWindow(mainWindow);
            WaitForProcessIdle(process, TimeSpan.FromSeconds(30));
            if (args.Length >= 3)
            {
                SelectVisibleRow(listView);
                CaptureWindow(listView, Path.GetFullPath(args[2]));
                SetCursorPos(0, 0);
                Thread.Sleep(150);
            }

            GetWindowRect(mainWindow, out Rect windowRect);
            GetWindowRect(listView, out Rect listRect);
            int originalWidth = windowRect.Right - windowRect.Left;
            int originalHeight = windowRect.Bottom - windowRect.Top;
            var original = (Width: originalWidth, Height: originalHeight);
            var narrower = (
                Width: Math.Max(800, originalWidth - 180),
                Height: originalHeight);
            var shorter = (
                Width: originalWidth,
                Height: Math.Max(600, originalHeight - 120));
            var smaller = (
                Width: narrower.Width,
                Height: shorter.Height);

            Console.WriteLine($"Process: {process.Id}");
            Console.WriteLine($"Items: {itemCount}");
            Console.WriteLine($"Groups: {SendMessage(listView, LvmGetGroupCount, IntPtr.Zero, IntPtr.Zero)}");
            Console.WriteLine(
                $"Group view: {SendMessage(listView, LvmIsGroupViewEnabled, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero}");
            Console.WriteLine($"Window: {originalWidth}x{originalHeight}");
            Console.WriteLine(
                $"List view: {listRect.Right - listRect.Left}x{listRect.Bottom - listRect.Top}");
            Measure("width natural", mainWindow, narrower, original, false);
            Measure("width forced", mainWindow, narrower, original, true);
            Measure("height natural", mainWindow, shorter, original, false);
            Measure("height forced", mainWindow, shorter, original, true);
            Measure("both natural", mainWindow, smaller, original, false);
            Measure("both forced", mainWindow, smaller, original, true);
            MeasureLiveResize("live resize", mainWindow, narrower, original);
            MeasureRedraw("list redraw", listView);

            PostMessage(mainWindow, WmClose, IntPtr.Zero, IntPtr.Zero);
            process.WaitForExit(5000);
            return 0;
        }
        finally
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);

            process.WaitForExit(5000);
            Directory.Delete(settingsCopy, recursive: true);
        }
    }

    private static void Measure(
        string label,
        IntPtr window,
        (int Width, int Height) first,
        (int Width, int Height) second,
        bool forceInvalidate)
    {
        for (int index = 0; index < 8; index++)
            ResizeAndPaint(window, index % 2 == 0 ? first : second, forceInvalidate);

        var samples = new double[30];
        for (int index = 0; index < samples.Length; index++)
        {
            samples[index] = ResizeAndPaint(
                window,
                index % 2 == 0 ? first : second,
                forceInvalidate);
        }

        Array.Sort(samples);
        double median = (samples[14] + samples[15]) / 2;
        double mean = samples.Average();
        double p95 = samples[(int)Math.Ceiling(samples.Length * 0.95) - 1];
        Console.WriteLine(
            $"{label,-16}: median {median,7:N2} ms, mean {mean,7:N2} ms, p95 {p95,7:N2} ms");
    }

    private static double ResizeAndPaint(
        IntPtr window,
        (int Width, int Height) size,
        bool forceInvalidate)
    {
        long start = Stopwatch.GetTimestamp();
        SetWindowPos(
            window,
            IntPtr.Zero,
            0,
            0,
            size.Width,
            size.Height,
            SwpNoZOrder | SwpNoActivate);
        RedrawWindow(
            window,
            IntPtr.Zero,
            IntPtr.Zero,
            (forceInvalidate ? RdwInvalidate : 0)
                | RdwUpdateNow
                | RdwAllChildren);
        return Stopwatch.GetElapsedTime(start).TotalMilliseconds;
    }

    private static void MeasureLiveResize(
        string label,
        IntPtr window,
        (int Width, int Height) first,
        (int Width, int Height) second)
    {
        SendMessage(window, WmEnterSizeMove, IntPtr.Zero, IntPtr.Zero);
        try
        {
            for (int index = 0; index < 8; index++)
                ResizeAndPaint(window, index % 2 == 0 ? first : second, false);

            var samples = new double[30];
            for (int index = 0; index < samples.Length; index++)
            {
                samples[index] = ResizeAndPaint(
                    window,
                    index % 2 == 0 ? first : second,
                    false);
            }

            Array.Sort(samples);
            double median = (samples[14] + samples[15]) / 2;
            double mean = samples.Average();
            double p95 = samples[(int)Math.Ceiling(samples.Length * 0.95) - 1];
            Console.WriteLine(
                $"{label,-16}: median {median,7:N2} ms, mean {mean,7:N2} ms, p95 {p95,7:N2} ms");

            // Keep the sizing session active without changing the window size so
            // the application's one-shot idle redraw has time to run.
            Thread.Sleep(250);
        }
        finally
        {
            SendMessage(window, WmExitSizeMove, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static void MeasureRedraw(string label, IntPtr window)
    {
        for (int index = 0; index < 8; index++)
            Redraw(window);

        var samples = new double[30];
        for (int index = 0; index < samples.Length; index++)
            samples[index] = Redraw(window);

        Array.Sort(samples);
        double median = (samples[14] + samples[15]) / 2;
        double mean = samples.Average();
        double p95 = samples[(int)Math.Ceiling(samples.Length * 0.95) - 1];
        Console.WriteLine(
            $"{label,-16}: median {median,7:N2} ms, mean {mean,7:N2} ms, p95 {p95,7:N2} ms");
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

    private static void WaitForMainWindow(Process process, TimeSpan timeout)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"Application exited with code {process.ExitCode}.");

            process.Refresh();
            if (process.MainWindowHandle != IntPtr.Zero)
                return;

            Thread.Sleep(50);
        }

        throw new TimeoutException("Timed out waiting for the main window.");
    }

    private static void WaitForProcessIdle(Process process, TimeSpan timeout)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        process.Refresh();
        TimeSpan previousProcessorTime = process.TotalProcessorTime;
        int quietSamples = 0;

        while (stopwatch.Elapsed < timeout)
        {
            Thread.Sleep(250);
            process.Refresh();
            TimeSpan currentProcessorTime = process.TotalProcessorTime;
            TimeSpan processorDelta = currentProcessorTime - previousProcessorTime;
            previousProcessorTime = currentProcessorTime;

            quietSamples = processorDelta <= TimeSpan.FromMilliseconds(10)
                ? quietSamples + 1
                : 0;
            if (quietSamples >= 4)
                return;
        }
    }

    private static (IntPtr MainWindow, IntPtr ListView) WaitForListView(
        Process process,
        int minimumItemCount,
        TimeSpan timeout)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            IntPtr bestMainWindow = IntPtr.Zero;
            IntPtr bestListView = IntPtr.Zero;
            int bestCount = -1;
            EnumWindows(
                (topLevelWindow, _) =>
                {
                    GetWindowThreadProcessId(topLevelWindow, out uint processId);
                    if (processId != process.Id)
                        return true;

                    EnumChildWindows(
                        topLevelWindow,
                        (handle, _) =>
                        {
                            var className = new StringBuilder(128);
                            GetClassName(handle, className, className.Capacity);
                            if (!className.ToString().Contains(
                                    "SysListView32",
                                    StringComparison.Ordinal))
                            {
                                return true;
                            }

                            int count = TryGetItemCount(handle);
                            if (count < 0)
                                return true;
                            if (count > bestCount)
                            {
                                bestCount = count;
                                bestListView = handle;
                                bestMainWindow = topLevelWindow;
                            }

                            return true;
                        },
                        IntPtr.Zero);

                    return true;
                },
                IntPtr.Zero);

            if (bestListView != IntPtr.Zero && bestCount >= minimumItemCount)
                return (bestMainWindow, bestListView);

            Thread.Sleep(100);
        }

        throw new TimeoutException("Timed out waiting for the populated mod list.");
    }

    private static int GetItemCount(IntPtr listView)
    {
        int itemCount = TryGetItemCount(listView);
        if (itemCount < 0)
            throw new TimeoutException("The mod list did not respond.");
        return itemCount;
    }

    private static int TryGetItemCount(IntPtr listView)
    {
        IntPtr succeeded = SendMessageTimeout(
            listView,
            LvmGetItemCount,
            IntPtr.Zero,
            IntPtr.Zero,
            SmtoAbortIfHung,
            500,
            out IntPtr result);
        return succeeded == IntPtr.Zero ? -1 : unchecked((int)result);
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (string file in Directory.EnumerateFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));

        foreach (string directory in Directory.EnumerateDirectories(source))
        {
            CopyDirectory(
                directory,
                Path.Combine(destination, Path.GetFileName(directory)));
        }
    }

    private static void CaptureWindow(IntPtr window, string outputPath)
    {
        GetWindowRect(window, out Rect rect);
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;
        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
        bitmap.Save(outputPath, ImageFormat.Png);
    }

    private static void SelectVisibleRow(IntPtr listView)
    {
        const int x = 200;
        const int y = 55;
        IntPtr coordinates = new((y << 16) | x);
        SendMessage(listView, WmMouseMove, IntPtr.Zero, coordinates);
        SendMessage(listView, WmLeftButtonDown, new IntPtr(1), coordinates);
        SendMessage(listView, WmLeftButtonUp, IntPtr.Zero, coordinates);
        Thread.Sleep(150);
    }

    private delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);

    [DllImport("user32.dll")]
    private static extern bool EnumChildWindows(
        IntPtr parent,
        EnumWindowsProc callback,
        IntPtr parameter);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(
        EnumWindowsProc callback,
        IntPtr parameter);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(
        IntPtr window,
        StringBuilder className,
        int maxCount);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr window, out Rect rect);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(
        IntPtr window,
        out uint processId);

    [DllImport("user32.dll")]
    private static extern bool RedrawWindow(
        IntPtr window,
        IntPtr updateRectangle,
        IntPtr updateRegion,
        uint flags);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr window,
        uint message,
        IntPtr wParam,
        IntPtr lParam,
        uint flags,
        uint timeoutMilliseconds,
        out IntPtr result);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        IntPtr window,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr window);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr window, int command);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
