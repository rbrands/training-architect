# tests/evaluations/run-plan.ps1
#
# SSE-Pendant zu run-assess.ps1. Der Plan-Endpoint liefert keine einzelne
# JSON-Response, sondern einen text/event-stream mit laufenden Status-Events
# (siehe UI: "Preparing plan request." -> ... -> "Correcting the plan (round X of 2)."
# -> ...). Invoke-WebRequest puffert das komplett und eignet sich dafuer nicht,
# deshalb hier HttpClient mit ResponseHeadersRead + zeilenweisem Lesen.
#
# WICHTIG: Endpoint-Pfad und Body-Felder unten sind Annahmen (analog zu
# /api/coach/assess). Bitte gegenpruefen/anpassen -- der Rohlog zeigt beim
# ersten Lauf ohnehin sofort das tatsaechliche Event-Format.
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

# Fuer den Plan-Flow braucht es (anders als bei assess) vermutlich keinen
# assessmentType. Namensschema: plan__<disciplineType>__<szenario>.json

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
    # (Scope=0, Constraints=null), was zu einem ganz anderen Fehler fuehren kann
    # als dem eigentlich gesuchten Live-Bug. Deshalb hier explizit mitschicken,
    # analog zur PlanConstraints-Struktur (WeeklyTssTarget, DayConstraints).
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

    # Diagnose: exakt sichern, was tatsaechlich rausgeschickt wird -- damit
    # sich "Daten kommen leer an" zweifelsfrei auf Client- oder Serverseite
    # verorten laesst.
    Write-Host "  weekDataJson: $($weekDataJson.Length) Zeichen" -ForegroundColor DarkGray
    Write-Host "  bodyJson gesamt: $($bodyJson.Length) Zeichen" -ForegroundColor DarkGray
    Set-Content -Path (Join-Path $resultsFolder "$baseName.request.json") -Value $bodyJson -Encoding utf8

    $planUri = "$($config.SiteUrl)$PlanEndpointPath"

    $rawLogPath   = Join-Path $resultsFolder "$baseName.raw.log"
    $mdPath       = Join-Path $resultsFolder "$baseName.md"
    $eventsJsonlPath = Join-Path $resultsFolder "$baseName.events.jsonl"

    $handler = New-Object System.Net.Http.HttpClientHandler
    $client  = New-Object System.Net.Http.HttpClient($handler)
    $client.Timeout = [System.TimeSpan]::FromMinutes(10)

    $request = New-Object System.Net.Http.HttpRequestMessage('Post', $planUri)
    $request.Headers.Add("X-Intervals-Athlete-Id", $config.IntervalsAthleteId)
    $request.Headers.Add("X-Intervals-Api-Key", $config.IntervalsApiKey)
    $request.Headers.Add("Origin", $config.SiteUrl)
    $request.Headers.Accept.Add([System.Net.Http.Headers.MediaTypeWithQualityHeaderValue]::new("text/event-stream"))
    $request.Content = New-Object System.Net.Http.StringContent($bodyJson, [System.Text.Encoding]::UTF8, "application/json")

    $rawLines = New-Object System.Collections.Generic.List[string]
    $parsedEvents = New-Object System.Collections.Generic.List[object]

    try {
        $response = $client.SendAsync($request, [System.Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()

        if ([int]$response.StatusCode -ge 400) {
            $errBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            Write-Host "Fehler ($([int]$response.StatusCode)): $errBody" -ForegroundColor Red
            Set-Content -Path $rawLogPath -Value $errBody -Encoding utf8
            Write-Host ""
            continue
        }

        $stream = $response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
        $reader = New-Object System.IO.StreamReader($stream)

        while (-not $reader.EndOfStream) {
            $line = $reader.ReadLine()
            if ($null -eq $line) { continue }
            $rawLines.Add($line)

            if ($line.StartsWith("data:")) {
                $jsonPart = $line.Substring(5).Trim()
                if ($jsonPart.Length -gt 0) {
                    try {
                        $evt = $jsonPart | ConvertFrom-Json
                        $parsedEvents.Add($evt)

                        # Best-effort Live-Anzeige -- Feldnamen sind Annahmen
                        # (stage/message/status), ggf. nach erstem Lauf anpassen.
                        $label = $null
                        foreach ($prop in @('stage', 'status', 'message', 'step')) {
                            if ($evt.PSObject.Properties.Name -contains $prop) {
                                $label = $evt.$prop
                                break
                            }
                        }
                        if ($label) {
                            Write-Host "  -> $label" -ForegroundColor DarkGray
                        }
                    }
                    catch {
                        Write-Host "  (raw, nicht JSON-parsbar): $jsonPart" -ForegroundColor DarkYellow
                    }
                }
            }
        }
        $reader.Dispose()
    }
    catch {
        Write-Host "Verbindungsfehler: $($_.Exception.Message)" -ForegroundColor Red
    }
    finally {
        $client.Dispose()
    }

    # Rohen Stream immer sichern -- das ist der wichtigste Output hier.
    Set-Content -Path $rawLogPath -Value ($rawLines -join "`n") -Encoding utf8
    if ($parsedEvents.Count -gt 0) {
        $parsedEvents | ForEach-Object { $_ | ConvertTo-Json -Depth 10 -Compress } | Set-Content -Path $eventsJsonlPath -Encoding utf8
    }

    $fullText = $rawLines -join "`n"
    $hasUploadJson = $fullText -match "BEGIN_UPLOAD_JSON" -and $fullText -match "END_UPLOAD_JSON"

    if ($hasUploadJson) {
        Write-Host "-> Upload-JSON-Block gefunden." -ForegroundColor Green
    }
    else {
        Write-Host "-> KEIN Upload-JSON-Block im Stream gefunden." -ForegroundColor Red
    }

    Write-Host "-> Rohlog: $rawLogPath" -ForegroundColor Green
    Write-Host ""
}

Write-Host "Ergebnisse liegen in: $resultsFolder" -ForegroundColor Cyan