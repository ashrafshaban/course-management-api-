# Requirements Specification Document (RSD)
## Course Management System — MVP

| | |
|---|---|
| **Document Owner** | Product Owner |
| **Status** | Draft v1.0 |
| **Target Release** | MVP |
| **Audience** | Engineering, QA, Design, Stakeholders |

---

## 1. Purpose & Product Vision

Build a lightweight, production-credible **Course Management System (CMS)** that lets an institution create and manage courses, enroll and manage learners, deliver lesson content, and track progress and grades.

The MVP's guiding principle: **ship the smallest system that behaves like a real product**, not a prototype. That means real authentication, real roles, real state transitions, and a clean API contract — but no gold-plating, no microservices, no premature scaling infrastructure.

A second, equally important product requirement: the **frontend must be independently demoable and deployable** without any backend running, via a **Mock Mode** that a static host (GitHub Pages) can serve. This de-risks demos, stakeholder reviews, and frontend-only development cycles.

---

## 2. Goals & Non-Goals

### 2.1 Goals (In Scope for MVP)
- Course catalog with CRUD (create, read, update, delete/archive)
- User roles: **Admin**, **Instructor**, **Student**
- Authentication & authorization (JWT-based)
- Enrollment workflow (self-enroll or admin-assigned)
- Lesson/module content structure per course (text, video link, attachment link)
- Simple assessments: quizzes with multiple-choice questions, auto-graded
- Progress tracking per student per course (lesson completion %, quiz scores)
- Instructor dashboard (roster, grades, content management)
- Student dashboard (my courses, progress, grades)
- Admin dashboard (users, courses, global oversight)
- Search & filter on course catalog
- **Mock Mode**: frontend runs 100% on mocked data, toggle-able to real API
- CI/CD: GitHub Actions pipeline deploying frontend (mock mode) to GitHub Pages

### 2.2 Non-Goals (Explicitly Out of Scope for MVP)
- Live video conferencing / webinar integration
- Payments & billing
- SCORM/xAPI content interoperability
- Certificates generation
- Discussion forums / social features
- Multi-tenancy (single organization only)
- Mobile native apps
- Advanced analytics/BI dashboards
- Email/SMS notification delivery (only in-app notices in MVP)
- Fine-grained permission system beyond 3 roles
- Localization/multi-language content

These are natural **Phase 2** candidates and should be called out as such to stakeholders, not silently dropped.

---

## 3. User Roles & Personas

| Role | Description | Key Needs |
|---|---|---|
| **Admin** | Manages the platform | Manage users, manage all courses, oversee enrollments, deactivate accounts |
| **Instructor** | Owns and teaches courses | Create/edit own courses, manage lessons & quizzes, view roster, grade/track progress |
| **Student** | Learner | Browse catalog, enroll, consume lessons, take quizzes, track own progress |

Authentication is required for all roles. Course catalog browsing (read-only, no enrollment) may be available to unauthenticated visitors — see §5.1.

---

## 4. Functional Requirements

Each requirement has an ID for traceability. Priority: **P0** = must-have for MVP, **P1** = should-have if time allows, cut first if scope pressure hits.

### 4.1 Authentication & User Management
| ID | Requirement | Priority |
|---|---|---|
| AUTH-01 | User can register with email/password (Student role by default) | P0 |
| AUTH-02 | User can log in and receive a JWT access token | P0 |
| AUTH-03 | Passwords stored hashed (bcrypt/PBKDF2), never plaintext | P0 |
| AUTH-04 | Admin can create Instructor/Admin accounts | P0 |
| AUTH-05 | Role-based route/API guarding (Admin/Instructor/Student) | P0 |
| AUTH-06 | Logged-in user can view/edit own profile (name, avatar URL, bio) | P1 |
| AUTH-07 | Admin can deactivate/reactivate a user | P1 |

### 4.2 Course Catalog & Management
| ID | Requirement | Priority |
|---|---|---|
| CRS-01 | Instructor/Admin can create a course (title, description, category, thumbnail URL, level, status draft/published) | P0 |
| CRS-02 | Instructor/Admin can edit their own course | P0 |
| CRS-03 | Instructor/Admin can archive (soft-delete) a course | P0 |
| CRS-04 | Any authenticated user can browse published courses with pagination | P0 |
| CRS-05 | Search courses by title/keyword | P0 |
| CRS-06 | Filter courses by category and level | P0 |
| CRS-07 | Course detail page shows description, instructor, module/lesson outline, enrollment count | P0 |
| CRS-08 | Admin can view/manage **all** courses regardless of owner | P0 |

### 4.3 Course Content (Modules & Lessons)
| ID | Requirement | Priority |
|---|---|---|
| CNT-01 | Instructor can add Modules (sections) to a course | P0 |
| CNT-02 | Instructor can add Lessons to a module (title, order, content type: text/video-link/attachment-link, body) | P0 |
| CNT-03 | Instructor can reorder modules/lessons (simple order-index field) | P1 |
| CNT-04 | Instructor can delete a module/lesson | P0 |
| CNT-05 | Student can view lesson content only if enrolled | P0 |

### 4.4 Enrollment
| ID | Requirement | Priority |
|---|---|---|
| ENR-01 | Student can self-enroll in a published course | P0 |
| ENR-02 | Student can unenroll (drop) a course | P1 |
| ENR-03 | Instructor/Admin can view course roster (enrolled students) | P0 |
| ENR-04 | Admin can manually enroll/remove a student from any course | P1 |
| ENR-05 | Enrollment record tracks status: active, completed, dropped | P0 |

### 4.5 Progress Tracking
| ID | Requirement | Priority |
|---|---|---|
| PRG-01 | Student can mark a lesson as complete | P0 |
| PRG-02 | System computes course completion % from lessons completed / total lessons | P0 |
| PRG-03 | Student dashboard shows progress bar per enrolled course | P0 |
| PRG-04 | Instructor can view per-student progress in their course | P0 |
| PRG-05 | Course marked "completed" for a student when 100% lessons done (and quizzes passed, if any) | P1 |

### 4.6 Assessments (Quizzes)
| ID | Requirement | Priority |
|---|---|---|
| QZ-01 | Instructor can create a quiz attached to a module (title, passing score) | P0 |
| QZ-02 | Instructor can add multiple-choice questions (question text, options, correct option) | P0 |
| QZ-03 | Student can take a quiz once enrolled; system auto-grades | P0 |
| QZ-04 | Student sees their score and pass/fail immediately | P0 |
| QZ-05 | Instructor can view all quiz attempts/scores for their course | P1 |
| QZ-06 | Student can retake a quiz (simple: unlimited retakes in MVP) | P1 |

### 4.7 Dashboards
| ID | Requirement | Priority |
|---|---|---|
| DSH-01 | Student dashboard: enrolled courses, progress, upcoming/incomplete items | P0 |
| DSH-02 | Instructor dashboard: my courses, total students, avg. progress | P0 |
| DSH-03 | Admin dashboard: platform totals (users, courses, enrollments), user management table | P0 |

### 4.8 Notifications (Lightweight, In-App Only)
| ID | Requirement | Priority |
|---|---|---|
| NTF-01 | In-app toast/banner on key actions (enrolled, quiz submitted, course published) | P1 |

---

## 5. Mock Mode — Critical MVP Requirement

This is a **first-class architectural requirement**, not an afterthought.

### 5.1 Objective
The React frontend must run **fully functional end-to-end** against an in-memory/mock data layer, with zero backend dependency, and be switchable to the real .NET API via a **single configuration flag** — no code branching scattered across components.

### 5.2 Design Approach
- Define a **Data Service Abstraction Layer** (interface-like contract in TypeScript) — e.g. `ICourseService`, `IAuthService`, `IEnrollmentService`, `IQuizService`.
- Implement two concrete providers per interface:
  - `mock/*.service.ts` — reads/writes an in-memory store seeded from static JSON fixtures (persisted to `localStorage` within the browser session so mock state survives refresh).
  - `api/*.service.ts` — calls the real .NET Core REST API via `fetch`/`axios`.
- A **service factory** (`services/index.ts`) picks the implementation based on an environment variable, e.g.:
  ```ts
  export const APP_MODE = import.meta.env.VITE_APP_MODE; // 'mock' | 'api'
  export const courseService = APP_MODE === 'mock' ? mockCourseService : apiCourseService;
  ```
- Components/pages **only ever import from `services/index.ts`**, never directly from `mock/` or `api/`. This guarantees switching modes requires **zero UI code changes**.
- Mock services simulate realistic latency (e.g. 300–600ms artificial delay) and simple error scenarios, so the UI's loading/error states are exercised identically in both modes.
- Mock auth: a small set of seeded users (one Admin, one Instructor, two Students) with hardcoded credentials shown on the login page for demo convenience.
- Mock data fixtures live in `src/mocks/fixtures/*.json` — courses, users, enrollments, quizzes — enough to populate a believable, non-trivial demo (8–10 courses, multiple modules/lessons each).

### 5.3 Switching Modes
| Mode | How to run | Use case |
|---|---|---|
| Mock | `VITE_APP_MODE=mock npm run dev` (or `.env.mock`) | Local frontend dev, demos, GitHub Pages deployment |
| API | `VITE_APP_MODE=api npm run dev` (or `.env.api`, pointing `VITE_API_BASE_URL` to the .NET backend) | Full-stack integration, staging, production |

No rebuild-time code changes are required to switch — only the env file/flag.

---

## 6. Technical Architecture

### 6.1 Stack
| Layer | Technology |
|---|---|
| Frontend | React 18+ (Vite), TypeScript, React Router, a lightweight UI kit (e.g. MUI or Tailwind — team's choice), Axios/fetch |
| Backend | .NET 8 (latest LTS), ASP.NET Core Web API, Entity Framework Core |
| Database | SQLite for MVP (file-based, zero infra) — EF Core makes migrating to SQL Server/PostgreSQL later a config change, not a rewrite |
| Auth | ASP.NET Core Identity + JWT Bearer tokens |
| API Docs | Swagger/OpenAPI (auto-generated) |
| Hosting (frontend, mock mode) | GitHub Pages (static) |
| Hosting (backend, future) | Any container host (Azure App Service, AWS, Render) — out of scope for MVP CI/CD, noted as Phase 2 |

### 6.2 Why this stack fits "no over-engineering"
- **SQLite** avoids provisioning a DB server for an MVP; EF Core's provider model means moving to PostgreSQL/SQL Server later is a connection-string + provider swap.
- **Monolithic ASP.NET Core Web API** (single project, layered internally: Controllers → Services → Repositories/EF) — no microservices, no message queues, no premature distributed-systems complexity.
- **No separate API gateway, no Redis cache, no Docker orchestration** for MVP — these are Phase 2/3 concerns once real usage data justifies them.
- **JWT** is stateless and simple to reason about; no session server needed.

### 6.3 Backend Project Structure (suggested)
```
CourseManagement.Api/
 ├─ Controllers/        (AuthController, CoursesController, EnrollmentsController, QuizzesController, UsersController)
 ├─ Services/            (business logic, one service per domain area)
 ├─ Data/                (DbContext, EF migrations)
 ├─ Models/              (EF entities)
 ├─ DTOs/                (request/response contracts — never expose EF entities directly)
 ├─ Auth/                (JWT config, role policies)
 └─ Program.cs
```

### 6.4 Frontend Project Structure (suggested)
```
course-management-web/
 ├─ src/
 │   ├─ services/
 │   │   ├─ mock/            (mock*.service.ts)
 │   │   ├─ api/              (api*.service.ts)
 │   │   ├─ interfaces/       (service contracts)
 │   │   └─ index.ts          (mode-switch factory)
 │   ├─ mocks/fixtures/       (seed JSON data)
 │   ├─ pages/                (role-based: student/, instructor/, admin/)
 │   ├─ components/
 │   ├─ context/              (AuthContext, etc.)
 │   ├─ routes/                (protected route wrappers per role)
 │   └─ App.tsx
 ├─ .env.mock
 ├─ .env.api
 └─ vite.config.ts
```

---

## 7. Data Model (Core Entities)

| Entity | Key Fields |
|---|---|
| **User** | Id, Email, PasswordHash, FullName, Role (Admin/Instructor/Student), IsActive, CreatedAt |
| **Course** | Id, Title, Description, Category, Level, ThumbnailUrl, Status (Draft/Published/Archived), InstructorId (FK→User), CreatedAt |
| **Module** | Id, CourseId (FK), Title, OrderIndex |
| **Lesson** | Id, ModuleId (FK), Title, ContentType (Text/VideoLink/AttachmentLink), Body, OrderIndex |
| **Enrollment** | Id, CourseId (FK), StudentId (FK→User), Status (Active/Completed/Dropped), EnrolledAt |
| **LessonProgress** | Id, EnrollmentId (FK), LessonId (FK), CompletedAt (nullable) |
| **Quiz** | Id, ModuleId (FK), Title, PassingScore |
| **QuizQuestion** | Id, QuizId (FK), Text, OrderIndex |
| **QuizOption** | Id, QuestionId (FK), Text, IsCorrect |
| **QuizAttempt** | Id, QuizId (FK), StudentId (FK), Score, Passed, SubmittedAt |

Relationships are conventional 1-to-many; no polymorphism or graph modeling needed at MVP scale.

---

## 8. API Contract (Representative Endpoints)

```
POST   /api/auth/register
POST   /api/auth/login
GET    /api/users/me
GET    /api/users                     (Admin)
PATCH  /api/users/{id}/deactivate     (Admin)

GET    /api/courses?search=&category=&level=&page=
GET    /api/courses/{id}
POST   /api/courses                   (Instructor/Admin)
PUT    /api/courses/{id}              (Owner/Admin)
DELETE /api/courses/{id}              (Owner/Admin — soft delete)

POST   /api/courses/{id}/modules
POST   /api/modules/{id}/lessons
PUT    /api/lessons/{id}
DELETE /api/lessons/{id}

POST   /api/courses/{id}/enroll       (Student)
DELETE /api/enrollments/{id}          (Student — unenroll)
GET    /api/courses/{id}/roster       (Instructor/Admin)
GET    /api/students/me/enrollments   (Student)

POST   /api/lessons/{id}/complete     (Student)
GET    /api/enrollments/{id}/progress

POST   /api/modules/{id}/quizzes
POST   /api/quizzes/{id}/questions
POST   /api/quizzes/{id}/attempts     (Student submits answers)
GET    /api/quizzes/{id}/attempts     (Instructor)
```

All endpoints return consistent DTOs and standard HTTP status codes; errors use a uniform `{ message, errors? }` shape so mock and API error states render identically in the frontend.

---

## 9. Non-Functional Requirements

| Category | Requirement |
|---|---|
| Performance | API responses < 500ms for typical MVP data volumes (hundreds of courses/users) |
| Security | Passwords hashed; JWT expiry + refresh not required for MVP (short-lived token acceptable); HTTPS enforced in deployed API |
| Usability | Responsive layout (desktop + tablet minimum); clear loading/empty/error states everywhere |
| Maintainability | Layered backend (Controller/Service/Data); typed frontend service contracts; no business logic in controllers |
| Portability | SQLite → any relational DB via EF Core provider swap; mock/API frontend swap via env var only |
| Observability | Basic structured logging (ILogger) on backend; no dedicated APM needed for MVP |

---

## 10. Deployment & CI/CD

### 10.1 Frontend (Mock Mode) → GitHub Pages
A GitHub Actions workflow triggers on push to `main` (or a `release` branch):
1. Checkout code
2. Install Node dependencies
3. Build with `VITE_APP_MODE=mock` and correct `base` path for GH Pages
4. Publish `dist/` to the `gh-pages` branch (via `actions/deploy-pages` or `peaceiris/actions-gh-pages`)

This gives stakeholders a **live, clickable, no-backend-required demo URL** at every merge — a major MVP-validation win with near-zero cost.

### 10.2 Backend (API Mode)
Out of scope for GitHub Pages (static-only). For MVP, backend can run locally (`dotnet run`) or be deployed manually to any container/app-service host when full-stack demos are needed. A basic "build + test" GitHub Actions workflow (no deployment) is still recommended to catch breakage early. Full backend CI/CD is a Phase 2 item once a hosting target is chosen.

---

## 11. MVP Delivery Milestones

| Milestone | Scope |
|---|---|
| **M1 — Foundations** | Repo setup, EF Core models + migrations, Auth (register/login/JWT), frontend service abstraction layer + mock fixtures, GitHub Pages pipeline live with mock data |
| **M2 — Course Management** | Course CRUD, modules/lessons CRUD, catalog browse/search/filter (mock + real API) |
| **M3 — Enrollment & Progress** | Enrollment flow, lesson completion, progress calculation, student/instructor dashboards |
| **M4 — Assessments** | Quiz creation, quiz taking, auto-grading, attempt history |
| **M5 — Polish & Admin** | Admin dashboard, role guards end-to-end, empty/error states, responsive pass, README + demo links |

---

## 12. Success Criteria for MVP Acceptance

- A stakeholder can open the GitHub Pages link with **no setup** and walk through: browse catalog → log in as each of the 3 seeded roles → create a course (Instructor) → enroll & complete lessons → take a quiz (Student) → view roster/progress (Instructor) → manage users (Admin).
- Switching `VITE_APP_MODE` from `mock` to `api` requires no code change, and the same UI functions identically against a running .NET backend.
- Backend exposes a complete, Swagger-documented API covering all P0 requirements in §4.
- No P0 requirement from §4 is missing at release.

---

## 13. Assumptions & Risks

| Item | Notes |
|---|---|
| Single organization/tenant | Multi-tenancy explicitly deferred |
| SQLite is sufficient for MVP data volumes | Revisit before real production traffic |
| No file upload service | Video/attachment fields are URLs only (e.g. YouTube link, external file link) — avoids building storage infra in MVP |
| Design system kept simple | Avoids design-phase over-investment; a component library (MUI) is acceptable to move fast |
| Mock data must stay representative | Fixtures should be revisited whenever a new P0 feature is added, to keep the demo credible |

---

## 14. Open Questions for Stakeholder Sign-off

1. Should unauthenticated visitors see the public course catalog (read-only), or is login required to see anything? (Assumed: public catalog browsing allowed, enrollment requires login.)
2. Is unlimited quiz retake acceptable for MVP, or is a retake limit a launch blocker?
3. Preferred UI kit — MUI, Tailwind + Headless UI, or Ant Design? (Affects §6.4 only, not architecture.)
4. Target GitHub Pages URL / repo name, to finalize the Vite `base` path in the deploy workflow.
