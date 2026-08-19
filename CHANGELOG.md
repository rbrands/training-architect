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

## [1.4.0] - 2026-08-19

### Added
- Added the `assess_consistency` prompt to check athlete data for completeness and internal consistency before coaching assessments and plan generation.
- Added a Data Consistency assessment card with a link to [The Workflow · Training Architect](https://training-architect.com/blog/workflow).
- Metrics assessments now check whether the athlete's current power profile matches the demands of the selected discipline.

### Changed
- Assessment prompt evaluation support now includes the consistency assessment.

### Fixed
-

### Removed
-

## [1.3.1] - 2026-08-18

### Fixed
- Fixed prompt configuration to improve training plan generation accuracy.

## [1.3.0] - 2026-08-17

### Added
- Generated training plans are now verified against the weekly TSS target and automatically sent back for correction when the plan misses the target.
- Plan creation shows the outcome of the TSS verification directly in the progress list, including the planned weekly load, the target, and the deviation.

### Changed
- The weekly load target and the day constraints are now read from the current dataset structure, so the planning logic uses the authoritative weekly target of the target week.

### Fixed
- The last status message of a plan run is no longer swallowed, so the validation step stays visible after the plan is finished.

### Removed
-

## [1.2.0] - 2026-08-15

### Added
- Planning support now incorporates a significantly improved weekly planning logic to create more realistic and better-balanced training recommendations.
- Weekly Plan now shows the training phase, target load, and the week note when available in the athlete dataset.

### Changed
- The training-plan generation logic was refined to better prioritize week structure, load distribution, and overall training coherence.
- UI ordering of planed workouts improved

### Fixed
- Planning quality issues caused by the previous logic were addressed to reduce unrealistic scheduling patterns and improve the recommended plan flow.
- Weekly plan uploads no longer duplicate workout duration and load while preserving the visible workout steps in Intervals.
- Library workout uploads now preserve the stored workout structure and TSS instead of overriding them with generated plan values.

### Removed
-

## [1.1.1] - 2026-08-14

### Added
- Metrics Snapshot now includes a direct help link with a question-mark icon to the training-load article covering TSS, CTL, ATL and TSB.

### Changed
- The Metrics Snapshot header was adjusted to keep the explanatory article link visible without cluttering the KPI summary.

### Fixed
-

### Removed
-

## [1.1.0] - 2026-08-10

### Added
- Metrics Snapshot now includes a responsive five-week training-load chart that compares actual Ride TSS with weekly targets.
- The chart distinguishes the incomplete current week and provides an accessible text description of load and target values.

### Changed
- Recent training-load history is combined with the current week directly in the Metrics Snapshot for immediate coaching context.

### Fixed
-

### Removed
-

## [1.0.5] - 2026-08-08

### Added
- Metrics Snapshot is now rendered in all Coach assessment cards.
- Snapshot context labels were added to distinguish rule-based intervals.icu data from the AI Coach assessment text.

### Changed
- Metrics Snapshot presentation was consolidated into a reusable component with dedicated styling and consistent KPI rendering.
- Footer spacing and line behavior were optimized for iPhone screens, including tighter `Legal | Privacy` link spacing.

### Fixed
- Restored a clear light-blue card background for the Metrics Snapshot block after UI refactoring.

### Removed
-

## [1.0.4] - 2026-08-07

### Added
- Dedicated `/privacy` page covering the data controller, Azure and Cloudflare infrastructure, intervals.icu integration, AI processing, operational data, retention, legal bases, GDPR rights, and contact details.
- Explicit consent notice with a Privacy Notice link before connecting an intervals.icu account.
- Optional copyright link setting in the About editor.
- Privacy page link in the footer and `/sitemap.xml`.
- The curated athlete dataset now provides a direct copy button as soon as the data has been loaded.

### Changed
- Privacy disclosures now reflect the implemented browser storage, Cosmos DB usage counters, Application Insights telemetry, Azure processing regions, retention periods, and international data transfer safeguards.
- Legal Notice was updated from the TMG to § 5 DDG, outdated TMG liability references were removed, and duplicated privacy details were replaced with a link to the Privacy Notice.
- Legal editor now requests a complete service provider postal address, and owners are warned when required provider details are incomplete.
- Footer navigation now presents `Legal | Privacy`, and the copyright text becomes a link when a copyright URL is configured.

### Fixed
- Preserved spacing between the copyright year and copyright name when the name is rendered as a link.

### Removed
- Duplicated cookie, hosting, and AI privacy disclosures from the Legal Notice.

## [1.0.3] - 2026-08-06

### Added
- Dynamic `/sitemap.xml` generation for canonical public URLs and published article pages.
- `/robots.txt` now references the sitemap for search engine discovery.
- CTL/ATL/Foram now as KPI on coach page as part of metrics.
- Periodic global usage counter rebuild service (startup + every 5 minutes) was added.
- Manual global usage refresh action was added to Admin (`POST /api/admin/usage/refresh`) with a new button in the Usage tab.
- Admin Usage view now shows `Last global refresh` based on current global counter rows.

### Changed
- Coach token limit checks now use point reads for current monthly/weekly usage documents instead of broader history reads.
- Global usage counters are now rebuilt periodically into existing `global_month_*` and `global_week_*` documents (no new snapshot naming).
- Usage TTL is now type-specific: `weekly_usage` = 30 days, `monthly_usage` = 365 days.

### Fixed
-

### Removed
-

## [1.0.1] - 2026-08-05

### Added
-

### Changed
-

### Fixed
- Coaching flow now works with the `gpt-5.6-luna` Foundry model, including improved compatibility handling for unsupported generation parameters.
- Foundry agent responses now use stricter assistant-message extraction to avoid leaking tool/intermediate HTML content into Coach output.
- Coach endpoint error handling for agent calls now returns concise API problem responses instead of surfacing full exception stack traces.
- Model compatibility errors (for example unsupported `temperature` parameter) are normalized to clearer operator-facing messages.

### Removed
-

## [1.0.0] - 2026-08-04

### Added
- Athlete connect/read flow now returns athlete level metadata (`Level`, `LevelLabel`) for UI usage.
- Coach status now shows a level label badge when configured for the connected athlete.
- Admin athlete editor now includes an "Apply Level Default Limits" action to copy default limits from the selected level.

### Changed
- Admin token values are now rendered with thousands separators in `Usage`, `Levels`, and `Athletes` grids.
- Athlete level selection in Admin no longer offers the `global` level.
- Coach Assess cards were visually updated (icon-before-title layout) and section heading cleanup was applied.

### Fixed
- Existing locked athlete configs now reject connect/read requests with explicit lock messaging and HTTP 423.
- Added explicit structured warning logs for token limit rejections (`/api/coach/assess`, `/api/coach/plan`) to improve App Insights diagnostics.
- Server-side `IAthleteSession` stub now fully implements the extended contract (`Level`, `LevelLabel`).

### Removed
- "Standard Prompts" heading from the Coach page.

## [0.4.2] - 2026-08-03

### Added
-

### Changed
- "Plan Week" now better considers already existing training sessions.

### Fixed
-

### Removed
-

## [0.4.1] - 2026-08-02

### Added
- New `/api/dataset` endpoint that returns the parsed JSON payload from the athlete-data flow.
- Swagger/OpenAPI documentation for `/api/dataset` with a filtered document that exposes only the dataset endpoint.

### Changed
- Missing request headers now return explicit JSON 400 responses for the dataset endpoint.
- For planning: Prompt now accepts time remaining on constrained days

### Fixed
- Swagger UI no longer shows unrelated schemas and extra endpoint definitions.

### Removed
-

## [0.3.4] - 2026-07-31

### Added
- Token usage is now persisted per athlete in Cosmos DB as monthly and weekly usage counters.
- Global usage counters aggregate token consumption across all athletes.
- The `/admin` page shows a usage grid with filters for scope, usage type, athlete, period and action.

### Changed
- The usage grid now follows the light editorial theme instead of the dark Syncfusion default, including header, rows, selection and pager buttons.
- The usage grid page size was raised from 20 to 40 rows.

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
