The plan you just produced did not pass the automated validation check. Fix the following
problems and return the corrected plan.

{{findings}}

How to fix each problem, depending on its kind:

- Requested strength phase for this plan: {{strength_phase}}. Include strength
  at the recommended frequency when it is due and the planning constraints
  allow it; do not silently omit an eligible strength session. Return at most
  one strength session and one main workout per calendar day.
  If the original plan omitted a strength session that is due and allowed,
  add it now. Do not use missing strength history as a reason to omit it.
  If there is no exact strength library match, use this prescription:
  {{strength_prescription}}
  - If it is `none`, remove every `WeightTraining` entry and do not mention
    strength in the plan.
  - Otherwise, every `WeightTraining` entry must carry exactly the canonical
    `strength-{{strength_phase}}` tag. Do not use a different phase tag.

- Weekly Ride total outside the load target or duration target: Adjust ride
  sessions only; TSS and duration comparisons use rides only. Do not add, remove,
  or resize a `WeightTraining` entry to fix a deviation. Do not modify any workout
  that came from planned_workouts — treat those as fixed and work around them.
  Prefer adjusting generated rides (no library_workout_id) first, typically by
  lengthening or shortening their Z2 blocks, warm-up, or cool-down. If the
  generated rides alone cannot close the gap realistically, replace one library
  workout of an easy domain (aerobic-threshold or recovery) with a generated ride
  of the same tag sized to fit; drop its library_workout_id and provide full
  steps. Never shorten or lengthen a library workout while keeping its
  library_workout_id.

- More than one ride, or more than one `WeightTraining` entry, on the same date:
  Keep at most one ride and at most one strength entry; one ride plus one
  strength entry is valid. Remove or merge duplicate rides; if their load or
  duration is still needed, adjust the remaining generated ride or move a ride
  to an available day without a ride. Never place a workout on a day with
  `training_allowed: false`.

- `WeightTraining` entry invalid: Fix its tag to exactly one canonical
  `strength-<phase>` tag, remove `steps` and `library_workout_id`, and set its
  duration within 30–75 minutes. Never convert it into a ride.

- In all cases, keep every workout realistic and consistent for its stated training purpose.
  Never distort an interval structure just to hit a TSS number. Never schedule
  more than one ride or more than one strength session on a date, and do not add
  new days beyond the available ones.

Return the complete corrected plan again in the exact same format as before: the readable
plan text, followed by the upload JSON between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.
After the readable plan text, output `BEGIN_UPLOAD_JSON` immediately. Do not insert an
"Upload JSON" heading, label, or numbered list item before the marker. The marker itself
starts the upload section; do not introduce or describe it in the prose.
Keep the plan text short: the context sentences (updated if a correction
changes a key decision) and the day-by-day list matching the corrected JSON.
Do not comment on the corrections themselves.

Your response MUST end with the complete upload JSON for ALL workouts of the
week (not only the corrected ones), enclosed between BEGIN_UPLOAD_JSON and
END_UPLOAD_JSON. A response without this block is invalid.