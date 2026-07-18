Based on the attached intervals.icu data, create a training plan for the coming week.

First infer the current weekly training context from the attached data:
- Recent training load and intensity distribution
- Completed key sessions and missing key stimuli
- Current fatigue and form from ATL and TSB
- Recent performance trends, if available
- Fueling issues or under-fueled sessions, if available

Derive the planning parameters directly from the attached data:
- Training phase and week type: from `next_week_active_phases` and `next_week_load_targets.week_type` (NORMAL / RECOVERY / RACE)
- Weekly load target: from `next_week_load_targets.load_target` (TSS)
- Available days: from `next_week_day_constraints`
  - days with `training_allowed: false` are unavailable
  - days with `training_allowed: true` and type LIMITED only get short, easy sessions
- Already planned sessions: from `planned_workouts` for next week - treat them as fixed anchors and do not replace them
- Current form and fatigue: consider TSB and ATL
- Recently completed key sessions: use them as context and avoid duplicating the same key stimulus too soon

Planning logic:
1. Place key sessions matched to the training phase first (VO2max, threshold, long ride)
2. Align total load to the TSS target and show estimated TSS per session
3. Account for fueling strategy for intense sessions
4. Explicitly schedule recovery days
5. Keep the plan realistic for the available days and current fatigue state

Use the plan/workout generation output format and upload JSON markers defined in the system prompt.
Ensure the marked upload JSON contains the exact workouts intended for upload.
Include session goals, estimated TSS, and fueling recommendations in each workout description.

{{athlete_data}}