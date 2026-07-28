# ADR-003: Authentication & Authorization Strategy

## Status
Accepted

## Context
RSD §4.1 (AUTH-01 through AUTH-07) requires registration, login, hashed passwords, and role-based access for three roles (Admin/Instructor/Student), enforced on both API routes and frontend routes. RSD §5 additionally requires a **mock mode** where auth must be fully simulated with seeded users, with identical UI behavior to real auth. RSD §9 (NFRs) explicitly says short-lived tokens are acceptable and full refresh-token flows are not required for MVP.

## Decision
- **Mechanism:** JWT Bearer tokens, issued by ASP.NET Core Identity + a custom token endpoint, no refresh-token flow in MVP (matches RSD §9 explicitly).
- **Password storage:** ASP.NET Core Identity's built-in hasher (PBKDF2-based), never custom crypto.
- **Authorization:** Role claims embedded in the JWT (`Admin` / `Instructor` / `Student`), enforced via `[Authorize(Roles = "...")]` on controllers and a parallel `ProtectedRoute` wrapper component on the frontend keyed off the decoded token's role claim.
- **Mock mode parity:** the mock auth service issues a fake but structurally identical token (same claims shape) so `ProtectedRoute` logic never needs to know which mode it's running in — this is the concrete mechanism satisfying RSD §5.2's "zero UI code changes" requirement for auth specifically.

## Alternatives Considered
- **Cookie-based session auth** — rejected. Sessions require server-side state (or a distributed cache) which conflicts with the "no session server" simplicity goal in RSD §6.2, and complicates the mock-mode requirement, since a static GitHub Pages site cannot participate in server-set cookies at all.
- **Full OAuth2/OIDC via a third-party identity provider (Auth0, Entra ID, etc.)** — rejected for MVP. Adds an external paid/rate-limited dependency and registration/login complexity with no RSD requirement (single-org, no SSO requirement, RSD §2.2 explicitly excludes multi-tenancy) to justify it. Revisit only if a future requirement demands SSO.
- **Refresh tokens + short-lived access tokens** — rejected for MVP per RSD §9's explicit allowance; noted here as a deliberate, requirement-backed simplification rather than a gap.

## Consequences
- **Positive:** Stateless auth fits the "no session server" principle; mock/API parity is achievable because both modes produce the same JWT *shape*, so frontend role-guarding logic is 100% shared code.
- **Negative / tradeoffs accepted:** No token revocation before natural expiry (e.g., can't force-logout a deactivated user instantly) — acceptable at MVP scale, must be flagged to the orchestrator before this becomes a real product with real users' data at stake.
- **Unblocks:** Frontend Dev agent can build `AuthContext` and `ProtectedRoute` once, against the token *shape* defined here, before the real backend even exists — enabling true mock/API parallel development per RSD §5.
