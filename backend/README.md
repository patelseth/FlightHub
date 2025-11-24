# FlightHub – Flight Information API

FlightHub is a RESTful API for managing flight information using C# and .NET, built with a clean architecture structure and full test coverage. The project demonstrates SOLID principles, CSV-based seeding, EF Core InMemory persistence, logging, rate limiting, and full Swagger/OpenAPI documentation.

---

## ⭐ Tech Stack

- **C#**, **.NET 8/9**
- **ASP.NET Core Web API**
- **Entity Framework Core (InMemory provider)**
- **xUnit**, **Moq**
- **Swagger / OpenAPI**
- **ILogger<T> logging**
- **Rate Limiting Middleware (100 req/min/IP)**

---

## 📁 Solution Structure

```text
backend/
  FlightHub.sln
  src/
    FlightHub.Domain/
    FlightHub.Application/
    FlightHub.Infrastructure/
    FlightHub.Api/
  tests/
    FlightHub.UnitTests/
```

### Layer Responsibilities

- **Domain** – Entities and enums  
- **Application** – Interfaces + business logic  
- **Infrastructure** – EF Core DbContext, repository, seeding  
- **API** – Controllers, DI setup, middleware  
- **Tests** – Unit tests for all layers  

---

## 🧰 Prerequisites

- Install **.NET 8 SDK** (and .NET 9 if running API)  
- Install **Git**

Verify installation:

```
dotnet --version
dotnet --list-sdks
```

---

## 🚀 Getting Started

### 1. Clone and Restore

```
git clone <your-repo-url>.git
cd backend
dotnet restore
```

### 2. Build

```
dotnet build
```

### 3. Run the API

```
cd src/FlightHub.Api
dotnet run
```

Default URL:

```
http://localhost:5213
```

---

## 📦 Database and Seeding

The API uses an **in-memory EF Core database**.

At startup, it seeds data from:

```
src/FlightHub.Infrastructure/Data/FlightInformation.csv
```

To verify:

```
curl http://localhost:5213/api/flights
```

---

## 📘 API Documentation (Swagger)

When running in Development:

- **Swagger UI:**  
  http://localhost:5213/swagger

- **OpenAPI JSON:**  
  http://localhost:5213/swagger/v1/swagger.json

XML documentation is included automatically.

---

## ✈️ API Endpoints Overview

Base URL:

```
http://localhost:5213
```

### GET /api/flights

Get all flights.

### GET /api/flights/{id}

Get a single flight by ID.

### POST /api/flights

Create a new flight.  
Returns **201 Created**.

### PUT /api/flights/{id}

Update existing flight.

### DELETE /api/flights/{id}

Delete a flight.  
Returns **204 No Content** or **404**.

### GET /api/flights/search

Query parameters:

- airline  
- departureAirport  
- arrivalAirport  
- departureFrom  
- departureTo  

---

## ⚠️ Validation & Error Handling

- Required fields on `Flight`
- `ArrivalTime` must be later than `DepartureTime`
- Responses:
  - **400** – invalid request
  - **404** – not found
  - **200/201/204** – success

---

## 📝 Logging

Logged via `ILogger<T>`:

- Controller:
  - Not found warnings
  - Created flight info

- Service:
  - Missing ID warnings

- Repository:
  - Delete-not-found warnings

All logging behavior is validated via Moq in unit tests.

---

## 🔒 Rate Limiting

Implemented using ASP.NET Core Rate Limiting Middleware:

- **100 requests/min per IP**
- Returns **429 Too Many Requests** when exceeded

Middleware is added cleanly via extension method.

---

## 🧪 Running Tests

```
dotnet test
```

Covers:

- Controllers  
- Services  
- Repositories  
- Logging verification  

Expected output:

```
Test summary: total: XX, failed: 0, succeeded: XX
```

---

## 🧠 Design Notes

- Clean Architecture  
- SOLID principles applied  
- DI everywhere  
- Minimal coupling  
- High testability  

---

## 🚧 Possible Improvements

- Pagination  
- Authentication  
- Move to SQL Server/PostgreSQL  
- Integration tests  
- FluentValidation  

---

You now have everything needed to build, run, test, and explore the FlightHub API with seeded data, Swagger, and full documentation.
