Analyze the current metrics, including wellness data if available, and the
training week.

Please cover the following points:

**Metrics**
Summarize and assess:
- Current performance data
- Wellness data: sleep, HRV, resting heart rate, weight

**Load Balance**
- Assess target achievement and actual Ride load across the four completed
  weeks in `training_load_history`
- Distinguish a single-week outlier from a repeated over- or under-target pattern
- Compare the current week's Ride load (TSS, hours) with the recent pattern
- Development of CTL, ATL, and form (TSB)
- Assessment: Was the week overdone, appropriate, or too light?

**Training Quality**
- Which key ride sessions were completed?
- How was ride intensity distribution (Zone 1-2 vs. Zone 4-6)? Do not include
  `WeightTraining` in ride zones or intensity distribution.
- Is an important training stimulus missing?
- If `week_summary.aerobic_threshold_estimate` is present, report
  `power_at_0_75` and `hr_at_0_75`, with `rides_used` and
  `threshold_windows_total` as confidence context. Compare the estimate with
  the Zone 2 ceiling (`z2_upper_power`, `delta_pct`) and give a concrete
  pacing recommendation for this week's long aerobic rides: specify the
  wattage and heart-rate range to stay below LT1. Treat one ride as a day-form
  reading and three rides as a pattern. Do not recommend changing training
  zones based on this estimate alone. If the field is absent, skip this point
  entirely.

**Strength**
- Strength is in use when `week_summary.strength` shows any WeightTraining
  session in the last 16 weeks: the sum of `sessions_per_week_last_4` is greater
  than 0, `phase_session_count` is greater than 0, or
  `untagged_sessions_last_16_weeks` is greater than 0. An explicit athlete
  request for strength also makes it in use. If neither condition applies, skip
  this block and do not mention strength in the analysis.
- Report the current phase and session count versus the phase target, sessions
  this week versus the phase frequency rule, and placement relative to key
  sessions.
- Weekly strength frequency (D2): adaptation/transition/maximum twice per week;
  maintenance once per week, with at least two days between sessions. Readiness
  applies only to sessions scheduled today, not tomorrow's sessions, and never
  reduces the weekly strength frequency. For any session scheduled tomorrow,
  include one sentence advising a readiness re-check in the morning.
- If strength is in use and `current_phase` is null, name adaptation as a
  working assumption and add one sentence that tagging sessions or letting the
  coach plan strength enables phase tracking. This is not a warning.

**Fueling**
- Average carb intake per hour on rides
- Critical ride sessions with insufficient fueling
- Relationship between ride fueling and performance drops

**Limiter**
- What is the primary performance limiter this week?
- Does the limiter match the current season phase (base, peak, race)?
- Recommendation for focus areas in the coming week

Provide a clear summary with actionable recommendations.

{{athlete_data}}
