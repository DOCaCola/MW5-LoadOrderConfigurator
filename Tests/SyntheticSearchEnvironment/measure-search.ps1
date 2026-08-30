param(
    [Parameter(Mandatory = $true)]
    [string] $WindowTitle
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type @"
using System;
using System.Runtime.InteropServices;

public static class NativeListView
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(
        IntPtr windowHandle,
        int message,
        IntPtr wParam,
        IntPtr lParam);
}
"@

function Find-FirstElement {
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

function Set-ToggleState {
    param(
        [System.Windows.Automation.AutomationElement] $Element,
        [bool] $Checked
    )

    $pattern = [System.Windows.Automation.TogglePattern] $Element.GetCurrentPattern(
        [System.Windows.Automation.TogglePattern]::Pattern)
    $wantedState = if ($Checked) {
        [System.Windows.Automation.ToggleState]::On
    } else {
        [System.Windows.Automation.ToggleState]::Off
    }
    if ($pattern.Current.ToggleState -ne $wantedState) {
        $pattern.Toggle()
    }
}

function Get-SearchTimingText {
    param(
        [System.Windows.Automation.AutomationElement] $StatusBar
    )

    $textCondition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Text)
    foreach ($element in $StatusBar.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $textCondition)) {
        if ($element.Current.Name.StartsWith("Search:")) {
            return $element.Current.Name
        }
    }
    return "<no timing>"
}

function Wait-ForSearchCompletion {
    param(
        [System.Windows.Automation.AutomationElement] $StatusBar,
        [string] $Mode,
        [string] $Term,
        [int] $TimeoutMilliseconds = 5000
    )

    $suffix = "[$Mode | $Term]"
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    do {
        $timing = Get-SearchTimingText $StatusBar
        if ($timing.EndsWith($suffix)) {
            return $timing
        }
        Start-Sleep -Milliseconds 20
    } while ($stopwatch.ElapsedMilliseconds -lt $TimeoutMilliseconds)

    throw "Timed out waiting for search '$Mode | $Term'. Last timing: $timing"
}

function Get-VisibleRowCount {
    param(
        [System.Windows.Automation.AutomationElement] $List
    )

    $LVM_GETITEMCOUNT = 0x1004
    $handle = [IntPtr] $List.Current.NativeWindowHandle
    return [NativeListView]::SendMessage(
        $handle,
        $LVM_GETITEMCOUNT,
        [IntPtr]::Zero,
        [IntPtr]::Zero).ToInt32()
}

$root = [System.Windows.Automation.AutomationElement]::RootElement
$windowCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    $WindowTitle)
$window = Find-FirstElement $root `
    ([System.Windows.Automation.TreeScope]::Children) `
    $windowCondition

$searchCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
    [System.Windows.Automation.ControlType]::Edit)
$searchBox = Find-FirstElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $searchCondition

$filterCondition = [System.Windows.Automation.AndCondition]::new(
    [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::CheckBox),
    [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::NameProperty,
        "Filter"))
$filterToggle = Find-FirstElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $filterCondition

$statusCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
    "statusStrip1")
$statusBar = Find-FirstElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $statusCondition

$listCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
    "modObjectListView")
$modList = Find-FirstElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $listCondition

$valuePattern = [System.Windows.Automation.ValuePattern] $searchBox.GetCurrentPattern(
    [System.Windows.Automation.ValuePattern]::Pattern)
$terms = @(
    "Synthetic",
    "Weapons",
    "Author 23",
    "Synthetic_Mod_0599",
    "no-such-mod"
)

foreach ($filterMode in @($false, $true)) {
    Set-ToggleState $filterToggle $filterMode
    $modeName = if ($filterMode) { "filter" } else { "highlight" }

    foreach ($term in $terms) {
        $valuePattern.SetValue("")
        Start-Sleep -Milliseconds 75

        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        $valuePattern.SetValue($term)
        $timing = Wait-ForSearchCompletion $statusBar $modeName $term
        $stopwatch.Stop()

        [PSCustomObject]@{
            Mode = $modeName
            Term = $term
            UiRoundTripMs = $stopwatch.Elapsed.TotalMilliseconds
            AppTiming = $timing
            VisibleRows = Get-VisibleRowCount $modList
        }
    }
}
