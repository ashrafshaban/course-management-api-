# ADR-002: Persistence — Database Choice

## Status
Accepted

## Context
RSD §6.1 asks for a "simple database" and RSD §9 (NFRs) sets performance targets appropriate to hundreds of courses/users, not enterprise scale. RSD §13 (Assumptions & Risks) already flags SQLite as an assumption to revisit before real production traffic — this ADR makes that decision explicit and sets the migration path.

## Decision
Use **SQLite** via **EF Core** (Entity Framework Core) as the ORM, with the database as a single file committed to `.gitignore` (not source-controlled) and created via EF Core migrations on first run.

## Alternatives Considered
- **PostgreSQL / SQL Server from day one** — rejected for MVP. Requires provisioning a DB server/container even for local dev and CI, which adds setup friction for every Dev agent session with no corresponding requirement gain — RSD has no concurrent-write-at-scale or multi-instance requirement that SQLite can't satisfy.
- **In-memory EF Core provider (no persistence at all)** — rejected. Data must survive an API restart for the mock-mode-to-API-mode comparison in RSD §5.3 to be meaningful, and for QA agents to test persistence-dependent flows (progress tracking, quiz history).
- **MongoDB / NoSQL** — rejected. The domain (RSD §7 data model) is strictly relational — Users, Courses, Modules, Lessons, Enrollments, all with clear foreign-key relationships and no schema-flexibility requirement. Relational modeling is the natural fit; NoSQL would add impedance mismatch for no benefit here.

## Consequences
- **Positive:** Zero infrastructure to provision; a fresh `git clone` + `dotnet ef database update` gets any Dev/QA agent to a working local DB in one command. CI can run against a throwaway SQLite file with no external service dependency.
- **Negative / tradeoffs accepted:** SQLite has real concurrency limits under heavy concurrent writes — explicitly acceptable at MVP scale, explicitly *not* acceptable if the product moves toward production traffic. This is a known, flagged debt, not an oversight.
- **Migration path (pre-committed, so it's not a future architecture debate):** Because persistence goes exclusively through EF Core, moving to PostgreSQL later is: swap the EF Core provider package, update the connection string, re-run migrations. No LINQ/query code should change. Backend Dev agents must avoid SQLite-specific raw SQL to keep this path clean — call this out in `backend-dev.md`.
