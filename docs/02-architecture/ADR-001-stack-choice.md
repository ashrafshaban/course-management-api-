# ADR-001: Frontend/Backend Technology Stack

## Status
Accepted

## Context
RSD §6.1 pre-selects React + .NET Core + a "simple database," and the RSD's overarching NFR is a **lightweight, non-over-engineered MVP** (RSD §1, §6.2) with an explicit **mock-mode requirement** (RSD §5) for a static-hostable frontend. This ADR formalizes those choices with rationale, since the stack was named by the Product Owner but not architecturally justified yet — that justification belongs here.

## Decision
- **Backend:** ASP.NET Core Web API on **.NET 8** (current LTS), single project, layered internally (Controllers → Services → Data), no microservices.
- **Frontend:** React 18+ with **Vite** as the build tool, TypeScript throughout, React Router for navigation.
- **API style:** REST over JSON, OpenAPI/Swagger auto-generated — not GraphQL.

## Alternatives Considered
- **GraphQL instead of REST** — rejected. GraphQL's main payoff (flexible client-driven queries, avoiding over/under-fetching) matters at a scale and client-diversity this MVP doesn't have (one frontend, well-known screens). REST + Swagger is simpler to mock (RSD §5 requires request/response shapes the mock layer can mirror 1:1) and simpler for a solo orchestrator to reason about.
- **Microservices (separate services per domain — Users, Courses, Enrollment)** — rejected. No requirement in the RSD implies independent scaling or independent deployment of these domains. A modular monolith gets the same internal separation (via the Services layer) without network calls, distributed tracing, or multi-deploy complexity.
- **Next.js (React framework with SSR) instead of plain Vite+React** — rejected for MVP. SSR/SEO is not an RSD requirement (course catalog SEO is not in scope), and Vite's simpler static-build output maps directly onto the GitHub Pages deployment requirement (RSD §10.1) without extra server-rendering infrastructure.
- **Blazor instead of React** — considered, given the team's .NET background. Rejected because the RSD explicitly specifies React (RSD preferred stack), and React's static-build/GitHub Pages story is more mature/simpler than Blazor WASM's larger payload and more complex Pages-hosting setup.

## Consequences
- **Positive:** Small surface area for a solo orchestrator + agent team to reason about; REST/Swagger gives Backend Dev and Frontend Dev agents a shared, machine-readable contract (`api-contract.md` + generated Swagger JSON) to build against independently and in parallel.
- **Negative / tradeoffs accepted:** If the product later needs true independent scaling of, say, the quiz-grading path under heavy load, splitting the monolith is a real future migration cost — accepted deliberately, since no current requirement justifies paying that cost now.
- **Unblocks:** Backend Dev and Frontend Dev agents can start in parallel once `api-contract.md` and `data-model.md` are committed, since neither needs the other's implementation to begin.
