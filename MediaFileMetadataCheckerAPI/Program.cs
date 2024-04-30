using Microsoft.EntityFrameworkCore;
using MediaFileMetadataCheckerAPI.Models;
using MediaFileMetadataCheckerAPI.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.AppConfiguration.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Register in-memory DBs
builder.Services.AddDbContext<FileUploadContext>(opt =>
    opt.UseInMemoryDatabase("FileUploadList"));
// Get connection string from Environment variables
string? connectionString = Environment.GetEnvironmentVariable("METADATA_API_CONFIG_CONNECTION_STRING");
double? configCacheExpiration = Convert.ToDouble(Environment.GetEnvironmentVariable("METADATA_API_CONFIG_CACHE_EXPIRATION"));

// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
        // Load all keys that start with `MetadataApp:` and have no label
        .Select("MetadataApp:*")
        // Configure to reload configuration if the registered sentinel key is modified
        .ConfigureRefresh(refreshOptions =>
            refreshOptions.Register("MetadataApp:Settings:Sentinel", refreshAll: true)
                .SetCacheExpiration(TimeSpan.FromHours(configCacheExpiration ?? 1)));
});
// Bind configuration "MetadataApp:Settings" section to the Settings object
builder.Services.Configure<MetadataAppConfig.Settings>(builder.Configuration.GetSection("MetadataApp:Settings"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadFilter>();
    // options.OperationFilter<FileDownloadFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Use Azure App Configuration middleware for dynamic configuration refresh.
app.UseAzureAppConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
