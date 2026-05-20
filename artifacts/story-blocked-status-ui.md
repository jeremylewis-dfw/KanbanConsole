# Story: Display and Interact with the Blocked Column in the Terminal UI

---

## Story (RTCC Format)

### Role
**As a** Kanban board user

### Task
**I want to** see blocked tasks in the board table and select "Blocked" as a column when adding or moving tasks

### Context
**In order to** have full visibility into stalled work and manage it from the terminal without switching tools

### Criteria
**The following acceptance criteria must be met:**

---

> **Note on test method names:** The UI layer (`Program.cs`) is exercised manually per project convention — no NUnit tests cover it. AC test identifiers below are manual verification scenarios following the `Subject_Condition_ExpectedResult` naming pattern; they are not automated test methods.

---

## Acceptance Criteria

### AC-1: Board table renders a Blocked column header

**Test:** `UI.RenderBoard_WithBlockedTask_ShowsBlockedColumnHeader`
```
GIVEN a board with at least one task in the Blocked column
WHEN  the board is rendered to the terminal
THEN  a "Blocked" column header is visible in the table alongside To Do, In Progress, and Done
```
**Rule:** The board table must display four columns; Blocked must appear as a distinct, labeled header.

---

### AC-2: Blocked tasks appear in the Blocked column

**Test:** `UI.RenderBoard_WithBlockedTask_TaskAppearsInBlockedColumn`
```
GIVEN a board with a task in the Blocked column
WHEN  the board is rendered
THEN  that task's Id and title are displayed in the Blocked column, not in any other column
```
**Rule:** Each blocked task must be listed under the Blocked column and must not appear in Todo, In Progress, or Done.

---

### AC-3: Empty board shows a placeholder in the Blocked column

**Test:** `UI.RenderBoard_EmptyBoard_BlockedColumnShowsPlaceholder`
```
GIVEN a board with no tasks in any column
WHEN  the board is rendered
THEN  the Blocked column displays the empty-state placeholder alongside the other columns
```
**Rule:** An empty Blocked column must render a placeholder consistent with the other empty columns, not a missing or broken cell.

---

### AC-4: Add task prompt includes Blocked as a selectable column

**Test:** `UI.AddTask_ColumnPrompt_IncludesBlockedOption`
```
GIVEN the user selects "Add task" from the main menu
WHEN  the column-selection prompt is displayed
THEN  "Blocked" appears as a selectable option alongside To Do, In Progress, and Done
```
**Rule:** The column-selection prompt must offer all four columns, including Blocked.

---

### AC-5: Adding a task to Blocked places it in the Blocked column

**Test:** `UI.AddTask_UserSelectsBlocked_TaskAppearsInBlockedOnRender`
```
GIVEN the user selects "Add task", enters a title, and chooses the Blocked column
WHEN  the action completes and the board re-renders
THEN  the new task appears in the Blocked column and is absent from all other columns
```
**Rule:** Choosing Blocked as the destination when adding a task must route the task to the Blocked column.

---

### AC-6: Move task destination prompt includes Blocked

**Test:** `UI.MoveTask_DestinationPrompt_IncludesBlockedOption`
```
GIVEN the user selects "Move task" and picks any task
WHEN  the destination-column prompt is displayed
THEN  "Blocked" appears as a selectable destination alongside the other columns
```
**Rule:** The move-destination prompt must include Blocked as a valid target column.

---

### AC-7: Moving a task to Blocked removes it from its source column

**Test:** `UI.MoveTask_ToBlocked_TaskRemovedFromSourceColumn`
```
GIVEN a task exists in the To Do, In Progress, or Done column
WHEN  the user moves that task to Blocked and the board re-renders
THEN  the task appears in the Blocked column and is no longer listed in its previous column
```
**Rule:** Moving a task to Blocked must relocate it, not duplicate it.

---

### AC-8: Moving a task out of Blocked places it in the chosen column

**Test:** `UI.MoveTask_FromBlocked_TaskAppearsInTargetColumn`
```
GIVEN a task exists in the Blocked column
WHEN  the user moves that task to To Do, In Progress, or Done and the board re-renders
THEN  the task appears in the chosen column and is no longer listed in Blocked
```
**Rule:** Blocked tasks must be moveable to any other column, with the same behavior as moves between non-Blocked columns.

---

### AC-9: "Blocked" column name maps explicitly and does not fall through to a default

**Test:** `UI.ColumnFromString_BlockedString_MapsToBlockedColumn`
```
GIVEN "Blocked" is the column name string produced by a user selection
WHEN  that string is resolved to a Column value
THEN  Column.Blocked is returned — not Column.Done or any other fallback
```
**Rule:** The mapping from display name to `Column` enum must include an explicit "Blocked" case; the value must never be silently coerced to another column by a wildcard default.
