using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Repositories;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services;
using BiometricAuthenticationAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Create HTTP Client
builder.Services.AddHttpClient<AzureFaceService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AzureFaceApi:Endpoint"]);
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", builder.Configuration["AzureFaceApi:Key"]);
});

// Add services to the container.
//builder.Services.AddSingleton<IAzureFaceService, AzureFaceService>();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

builder.Services.AddScoped<IFaceMatchService, FaceMatchService>();
builder.Services.AddScoped<IAwsFaceService, AwsFaceService>();
builder.Services.AddScoped<IUserIdentificationDataService, UserIdentificationDataService>();
builder.Services.AddScoped<IUserIdentificationTypeService, UserIdentificationTypeService>();
builder.Services.AddScoped<IRecognitionLogService, RecognitionLogService>();

builder.Services.AddTransient<IUserIdentificationDataRepository, UserIdentificationDataRepository>();
builder.Services.AddTransient<IRecognitionLogRepository, RecognitionLogRepository>();
builder.Services.AddTransient<IUserIdentificationTypeRepository, UserIdentificationTypeRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();


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
