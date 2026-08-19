# tests/evaluations/run-plan.ps1
#
# SSE-Pendant zu run-assess.ps1. Der Plan-Endpoint liefert keine einzelne
# JSON-Response, sondern einen text/event-stream mit laufenden Status-Events
# ("Preparing plan request." -> ... -> "Correcting the plan (round X of 2)."
# -> ... -> "Completed"/"Failed"). Invoke-WebRequest puffert das komplett und
# eignet sich dafuer nicht, deshalb hier HttpClient mit ResponseHeadersRead +
# zeilenweisem Lesen.
#
# data/ wird mit run-assess.ps1 geteilt. Namensschema fuer Plan-Testdaten:
# plan__<disciplineType>__<szenario>.json (z.B. plan__climber__notargets.json).
# run-assess.ps1 ignoriert alles mit "plan__"-Praefix, run-plan.ps1 laedt
# ausschliesslich Dateien mit diesem Praefix.

param(
    [string]$DatasetName,
    [string]$PlanEndpointPath = "/api/coach/plan",
    [string]$SchedulingPreference = "",
    # PlanningScope-Enum: CurrentWeek=0, NextWeek=1 (analog zum AssessmentType-Enum
    # in run-assess.ps1, das auch als Zahl statt als String gesendet wird).
    [int]$Scope = 0,
    [Nullable[int]]$WeeklyTssTarget = $null
)

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

Add-Type -AssemblyName System.Net.Http

$dataFolder    = Join-Path $PSScriptRoot "data"
$timestamp     = Get-Date -Format "yyyy-MM-dd_HH-mm"
$resultsFolder = Join-Path $PSScriptRoot "results\$timestamp"

# data/ wird mit run-assess.ps1 geteilt. Plan-Testdaten tragen das Praefix
# "plan__" (Schema: plan__<disciplineType>__<szenario>.json) und werden hier
# bewusst als einzige geladen -- alles andere gehoert zu run-assess.ps1.
$allTestFiles = Get-ChildItem -Path $dataFolder -Filter "plan__*.json"

if ($PSBoundParameters.ContainsKey('DatasetName')) {
    $normalizedDatasetName = $DatasetName.Trim()

    if ([string]::IsNullOrWhiteSpace($normalizedDatasetName)) {
        Write-Host "DatasetName darf nicht leer sein." -ForegroundColor Red
        exit 1
    }

    $matchingFiles = @(
        $allTestFiles | Where-Object {
            $_.Name -ieq $normalizedDatasetName -or
            $_.BaseName -ieq $normalizedDatasetName -or
            $_.Name -ieq "$normalizedDatasetName.json"
        }
    )

    if ($matchingFiles.Count -eq 0) {
        Write-Host "Kein Datensatz gefunden fuer: $normalizedDatasetName" -ForegroundColor Red
        Write-Host "Verfuegbare Dateien:" -ForegroundColor Yellow
        $allTestFiles | ForEach-Object { Write-Host " - $($_.Name)" -ForegroundColor Yellow }
        exit 1
    }

    $testFiles = $matchingFiles
    Write-Host "Nur Datensatz: $normalizedDatasetName" -ForegroundColor Cyan
}
else {
    $testFiles = $allTestFiles
}

New-Item -ItemType Directory -Path $resultsFolder -Force | Out-Null

$language = "de"

foreach ($file in $testFiles) {
    Write-Host "=== $($file.Name) ===" -ForegroundColor Cyan

    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    $parts    = $baseName -split "__"

    if ($parts.Count -lt 3) {
        Write-Host "Ungueltiger Dateiname (erwartet plan__<disciplineType>__<szenario>.json)" -ForegroundColor Red
        Write-Host ""
        continue
    }

    $disciplineTypeName = $parts[1]
    $disciplineType = (Get-Culture).TextInfo.ToTitleCase($disciplineTypeName.ToLower())

    $weekDataJson = Get-Content -Raw -Path $file.FullName

    # PlanRequest verlangt Scope und Constraints zwingend (kein Default in C#) --
    # ohne diese Felder bindet .NET sie stillschweigend auf CLR-Defaults
    # (Scope=0, Constraints=null).
    $bodyObj = @{
        weekDataJson         = $weekDataJson
        disciplineType       = $disciplineType
        language              = $language
        schedulingPreference  = $SchedulingPreference
        scope                 = $Scope
        constraints           = @{
            weeklyTssTarget = $WeeklyTssTarget
            dayConstraints  = @()
        }
    }
    $bodyJson = $bodyObj | ConvertTo-Json -Depth 10

    $planUri = "$($config.SiteUrl)$PlanEndpointPath"
    $outPath = Join-Path $resultsFolder "$baseName.md"

    $handler = New-Object System.Net.Http.HttpClientHandler
    $client  = New-Object System.Net.Http.HttpClient($handler)
    $client.Timeout = [System.TimeSpan]::FromMinutes(10)

    $request = New-Object System.Net.Http.HttpRequestMessage('Post', $planUri)
    $request.Headers.Add("X-Intervals-Athlete-Id", $config.IntervalsAthleteId)
    $request.Headers.Add("X-Intervals-Api-Key", $config.IntervalsApiKey)
    $request.Headers.Add("Origin", $config.SiteUrl)
    $request.Headers.Accept.Add([System.Net.Http.Headers.MediaTypeWithQualityHeaderValue]::new("text/event-stream"))
    $request.Content = New-Object System.Net.Http.StringContent($bodyJson, [System.Text.Encoding]::UTF8, "application/json")

    $lastEvent = $null

    try {
        $response = $client.SendAsync($request, [System.Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()

        if ([int]$response.StatusCode -ge 400) {
            $errBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            Write-Host "Fehler ($([int]$response.StatusCode)): $errBody" -ForegroundColor Red
            Write-Host ""
            continue
        }

        $stream = $response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
        $reader = New-Object System.IO.StreamReader($stream)

        while (-not $reader.EndOfStream) {
            $line = $reader.ReadLine()
            if ([string]::IsNullOrEmpty($line) -or -not $line.StartsWith("data:")) { continue }

            $jsonPart = $line.Substring(5).Trim()
            if ($jsonPart.Length -eq 0) { continue }

            $evt = $jsonPart | ConvertFrom-Json
            $lastEvent = $evt
            Write-Host "  -> $($evt.message)" -ForegroundColor DarkGray
        }
        $reader.Dispose()
    }
    catch {
        Write-Host "Verbindungsfehler: $($_.Exception.Message)" -ForegroundColor Red
    }
    finally {
        $client.Dispose()
    }

    if ($null -eq $lastEvent -or $lastEvent.stage -eq "Failed") {
        Write-Host "-> Fehlgeschlagen: $($lastEvent.message)" -ForegroundColor Red
    }
    else {
        Set-Content -Path $outPath -Value $lastEvent.result.content -Encoding utf8
        Write-Host "-> $outPath" -ForegroundColor Green

        if ($lastEvent.warnings) {
            Write-Host "-> Offene Punkte:" -ForegroundColor Yellow
            $lastEvent.warnings | ForEach-Object { Write-Host "   - $_" -ForegroundColor Yellow }
        }
    }

    Write-Host ""
}

Write-Host "Ergebnisse liegen in: $resultsFolder" -ForegroundColor Cyan