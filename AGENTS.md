# Codex Operating Rules

## Workflow Orchestration

### 1) Plan-First Default
- For any non-trivial task (3+ steps or architectural decisions), enter plan-first execution.
- Before coding, write a concrete plan in `tasks/todo.md` with:
  - what will change
  - why it is needed
  - how it will be verified
- If execution goes sideways, stop and re-plan before continuing.
- Include verification work in the plan, not only implementation steps.
- Write detailed specs up front to reduce ambiguity.

### 2) Parallel Strategy (Codex Equivalent to Subagents)
- Keep primary context clean by splitting research/exploration into focused parallel work where safe.
- Offload independent analysis tasks into separate tool runs.
- For complex problems, use more compute in parallel investigations.
- Keep one clear objective per parallel work item.

### 3) Self-Improvement Loop
- After any user correction, update `tasks/lessons.md` with:
  - what was wrong
  - the trigger pattern
  - a preventive rule
- Iterate on lessons until repeated mistakes drop.
- Review relevant lessons at the start of each session.

### 4) Verification Before "Done"
- Never mark work complete without evidence it works.
- If verification fails, fix and re-verify autonomously.
- Hold work to staff-level review quality.
- If full verification is not possible, explicitly state:
  - what was verified
  - what could not be verified
  - what manual QA is still needed

### 5) Demand Elegance (Balanced)
- For non-trivial changes, pause and challenge the design for a simpler or cleaner approach.
- If a fix feels hacky, replace it with the elegant solution informed by latest context.
- For trivial and obvious fixes, avoid over-engineering.

### 6) Autonomous Development
- Bug reports: fix directly end-to-end.
- Feature requests: plan, build, verify, then present results.
- Follow logs/errors/failing tests to root cause and resolve.
- Minimize context switching for the user.
- Goal: user states intent and receives working, verified output.

## Session Persistence (Files as Memory)

There is no persistent model memory between sessions. Project files are the memory layer.

- `tasks/todo.md`: current plan, progress, status, and review notes
- `tasks/lessons.md`: mistakes, corrections, and preventive rules
- `tasks/context.md`: architecture decisions, assumptions, and key context

### Session Start
- Read all files in `tasks/` to rebuild context.

### Task/Session End
- Update `tasks/` files with anything needed by a future session.
- Record non-obvious discoveries immediately.

## Task Management

### Before Starting Any Task
1. Write/refresh the plan in `tasks/todo.md` with checkable items before coding.
2. Include what/why/verification in the plan.
3. Approval is not required, but the plan must be visible.

### During a Task
1. Mark completed items in `tasks/todo.md` as work progresses.
2. Add high-level progress notes per major step.

### After a Task
1. Add a concise review/results section to `tasks/todo.md`.
2. If corrected by the user, capture lessons in `tasks/lessons.md`.

## Verification Workflows (Unity Project)

### After Code Changes (Always)
1. Compile check: `check_compile_errors`
2. Log check: `get_unity_logs` (focus on errors/warnings and regressions)
3. Fix loop: if issues exist, fix -> re-check -> repeat until clean
4. Do not present completion before steps 1-3 are clean

### After Gameplay/Behavior Changes

Limitation: `play_game` only enters Play Mode and cannot perform real player input.

1. Boot test: `play_game` -> wait -> `get_unity_logs` -> `stop_game`
2. Visual check: `capture_scene_object` or `capture_ui_canvas`
3. State check: `get_game_object_info` for components/values
4. Programmatic exercise: `execute_script` for public methods, transitions, handlers, runtime values
5. Be explicit about what required manual interaction and was not directly verifiable

### After Significant Changes (New systems/refactors)
1. Review full diff with `git diff` before presenting

### Custom Validation
- Use `execute_script` for project-specific validation (missing refs, null serialized fields, broken prefabs, etc.).
- If a new recurring error class appears, add a permanent validation check.

### When CoPlay/Editor Is Unavailable
- Use batch-mode compile/tests and inspect exit codes.
- Clearly state what interactive checks were not possible.

## Project Folder Structure

### Naming Conventions
- No spaces or special characters in names; use `PascalCase` where applicable.
- Classes: `PascalCase` (example: `BattlePassManager.cs`)
- Interfaces: prefix with `I` (example: `IRewardProvider.cs`)
- Namespaces: match folder structure (example: `Game.Features.BattlePass`)
- Prefabs: `FeatureNameDescription.prefab`
- ScriptableObjects: `DescriptiveName.asset`

### Third-Party Assets
- Keep third-party assets in original import locations.
- Do not modify third-party files directly.
- Use wrappers/extensions for customization.

## External Projects Policy

`.cursor/external/` is a read-only reference archive of linked external Unity projects.

- Never create/modify/delete files in `.cursor/external/`.
- Do not include `.cursor/external/` in searches unless the user explicitly asks.
- Access these projects only for comparison/reference/porting when requested.

## Core Principles

- Stability First: do not introduce regressions or breaking changes.
- Simplicity Above All: prefer the clearest minimal solution.
- Modular by Default: keep code decoupled and single-responsibility.
- No Laziness: find root causes; avoid temporary patches.
- Minimal Impact: change only what is necessary.
- Atomic Changes: keep each proposal/commit safe and buildable.
- Justify Decisions: when multiple approaches exist, provide pros/cons and recommendation.

## Suggested Enhancements (Optional)

These are recommendations, not mandatory process rules:

1. Add a "Definition of Done" checklist section to `tasks/todo.md` for each task.
2. Add a risk label per task (`low`, `medium`, `high`) to tune verification depth.
3. Add a "verification evidence" line with command/tool outputs used to support completion.
