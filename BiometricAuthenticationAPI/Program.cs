using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Repositories;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services;
using BiometricAuthenticationAPI.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Singleton Lifetime Services
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

// Scoped Lifetime Services
builder.Services.AddScoped<IFaceMatchService, FaceMatchService>();
builder.Services.AddScoped<IAwsFaceService, AwsFaceService>();
builder.Services.AddScoped<IUserIdentificationDataService, UserIdentificationDataService>();
builder.Services.AddScoped<IRecognitionLogService, RecognitionLogService>();

// Transient Lifetime Repositories
builder.Services.AddTransient<IRecognitionLogRepository, RecognitionLogRepository>();
builder.Services.AddTransient<IDocumentValidationResponseRepository, DocumentValidationResponseRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Configure Serilog for file logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Error() // Set the minimum log level to Error
        .WriteTo.File(SystemConstants.Configuration.LOG_FILE_PATH);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
