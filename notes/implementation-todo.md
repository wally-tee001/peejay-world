# Implementation track — getting Peejay-world to production

Target deployment: **Render** (Free tier) — Web Service + managed PostgreSQL.

## Steps
- [x] Review project, run it, smoke-test all routes (all → 200)
- [x] Step A: Add `UseForwardedHeaders` + DB migration-on-startup (`Program.cs`)
- [x] Step B: Fix production `Error.cshtml` (remove dev boilerplate)
- [x] Step C: Apply migrations + smoke-test contact submission
- [x] Step C2: Discovered + fixed missing `newsletter_subscriptions` table (added `AddNewsletterTable` migration); applied DB update successfully
- [x] Step C3: Smoke-tested contact POST + newsletter POST — both wrote rows to PG (confirmed via Serilog logs)
- [x] Step D: Update `TODO.md` + `production-checklist.md`
- [x] Step E: Add Render deployment config (render.yaml) + docs

## Notes
- Render free tier uses ephemeral disk → Serilog `Logs/` file sink won't persist, but logs also stream to console.
- `render.yaml` uses Render's managed Postgres `fromDatabase` connection string (includes password), so no separate password env var needed.
