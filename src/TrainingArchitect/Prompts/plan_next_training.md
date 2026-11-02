Use only the attached athlete data to recommend the next best training session
for the recommendation date {{recommendation_date}}. The only tool you may call
is `list_library_workouts` (see below).
Do not ask follow-up questions, even if the attached data is incomplete or
outdated; decide based on the available data and explicitly state any limitations.

The recommendation date is determined by the application: it is today's
`current_date` if no activity has been completed today, otherwise the next day.
Do not derive it yourself.
- If {{recommendation_date}} equals `current_date`, today's
  `training_readiness` and wellness values ARE the measured readiness for the
  recommendation date. Use them directly; never call them unavailable or
  non-authoritative.
- Only if {{recommendation_date}} is later than `current_date`,
  mark the recommendation as preliminary, because that day's sleep and HRV are
  not yet available (re-check in the morning); today's readiness is then a proxy.

Take the following into account:

**Starting point**
- Training readiness (`week_summary.training_readiness`): traffic light, score, and reasons
- CTL/ATL/TSB and the load of the last 48-72 hours (last hard session?)
- Wellness: sleep, HRV, resting heart rate - only if available
- Strength: current phase, sessions this week versus the phase frequency rule,
  and days since the last session from `week_summary.strength`. Strength is in
  use when `week_summary.strength` shows any WeightTraining session in the last
  16 weeks: the sum of `sessions_per_week_last_4` is greater than 0,
  `phase_session_count` is greater than 0, or
  `untagged_sessions_last_16_weeks` is greater than 0. An explicit athlete
  request for strength also makes it in use. Otherwise, do not plan or mention
  strength. When strength is in use and `current_phase` is null, use adaptation
  as a working assumption and say so.

**Plan context**
- Current phase (Base/Build/Peak/Transition); if no phase is provided, apply Base.
- Weekly ride targets (`weekly_load_target` or time target)
- Ride TSS and hours completed so far versus the remaining ride budget for the
  remaining days; strength never counts toward either weekly ride target
- Today's planned workout from the calendar (`planned_workouts` for the
  recommendation date, if any) - does it still fit? See the rule below.
- When strength is in use (D7) and due today (`sessions_this_week` is below the
  phase's weekly frequency, the last strength session was at least two days
  ago, and placement rules allow it), you may recommend a ride plus one
  `WeightTraining` entry on the same date, or the strength entry alone when no
  ride is advisable. Do not place strength on a VO2max, long-ride, or race day.
  During maximum strength, also do not place it the day before a planned VO2max
  or long ride in `planned_workouts`.
- Readiness applies fully: yellow readiness rules out `strength-maximum` today;
  red readiness rules out all strength today.
  These restrictions apply only to today/tomorrow (the recommendation date),
  with tomorrow's readiness re-checked in the morning. Readiness never reduces
  the weekly strength frequency (D2): adaptation/transition/maximum twice per
  week; maintenance once per week, with at least two days between sessions.
  Adjust or defer the immediate session if needed, not the weekly frequency.
- The combined ride and strength duration, plus workouts already completed
  that day, must not exceed `max_training_time_hours`.
- Upcoming races (RACE_A/B/C) and day constraints (Sick/Travel)
- Use the entry in `week_summary.training_plan` matching the recommendation date's
  week, including when tomorrow is in a new week. Never use another week's target.
- Respect `training_allowed: false` absolutely; LIMITED days must respect
  `max_training_time_hours` and allow only short, easy training.
- An absent `weekly_load_target` is a valid, expected state (no season plan or
  no target configured) - it is NOT missing key data and never justifies a
  rest day on its own. Do not invent a target and do not compute a remaining
  budget. Size the session from `ctl` instead: a daily load near CTL maintains
  fitness; use roughly 0.5-1.0 x CTL for an easy/aerobic day and up to about
  1.5 x CTL for a key session, bounded by phase, readiness, and day constraints.
  `training_load_history` is secondary context only. If no phase is provided,
  apply Base.

**Already planned workout on the recommendation date**
- If it still fits readiness, form, budget, and constraints: confirm it in prose
  (name, duration, TSS, one reason) and do not emit upload JSON. It is already
  in the calendar; uploading again would create a duplicate.
- If it no longer fits (for example readiness red, hard session too soon after
  the last one, exceeds the remaining budget, violates a day constraint):
  recommend a replacement with upload JSON as described below. In the prose,
  name the planned workout that should be replaced and state that the athlete
  must delete it from the calendar before uploading. Never silently keep both.
- A race event on that date is never replaced; recommend around it in prose only.

**Gaps in the week**
- Which stimulus is still missing (VO2max, threshold, base, recovery)? Use the activity tags.
- Fueling pattern of the recent sessions

**Remaining budget and rest-day rules**
- If a `weekly_load_target` exists, compute the remaining Ride budget:
  `weekly_load_target` minus `week_summary.ride_load` for the matching week,
  divided by the remaining days including
  the recommendation date. Only activities dated within that week count against
  its budget; older lookback activities belong to the previous week. Strength
  and run load do not count toward the Ride target or remaining budget.
- A RECOVERY week means reduced weekly load, not zero load. The remaining budget
  still has to be spent; the default is an aerobic/endurance session near the
  daily budget, optionally with a short low-dose quality touch to keep sharpness.
  Avoid high-dose VO2max/threshold work in a RECOVERY week.
- `fueling_form_analysis` text describes fueling and total weekly load. Do not
  treat it as an instruction for today's session.
- `is_hard_session` is a coarse flag (Z3+Z4 >= 20%). Judge real stress by
  `training_load`, `rpe`, `norm_power` vs FTP, and the activity notes, not by
  the flag alone.
- Readiness red or a safety veto means "no hard training", not "no training".
  Read the `training_readiness.recommendation` text and the individual
  `signals`: mild deviations (for example HRV a few percent below its 7-day
  average with a stable trend, or average-quality sleep of 7+ hours) rule out
  Z4+ and high-dose work but still allow a short, easy Z1/Z2 session
  (`recovery-low` or `aerobic-threshold-low`, typically 45-75 min). Prefer that
  easy session over rest when form is positive and there was no hard session
  in the last 48 hours. Only in this degraded-readiness case, add one short
  sentence that the athlete should stop if feeling unwell.
- Recommend a full rest day only if at least one applies: `training_allowed`
  is false; readiness is red or has a safety veto AND form is negative or
  yesterday was a hard session; the signals or notes indicate illness, injury,
  or strongly depressed HRV / clearly elevated resting HR (not a few percent);
  a `weekly_load_target` exists and the remaining budget is near zero; or
  essential metrics (`ctl`/`atl` and all wellness data) are missing entirely.
  If readiness is green with positive form and budget remains, recommend a
  session, not rest - even in a RECOVERY week.

Key data is missing only when `training_readiness` is absent or "Unknown" AND
no wellness values are available, or when `ctl`/`atl` are absent. An absent
weekly target, phase, or planned workout is not missing key data. If key data
is truly missing, state this explicitly and recommend conservatively (an easy
Z1/Z2 session unless a rest-day rule applies) instead of asking.

Evaluate the plan, week target, and constraints of {{recommendation_date}}.

**Recommendation and output**
Use the plan/workout generation output format and upload JSON markers defined in
the system prompt, but for one main workout with an optional same-day strength
entry:
- At most 100 words of prose: date, session type, duration, target zone, TSS and
  carbs/h for a ride; if strength is included, give it one sentence with the
  phase and informational TSS. Add one short reason and an easier alternative
  with a concrete downgrade criterion (for example shorten the ride or drop the
  sprints when fatigued).
  No separate analysis or metrics table. Do not add generic safety disclaimers
  such as "stop if feeling unwell" when readiness is green; they are
  self-evident and only allowed in the degraded-readiness case above.
- Then emit upload JSON for the main recommendation, optionally with one
  same-date `WeightTraining` entry, using the system prompt's existing schema,
  BEGIN_UPLOAD_JSON / END_UPLOAD_JSON markers, and ride TSS rules. The easier
  alternative stays in prose only and is never a second upload entry. Do not add
  other calendar workouts.
- For a ride, include session goal, TSS determined per the TSS Calculation
  rules (system prompt), and fueling guidance in the description; the `tss`
  field must match. A `WeightTraining` TSS is informational only and never
  counts toward the Ride budget.
- Your output ends with the upload JSON. Validation and upload are performed by
  the application afterwards. Do not describe or simulate an upload and do not
  ask for upload confirmation.

Workout library lookup (REQUIRED because `list_library_workouts` is configured):
1. Determine the session and its full canonical tag first.
2. Before creating any generated workout steps, call `list_library_workouts`
  exactly once with that tag, `match_mode="any"`, `include_untagged=false`,
  and `limit=100`.
3. Use a result only when its exact workout tag and dose/strength phase fit the planned
  session. Prefer the closest duration and TSS as soft ranking criteria; neither
  value needs to equal the planned value. Day constraints are not library
  matching criteria. If an exact tag-and-dose/phase match exists, use it instead
  of generating a replacement. For a ride, preserve its `library_workout_id`,
  name, duration, description, tags, and TSS verbatim. For strength, preserve
  the workout prescription, name, duration, description, tags, and TSS verbatim
  but omit `library_workout_id`. In the upload JSON, omit `steps` for a selected
  library workout as required by the system output contract; the stored library
  workout is authoritative during upload.
  Include a strength tag in the call like any other tag. Use a strength library
  hit as `activity_type: "WeightTraining"` with no `steps`.
4. If there is no matching workout, generate it normally without
  `library_workout_id`. Do not broaden or repeat the search. Never skip the
  library call merely because a generated structure is easy to create.

Workout structure and realism rules (CRITICAL):
- Use the athlete context and training-phase goals to select a concrete
  interval structure (high/moderate/low dose) instead of an ad-hoc continuous
  maximal block.
- Dose, structure, and tag mapping must follow the knowledge source of truth
  (`workout-library.md`, `decision-process.md`, `training-zones.md`,
  `interpretation-rules.md`) without re-defining it here.
- Use training zones (Z1-Z7) consistently in rationale/description and tags;
  ensure `power_pct_ftp` values in steps map to the same intended zones from
  `training-zones.md`.
- Determine dose level from the actual main set structure first, then assign
  the tag. Never choose the tag first and back-fit the structure.
- Keep the prescription physiologically plausible for amateurs:
    - VO2max (Z5): short, repeat-based main set (for example 30 s to 5 min
      reps). No continuous or near-continuous VO2 blocks.
    - Threshold (Z4): block-based structures with recoveries (for example 3x8
      to 3x12 min, or 2x20 min), not uninterrupted maximal efforts.
    - Long aerobic rides: mostly steady Z2 with controlled progression, not
      prolonged high-intensity drift.
- For every workout domain (`vo2max`, `lactate-threshold`, `aerobic-threshold`,
  `race-specific`, `recovery`), derive dose (`high` / `moderate` / `low`) from
  the structure rules in `workout-library.md` and the attached interpretation
  rules. Strength uses the canonical phase tags from the strength table, not a
  dose.
- Never use `moderate` as a safe default. Use `moderate` only when the
  structure clearly falls into the moderate band; otherwise choose `high` or
  `low` as appropriate.
- If a structure is on the boundary between two bands, prefer the higher dose
  tag only when the main set clearly matches the higher band; otherwise reduce
  the structure to fit the tag.
- Examples that must not be tagged `moderate`: `5x3 min` VO2max, `2-4 h`
  steady aerobic-threshold ride.
- If selected structure and selected dose tag conflict, correct the tag (or
  structure) before returning output.
- Ensure internal consistency: interval structure, zone wording, training goal,
  tags, and `steps` must match each other.
- Tag format must follow `<domain>-<level>` and use canonical domains only:
  `vo2max`, `lactate-threshold`, `aerobic-threshold`, `race-specific`,
  `recovery`, and `strength`. Strength is the only domain without
  `low`/`moderate`/`high`; its levels are `adaptation`, `transition`,
  `maximum`, and `maintenance`.

Workout construction quality gate (CRITICAL):
- For rides, build `steps` explicitly as warmup -> main set -> cooldown with concrete
  durations for every interval and recovery segment.
- `WeightTraining` entries have `activity_type: "WeightTraining"`, no `steps`,
  no `library_workout_id`, exactly one `strength-<phase>` tag, and a duration
  from the strength table in `workout-library.md`. Their description follows
  its template: phase, session number of target, exercises, sets × reps @ % 1RM,
  rest, and progression note.
- TSS and step-consistency rules apply to ride entries only.
- Do not use compressed repetition notation inside `steps` (no implicit loops).
  Repetitions must be fully expanded as explicit step entries.
- If the description states a structure such as `N x M min` with `R min rec`,
  the main set in `steps` must contain exactly `N` work intervals of `M`
  minutes and the corresponding recovery intervals.
- For rides, `duration_minutes` must match the total step duration (sum of
  `duration_seconds`) within +/- 1 minute.
- For a generated ride, `description` must contain coaching context, session
  goal, TSS, and fueling guidance only. Do not include Intervals workout syntax,
  step lists, or repeat blocks in this field.
- For a generated ride, `steps` are the single source of truth for the
  workout structure. The uploader converts them to the format required by
  Intervals.
- For a selected library workout, preserve the library result verbatim as
  required above; the generated-workout description and steps rules do not
  override library data.
- Described key set and actual key set must be identical. Never describe
  `5x2 min` and then encode a different main set.

Do not run iterative self-checks or re-verification loops. Build a ride once,
consistently (repetition count, work/recovery durations, total duration, zone
intent vs `power_pct_ftp`), and state ride TSS once per the TSS Calculation
rules, computed from the final `steps` in a single pass. A strength entry has no
steps and uses informational TSS. No correction round
follows for this request, so get the workout structure right the first time.
Never place the workout on a `training_allowed: false` day, and respect
`max_training_time_hours` on a LIMITED day, including completed sessions that
day. There is no weekly load total check here; evaluate the main workout and
any same-day strength entry together.

If rest is recommended or essential workout data is missing, explain briefly
instead of inventing a workout. Follow the system prompt's prose-only rule when
no workout is created: no upload JSON or empty upload markers.

Attached athlete data:
<athlete_data>
{{athlete_data}}
</athlete_data>
