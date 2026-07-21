# ledgr

[![CI](https://github.com/EnzoAGW/ledgr/actions/workflows/ci.yml/badge.svg)](https://github.com/EnzoAGW/ledgr/actions/workflows/ci.yml)

B2B fintech dashboard for multi-tenant financial management. Organizations track accounts, transactions, and cash flow through a role-scoped interface with real-time KPIs and charts.

## Features

- **Multi-tenant** — complete data isolation per organization
- **Role hierarchy** — Admin · Manager · Analyst with scoped permissions
- **Dashboard** — KPIs (income, expenses, balance, pending count), 7-day volume chart, top expense categories
- **Transactions** — create, paginate, filter, and update status (Pending → Confirmed / Cancelled)
- **Accounts** — Checking, Credit, and Savings accounts with computed balance
- **Categories** — income and expense categories with color coding
- **Team management** — invite users and assign roles (Admin only)
- **JWT authentication** — 7-day token, role claims enforced on every endpoint

## Stack

| Layer      | Technology                                      |
| ---------- | ----------------------------------------------- |
| Frontend   | Angular 19 · NgRx 19 · Angular Material 19 · ngx-charts 24 |
| Backend    | .NET 10 · ASP.NET Core · MediatR 14 (CQRS)     |
| Database   | PostgreSQL 17 · EF Core 10 (Npgsql)             |
| Auth       | JWT Bearer · BCrypt                             |
| Container  | Docker · Docker Compose                         |
| CI         | GitHub Actions                                  |

## Architecture

```
back/
├── Ledgr.Domain/          # Entities, enums — no dependencies
├── Ledgr.Application/     # MediatR handlers, interfaces, exceptions
├── Ledgr.Infrastructure/  # EF Core, JWT, BCrypt, seed data
├── Ledgr.Api/             # Controllers, middleware, DI wiring
└── Ledgr.Tests/           # xUnit — Application layer unit tests

front/
├── src/app/
│   ├── core/              # Interceptors, guards, services
│   ├── store/             # NgRx feature stores
│   └── features/          # auth · dashboard · transactions · accounts · categories · team
```

Clean Architecture dependency rule: `Domain ← Application ← Infrastructure → Api`.

## Running with Docker

```bash
cp .env.example .env
# edit .env — set POSTGRES_PASSWORD and JWT_SECRET

docker compose up --build
```

| Service  | URL                      |
| -------- | ------------------------ |
| Frontend | http://localhost         |
| API      | http://localhost:5142    |
| Postgres | localhost:5432           |

The API automatically runs EF Core migrations and seeds demo data on first start.

### Demo accounts

| Email               | Password | Role    | Org      |
| ------------------- | -------- | ------- | -------- |
| alice@techcorp.dev  | admin123 | Admin   | TechCorp |
| bruno@techcorp.dev  | mgr123   | Manager | TechCorp |
| carla@techcorp.dev  | ana123   | Analyst | TechCorp |
| diego@retailco.dev  | admin123 | Admin   | RetailCo |
| elena@retailco.dev  | mgr123   | Manager | RetailCo |
| fabio@retailco.dev  | ana123   | Analyst | RetailCo |

## Running locally (without Docker)

**Prerequisites:** .NET 10 SDK, Node 22, PostgreSQL 17

```bash
# Backend
cd back
dotnet restore
dotnet run --project Ledgr.Api
# API available at https://localhost:7xxx / http://localhost:5xxx

# Frontend (separate terminal)
cd front
npm install --legacy-peer-deps
ng serve
# App available at http://localhost:4200
```

Set the connection string in `back/Ledgr.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=ledgr;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "your-local-dev-secret-min-32-chars"
  }
}
```

## API Reference

All endpoints require `Authorization: Bearer <token>` except `/api/auth/login`.

### Auth

| Method | Endpoint          | Body                        | Description    |
| ------ | ----------------- | --------------------------- | -------------- |
| POST   | /api/auth/login   | `{ email, password }`       | Returns JWT    |

### Dashboard

| Method | Endpoint        | Description                                  |
| ------ | --------------- | -------------------------------------------- |
| GET    | /api/dashboard  | KPIs, 7-day volume series, top categories    |

### Transactions

| Method | Endpoint                            | Description                                  |
| ------ | ----------------------------------- | -------------------------------------------- |
| GET    | /api/transactions                   | Paginated list — filters: search, accountId, categoryId, type, status, from, to |
| POST   | /api/transactions                   | Create transaction                           |
| PATCH  | /api/transactions/{id}/status       | Update status (Pending → Confirmed/Cancelled)|

### Accounts

| Method | Endpoint          | Description              |
| ------ | ----------------- | ------------------------ |
| GET    | /api/accounts     | List org accounts        |
| POST   | /api/accounts     | Create account (Manager+)|

### Categories

| Method | Endpoint          | Description                  |
| ------ | ----------------- | ---------------------------- |
| GET    | /api/categories   | List org categories          |
| POST   | /api/categories   | Create category (Manager+)   |

### Team *(Admin only)*

| Method | Endpoint        | Description           |
| ------ | --------------- | --------------------- |
| GET    | /api/team       | List org members      |
| POST   | /api/team       | Invite user           |

## Tests

```bash
cd back
dotnet test
```

Unit tests cover the Application layer handlers: `CreateTransaction`, `Login`, `GetDashboard`, `InviteUser`, `UpdateTransactionStatus`.

## CI

GitHub Actions runs on every push to `main` / `develop` and on pull requests:

1. **Backend** — `dotnet build` + `dotnet test` against a live PostgreSQL service container
2. **Frontend** — `npm ci` + `ng build --configuration production`
3. **Docker** — `docker compose build` (main branch only, after both jobs pass)
