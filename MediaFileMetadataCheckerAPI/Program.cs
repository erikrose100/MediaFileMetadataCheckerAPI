using Microsoft.EntityFrameworkCore;
using MediaFileMetadataCheckerAPI.Models;
using MediaFileMetadataCheckerAPI.Filters;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Register in-memory DBs
builder.Services.AddDbContext<FileUploadContext>(opt =>
    opt.UseInMemoryDatabase("FileUploadList"));
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
