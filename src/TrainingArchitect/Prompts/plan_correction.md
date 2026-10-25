The plan you just produced did not pass the automated validation check. Fix the following
problems and return the corrected plan.

{{findings}}

How to fix each problem, depending on its kind:

- TSS mismatch on a specific workout (stated vs. computed TSS differ): This means the
  workout structure is fine but the "tss" value in its description is wrong. Recompute TSS
  from the workout's actual steps using the TSS formula from your instructions, and correct
  only the stated value. Do not change the workout's structure just to produce a different
  number.

- Weekly total outside the load target or the weekly duration target: Adjust duration
  and/or intensity of non-key sessions (easy/aerobic/recovery rides) to close the gap. Do
  not modify any workout that came from planned_workouts — treat those as fixed and work
  around them. Prefer adjusting generated sessions (no library_workout_id) first, typically
  by lengthening or shortening their Z2 blocks, warm-up, or cool-down. If the generated
  sessions alone cannot close the gap realistically, replace one library workout of an easy
  domain (aerobic-threshold or recovery) with a generated session of the same tag sized to
  fit; drop its library_workout_id and provide full steps. Never shorten or lengthen a
  library workout while keeping its library_workout_id.

- More than one workout on the same date: Keep exactly one workout per calendar day.
  Remove or merge the extra session; if its load or duration is still needed, fold it into
  the remaining session on that day (for generated sessions) or move it to an available day
  that has no workout yet. Never place a workout on a day with `training_allowed: false`.

- In all cases, keep every workout realistic and consistent for its stated training purpose.
  Never distort an interval structure just to hit a TSS number. Never add a second workout
  to a day that already has one, and do not add new days beyond the available ones.

Return the complete corrected plan again in the exact same format as before: the readable
plan text, followed by the upload JSON between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.
After the readable plan text, output `BEGIN_UPLOAD_JSON` immediately. Do not insert an
"Upload JSON" heading, label, or numbered list item before the marker. The marker itself
starts the upload section; do not introduce or describe it in the prose.
Do not comment on the corrections — return only the corrected plan.