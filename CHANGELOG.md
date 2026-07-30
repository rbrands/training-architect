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

## [0.3.3] - 2026-07-30

### Added
-

### Changed
- Feedback evaluation tags were expanded to support more granular negative feedback categorization.

### Fixed
-

### Removed
-

## [0.3.2] - 2026-07-26

### Added
- Coach feedback loop was added for Assess and Plan results, including thumbs up/down feedback capture and negative tag selection.
- Feedback telemetry is now sent through `/api/feedback` and recorded in Application Insights with correlated response IDs.
- Token usage is now surfaced directly in Coach Assess and Plan cards.

### Changed
- Coach token usage text was visually de-emphasized in Assess cards and aligned with existing text color in the Plan card.

### Fixed
- `/coach` route startup no longer fails due to missing service resolution in server-side route handling.

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

## [0.3.0] - 2026-07-19

### Added
- Full coaching planning workflow is now available as a major feature block.
- Weekly plan generation from the Coach page was added.
- Upload of generated plan workouts to intervals.icu was added.
- Editing planned workout days before upload was added.
- Athlete scheduling preference text is now used for plan generation.
- Automatic planning scope selection (`CurrentWeek` vs `NextWeek`) based on current weekday was added.
- New planning prompt for next-week plan generation was added.
- Coach page now persists Assess and Plan UI state across in-app navigation.

### Changed
- Coach plan UI was extended with tabbed plan output handling and better plan adaptation flow.
- Coach API hardening was improved with scoped CORS policy and stricter request validation (HTTPS, JSON-only, payload size bound).
- Coach API rate limiting now uses a proxy-aware client key strategy instead of a simple IP-only limiter.

### Fixed
- Plan-related tab layout and interaction behavior in the Coach page were improved.
- Navigating away from the Coach page (for example to Blog) and back no longer drops existing Assess and Plan results.

### Removed
- Simple IP-only partitioning for coach endpoint rate limiting.

## [0.2.3] - 2026-07-12

### Added
- Expandable Overview sections in the Coach data modal for `Header Data`, `Metrics`, `Week Summary`, `Activities`, `Fueling Analysis`, and `Planned Workouts`.

### Changed
- Coach athlete-data flow now remains JSON-only end-to-end for parsed/raw payload handling.
- Overview tab labeling and presentation were streamlined (`Overview` tab title only, redundant in-panel heading removed).

### Fixed
- Removed tight coupling to typed intervals sync payload models in the runtime data path.
- Blog overview card header images now preserve the full image composition instead of cropping the sides aggressively.

### Removed
- Typed `DataDeserialized` contract usage and generated `WeekDataDto` model from the active coach data flow.

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
