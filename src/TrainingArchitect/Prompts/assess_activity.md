Analyse ONLY the most recent activity in the data. Use summaries and planned
workouts as context, not as additional activities to assess.

## Strength activity

If the most recent activity has `type: "WeightTraining"`, answer ONLY in this
short strength format (at most 80 words), instead of the five questions below:

a) Classification: use `strength_phase` and `strength_phase_source` from the
   latest entry in `activities[]`, not the summary's `current_phase` or the
   presence of tags. Use `week_summary.strength` for `phase_session_count` and
   `phase_target_sessions`; do not increment the exported count.
   - Source `activity`, `planned`, or `planned_week`:
     "<Phase>, session n of <target>".
   - Source `inferred`: "<Phase>, session n of <target> (inferred, no tag)".
   - Source `none`: "phase unknown, not counted".
   Do not mention tagging otherwise; the data quality check handles the reminder.
b) RPE and duration: one sentence, only if `rpe` is present and non-null.
   Never ask for missing RPE.
c) Next 48-72 hours: check `planned_workouts` for concrete conflicts, especially
   a hard ride or race tomorrow after eccentric work or a maximum session.
   Do not assume eccentric work when it is not recorded. State the next
   strength session per the frequency rule (D2): adaptation/transition/maximum
   twice per week; maintenance once per week. Readiness applies only to
   today/tomorrow and never reduces the weekly strength frequency. Adjust or
   defer the immediate session if needed, not the weekly frequency.
   Recovery weeks allow at most one, never maximum; race weeks allow
   maintenance only, not within three days before the race. Leave at least two
   days between strength sessions and avoid maximum before a key ride.

Do not discuss fueling, pacing, decoupling, W', cycling zones, power-based
training quality, or CTL/ATL/form for strength sessions. Do not treat absent
ride-specific metrics as zero. Stop here for WeightTraining.

## Other activities

For activities other than WeightTraining, answer the following questions:
1. What was the training quality (VO2max stimulus, threshold stimulus, base)?
2. How was load management (pacing, heart rate decoupling, W' usage)?
   If the activity has a `dfa_a1_summary`, include the in-ride aerobic reading:
   share of time above DFA a1 = 0.75 (was the easy ride really aerobic?),
   `power_at_0_75`, `hr_at_0_75`, and `power_at_0_75_pct_ftp`, compared with
   the athlete's Zone 2 ceiling (`metrics.power_zones_pct[1]`, default 75 % FTP).
   Include `threshold_windows` as confidence context. Treat this as a day-form
   observation, not a zone verdict. If the field is absent, say nothing about
   DFA a1; hard sessions are not analyzed by design.
3. What does the fueling tell us — carbs per hour relative to intensity?
4. What is the CTL, ATL, and form trajectory since the previous exported activity?
   `ctl_delta` and `atl_delta` are differences to the previous exported activity
   and include rest-day decay; never describe them as the effect of this session
   alone.
5. What do I take away for the next 48–72 hours (recovery, next session)?

Keep the response structured and focused on the essentials.

{{athlete_data}}
