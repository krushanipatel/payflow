# PayFlow

A payment-processing platform (bank back-office style): accounts, payments, batch files, fraud rules and a dashboard.

**Status:** work in progress. I'm building it step by step and sharing progress on LinkedIn.

## Planned tech stack
- **Backend:** C#, .NET 10 Web API, Clean Architecture, EF Core, SQL Server / Azure SQL
- **Messaging:** Azure Service Bus, Apache Kafka (event streaming)
- **Cloud (Azure):** Azure Functions (isolated worker), Logic Apps, Blob Storage, Key Vault, Application Insights
- **AI:** Azure OpenAI to explain flagged payments
- **Security:** Microsoft Entra ID, JWT with roles
- **Front end:** Angular, TypeScript
- **Testing:** xUnit, Moq, Testcontainers, Playwright, Postman
- **DevOps:** Docker, Kubernetes (AKS), GitHub Actions CI/CD, Bicep, Azure Container Apps

## Planned features
- REST API to send payments between accounts
- Event-driven payment processing with Service Bus, Kafka and Azure Functions
- Batch payment uploads (CSV / XML) and simple fraud rules
- Daily reconciliation report and audit log
- Angular dashboard for payments and statuses

## Built so far
- Repository set up (more coming each week)

## How it's built
This is a portfolio and learning project. AI tools (GitHub Copilot and Claude) help me with planning and code review; I write, run and test the code myself.
