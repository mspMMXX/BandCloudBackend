using BandCloudBackend.Data;
using BandCloudBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BandCloudBackend", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>(); // <<< FIX für IFormFile
});

// EF DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Blob Storage Service
builder.Services.AddSingleton<BandCloudBackend.Services.BlobStorageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var cfg = app.Configuration;

app.MapGet("/health/config", () => new
{
    StorageAccountName = cfg["STORAGE_ACCOUNT_NAME"],
    StorageContainer = cfg["STORAGE_CONTAINER"],
    SqlServer = cfg["SQL_SERVER"],
    SqlDatabase = cfg["SQL_DATABASE"]
});

app.Run();
