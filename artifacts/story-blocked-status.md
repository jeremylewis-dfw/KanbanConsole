# Story: Add "Blocked" Column to Kanban Board

---

## Story (RTCC Format)

### Role
**As a** Kanban board user

### Task
**I want to** move tasks into a "Blocked" status

### Context
**In order to** signal that a task cannot proceed due to an external dependency or impediment, so the team has clear visibility into work that is stalled

### Criteria
**The following acceptance criteria must be met:**

---

## Acceptance Criteria (Test-Driven)

### AC-1: A task can be added directly to the Blocked column

**Test:** `AddTask_ToBlockedColumn_TaskAppearsInBlockedList`
```
GIVEN a board with no tasks
WHEN  a task is added with the Blocked column as the target
THEN  the task appears in the Blocked column and is absent from all other columns
```
**Rule:** `AddTask` must accept `Blocked` as a valid destination column.

---

### AC-2: A task in any other column can be moved to Blocked

**Test:** `MoveTask_FromInProgressToBlocked_TaskMovedSuccessfully`
```
GIVEN a board with a task in the InProgress column
WHEN  that task is moved to the Blocked column
THEN  MoveTask returns true, the task appears in Blocked, and InProgress no longer contains it
```
**Rule:** Moving a task to Blocked follows the same behavior as moving to any other column.

---

### AC-3: Blocked tasks appear in GetAllTasks

**Test:** `GetAllTasks_BoardWithBlockedTask_IncludesBlockedTask`
```
GIVEN a board that has at least one task in the Blocked column
WHEN  GetAllTasks is called
THEN  the blocked task is included in the returned list
```
**Rule:** `GetAllTasks` must include tasks from the Blocked column.

---

### AC-4: GetTaskColumn returns Blocked for a blocked task

**Test:** `GetTaskColumn_TaskInBlocked_ReturnsBlocked`
```
GIVEN a board with a task in the Blocked column
WHEN  GetTaskColumn is called with that task's Id
THEN  the returned value is the Blocked column
```
**Rule:** `GetTaskColumn` must correctly identify the Blocked column for tasks residing there.

---

### AC-5: Moving a task already in Blocked to Blocked returns false

**Test:** `MoveTask_AlreadyInBlocked_ReturnsFalse`
```
GIVEN a board with a task in the Blocked column
WHEN  MoveTask is called targeting Blocked for that same task
THEN  MoveTask returns false and the board state is unchanged
```
**Rule:** Moving a task to its current column is a no-op and returns false, consistent with existing column behavior.

---

### AC-6: Moving a non-existent task to Blocked returns false

**Test:** `MoveTask_NonExistentIdToBlocked_ReturnsFalse`
```
GIVEN a board with no task matching a given Id
WHEN  MoveTask is called with that Id targeting the Blocked column
THEN  MoveTask returns false and no task is added to Blocked
```
**Rule:** Moving an unknown Id to Blocked must fail gracefully with no side effects.

---

### AC-7: Blocked tasks are included in NextId calculation

**Test:** `NextId_BoardWithOnlyBlockedTasks_ReturnsMaxIdPlusOne`
```
GIVEN a board whose only tasks are in the Blocked column
WHEN  NextId is called
THEN  the returned Id is one greater than the highest Id among the blocked tasks
```
**Rule:** Ids assigned to blocked tasks must participate in the uniqueness guarantee — NextId must never produce a duplicate.
