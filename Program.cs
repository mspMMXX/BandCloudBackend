var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Standard-Setup
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BandCloudBackend", Version = "v1" });

    // --- FIX f�r File-Uploads ---
    c.OperationFilter<FileUploadOperationFilter>();
});

builder.Services.AddSingleton<BandCloudBackend.Services.BlobStorageService>();

var app = builder.Build();

var cfg = app.Configuration;

app.MapGet("/health/config", () => new
{
    StorageAccountName = cfg["STORAGE_ACCOUNT_NAME"],
    StorageContainer = cfg["STORAGE_CONTAINER"],
    SqlServer = cfg["SQL_SERVER"],
    SqlDatabase = cfg["SQL_DATABASE"]
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
