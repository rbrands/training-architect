Summarize the current metrics and wellness data.

**Performance Metrics**
- FTP and eFTP (current value and trend). Use only `metrics.ftp`,
  `metrics.rolling_ftp`, `metrics.eftp`, and `metrics.ftp_eftp_delta_pct` for
  this. Ignore the per-activity `activity_eftp` values in `recent_activities`:
  they only reflect the best effort within that single session, so low values
  from endurance or recovery rides are expected and are **not** evidence of a
  fitness change or of a power-meter/trainer calibration problem. Never
  speculate about equipment or sensor discrepancies based on them.
- VO2max (current value and trend)
- W' (anaerobic capacity)
- CTL (fitness), ATL (fatigue), form (TSB) — absolute and as %
- Rider type from `metrics.power_profile` based on `type`, `type_key`, `heuristic_score`, `type_scores`, and `type_method` (heuristic classification; explicitly mention uncertainty when scores are close)

Use this fixed rider-type output template:
`Power profile: <type> (<confidence_level>, similarity <heuristic_score>). <Short interpretation in 1-2 sentences based on p15s/p30s/p1min/p3min/p5min/p20min and curve_slope>.`

Confidence level rule:
- `high` when `heuristic_score >= 0.45`
- `moderate` when `heuristic_score >= 0.30` and `< 0.45`
- `low` when `< 0.30`

Discipline fit:
Compare the current power profile (rider type and w_per_kg across
p15s/p30s/p1min/p3min/p5min/p20min) against the priorities of the athlete's
discipline (`{{discipline}}`, as defined in the system prompt above). In 1-2
sentences: name the discipline demand the current profile already covers
well, and name the single biggest gap between the current profile and what
the discipline needs most. Do not repeat the rider-type paragraph above; add
only the discipline-specific comparison.

**Wellness**
- HRV: current value and trend (last 7 days)
- Resting heart rate: current value and trend
- Sleep: quality and duration (if available)
- Weight: current value and trend

**Assessment**
- What is the current form state (fresh / transition / optimal / high risk)?
- Are there any anomalies in the wellness data indicating overload or insufficient recovery?
- Recommendation for the **current week**: Can training load be increased
  this week, or is recovery the priority?

Week context: the week entries live in `week_summary.training_plan`. The
current week is the entry whose `week` equals `week_starting`; next week is
the entry whose `week` equals `week_starting` + 7 days. Today is
`current_date`, so the current-week recommendation covers the remaining days
of this week.

- Always address the current week first. Use `phase`, `week_type`,
  `weekly_load_target`, `weekly_time_target_hours`, and `day_constraints` of
  the current-week entry together with `week_summary.training_readiness`.
  Mention days with `training_allowed: false` or LIMITED constraints, as they
  reduce the available training days this week.
- If the current-week entry has no `weekly_load_target` or
  `weekly_time_target_hours`, say so explicitly and base the current-week
  recommendation on readiness, form, and the recent load pattern. Never present
  next week's target as this week's target.
- Outlook for next week: only if a next-week entry exists, add one sentence on
  its `phase`, `week_type`, and targets and whether the current wellness state
  supports them. Label it clearly as next week.

Keep the summary compact and action-oriented.
...

{{athlete_data}}