param(
    [Parameter(Mandatory = $true)]
    [string] $WindowTitle,

    [int] $ExpectedLoadedMods = 599
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

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

$root = [System.Windows.Automation.AutomationElement]::RootElement
$windowCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    $WindowTitle)
$window = Wait-ForElement $root `
    ([System.Windows.Automation.TreeScope]::Children) `
    $windowCondition

$textCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
    [System.Windows.Automation.ControlType]::Text)
$warningHeadingCondition = [System.Windows.Automation.PropertyCondition]::new(
    [System.Windows.Automation.AutomationElement]::NameProperty,
    "1 mod could not be loaded.")
[void] (Wait-ForElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $warningHeadingCondition)

$warningText = ($window.FindAll(
    [System.Windows.Automation.TreeScope]::Descendants,
    $textCondition) | ForEach-Object { $_.Current.Name }) -join "`n"

if (-not $warningText.Contains("Synthetic_Mod_0007")) {
    throw "The aggregate warning did not identify the broken synthetic mod."
}
if (-not $warningText.Contains("purge and redeploy")) {
    throw "The aggregate warning did not include Vortex recovery guidance."
}

$okCondition = [System.Windows.Automation.AndCondition]::new(
    [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Button),
    [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::NameProperty,
        "OK"))
$okButton = Wait-ForElement $window `
    ([System.Windows.Automation.TreeScope]::Descendants) `
    $okCondition
$invokePattern = [System.Windows.Automation.InvokePattern] $okButton.GetCurrentPattern(
    [System.Windows.Automation.InvokePattern]::Pattern)
$invokePattern.Invoke()

$expectedText = "Total: $ExpectedLoadedMods"
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
do {
    $loadedCountFound = $false
    foreach ($element in $window.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $textCondition)) {
        if ($element.Current.Name -eq $expectedText) {
            $loadedCountFound = $true
            break
        }
    }
    if ($loadedCountFound) {
        break
    }
    Start-Sleep -Milliseconds 50
} while ($stopwatch.ElapsedMilliseconds -lt 5000)

if (-not $loadedCountFound) {
    throw "Expected '$expectedText' after dismissing the warning."
}

[PSCustomObject]@{
    BrokenModIdentified = $true
    VortexGuidanceShown = $true
    LoadedMods = $ExpectedLoadedMods
}
