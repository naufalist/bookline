<div id="top"></div>

<!-- PROJECT TITLE -->
<br />
<div align="center">
  <h3 align="center">Bookline - Appointment Booking Platform</h3>

  <p align="center">
    A simple web-based appointment booking system built with .NET and PostgreSQL. Customers can register and book appointments with agencies through a minimal frontend. Backend is hosted on Azure App Service and frontend via Azure Static Web Apps.
    <br />
    <a href="https://ashy-field-0d34dc710.6.azurestaticapps.net/" target="_blank">View Frontend</a>
    ·
    <a href="https://bookline-api-d8dza8fchadmhze7.indonesiacentral-01.azurewebsites.net/index.html" target="_blank">API Swagger</a>
    ·
    <a href="https://github.com/naufalist/bookline/issues">Report Bug</a>
    ·
    <a href="https://github.com/naufalist/bookline/issues">Request Feature</a>
  </p>
</div>

## Note

- __2025-05-09__  
  This project is still under active development. Agency-side authentication and admin features are not implemented yet. Public-facing endpoints such as `GET /api/appointments` do not support pagination or query limits, which may impact scalability. For more details, please refer to the Known Limitations section below.

<p align="right">(<a href="#top">back to top</a>)</p>

---

## About

Bookline consists of a .NET Web API backend and a static HTML/JS frontend for online appointment booking. Customers can submit a booking date and receive a token like `BL001`. Admins (agencies) can later view appointments and configure available days.

## Built With

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL via Neon](https://neon.tech/)
- [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)
- [Azure Static Web Apps](https://azure.microsoft.com/en-us/products/app-service/static)

<p align="right">(<a href="#top">back to top</a>)</p>

---

## Installation

```bash
git clone https://github.com/naufalist/bookline.git
cd bookline
```

### Backend API

1. Navigate to `bookline-api/Bookline.Api`
2. Add a file named `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=bookline_db;Username=postgres;Password=yourpassword"
  }
}
```

3. Run the API:
```bash
dotnet run --project bookline-api/Bookline.Api
```

### Frontend

Just open `bookline-web/index.html` in your browser, or serve it using any static server:

```bash
cd bookline-web
npx serve .
```

<p align="right">(<a href="#top">back to top</a>)</p>

---

## Deployment

### Backend (API)
- Hosted on Azure App Service (Free F1 Plan)
- Deployed automatically via GitHub Actions on push to `main`
- Connection string managed in Azure > Configuration > Connection Strings

### Frontend (Static HTML)
- Deployed via Azure Static Web Apps
- Folder `bookline-web/` is served directly (no build step required)

API URL switching is handled in `api-constants.js` using `window.location.hostname` to distinguish environments.

<p align="right">(<a href="#top">back to top</a>)</p>

---

## Usage

- Register customer at `customer-register.html`
- Book appointment via `customer-booking.html`
- View booked appointments in `appointment-list.html`
- Manage public holiday & max booking slot via `day-config.html`

Token format: `BL001` through `BL099` depending on available slots per day.

<p align="right">(<a href="#top">back to top</a>)</p>

---

## Known Limitations

- No admin or agency login system yet
- Public API endpoints return full data without pagination
- Token assignment is sequential but not validated for uniqueness per day
- Tokens are sequentially generated up to a maximum of 100 per day (from BL001 to BL100).
- No rate limiting, validation, or retry protection
- Available appointment days up to a maximum of 30 days ahead, calculated from the date input by the customer

<p align="right">(<a href="#top">back to top</a>)</p>

---

## Contributing

1. Fork the repo
2. Create a feature branch (`git checkout -b feat/my-feature`)
3. Commit with semantic commit messages (Conventional Commits)
4. Push and open a pull request

<p align="right">(<a href="#top">back to top</a>)</p>

---t

## Contact

[@naufalist](https://twitter.com/naufalist) — contact.naufalist@gmail.com

<p align="right">(<a href="#top">back to top</a>)</p>