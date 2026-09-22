$ErrorActionPreference = "Stop"

$dest = Join-Path $PSScriptRoot "..\lib\SimConnect"
New-Item -ItemType Directory -Force -Path $dest | Out-Null

$candidates = @(
    "C:\MSFS 2024 SDK\SimConnect SDK\lib",
    "C:\MSFS 2024 SDK\SimConnect SDK\lib\managed"
)

$managed = $null
$native = $null

foreach ($root in $candidates) {
    $m = Join-Path $root "Microsoft.FlightSimulator.SimConnect.dll"
    $n = Join-Path $root "SimConnect.dll"
    if (Test-Path $m) { $managed = $m }
    if (Test-Path $n) { $native = $n }
}

if (-not $managed) { throw "Microsoft.FlightSimulator.SimConnect.dll was not found. Install the MSFS 2024 SDK or update the path in this script." }
if (-not $native) { throw "SimConnect.dll was not found. Install the MSFS 2024 SDK or update the path in this script." }

Copy-Item $managed (Join-Path $dest "Microsoft.FlightSimulator.SimConnect.dll") -Force
Copy-Item $native (Join-Path $dest "SimConnect.dll") -Force

Write-Host "SimConnect files prepared in $dest"
