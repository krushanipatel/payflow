# PayFlow

A small version of a bank's back office: it takes payments from an app or from a business's payroll file, checks each one for mistakes, duplicates and fraud before any money moves, and gives the operations team a daily report proving every dollar adds up.

**This is a portfolio project, not a live bank system.** I'm building it to show how I'd solve the problems bank and credit union payment teams deal with every day.

**Status:** work in progress. I'm building it step by step and sharing progress on LinkedIn. See [PROGRESS.md](PROGRESS.md) for my build diary.

## The problem
Banks and credit unions take payments from two places: one-off transfers from customers, and batch files from business clients (payroll, supplier payments). Before money moves, someone has to check every payment for mistakes, duplicates and fraud, explain why a payment was held, and prove at the end of the day that the books balance. Done by hand or across separate tools, that leads to double payments, unexplained holds and slow end-of-day checks.

## Who it's for
- **Priya, a payments operations analyst at a credit union.** She reviews held payments and signs off the end-of-day report.
- **A small business owner** who uploads a payroll file and needs to know which rows went through and why the others didn't.

## What I'll prove
- A payment sent twice with the same idempotency key is posted exactly once.
- A 1,000-row batch file is processed locally in seconds, and every bad row comes back with its line number and reason.
- End-of-day reconciliation shows total debits = total credits, with zero unexplained differences.
- Every held payment has a reason a non-technical person can read.

These are measurements from my own demo and tests, not production numbers.

## Planned features
- REST API to send payments between accounts, with idempotency keys
- Double-entry ledger (balances are calculated from debit and credit lines, never edited directly)
- Rules before money moves: amount limit, duplicate check, velocity
- Batch payroll uploads in simplified Canadian formats (CPA 005 fixed-width, ISO 20022 pain.001 XML)
- Daily reconciliation report and audit log
- "Explain" button: Azure OpenAI writes a plain-English reason for a held payment (AI explains, people decide)
- Angular dashboard for the operations team

## Roadmap
I'm building one thin slice that works end to end first, then adding layers.

| Milestone | What you'll be able to see |
|---|---|
| M1 Thin slice | Send a payment by API, rules run, ledger updates, check its status. Runs with `docker compose up`. |
| M2 Batch files | Upload a payroll file and get per-row results |
| M3 Queue + Function | Processing moves to Azure Service Bus + Azure Functions, with retries and a dead-letter queue |
| M4 Kafka events | Completed payments are published as events; reconciliation and notifications read them independently |
| M5 Dashboard | Angular dashboard with Entra ID login and Playwright tests |
| M6 AI explain | Azure OpenAI explanation for held payments |
| M7 Cloud | Bicep + GitHub Actions deploy to Azure Container Apps, plus an AKS deployment exercise |

## Planned tech stack
- **Backend:** C#, .NET 10 Web API, Clean Architecture, EF Core, SQL Server / Azure SQL
- **Messaging:** Azure Service Bus for commands ("process this payment"), Apache Kafka for events ("this payment completed")
- **Cloud (Azure):** Azure Functions (isolated worker), Logic Apps, Blob Storage, Key Vault, Application Insights
- **AI:** Azure OpenAI to explain held payments
- **Security:** Microsoft Entra ID, JWT with roles
- **Front end:** Angular, TypeScript
- **Testing:** xUnit, Moq, Testcontainers, Playwright, Postman
- **DevOps:** Docker, GitHub Actions CI/CD, Bicep, Azure Container Apps (live demo), Kubernetes (AKS)

## Built so far
- Clean Architecture solution on .NET 10 (Domain, Application, Infrastructure, Api, tests)
- `POST /payments` with an `Idempotency-Key` header: the same payment sent twice is saved once
  (201 Created the first time, 200 OK with the same payment on a retry). `GET /payments/{id}` too.
- Double-entry ledger in the Domain: completing a payment writes one debit and one credit line
- Payment rules in the Domain: amount above 0, different accounts, 3-letter currency
- 11 automated tests: 8 xUnit unit tests + 3 API integration tests with WebApplicationFactory
- Storage is in memory for now; SQL Server with EF Core is next

## Run it
    dotnet test                              # 11 tests
    dotnet run --project src/PayFlow.Api     # API on http://localhost:5201
    
## How it's built
This is a portfolio and learning project. AI tools (GitHub Copilot and Claude) help me with planning and code review; I write, run and test the code myself.
