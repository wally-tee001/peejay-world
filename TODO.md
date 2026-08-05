# TODO - Peejay-world project hardening

## Current Status (2026-05-07)

### Completed
- [x] Deleted stray duplicate `peejayworld-mvc/peejayworld-mvc/` folder + empty migration
- [x] Fixed `.gitignore`: tracks `appsettings.json` (no secrets), ignores `*.Development.json` / `*.local.json`
- [x] Removed plaintext DB password from tracked `appsettings.json`
- [x] Added runtime env-var password support (`PGPASSWORD`, `DB_PASSWORD`, `ConnectionStrings__DefaultPassword`)
- [x] Build succeeds — 0 errors, 0 warnings
- [x] Step 3: Added security headers — HSTS (365d + preload), CSP, X-Content-Type-Options, X-Frame-Options, Referrer-Policy, Permissions-Policy

### Remaining (ordered)
- [x] Step 4: Reconcile two Contact views — deleted dead `Home/Contact.cshtml`, removed `HomeController.Contact()` action, pointed nav/footer/Index to `Contact/Index`
- [x] Step 5: Add validation UI — `asp-validation-summary`, `asp-validation-for`, validation scripts partial, CSS validation styles, layout `RenderSectionAsync("Scripts")`
- [x] Step 6: Write a real Privacy Policy
- [x] Step 7: Wire up or remove the newsletter form
- [x] Step 8: Add `UseForwardedHeaders` + DB migration-on-startup logic
- [x] Step 9: Build + run, smoke-test all routes (all → 200); contact submission pending DB write check
- [x] Step 10: Fix `Error.cshtml` "Development Mode" section for production
- [x] Step 11: Decide deployment approach (Render) + estimate time-to-production (~1–3 days)

