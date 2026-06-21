# Production checklist (Peejay-world)

- [ ] Add `ConnectionStrings` for PostgreSQL (and keep secrets out of source control).
- [ ] Run `dotnet ef migrations add InitialCreate` and `dotnet ef database update`.
- [ ] Verify Serilog sink path is writeable on the host.
- [ ] Ensure contact form POST endpoint is wired + anti-forgery works.
- [ ] Add security headers (HSTS, CSP, X-Content-Type-Options, etc.)
- [ ] Configure environment variables in the deployment platform.
- [ ] Smoke test: home pages render + contact submission writes a row.

