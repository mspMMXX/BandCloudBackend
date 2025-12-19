# BandCloud Backend (.NET)

BandCloud Backend is a small **ASP.NET Core Web API** for managing **recording metadata** and uploading audio files to **Azure Blob Storage**.  
It uses **EF Core with SQL Server** and provides a documented API via **Swagger / OpenAPI**.

## Key Features
- REST API for recording-related operations (`RecordingsController`)
- File upload via API (Swagger supports file upload via custom operation filter)
- Store recording metadata in **SQL Server** (EF Core `AppDbContext`)
- Upload/store audio files in **Azure Blob Storage** (`BlobStorageService`)
- Configuration health endpoint: `GET /health/config`

## Tech Stack
- .NET 8 (ASP.NET Core Web API)
- C#
- Entity Framework Core (SQL Server)
- Swagger / OpenAPI
- Azure Blob Storage

## Project Structure
- `Controllers/` – API endpoints  
  - `RecordingsController.cs`
- `Models/` – DTOs & Swagger helpers  
  - `FileUploadDto`  
  - `FileUploadOperationFilter`  
  - `RecordingMetaData`
- `Services/` – infrastructure/services  
  - `AppDbContext` (EF Core)  
  - `BlobStorageService` (Azure Blob Storage)

## Getting Started (Local)

### Prerequisites
- .NET 8 SDK
- Access to a SQL Server instance (local or remote)
- Azure Storage account + container (or emulator)

### 1) Configuration
Set the SQL connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;"
  }
}
```

The application also reads the following values (visible via GET /health/config):

STORAGE_ACCOUNT_NAME

STORAGE_CONTAINER

SQL_SERVER

SQL_DATABASE

These can be provided via environment variables or appsettings.*.json.

### 2) Run the API
dotnet restore
dotnet run

### 3) Swagger UI

Swagger is enabled in Development mode.

After starting the app, open:

https://localhost:<port>/swagger

## API Notes
File upload endpoints are documented in Swagger using FileUploadOperationFilter.

The /health/config endpoint helps verify that configuration values are loaded correctly.

## Roadmap
- Add DTO validation and consistent error responses
- Add authentication/authorization (JWT)
- Add integration tests (WebApplicationFactory)
- Add logging and observability (e.g. Serilog, Application Insights)

## Author
Markus Platter
