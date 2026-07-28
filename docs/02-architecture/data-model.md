# Data Model — Course Management System

Derived from RSD §7, expanded to implementation-ready field types and constraints. This is the single source of truth for Backend Dev (EF Core entities) and Frontend Dev (TypeScript interfaces/mock fixtures) — both must match this exactly.

Traceability: every entity is annotated with the RSD requirement IDs it exists to satisfy.

---

## User
*Satisfies: AUTH-01 to AUTH-07*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| Email | string | Unique, required, max 256 |
| PasswordHash | string | Required, never returned in any DTO |
| FullName | string | Required, max 200 |
| Role | enum: `Admin` \| `Instructor` \| `Student` | Required, default `Student` |
| AvatarUrl | string? | Optional |
| Bio | string? | Optional, max 1000 |
| IsActive | bool | Default `true` |
| CreatedAt | DateTime (UTC) | Set on creation, immutable |

## Course
*Satisfies: CRS-01 to CRS-08*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| Title | string | Required, max 200 |
| Description | string | Required, max 4000 |
| Category | string | Required, max 100 (free text at MVP — no separate Category entity; see "Deferred" below) |
| Level | enum: `Beginner` \| `Intermediate` \| `Advanced` | Required |
| ThumbnailUrl | string? | Optional |
| Status | enum: `Draft` \| `Published` \| `Archived` | Default `Draft` |
| InstructorId | Guid | FK → User.Id, required, User.Role must be `Instructor` or `Admin` |
| CreatedAt | DateTime (UTC) | Immutable |
| UpdatedAt | DateTime (UTC) | Updated on every write |

## Module
*Satisfies: CNT-01, CNT-03, CNT-04*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| CourseId | Guid | FK → Course.Id, required |
| Title | string | Required, max 200 |
| OrderIndex | int | Required, unique within CourseId |

## Lesson
*Satisfies: CNT-02, CNT-03, CNT-04, CNT-05*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| ModuleId | Guid | FK → Module.Id, required |
| Title | string | Required, max 200 |
| ContentType | enum: `Text` \| `VideoLink` \| `AttachmentLink` | Required |
| Body | string | Required — for `Text` this is the content itself; for `VideoLink`/`AttachmentLink` this is the URL |
| OrderIndex | int | Required, unique within ModuleId |

## Enrollment
*Satisfies: ENR-01 to ENR-05*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| CourseId | Guid | FK → Course.Id, required |
| StudentId | Guid | FK → User.Id, required, User.Role must be `Student` |
| Status | enum: `Active` \| `Completed` \| `Dropped` | Default `Active` |
| EnrolledAt | DateTime (UTC) | Immutable |
| **Unique constraint** | | (CourseId, StudentId) — a student cannot double-enroll in the same course |

## LessonProgress
*Satisfies: PRG-01 to PRG-05*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| EnrollmentId | Guid | FK → Enrollment.Id, required |
| LessonId | Guid | FK → Lesson.Id, required |
| CompletedAt | DateTime? (UTC) | Null = not completed |
| **Unique constraint** | | (EnrollmentId, LessonId) |

Course completion % (RSD PRG-02) is a **computed value, not a stored field** — `count(LessonProgress where CompletedAt != null) / count(Lesson in Course)` — to avoid the classic bug class of stored derived data drifting from source truth.

## Quiz
*Satisfies: QZ-01*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| ModuleId | Guid | FK → Module.Id, required |
| Title | string | Required, max 200 |
| PassingScore | int | Required, 0–100 (percentage) |

## QuizQuestion
*Satisfies: QZ-02*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| QuizId | Guid | FK → Quiz.Id, required |
| Text | string | Required, max 1000 |
| OrderIndex | int | Required, unique within QuizId |

## QuizOption
*Satisfies: QZ-02*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| QuestionId | Guid | FK → QuizQuestion.Id, required |
| Text | string | Required, max 500 |
| IsCorrect | bool | Required — **exactly one** `IsCorrect = true` per QuestionId, enforced in Service layer (not a DB constraint EF Core can express cleanly at MVP scale) |

## QuizAttempt
*Satisfies: QZ-03 to QZ-06*

| Field | Type | Constraints |
|---|---|---|
| Id | Guid | PK |
| QuizId | Guid | FK → Quiz.Id, required |
| StudentId | Guid | FK → User.Id, required |
| Score | int | 0–100, computed at submission time, immutable after |
| Passed | bool | Computed: `Score >= Quiz.PassingScore` |
| SubmittedAt | DateTime (UTC) | Immutable |

Selected answers per attempt are **not persisted individually in MVP** (RSD has no requirement to review a past attempt's specific answers, only score/pass-fail history — QZ-05). If that becomes a requirement later, add a `QuizAttemptAnswer` join entity; don't build it speculatively now.

---

## Relationships Diagram (textual)

```
User (Instructor) 1───* Course
Course 1───* Module 1───* Lesson
Course 1───* Enrollment *───1 User (Student)
Enrollment 1───* LessonProgress *───1 Lesson
Module 1───* Quiz 1───* QuizQuestion 1───* QuizOption
Quiz 1───* QuizAttempt *───1 User (Student)
```

---

## Deferred (explicitly NOT modeled in MVP — do not build ahead of these)
- **Category as its own entity** — free-text field is sufficient for RSD's filter requirement (CRS-06); a full taxonomy table is unjustified complexity until course volume demands it.
- **QuizAttemptAnswer** (per-answer history) — see QuizAttempt note above.
- **Soft-delete audit trail** (who archived what, when) — RSD has no audit requirement; `Status = Archived` is sufficient.
