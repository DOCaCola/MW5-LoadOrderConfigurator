param(
    [Parameter(Mandatory = $true)]
    [string] $WindowTitle
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Threading;

public static class SafeMouseInput
{
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(
        uint flags,
        uint dx,
        uint dy,
        uint data,
        UIntPtr extraInfo);

    public static void Click(int x, int y)
    {
        SetCursorPos(x, y);
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }

    public static void Drag(int fromX, int fromY, int toX, int toY)
    {
        SetCursorPos(fromX, fromY);
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(150);
        for (int step = 1; step <= 12; step++)
        {
            int x = fromX + ((toX - fromX) * step / 12);
            int y = fromY + ((toY - fromY) * step / 12);
            SetCursorPos(x, y);
            Thread.Sleep(35);
        }
        Thread.Sleep(150);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }
}
"@

function Wait-ForElement {
    param(
        [System.Windows.Automation.AutomationElement] $Root,
        [System.Windows.Automation.TreeScope] $Scope,
        [System.Windows.Automation.Condition] $Condition,
        [int] $TimeoutMilliseconds = 10000
    )

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    do {
        $element = $Root.FindFirst($Scope, $Condition)
        if ($null -ne $element) {
            return $element
        }
        Start-Sleep -Milliseconds 50
    } while ($stopwatch.ElapsedMilliseconds -lt $TimeoutMilliseconds)

    throw "Timed out waiting for an automation element."
}

function Get-NamedElement {
    param(
        [System.Windows.Automation.AutomationElement] $Window,
        [string] $Name
    )

    $condition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::NameProperty,
        $Name)
    return Wait-ForElement $Window `
        ([System.Windows.Automation.TreeScope]::Descendants) `
        $condition
}

function Click-NamedElement {
    param(
        [System.Windows.Automation.AutomationElement] $Window,
        [string] $Name
    )

    $element = Get-NamedElement $Window $Name
    $bounds = $element.Current.BoundingRectangle
    if ($bounds.IsEmpty) {
        throw "The element '$Name' is not visible."
    }
    [SafeMouseInput]::Click(
        [int] ($bounds.Left + $bounds.Width / 2),
        [int] ($bounds.Top + $bounds.Height / 2))
}

function Invoke-Button {
    param(
        [System.Windows.Automation.AutomationElement] $Window,
        [string] $Name
    )

    $button = Get-NamedElement $Window $Name
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    while (-not $button.Current.IsEnabled -and $stopwatch.ElapsedMilliseconds -lt 5000) {
        Start-Sleep -Milliseconds 25
    }
    if (-not $button.Current.IsEnabled) {
        throw "Button '$Name' did not become enabled."
    }

    $pattern = [System.Windows.Automation.InvokePattern] $button.GetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern)
    $pattern.Invoke()
}

function Wait-ForRowPosition {
    param(
        [System.Windows.Automation.AutomationElement] $Window,
        [string] $ModName,
        [scriptblock] $Predicate,
        [int] $TimeoutMilliseconds = 5000
    )

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    do {
        $element = Get-NamedElement $Window $ModName
        $bounds = $element.Current.BoundingRectangle
        if (-not $bounds.IsEmpty -and (& $Predicate $bounds)) {
            return $bounds
        }
        Start-Sleep -Milliseconds 25
    } while ($stopwatch.ElapsedMilliseconds -lt $TimeoutMilliseconds)

    throw "The row '$ModName' did not move to the expected position."
}

$root = [System.Windows.Automation.AutomationElement]::RootElement
$windowCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    $WindowTitle)
$window = Wait-ForElement $root `
    ([System.Windows.Automation.TreeScope]::Children) `
    $windowCondition

$totalCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    "Total: 600")
[void] (Wait-ForElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $totalCondition)

$standardGroupCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    "Standard")
if ($null -ne $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    $standardGroupCondition)) {
    throw "The Standard group header is visible."
}

$sourceMod = "Synthetic Mechs Test Mod 0589"
$targetMod = "Synthetic Visuals Test Mod 0585"
$initialBounds = (Get-NamedElement $window $sourceMod).Current.BoundingRectangle
Click-NamedElement $window $sourceMod
Invoke-Button $window "To top"
$topBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -lt ($initialBounds.Top - 50)
}

Click-NamedElement $window $sourceMod
Invoke-Button $window "To bottom"
$bottomBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -gt ($topBounds.Top + 100)
}

Click-NamedElement $window $sourceMod
Invoke-Button $window "To top"
$topBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -lt ($bottomBounds.Top - 100)
}

$sourceBounds = (Get-NamedElement $window $sourceMod).Current.BoundingRectangle
$targetBounds = (Get-NamedElement $window $targetMod).Current.BoundingRectangle
[SafeMouseInput]::Drag(
    [int] ($sourceBounds.Left + $sourceBounds.Width / 2),
    [int] ($sourceBounds.Top + $sourceBounds.Height / 2),
    [int] ($targetBounds.Left + $targetBounds.Width / 2),
    [int] ($targetBounds.Bottom - 2))
$draggedBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -gt ($topBounds.Top + 50)
}

Click-NamedElement $window $sourceMod
Invoke-Button $window "Up"
$upBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -lt ($draggedBounds.Top - 5)
}

Click-NamedElement $window $sourceMod
Invoke-Button $window "Down"
$downBounds = Wait-ForRowPosition $window $sourceMod {
    param($bounds)
    $bounds.Top -gt ($upBounds.Top + 5)
}

if ($null -ne $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    $standardGroupCondition)) {
    throw "The Standard group header became visible after regrouping."
}

[PSCustomObject]@{
    StandardHeaderHidden = $true
    MoveToTopVisible = $true
    MoveToBottomVisible = $true
    MoveUpVisible = $true
    MoveDownVisible = $true
    DragDropMoveVisible = $true
    InitialY = [int] $initialBounds.Top
    TopY = [int] $topBounds.Top
    BottomY = [int] $bottomBounds.Top
    DraggedY = [int] $draggedBounds.Top
    UpY = [int] $upBounds.Top
    DownY = [int] $downBounds.Top
}
