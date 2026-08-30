param(
    [Parameter(Mandatory = $true)]
    [string] $LinkPath,

    [Parameter(Mandatory = $true)]
    [string] $MissingTargetPath
)

$resolvedLinkPath = [System.IO.Path]::GetFullPath($LinkPath)
$resolvedTargetPath = [System.IO.Path]::GetFullPath($MissingTargetPath)

[System.IO.File]::Delete($resolvedLinkPath)
[void] [System.IO.File]::CreateSymbolicLink(
    $resolvedLinkPath,
    $resolvedTargetPath)

Get-Item -LiteralPath $resolvedLinkPath |
    Select-Object FullName, LinkType, Target, Length
