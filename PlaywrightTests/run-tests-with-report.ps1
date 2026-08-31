# ==============================================
# run-tests-with-report.ps1
# KAN-4: HTML Test Reporting
# Project: Playwright Capstone (KAN)
# Owner: Charan Kumar
# Usage:
#   .\run-tests-with-report.ps1
#   .\run-tests-with-report.ps1 -Filter "TestName"
#   .\run-tests-with-report.ps1 -Category "Negative"
#   .\run-tests-with-report.ps1 -Browser "firefox"
#   .\run-tests-with-report.ps1 -Headless
# ==============================================

param(
    [string]$Filter    = "",
    [string]$Category  = "",
    [string]$Browser   = "",
    [switch]$Headless  = $false
)

# ──────────────────────────────────────────────
# Configuration
# ──────────────────────────────────────────────

# Project structure - matches Solution Explorer
$CsprojPath  = "PlaywrightTests\PlaywrightTests.csproj"

# Report path matches appsettings.json ReportPath
# "ReportPath": "./TestResults/HtmlReports/"
$ReportBase  = "PlaywrightTests\TestResults\HtmlReports"
$Timestamp   = Get-Date -Format "yyyyMMdd_HHmmss"
$ReportFile  = Join-Path $ReportBase `
    "TestReport-$Timestamp.html"
$TrxFile     = Join-Path $ReportBase `
    "TestResults-$Timestamp.trx"
$ScreenDir   = Join-Path $ReportBase "Screenshots"
$TraceDir    = Join-Path $ReportBase "Traces"

# ──────────────────────────────────────────────
# Header
# ──────────────────────────────────────────────
Write-Host ""
Write-Host "==========================================" `
    -ForegroundColor Cyan
Write-Host "  PlaywrightTests - HTML Test Report" `
    -ForegroundColor Cyan
Write-Host "  KAN-4: HTML Reporting | Project: KAN" `
    -ForegroundColor Cyan
Write-Host "  $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" `
    -ForegroundColor Gray
Write-Host "==========================================" `
    -ForegroundColor Cyan

# ──────────────────────────────────────────────
# Apply overrides via environment variables
# These override appsettings.json values
# ──────────────────────────────────────────────
if ($Browser) {
    $env:TESTSETTINGS__BROWSER = $Browser
    Write-Host "  Browser override : $Browser" `
        -ForegroundColor Yellow
}

if ($Headless) {
    $env:TESTSETTINGS__HEADLESS = "true"
    $env:TESTSETTINGS__SLOWMO   = "0"
    Write-Host "  Mode             : Headless" `
        -ForegroundColor Yellow
}
else {
    Write-Host "  Mode             : Headed (visible)" `
        -ForegroundColor Gray
}

# ──────────────────────────────────────────────
# Ensure output directories exist
# Matches folder structure in Solution Explorer:
# TestResults/HtmlReports/Screenshots/
# TestResults/HtmlReports/Traces/
# ──────────────────────────────────────────────
Write-Host ""
Write-Host "[1/3] Preparing output directories..." `
    -ForegroundColor Yellow

New-Item -ItemType Directory -Force `
    -Path $ReportBase  | Out-Null
New-Item -ItemType Directory -Force `
    -Path $ScreenDir   | Out-Null
New-Item -ItemType Directory -Force `
    -Path $TraceDir    | Out-Null

Write-Host "  Report  : $ReportFile" `
    -ForegroundColor Gray
Write-Host "  TRX     : $TrxFile" `
    -ForegroundColor Gray
Write-Host "  Screens : $ScreenDir" `
    -ForegroundColor Gray
Write-Host "  Traces  : $TraceDir" `
    -ForegroundColor Gray

# ──────────────────────────────────────────────
# Build filter argument
# ──────────────────────────────────────────────
$FilterArg = ""

if ($Filter -and $Category) {
    $FilterArg = "--filter `"$Filter&Category=$Category`""
}
elseif ($Filter) {
    $FilterArg = "--filter `"$Filter`""
}
elseif ($Category) {
    $FilterArg = "--filter `"Category=$Category`""
}

# ──────────────────────────────────────────────
# Run tests
# ──────────────────────────────────────────────
Write-Host ""
Write-Host "[2/3] Running tests..." `
    -ForegroundColor Yellow

if ($FilterArg) {
    Write-Host "  Filter : $FilterArg" `
        -ForegroundColor Gray
}
else {
    Write-Host "  Filter : None (running all tests)" `
        -ForegroundColor Gray
}

Write-Host ""

# Build the dotnet test command
# Using multiple loggers:
#   html    → opens in browser
#   trx     → for CI/CD integration
#   console → visible in terminal
$dotnetCmd = "dotnet test `"$CsprojPath`" " +
    "$FilterArg " +
    "--logger `"html;LogFileName=$ReportFile`" " +
    "--logger `"trx;LogFileName=$TrxFile`" " +
    "--logger `"console;verbosity=normal`" " +
    "--results-directory `"$ReportBase`""

Write-Host "  Command: dotnet test ..." `
    -ForegroundColor Gray
Write-Host ""

# Execute
Invoke-Expression $dotnetCmd
$ExitCode = $LASTEXITCODE

# ──────────────────────────────────────────────
# Results summary
# ──────────────────────────────────────────────
Write-Host ""
Write-Host "[3/3] Results Summary" `
    -ForegroundColor Yellow
Write-Host "==========================================" `
    -ForegroundColor Cyan

# Check HTML report
if (Test-Path $ReportFile) {
    Write-Host "  HTML Report : $ReportFile" `
        -ForegroundColor Green
}
else {
    # dotnet test may name it differently
    # Look for any recent HTML file
    $htmlFiles = Get-ChildItem $ReportBase `
        -Filter "*.html" -ErrorAction SilentlyContinue `
        | Sort-Object LastWriteTime -Descending `
        | Select-Object -First 1

    if ($htmlFiles) {
        $ReportFile = $htmlFiles.FullName
        Write-Host "  HTML Report : $ReportFile" `
            -ForegroundColor Green
    }
    else {
        Write-Host "  HTML Report : Not generated" `
            -ForegroundColor Yellow
        Write-Host "  Tip: Check dotnet test output above" `
            -ForegroundColor Gray
    }
}

# Check TRX
if (Test-Path $TrxFile) {
    Write-Host "  TRX File    : $TrxFile" `
        -ForegroundColor Green
}

# Count screenshots (failures only)
$screenshots = @()
if (Test-Path $ScreenDir) {
    $screenshots = Get-ChildItem `
        -Path $ScreenDir `
        -Filter "*.png" `
        -ErrorAction SilentlyContinue
}

if ($screenshots.Count -gt 0) {
    Write-Host ""
    Write-Host "  FAILURES: $($screenshots.Count) test(s) failed" `
        -ForegroundColor Red
    Write-Host "  Screenshots captured:" `
        -ForegroundColor Red
    foreach ($s in $screenshots) {
        Write-Host "    📸 $($s.Name)" `
            -ForegroundColor Red
    }
}
else {
    Write-Host "  Screenshots : 0 (no failures)" `
        -ForegroundColor Green
}

# Count traces (failures only)
$traces = @()
if (Test-Path $TraceDir) {
    $traces = Get-ChildItem `
        -Path $TraceDir `
        -Filter "*.zip" `
        -ErrorAction SilentlyContinue
}

if ($traces.Count -gt 0) {
    Write-Host "  Traces saved: $($traces.Count)" `
        -ForegroundColor Red
    Write-Host "  View traces : https://trace.playwright.dev" `
        -ForegroundColor Gray
}
else {
    Write-Host "  Traces      : 0 (no failures)" `
        -ForegroundColor Green
}

# Final result
Write-Host ""
Write-Host "==========================================" `
    -ForegroundColor Cyan

if ($ExitCode -eq 0) {
    Write-Host "  RESULT: ALL TESTS PASSED" `
        -ForegroundColor Green
    Write-Host "  TestResults/HtmlReports/ is clean" `
        -ForegroundColor Green
}
else {
    Write-Host "  RESULT: SOME TESTS FAILED" `
        -ForegroundColor Red
    Write-Host "  Check screenshots and traces above" `
        -ForegroundColor Red
}

Write-Host "==========================================" `
    -ForegroundColor Cyan
Write-Host ""

# Auto-open HTML report in default browser
if (Test-Path $ReportFile) {
    Write-Host "  Opening report in browser..." `
        -ForegroundColor Gray
    Start-Process $ReportFile
}

# Clean up env var overrides
if ($Browser) {
    Remove-Item Env:\TESTSETTINGS__BROWSER `
        -ErrorAction SilentlyContinue
}
if ($Headless) {
    Remove-Item Env:\TESTSETTINGS__HEADLESS `
        -ErrorAction SilentlyContinue
    Remove-Item Env:\TESTSETTINGS__SLOWMO `
        -ErrorAction SilentlyContinue
}

exit $ExitCode