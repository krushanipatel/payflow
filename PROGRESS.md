# PayFlow progress

My build log for PayFlow. I update it at the end of every session.

## Done
- **Oct 23:** Got my MacBook ready: .NET 10, Docker with SQL Server, Rider, VS Code and the GitHub CLI.
- **Oct 26:** Created the GitHub repo and wrote the README. Set up the solution with Clean Architecture:
  Domain, Application, Infrastructure, Api and a test project. I pinned the SDK with global.json so CI
  will use the same version later. Everything builds, the template test passes, and the API answers
  on /weatherforecast.

## Next step
- Add the first real code: Account and Payment in the Domain project, with a few basic rules
  (for example, amount must be more than 0), and my first real unit tests.

## Blockers
- Nothing right now.
