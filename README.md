<div align="center">

# CarRental 5.0 — ASP.NET Core MVC + Three.js

Fast, full‑stack car rental platform with an interactive Three.js landing (no React), robust MVC backend, and PayPal sandbox payments.

<br />

![Build](https://github.com/LeulTew/CarRental-5.0-ThreeJS-MVC/actions/workflows/dotnet.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Identity](https://img.shields.io/badge/Auth-ASP.NET%20Identity-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-SqlServer-512BD4)
![Three.js](https://img.shields.io/badge/Three.js-Landing-black)
![PayPal](https://img.shields.io/badge/Payments-PayPal%20Sandbox-00457C?logo=paypal&logoColor=white)
![Git LFS](https://img.shields.io/badge/Repo-Git%20LFS%20enabled-FF4081)

</div>

## Contents
- Overview
- Demo & Screens
- Features
- Architecture
  - Module map
  - Domain model (ER)
  - Key flows
- Tech stack
- Setup & Run
- Configuration (env vars)
- Data & Migrations
- Routes of interest
- Performance & UX notes
- Repository hygiene (LFS)
- Resume highlights

## Overview
This project showcases a pragmatic end‑to‑end car rental experience:
- Landing page built with raw Three.js and GLTF models for an immersive first impression and excellent TTI (no React runtime cost).
- MVC application for catalog, filtering, booking, payments, reviews, favorites, and role‑based management.

Key paths for quick orientation:
- App root: `Carrental/Carrental`
- Landing: `Views/Home/Index.cshtml` + `wwwroot/Home/assets/js/main.js`
- 3D assets: `wwwroot/car1..car4` (GLTF), content types mapped in `Program.cs`
- Domain + Repos: `Carrental/Carrental/Models`
- Controllers: `Carrental/Carrental/Controllers`
- Payments: `Services/PaypalServices.cs` (+ `UnitOfWork.cs`)

## Demo & Screens
- Home (Three.js, model switcher): `Views/Home/Index.cshtml`
- Catalog & filters: `/Car`
- Payment screen: `Views/Car/ProcessPayment.cshtml`

Placeholders (add your screenshots in `docs/screens/` and update paths):
- Landing: `docs/screens/landing.png`
- Catalog: `docs/screens/catalog.png`
- Details/Booking: `docs/screens/details.png`
- Payment: `docs/screens/payment.png`

## Features
- Three.js landing page with GLTF loader and OrbitControls; responsive model scaling and arrow navigation
- Comprehensive filtering: Make, Model, Type, Color (incl. "Other"), Price range, Availability, Favorites, Sort
- Car details with gallery, versions (for sale), reviews, favorites
- Booking: date range conflict check, total tracking, completion toggle 
- Payments: PayPal sandbox checkout (redirect/return), exchange rate example for ETB->USD
- Auth: ASP.NET Identity + Google OAuth; roles: Admin, Manager, Provider, Customer
- Email: confirmation (HTML template `Views/Home/EmailConfirm.cshtml`) and forgot‑password via SMTP
- Favorites: many‑to‑many between users and cars
- Speech to text endpoint (Google Cloud Speech V1) ready for voice inputs

## Architecture
### Module map
```
Request -> Controller -> Repository/DbContext -> Domain -> ViewModel -> Razor View
								 |-> Services (Payments | Email)
Landing -> Three.js (GLTFLoader) -> wwwroot/car{1..4} models (served with correct MIME types)
```

Key controllers: `Home`, `Car`, `Account`, `Admin`, `Role`, `SpeechToText`

### Domain model (ER)
```
 AppUser 1─* Favorite *─1 Car
	 |                    |
	 |                    *─* Review (to AppUser via UserId)
	 |
	 *─* Booking 1─1 Payment

 Car 1─* CarImage
 Car 1─* CarVersion (for sale variants)
```

Notes:
- `Favorite(UserId, CarId)` composite key; many‑to‑many between users and cars
- `Booking` controls `IsCompleted` and links to `Payment` by `TransactionId`
- Monetary columns mapped as `decimal(18,2)` via `OnModelCreating`

### Key flows
- Search/List: CarController.Index -> `CarFilters` pipeline (availability, sale/rent, make/model, type, color, price, favorites, sort)
- Book (rent): create `Booking`, redirect to `ProcessPayment`, set `Payment.TransactionId`, update `Car.IsBooked`
- Buy now (sale): simplified booking with `IsForSale`, same payment flow
- PayPal: `PayUsingPayPal` -> PayPal approval -> `Success` sets `Booking.TransactionId`, persists `Payment`, toggles booking/car flags
- Email confirmation: `BookingConfirmation` builds HTML from template, sends via SMTP

## Tech stack
- ASP.NET Core MVC (net6.0)
- EF Core (SqlServer), IdentityDbContext<AppUser>
- ASP.NET Identity + Google OAuth
- PayPal .NET SDK (sandbox)
- Three.js + GLTFLoader + OrbitControls
- Razor views, vanilla JS/CSS, Swiper

## Setup & Run
1) Prereqs: .NET 6 SDK and SQL Server
2) Copy config: `Carrental/Carrental/appsettings.example.json` -> `appsettings.Development.json` and fill values
3) Ensure GLTF assets exist under `wwwroot/car1..car4` (tracked by Git LFS)
4) Run the app from `Carrental/Carrental`

### Optional commands (fish shell)
```fish
# restore & run
dotnet restore Carrental/Carrental/Carrental.csproj
dotnet run --project Carrental/Carrental/Carrental.csproj
```

Browse: https://localhost:7022/ (default launch settings may vary)

## Configuration (env vars)
Recommended to use environment variables for secrets in production:
- `ConnectionStrings__MyConnection`
- `GoogleKeys__ClientId`, `GoogleKeys__ClientSecret`
- `AuthMessageSenderOptions__Email`, `AuthMessageSenderOptions__Password`, `AuthMessageSenderOptions__SmtpServer`, `AuthMessageSenderOptions__SmtpPort`
- `PayPal__ClientId`, `PayPal__ClientSecret`

See `appsettings.example.json` for structure.

## Data & Migrations
- DbContext: `CarContext`
- Decimal precision configured for prices/payments
- Many‑to‑many `Favorite` with composite key

Apply migrations (if present) or create new ones:
```fish
dotnet ef database update --project Carrental/Carrental/Carrental.csproj
# or
dotnet ef migrations add Init --project Carrental/Carrental/Carrental.csproj
dotnet ef database update --project Carrental/Carrental/Carrental.csproj
```

## Routes of interest
- Landing: `GET /` (HomeController.Index)
- Catalog: `GET /Car`
- Search: `GET /Car/Search?searchTerm=...`
- Details: `GET /Car/Details/{id}`
- Book: `POST /Car/Book`
- Buy Now: `POST /Car/BuyNow`
- PayPal checkout: `POST /Car/PayUsingPayPal` -> `GET /Car/Success`
- Favorites: `POST /Car/ToggleFavorite`
- Auth: `GET/POST /Account/Login`, Google external login flow
- Speech: `POST /SpeechToText` (binary audio, Google STT)

## Performance & UX notes
- Landing uses pure Three.js with import maps and CDN to minimize bundle size and time‑to‑interactive
- GLTF assets served with correct MIME types via `FileExtensionContentTypeProvider`
- Pagination and filter state preserved via query params

## Repository hygiene (LFS)
- `.gitattributes` tracks images, GLTF/GLB/ BIN and `.bacpac` via Git LFS to keep the repo lean
- `.gitignore` excludes build output, local overrides, and Windows `:Zone.Identifier` artifacts
- The large `Cars/` dataset is ignored by default (not required at runtime)

## Resume highlights
- Built a production‑style MVC application integrating auth, payments, email, and rich catalog filtering
- Implemented a custom Three.js landing for visual impact without a front‑end framework
- Applied repository pattern, clean entities, and precise EF mappings; handled many‑to‑many and monetary fields
- Secured secrets via configuration binding and environment variables; enabled Git LFS for heavy assets

---

See `docs/summary.md` for engineering notes captured during the code read‑through.