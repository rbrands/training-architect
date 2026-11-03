Summarize the current metrics and wellness data.

**Performance Metrics**
- FTP and eFTP (current value and trend). Use only `metrics.ftp`,
  `metrics.rolling_ftp`, `metrics.eftp`, and `metrics.ftp_eftp_delta_pct` for
  this. Ignore the per-activity `activity_eftp` values in `recent_activities`:
  they only reflect the best effort within that single session, so low values
  from endurance or recovery rides are expected and are **not** evidence of a
  fitness change or of a power-meter/trainer calibration problem. Never
  speculate about equipment or sensor discrepancies based on them. Do not use
  `WeightTraining` activity values as cycling-performance evidence.
- VO2max (current estimate and trend), using
  `metrics.vo2max_classification` and the estimate-quality rules below.
- W' (anaerobic capacity)
- CTL (fitness), ATL (fatigue), form (TSB) — absolute and as %
- Rider type from `metrics.power_profile` based on `type`, `type_key`, `heuristic_score`, `type_scores`, and `type_method` (heuristic classification; explicitly mention uncertainty when scores are close)

VO2max estimate quality:
- Read `metrics.vo2max_classification.estimate_representative`. Report the
  value and category as estimates, not as measured physiological capacity.
- When `estimate_representative` is `false`, explicitly explain in the VO2max
  bullet that the best five-minute power does not reflect a maximal effort,
  so the derived VO2max and category are not representative and are likely
  understated. Do not stop at "not representative" or present a category such
  as "excellent" as a reliable current fitness classification. Recommend a
  maximal five-minute test or a hard VO2max session to update the estimate.
- When `estimate_representative` is `"likely_understated"`, explain that recent
  data probably lacks a maximal five-minute effort, so the derived VO2max and
  category may be understated. Suggest a fresh five-minute effort to reassess.
- For either flagged status, include `p5min_pct_ftp` as the best five-minute
  power's percentage of FTP and `power_profile_period_days` as the observation
  window when present and non-null. These fields are inside
  `metrics.vo2max_classification`. Omit missing context; never invent it.
  The observation window is not the age of the estimate: do not claim it is
  chronologically outdated without an explicit supporting date.
- When the status is `true`, do not add a quality warning. When it is null or
  missing, representativeness is unknown; do not infer either a representative
  effort or an understated estimate. Never infer a VO2max decline or improvement
  from a single estimate or from the quality flag alone.

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
When the VO2max estimate is flagged as non-representative or likely understated,
also qualify any inferred 3-5-minute power gap: it may reflect missing maximal
efforts rather than a confirmed physiological weakness. Do not call it a
confirmed limitation or prescribe extra VO2max training solely from that gap.

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
- For strength recommendations, readiness applies only to today/tomorrow and
  never reduces the weekly strength frequency (D2): adaptation/transition/maximum
  twice per week; maintenance once per week, with at least two days between
  sessions. Adjust or defer the immediate session if needed, not the weekly
  frequency.

Keep the summary compact and action-oriented.
...

{{athlete_data}}