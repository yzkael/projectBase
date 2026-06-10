# [Project Name] Web API

> A robust, production-ready ASP.NET Core 10.0 Web API boilerplate pre-configured with Identity, Audit logging, PostgreSQL, JWT Authentication, and Scalar OpenAPI documentation.

---

## 🛠️ Tech Stack & Features

This boilerplate comes pre-configured with an enterprise-grade stack, saving you hours of initial setup:

*   **Framework:** .NET 10.0 (Web SDK) with implicit usings and nullable checks enabled.
*   **Database & ORM:** PostgreSQL via Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`).
*   **Authentication & Identity:** ASP.NET Core Identity with EF Core integration and JWT Bearer Token validation.
*   **Audit Logging:** Comprehensive change-tracking via `Audit.NET` (tracking EF Core, Identity, and saving audit trails directly to PostgreSQL).
*   **Application Logging:** Structured logging using `Serilog`, configured to sink to the Console, local Files, and a PostgreSQL database table.
*   **Localization:** JSON-based localization powered by `Askmethat.Aspnet.JsonLocalizer`.
*   **API Documentation:** Modern, interactive API docs using `Scalar.AspNetCore` (built on top of Microsoft OpenApi).

---

