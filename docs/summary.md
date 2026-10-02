# Working summary

We are documenting the CarRental 5.0 project as we go, toward an A++ README. This file captures detailed notes and findings.

## Architecture
- ASP.NET Core MVC (net6.0) solution at `Carrental/Carrental`
- EF Core with SQL Server (`CarContext : IdentityDbContext<AppUser>`) and Identity tables
- Repositories pattern for Cars, Bookings, Payments, Favorites, Reviews, CarVersions; Filters for composite querying
- Google OAuth configured via `AddGoogle()`; SMTP email via `AuthMessageSenderOptions`
- PayPal sandbox handled via `PaypalServices` + `UnitOfWork`
- Three.js landing page: `Views/Home/Index.cshtml` and `wwwroot/Home/assets/js/main.js`; model folders `wwwroot/car1..car4`

## Key flows
- Car listing and search with multiple filters, favorites toggle, pagination
- Car details: versions (for sale), reviews, favorites; booking date ranges checked against existing bookings
- Booking flow: adds Booking, redirects to payment; supports PayPal and direct (view) processing
- Payment flow: updates Booking.TransactionId and completion status; toggles Car.IsBooked appropriately
- Email: booking confirmation with HTML template `Views/Home/EmailConfirm.cshtml`; forgot-password email via SMTP
- Roles: Admin, Manager, Provider, Customer; RoleController for management

## Security and configs
- Replaced hard-coded Google ClientSecret and SMTP creds with configuration bindings
- Program.cs: `AuthMessageSenderOptions` bound from `appsettings` section
- EmailSender now consumes `IOptions<AuthMessageSenderOptions>`
- Sensitive values should come from environment variables/app secrets in production

## Large assets
- `.gitattributes` marks images, 3D models, fonts, bacpac dumps, and videos as binary. They are plain Git blobs (no LFS), so clones run as-is and GitHub Pages can serve `docs/assets`
- `wwwroot` assets, including all four GLTF models, are committed. Raw design sources (`Cars/`) live in the `v5.0.0-legacy-desktop` release
- `db/CarR.bacpac` is a sanitized sample database (password `Demo@123` for every account; admin `demoacc2@email.com`)
- `.gitignore` excludes build outputs and local overrides; `:Zone.Identifier` metadata ignored

## Open items
- Document DB schema and relationships succinctly
- Add run instructions (dotnet, EF migrations), seed guidance, and screenshots
- Validate PayPal sandbox notes and exchange rate step
- Add badges (build, license), and a concise ASCII component diagram
