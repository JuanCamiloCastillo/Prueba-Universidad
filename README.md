# Student Enrollment System

A full-stack student enrollment web application built with .NET 8 Clean Architecture backend and Angular 17 frontend.

## Architecture Overview

### Tech Stack
- **Backend**: .NET 8 Web API (Clean Architecture)
- **Frontend**: Angular 17 (Standalone Components, Lazy Loading)
- **Database**: SQL Server (EF Core 8)
- **Auth**: JWT Bearer Tokens
- **Cache**: Redis (StackExchange.Redis)

### Project Structure
```
├── backend/                        # .NET 8 solution
│   ├── StudentEnrollment.Domain/   # Entities, Domain Exceptions
│   ├── StudentEnrollment.Application/ # CQRS Commands/Queries (MediatR), Interfaces
│   ├── StudentEnrollment.Infrastructure/ # EF Core, Repositories, Redis, JWT
│   └── StudentEnrollment.API/      # Controllers, Middleware, Program.cs
├── frontend/                       # Angular 17 app
│   └── src/app/
│       ├── core/                   # Services, Interceptors, Guards
│       └── features/               # Auth, Dashboard, Enrollment, Classmates
├── database/                       # SQL scripts
│   ├── 01_schema.sql
│   ├── 02_seed.sql
│   └── 03_stored_procedures.sql
└── .github/workflows/deploy.yml    # CI/CD pipeline
```

## Architecture Decisions

### Why Modular Monolith over Microservices
For a university enrollment system of this scale, a modular monolith provides simpler deployment, easier transactions (critical for enrollment concurrency), and lower operational overhead. The Clean Architecture layers (Domain → Application → Infrastructure → API) enforce strong separation of concerns without the network latency and distributed transaction complexity of microservices. If scale demands it later, each bounded context (Auth, Enrollment, Subjects) can be extracted independently.

### Why Pessimistic Locking (UPDLOCK) for Enrollment Concurrency
Enrollment has a strict capacity constraint (MaxCapacity = 30). With optimistic locking, two students could both read "29/30 enrolled" and both attempt to insert, exceeding capacity. `UPDLOCK + ROWLOCK` in `sp_EnrollStudent` acquires an exclusive lock on the subject row during the check-then-insert operation, guaranteeing serialized access. The performance cost is acceptable given enrollment windows are time-bounded and high-contention is short-lived.

### Why Stored Procedures Only for Critical Operations
`sp_EnrollStudent` encapsulates all business rule checks (3-subject limit, duplicate professor, capacity) and the insert in a single atomic transaction with pessimistic locking — impossible to replicate safely with EF Core alone. `sp_GetClassmates` enforces the data privacy rule (only FirstName + LastName) at the database level. All standard CRUD (student registration, subject listing) uses EF Core for developer productivity and maintainability.

### Why JWT Bearer Tokens
Stateless authentication scales horizontally without shared session storage. The `studentId` claim is read server-side from the token — the client never passes it as a parameter, preventing IDOR attacks.

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | ❌ | Register student account |
| POST | `/api/auth/login` | ❌ | Returns JWT token |
| GET | `/api/subjects` | ✅ | List subjects with professor and available spots |
| POST | `/api/enrollments` | ✅ | Enroll in a subject (calls sp_EnrollStudent) |
| GET | `/api/enrollments/my` | ✅ | Current student's enrollments |
| GET | `/api/enrollments/classmates/{subjectId}` | ✅ | Classmates (FirstName + LastName only) |
| GET | `/api/students` | ✅ | List all students (public names only) |

## Security

- JWT tokens signed with HMAC-SHA256
- Passwords hashed with BCrypt (never plain text)
- Rate limiting: 10 requests/second per user (.NET 8 native RateLimiter)
- CORS configured for frontend origin only
- HTTPS redirect enforced
- FluentValidation on all Commands
- studentId always read from JWT claims, never from client parameters

## Business Rules

1. Maximum 3 subjects per student
2. Student cannot have two classes with the same professor
3. Student can only see classmates' FirstName + LastName (never email)
4. Passwords hashed with BCrypt

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Redis
- Node.js 20+

### Database Setup
```bash
# Run scripts in order
sqlcmd -S localhost -d StudentEnrollmentDb -i database/01_schema.sql
sqlcmd -S localhost -d StudentEnrollmentDb -i database/02_seed.sql
sqlcmd -S localhost -d StudentEnrollmentDb -i database/03_stored_procedures.sql
```

### Backend
```bash
cd backend
dotnet restore
# Update appsettings.json with your connection strings
dotnet run --project StudentEnrollment.API
```

### Frontend
```bash
cd frontend
npm install
ng serve
```

## Deployment URLs
- **Backend API**: `https://YOUR_AZURE_API_APP_NAME.azurewebsites.net`
- **Frontend**: `https://YOUR_AZURE_STATIC_WEB_APP_URL.azurestaticapps.net`

## GitHub Secrets Required
- `AZURE_API_APP_NAME` - Azure App Service name
- `AZURE_API_PUBLISH_PROFILE` - Azure App Service publish profile
- `AZURE_STATIC_WEB_APPS_API_TOKEN` - Azure Static Web Apps deployment token