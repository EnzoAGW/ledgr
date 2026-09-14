# Ledgr — Interactive Demo

A standalone build of the Ledgr frontend with the entire backend swapped for an
in-browser fake: an HTTP interceptor answers every API call from an in-memory
dataset instead of hitting the real .NET API and Postgres. No server, no
Docker — click around freely, nothing you do here can break anything.

This folder is deployed independently to Vercel; it does not affect `../front`
or `../back`, which still talk to the real backend.

## What's mocked

- `src/app/core/mock/mock-data.ts` — the fake dataset (accounts, categories,
  team, and ~60 days of seeded transactions) plus the business logic that
  would normally live in the API: transaction filtering/pagination, dashboard
  aggregation (monthly totals, last-7-days volume, top spending categories),
  and account balances that actually update when a pending transaction is
  confirmed.
- `src/app/core/interceptors/mock-api.interceptor.ts` — routes every
  `HttpClient` call to the matching mock-data function instead of the
  network, with a randomized delay so it still feels like a real request.

`error.interceptor.ts` (401 → logout) is untouched — it works the same
whether the response came from the real API or the mock. Transaction data
resets on every full page reload (it's an in-memory array, not a database).

## Demo login

| Role | Email | Password |
|---|---|---|
| Admin | `admin@ledgr.dev` | `Admin@123` |
| Manager | `manager@ledgr.dev` | `Manager@123` |
| Analyst | `analyst@ledgr.dev` | `Analyst@123` |

Only Admin can access the Team page — try the other roles to see the guard
redirect back to the dashboard.

## Local preview

```bash
npm install
npm start
```

## Deploying

This folder deploys as-is on Vercel (Root Directory = `demo`). `vercel.json`
sets the build command, output directory, and the SPA rewrite Angular's
router needs.
