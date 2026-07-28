# API Contract — Course Management System

Expands RSD §8 into a binding, implementation-ready contract. Both `api/*.service.ts` (real) and `mock/*.service.ts` (simulated) implementations must conform to this exactly — this document is what makes them interchangeable per ADR-004.

**Conventions:**
- Base path: `/api`
- Auth: `Authorization: Bearer <jwt>` header, except where marked "Public"
- All list endpoints support `?page=1&pageSize=20` and return `{ items: [...], totalCount, page, pageSize }`
- All error responses: `{ message: string, errors?: Record<string,string[]> }`
- Dates: ISO 8601 UTC strings over the wire

---

## Auth

### `POST /api/auth/register` — Public
**Request:** `{ email, password, fullName }`
**Response 201:** `{ id, email, fullName, role, token }`
**Errors:** 400 (validation), 409 (email already exists)

### `POST /api/auth/login` — Public
**Request:** `{ email, password }`
**Response 200:** `{ id, email, fullName, role, token }`
**Errors:** 401 (invalid credentials)

### `GET /api/users/me` — Any authenticated role
**Response 200:** `{ id, email, fullName, role, avatarUrl, bio, createdAt }`

### `GET /api/users` — Admin only
**Response 200:** paginated `User` list (no `passwordHash` ever included)

### `PATCH /api/users/{id}/deactivate` — Admin only
**Response 200:** updated `User`
**Errors:** 404, 403 (non-admin)

---

## Courses

### `GET /api/courses` — Public (published only) / any role sees more per role rules below
**Query:** `search`, `category`, `level`, `page`, `pageSize`
**Response 200:** paginated `Course` list. Unauthenticated and Student roles see `Status=Published` only; Instructor sees own courses of any status plus all Published; Admin sees all.
**Satisfies:** CRS-04, CRS-05, CRS-06

### `GET /api/courses/{id}` — Public if Published, else Owner/Admin only
**Response 200:** `Course` + nested `modules[].lessons[]` outline (lesson `body` omitted unless caller is enrolled or is the Owner/Admin — see CNT-05)
**Errors:** 404, 403 (draft course, non-owner)

### `POST /api/courses` — Instructor, Admin
**Request:** `{ title, description, category, level, thumbnailUrl? }`
**Response 201:** created `Course` (`status: Draft`, `instructorId` = caller's id, or specified by Admin)

### `PUT /api/courses/{id}` — Owner Instructor, Admin
**Request:** partial `Course` fields
**Response 200:** updated `Course`
**Errors:** 403 (non-owner, non-admin), 404

### `DELETE /api/courses/{id}` — Owner Instructor, Admin
**Effect:** soft delete → `Status = Archived`
**Response 204**

---

## Modules & Lessons

### `POST /api/courses/{courseId}/modules` — Owner Instructor, Admin
**Request:** `{ title, orderIndex }`
**Response 201:** created `Module`

### `PUT /api/modules/{id}` — Owner Instructor, Admin — reorder/rename
### `DELETE /api/modules/{id}` — Owner Instructor, Admin

### `POST /api/modules/{moduleId}/lessons` — Owner Instructor, Admin
**Request:** `{ title, contentType, body, orderIndex }`
**Response 201:** created `Lesson`

### `PUT /api/lessons/{id}` — Owner Instructor, Admin
### `DELETE /api/lessons/{id}` — Owner Instructor, Admin

---

## Enrollment

### `POST /api/courses/{courseId}/enroll` — Student
**Response 201:** created `Enrollment` (`status: Active`)
**Errors:** 409 (already enrolled), 400 (course not Published)
**Satisfies:** ENR-01

### `DELETE /api/enrollments/{id}` — Owning Student
**Effect:** `Status = Dropped`
**Response 204**
**Satisfies:** ENR-02

### `GET /api/courses/{courseId}/roster` — Owner Instructor, Admin
**Response 200:** list of `{ studentId, fullName, email, enrollmentStatus, progressPercent, enrolledAt }`
**Satisfies:** ENR-03, PRG-04

### `POST /api/admin/enrollments` — Admin
**Request:** `{ courseId, studentId }`
**Satisfies:** ENR-04

### `GET /api/students/me/enrollments` — Student
**Response 200:** list of caller's `Enrollment` + `Course` summary + `progressPercent`
**Satisfies:** DSH-01

---

## Progress

### `POST /api/lessons/{lessonId}/complete` — Enrolled Student only
**Response 200:** `{ lessonId, completedAt }`
**Errors:** 403 (not enrolled)
**Satisfies:** PRG-01

### `GET /api/enrollments/{id}/progress` — Owning Student, Course Owner Instructor, Admin
**Response 200:** `{ enrollmentId, totalLessons, completedLessons, progressPercent, quizzesPassed, quizzesTotal }`
**Satisfies:** PRG-02, PRG-03

---

## Quizzes

### `POST /api/modules/{moduleId}/quizzes` — Owner Instructor, Admin
**Request:** `{ title, passingScore }`
**Response 201:** created `Quiz`

### `POST /api/quizzes/{quizId}/questions` — Owner Instructor, Admin
**Request:** `{ text, orderIndex, options: [{ text, isCorrect }] }` (exactly one `isCorrect: true` — 400 if violated)

### `GET /api/quizzes/{id}` — Enrolled Student (options returned **without** `isCorrect` field), Owner Instructor/Admin (full, including `isCorrect`)
**Note:** two different DTO shapes for the same resource, keyed by caller role — call this out explicitly to Backend Dev, it's a common miss.

### `POST /api/quizzes/{id}/attempts` — Enrolled Student
**Request:** `{ answers: [{ questionId, selectedOptionId }] }`
**Response 201:** `{ attemptId, score, passed, submittedAt }` (auto-graded server-side; never trust client-computed score)
**Satisfies:** QZ-03, QZ-04

### `GET /api/quizzes/{id}/attempts` — Owner Instructor, Admin
**Response 200:** list of all students' attempts for this quiz
**Satisfies:** QZ-05

---

## Dashboards (aggregation endpoints — thin wrappers over the above, not new domain logic)

### `GET /api/dashboard/instructor` — Instructor
**Response 200:** `{ courses: [{ id, title, studentCount, avgProgressPercent }] }`
**Satisfies:** DSH-02

### `GET /api/dashboard/admin` — Admin
**Response 200:** `{ totalUsers, totalCourses, totalEnrollments, recentUsers: User[] }`
**Satisfies:** DSH-03

---

## Role Access Summary (quick reference for Reviewer agent)

| Endpoint group | Public | Student | Instructor | Admin |
|---|---|---|---|---|
| Auth register/login | ✅ | — | — | — |
| Browse published courses | ✅ | ✅ | ✅ | ✅ |
| Create/edit/delete course | ❌ | ❌ | own only | any |
| Enroll/drop | ❌ | ✅ | ❌ | via admin endpoint |
| View roster/progress | ❌ | own only | own courses | any |
| Take quiz | ❌ | if enrolled | ❌ | ❌ |
| View quiz answer key | ❌ | ❌ | own courses | any |
| User management | ❌ | ❌ | ❌ | ✅ |

Every row here should become an explicit test case in QA's `test-plan.md` — flag this to the QA agent playbook.
