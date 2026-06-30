# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added
-

### Changed
-

### Fixed
-

### Removed
-

---

## Release Template

Copy this section for each new release and place it below `Unreleased`.

```markdown
## [x.y.z] - YYYY-MM-DD

### Added
-

### Changed
-

### Fixed
-

### Removed
-
```

## [0.2.0] - 2026-06-30

### Added
- Foundry Agent integration is now active in the Coach workflow for assessment requests.
- Coach can now return actionable assessment responses directly from the Foundry Agent.

### Changed
- Coach assessment result rendering now uses Markdown (Markdig) in the client UI.

### Fixed
- Assessments now complete successfully in the Coach flow and no longer fail in normal usage.

### Removed
-

## [0.1.0] - 2026-06-28

### Added
- MCP-based athlete-data retrieval via `/api/athlete-data` using HTTP transport.
- Strict header-based credential forwarding (`X-Intervals-Athlete-Id`, `X-Intervals-Api-Key`).
- Normalized data contract with `DataRaw`, `DataParsed`, and typed `DataDeserialized` (`WeekDataDto`).
- Coach page data modal with copy/download actions and refresh support.
- Data presentation tabs for deserialized, parsed, and raw JSON using Syncfusion Tab.
- Central application version model in Core and version display in the app footer.

### Changed
- MCP endpoint configuration is now centralized via `Mcp:AthleteData:Endpoint` and propagated through setup/deployment files.
- Coach data UI now uses tab pages instead of stacked sections for payload views.
