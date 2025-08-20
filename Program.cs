using BandCloudBackend.Data;
using BandCloudBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Service-Registrierungen für Dependency Injection.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger-Dokumentation konfigurieren
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BandCloudBackend", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>(); // Swagger-Fix für Datei-Uploads
});

// Entity Framework Core: SQL Server-Anbindung
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Azure Blob Storage Service (Singleton, da thread-sicher)
builder.Services.AddSingleton<BandCloudBackend.Services.BlobStorageService>();

var app = builder.Build();

/// <summary>
/// Middleware-Pipeline konfigurieren.
/// Swagger wird nur in Development aktiviert.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

/// <summary>
/// Routen-Mapping für Controller.
/// </summary>
app.MapControllers();

/// <summary>
/// Health-Check-Endpunkt für Konfigurationswerte.
/// Zeigt an, welche Werte aus appsettings/Environment geladen wurden.
/// </summary>
var cfg = app.Configuration;
app.MapGet("/health/config", () => new
{
    StorageAccountName = cfg["STORAGE_ACCOUNT_NAME"],
    StorageContainer = cfg["STORAGE_CONTAINER"],
    SqlServer = cfg["SQL_SERVER"],
    SqlDatabase = cfg["SQL_DATABASE"]
});

app.Run();
