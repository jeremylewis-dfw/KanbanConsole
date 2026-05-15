# dev-story Skill

Implement a user story's acceptance criteria as working code with tests.

## Input

Provide:
- **Story:** The full RTCC-formatted user story with acceptance criteria
- **Context:** Any relevant existing code, design docs, or project conventions

Reference: [artifacts/project-reference.md](../artifacts/project-reference.md)

---

## Process

### 1. Parse Acceptance Criteria
Extract each AC with its test method signature and rule. Organize by:
- Happy path (primary workflows)
- Edge cases / boundary conditions
- Negative cases (failures, error handling)

### 2. Test-Driven Development (TDD)
For each AC:

**Red:** Write a failing test
- Use the exact test method signature from AC
- Implement assertions based on the rule
- Run test; confirm it fails

**Green:** Write minimal code to pass
- Implement only what's needed to satisfy the test
- Do not over-engineer or add features beyond the AC

**Refactor:** Clean up and optimize
- Improve code clarity
- Add comments if logic is non-obvious
- Ensure consistency with project style

### 3. Integration
- Ensure all ACs pass
- Check that no existing tests break
- Update related code or dependencies as needed

### 4. Code Review Checklist
- [ ] All test method names match AC signatures
- [ ] Each test covers exactly one AC
- [ ] Tests are independent and repeatable
- [ ] Implementation matches acceptance criteria (no extra features)
- [ ] Code follows project conventions (see project-reference.md)
- [ ] No breaking changes to existing functionality

---

## Output

Deliver:
1. **Test file(s)** with all AC tests passing
2. **Implementation code** satisfying each test
3. **Summary** of completed ACs and any deviations

Place artifact deliverables in: `artifacts/`

**Project Context & Conventions:** [artifacts/project-reference.md](../artifacts/project-reference.md)
