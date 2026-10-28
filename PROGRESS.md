# PayFlow progress

My build log for PayFlow. I update it at the end of every session.

## Done
- **Oct 23:** Got my MacBook ready: .NET 10, Docker with SQL Server, Rider, VS Code and the GitHub CLI.
- **Oct 26:** Created the GitHub repo and wrote the README. Set up the solution with Clean Architecture:
  Domain, Application, Infrastructure, Api and a test project. I pinned the SDK with global.json so CI
  will use the same version later. Everything builds, the template test passes, and the API answers
  on /weatherforecast.

- **Oct 28:** Built the first real feature: no duplicate payments.
  - Domain: `Payment` with an idempotency key and rules (amount must be more than 0, can't pay
    the same account, 3-letter currency). Completing a payment writes two ledger lines, one debit
    and one credit, for the same amount (double-entry).
  - API: `POST /payments` and `GET /payments/{id}`. If the same `Idempotency-Key` comes twice,
    the first call gets 201 Created and the retry gets 200 OK with the same payment. Storage is
    in memory for now, behind an `IPaymentRepository` interface.
  - Tests: 8 xUnit unit tests for the Domain rules and 3 integration tests with
    WebApplicationFactory. All 11 pass.
  

## Next step
- Day 4: EF Core + SQL Server in Docker. Swap the in-memory repository for a real one, with a
  unique index on IdempotencyKey.

## Blockers
- Nothing right now.