Write a user story for the following feature request using RTCC format.

Feature request: $ARGUMENTS
---

## Story (RTCC Format)

**Role:** As a [specific user role or persona],
**Task:** I want to [action or capability],
**Context:** In order to [business context or situation],
**Criteria:** The following acceptance criteria must be met:

---

## Acceptance Criteria (Test-Driven)

Write each criterion in test-first (red) style: name the failing test before stating the rule.

Format:

### AC-1: [short title]

**Test:** `[ClassName].[methodName]_[condition]_[expectedOutcome]`
```
GIVEN [precondition or system state]
WHEN  [action, event, or trigger]
THEN  [observable, verifiable outcome]
```
**Rule:** [one sentence plain-language restatement of the criterion]

---

Requirements for the full set of criteria:
- Cover the happy path first
- Follow with edge cases and boundary conditions
- Include at least one negative/failure case (what must NOT happen or what should fail gracefully)
- Each criterion must be independently testable — no compound assertions in a single AC
- Test method names must follow the pattern: `Subject_Condition_ExpectedResult`
- Do not include implementation details, technology choices, or solution hints

Stop when the story is fully specified.

---

## Output

Save the story artifact to: `artifacts/`

**Project Context & Conventions:** [artifacts/project-reference.md](../artifacts/project-reference.md)
