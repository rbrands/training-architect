Use only the attached athlete data to recommend the next best training session
for the recommendation date {{recommendation_date}}. The only tool you may call
is `list_library_workouts` (see below).
Do not ask follow-up questions, even if the attached data is incomplete or
outdated; decide based on the available data and explicitly state any limitations.

The recommendation date is determined by the application: it is today's
`current_date` if no activity has been completed today, otherwise the next day.
Do not derive it yourself. If {{recommendation_date}} is later than
`current_date`, mark the recommendation as preliminary, because that day's sleep
and HRV are not yet available (re-check in the morning).

Take the following into account:

**Starting point**
- Training readiness (`week_summary.training_readiness`): traffic light, score, and reasons
- CTL/ATL/TSB and the load of the last 48-72 hours (last hard session?)
- Wellness: sleep, HRV, resting heart rate - only if available

**Plan context**
- Current phase (Base/Build/Peak/Transition); if no phase is provided, apply Base.
- Weekly target (`weekly_load_target` or time target)
- Weekly TSS completed so far vs. remaining budget for the remaining days
- Today's planned workout from the calendar (`planned_workouts` for the
  recommendation date, if any) - does it still fit? See the rule below.
- Upcoming races (RACE_A/B/C) and day constraints (Sick/Travel)
- Use the entry in `week_summary.training_plan` matching the recommendation date's
  week, including when tomorrow is in a new week. Never use another week's target.
- Respect `training_allowed: false` absolutely; LIMITED days must respect
  `max_training_time_hours` and allow only short, easy training.
- An absent weekly target is valid: do not invent one. If no phase is provided,
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
- Compute the remaining budget: `weekly_load_target` minus `total_training_load`
  of the matching `training_plan` week, divided by the remaining days including
  the recommendation date. Only activities dated within that week count against
  its budget; older lookback activities belong to the previous week.
- A RECOVERY week means reduced weekly load, not zero load. The remaining budget
  still has to be spent; the default is an aerobic/endurance session near the
  daily budget, optionally with a short low-dose quality touch to keep sharpness.
  Avoid high-dose VO2max/threshold work in a RECOVERY week.
- `fueling_form_analysis` text describes fueling and total weekly load. Do not
  treat it as an instruction for today's session.
- `is_hard_session` is a coarse flag (Z3+Z4 >= 20%). Judge real stress by
  `training_load`, `rpe`, `norm_power` vs FTP, and the activity notes, not by
  the flag alone.
- Recommend a rest day only if at least one applies: readiness is red or has a
  safety veto; `training_allowed` is false; yesterday was a hard session and
  form is negative; the remaining budget is near zero; or key data is missing.
  If readiness is green with positive form and budget remains, recommend a
  session, not rest - even in a RECOVERY week.

If readiness is "Unknown" or key data is missing, state this explicitly and
recommend conservatively instead of asking.

Evaluate the plan, week target, and constraints of {{recommendation_date}}.
Do not treat today's readiness as the recommendation date's measured readiness.

**Recommendation and output**
Use the plan/workout generation output format and upload JSON markers defined in
the system prompt, but for exactly one workout:
- At most 100 words of prose: date, session type, duration, target zone, TSS and
  carbs/h, one short reason, and an easier alternative with a downgrade/stop criterion.
  No separate analysis or metrics table.
- Then emit the upload JSON for the main recommendation using the system prompt's
  existing schema, BEGIN_UPLOAD_JSON / END_UPLOAD_JSON markers, and TSS rules.
  The alternative stays in prose only. Do not add other calendar workouts.
- Include session goal, the TSS determined per the TSS Calculation rules (system
  prompt), and fueling guidance in the workout description. The `tss` field
  must carry the same determined value.
- Your output ends with the upload JSON. Validation and upload are performed by
  the application afterwards. Do not describe or simulate an upload and do not
  ask for upload confirmation.

Workout library lookup (REQUIRED because `list_library_workouts` is configured):
1. Determine the session and its full canonical tag first.
2. Before creating any generated workout steps, call `list_library_workouts`
  exactly once with that tag, `match_mode="any"`, `include_untagged=false`,
  and `limit=100`.
3. Use a result only when its exact workout tag and dose fit the planned
  session. Prefer the closest duration and TSS as soft ranking criteria; neither
  value needs to equal the planned value. Day constraints are not library
  matching criteria. If an exact tag-and-dose match exists, use it instead of
  generating a replacement. Preserve its `library_workout_id`, name, duration,
  description, tags, and TSS verbatim. In the upload JSON, omit `steps` for a
  selected library workout as required by the system output contract; the
  stored library workout is authoritative during upload.
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
  rules.
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
  `recovery`.

Workout construction quality gate (CRITICAL):
- Build `steps` explicitly as warmup -> main set -> cooldown with concrete
  durations for every interval and recovery segment.
- Do not use compressed repetition notation inside `steps` (no implicit loops).
  Repetitions must be fully expanded as explicit step entries.
- If the description states a structure such as `N x M min` with `R min rec`,
  the main set in `steps` must contain exactly `N` work intervals of `M`
  minutes and the corresponding recovery intervals.
- `duration_minutes` must match the total step duration (sum of
  `duration_seconds`) within +/- 1 minute.
- For a generated workout, `description` must contain coaching context, session
  goal, TSS, and fueling guidance only. Do not include Intervals workout syntax,
  step lists, or repeat blocks in this field.
- For a generated workout, `steps` are the single source of truth for the
  workout structure. The uploader converts them to the format required by
  Intervals.
- For a selected library workout, preserve the library result verbatim as
  required above; the generated-workout description and steps rules do not
  override library data.
- Described key set and actual key set must be identical. Never describe
  `5x2 min` and then encode a different main set.

Do not run iterative self-checks or re-verification loops. Build the workout
once, consistently (repetition count, work/recovery durations, total duration,
zone intent vs `power_pct_ftp`), and state TSS once per the TSS Calculation
rules, computed from the final `steps` in a single pass. No correction round
follows for this request, so get the workout structure right the first time.
Never place the workout on a `training_allowed: false` day, and respect
`max_training_time_hours` on a LIMITED day. There is no weekly load total
check here; only the single session is evaluated.

If rest is recommended or essential workout data is missing, explain briefly
instead of inventing a workout. Follow the system prompt's prose-only rule when
no workout is created: no upload JSON or empty upload markers.

Attached athlete data:
<athlete_data>
{{athlete_data}}
</athlete_data>
