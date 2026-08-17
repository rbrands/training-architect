The plan you just produced did not pass the automated validation check. Fix the following
problems and return the corrected plan.

{{findings}}

How to fix each problem, depending on its kind:

- TSS mismatch on a specific workout (stated vs. computed TSS differ): This means the
  workout structure is fine but the "tss" value in its description is wrong. Recompute TSS
  from the workout's actual steps using the TSS formula from your instructions, and correct
  only the stated value. Do not change the workout's structure just to produce a different
  number.

- Weekly total outside the load target: Adjust duration and/or intensity of non-key sessions
  (easy/aerobic/recovery rides) to close the gap. Do not modify any workout that came from
  planned_workouts or references a library_workout_id — treat those as fixed and work around
  them.

- In all cases, keep every workout realistic and consistent for its stated training purpose.
  Never distort an interval structure just to hit a TSS number.

Return the complete corrected plan again in the exact same format as before: the readable
plan text, followed by the upload JSON between BEGIN_UPLOAD_JSON and END_UPLOAD_JSON.
Do not comment on the corrections — return only the corrected plan.