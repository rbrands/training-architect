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
- Compare the current week's load (TSS, hours) with the recent pattern
- Development of CTL, ATL, and form (TSB)
- Assessment: Was the week overdone, appropriate, or too light?

**Training Quality**
- Which key sessions were completed?
- How was intensity distribution (Zone 1-2 vs. Zone 4-6)?
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

**Fueling**
- Average carb intake per hour
- Critical sessions with insufficient fueling
- Relationship between fueling and performance drops

**Limiter**
- What is the primary performance limiter this week?
- Does the limiter match the current season phase (base, peak, race)?
- Recommendation for focus areas in the coming week

Provide a clear summary with actionable recommendations.

{{athlete_data}}
