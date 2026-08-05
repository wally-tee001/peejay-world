# Production checklist (Peejay-world)

Target: **Render** — Web Service (ASP.NET Core) + managed PostgreSQL.

## Database
- [x] EF Core migration `InitialCreate` exists (contact_messages, newsletter_subscriptions).
- [x] Migration-on-startup added in `Program.cs` (`db.Database.MigrateAsync()`).
- [ ] Provision a managed PostgreSQL DB on Render (or your provider).
- [ ] Set `ConnectionStrings__DefaultConnection` env var to the host's connection string (without the password).
- [ ] Set the DB password via env var: `ConnectionStrings__DefaultPassword` / `PGPASSWORD` / `DB_PASSWORD` (kept out of source control).
- [ ] Verify migrations applied on the production DB (first deploy).

## App / Runtime
- [x] `dotnet build` succeeds (0 errors, 0 warnings).
- [x] All routes render (Home, About, PersonalCare, CosmeticsStore, BeautySalon, Contact, Privacy, Error → 200).
- [x] `UseForwardedHeaders` added for correct HTTPS handling behind Render's proxy.
- [x] HSTS, CSP, X-Content-Type-Options, X-Frame-Options, Referrer-Policy, Permissions-Policy set.
- [x] Contact POST endpoint wired with anti-forgery + validation UI.
- [x] Newsletter POST endpoint wired with anti-forgery + dedup + PRG.
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production` in the environment.
- [ ] Verify Serilog file sink path (`Logs/`) is writeable on the host (Render uses ephemeral disk; logs stream to console).

## Deployment (Render)
- [ ] Create a Web Service pointing at the repo; build command `dotnet build -c Release -o out`; start command `dotnet out/peejayworld-mvc.dll`.
- [ ] Create a PostgreSQL instance and link it.
- [ ] Configure env vars (connection string, password, `ASPNETCORE_ENVIRONMENT=Production`, `ASPNETCORE_URLS=http://0.0.0.0:10000`).
- [ ] Add a custom domain + automatic HTTPS (Render provides it).
- [ ] Smoke test: home pages render over HTTPS + contact submission writes a row.

## Monitoring / Ops
- [ ] Confirm Serilog logs appear (console on Render).
- [ ] Set up uptime/health monitoring (UptimeRobot / Render health check on `/Home/Error` or a health endpoint).
- [ ] Configure DB backups (Render managed Postgres offers this).
