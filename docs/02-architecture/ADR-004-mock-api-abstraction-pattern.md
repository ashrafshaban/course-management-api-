# ADR-004: Frontend Mock/API Service Abstraction Pattern

## Status
Accepted

## Context
RSD §5 makes Mock Mode a first-class MVP requirement: the frontend must run fully against mock data with zero backend, switchable to the real API with **zero UI code changes**, deployable to GitHub Pages in mock mode (RSD §10.1). This ADR formalizes the exact mechanism, expanding RSD §5.2's sketch into a binding contract for the Frontend Dev agent.

## Decision
- Define one **TypeScript interface per domain area** under `src/services/interfaces/` (e.g. `ICourseService`, `IAuthService`, `IEnrollmentService`, `IQuizService`), each method signature matching `api-contract.md` 1:1.
- Implement two providers per interface: `src/services/mock/*.service.ts` and `src/services/api/*.service.ts`.
- A single factory module, `src/services/index.ts`, selects the implementation via `import.meta.env.VITE_APP_MODE` (`'mock' | 'api'`).
- **Enforced rule:** application code (pages, components, hooks) imports exclusively from `services/index.ts`. Direct imports from `services/mock/*` or `services/api/*` outside of `services/index.ts` and their own test files are treated as a code-review rejection, not a style nit.
- Mock services read/write an in-memory store seeded from `src/mocks/fixtures/*.json`, persisted to the browser's `localStorage` for session continuity, and simulate 300–600ms latency plus occasional error responses so loading/error UI states are exercised identically in both modes.

## Alternatives Considered
- **Feature flags / conditionals inside components** (`if (mockMode) {...} else {...}`) — rejected outright. This is exactly the anti-pattern the RSD's "zero UI code changes" requirement (§5.2) is designed to prevent; it would scatter mode-awareness across the codebase instead of isolating it to one factory file.
- **A local mock backend process (e.g., JSON Server) instead of an in-browser mock layer** — rejected. Doesn't satisfy the "runs fully static on GitHub Pages" requirement (RSD §10.1), since GitHub Pages cannot run any server process at all.
- **MSW (Mock Service Worker) intercepting real fetch calls** — considered as a legitimate alternative. Not chosen as the *primary* mechanism because it still requires the app to always call `api/*` code paths and intercept at the network layer, which is a valid pattern but adds a service-worker layer of indirection the simpler factory-function approach doesn't need. Worth revisiting if the team later wants network-level mocking for other purposes (e.g., Cypress/Playwright test mocking) — that can coexist with this ADR's approach, not replace it.

## Consequences
- **Positive:** Switching modes is a `.env` file change only, verified by RSD §12's acceptance criteria; Frontend Dev and Backend Dev agents can work fully in parallel since the frontend never blocks on a running backend.
- **Negative / tradeoffs accepted:** Two implementations per service interface roughly doubles the service-layer code volume versus a single API-only client. Accepted because the RSD treats demo-ability without a backend as a core product requirement, not a nice-to-have.
- **Unblocks:** The GitHub Actions GitHub Pages deployment (RSD §10.1) becomes a pure static build with `VITE_APP_MODE=mock` baked in at build time — no server, no secrets, no backend dependency in that pipeline at all.
