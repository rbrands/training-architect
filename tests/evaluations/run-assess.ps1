# tests/evaluations/run-assess.ps1

# ---------------------------------------------------------------------------
# Load config
# ---------------------------------------------------------------------------
$configPath = Join-Path $PSScriptRoot "..\..\config.ps1"
if (-not (Test-Path $configPath)) {
    Write-Host "config.ps1 not found." -ForegroundColor Red
    Write-Host "Copy config.example.ps1 to config.ps1 and fill in your values." -ForegroundColor Yellow
    exit 1
}
. $configPath

$dataFolder    = Join-Path $PSScriptRoot "data"
$timestamp     = Get-Date -Format "yyyy-MM-dd_HH-mm"
$resultsFolder = Join-Path $PSScriptRoot "results\$timestamp"

New-Item -ItemType Directory -Path $resultsFolder -Force | Out-Null

$language = "de"

# AssessmentType-Enum: Activity=0, Week=1, Metrics=2
$assessmentTypeMap = @{
    "activity" = 0
    "week"     = 1
    "metrics"  = 2
}

$testFiles = Get-ChildItem -Path $dataFolder -Filter "*.json"

foreach ($file in $testFiles) {
    Write-Host "=== $($file.Name) ===" -ForegroundColor Cyan

    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    $parts    = $baseName -split "__"

    if ($parts.Count -lt 2) {
        Write-Host "Ungueltiger Dateiname (erwartet <assessmentType>__<disciplineType>__<szenario>.json)" -ForegroundColor Red
        Write-Host ""
        continue
    }

    $assessmentTypeName = $parts[0].ToLower()
    $disciplineTypeName = $parts[1]

    if (-not $assessmentTypeMap.ContainsKey($assessmentTypeName)) {
        Write-Host "Unbekannter assessmentType '$assessmentTypeName' (erlaubt: activity, week, metrics)" -ForegroundColor Red
        Write-Host ""
        continue
    }

    $assessmentType = $assessmentTypeMap[$assessmentTypeName]
    $disciplineType = (Get-Culture).TextInfo.ToTitleCase($disciplineTypeName)

    $weekDataJson = Get-Content -Raw -Path $file.FullName

    $body = @{
        weekDataJson   = $weekDataJson
        disciplineType = $disciplineType
        language       = $language
        assessmentType = $assessmentType
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Method Post -Uri "$($config.SiteUrl)/api/coach/assess" `
        -ContentType "application/json" `
        -Headers @{
            "X-Intervals-Athlete-Id" = $config.IntervalsAthleteId
            "X-Intervals-Api-Key"    = $config.IntervalsApiKey
            "Origin"                 = $config.SiteUrl
        } `
        -Body $body `
        -SkipHttpErrorCheck

    if ($response.StatusCode -ge 400) {
        Write-Host "Fehler ($($response.StatusCode)): $($response.Content)" -ForegroundColor Red
    }
    else {
        $parsed  = $response.Content | ConvertFrom-Json
        $outPath = Join-Path $resultsFolder "$baseName.md"

        Set-Content -Path $outPath -Value $parsed.content -Encoding utf8
        Write-Host "-> $outPath" -ForegroundColor Green
    }

    Write-Host ""
}

Write-Host "Ergebnisse liegen in: $resultsFolder" -ForegroundColor Cyan
