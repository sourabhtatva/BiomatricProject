using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Repositories;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Singleton Lifetime Services
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

// Scoped Lifetime Services
builder.Services.AddScoped<IFaceMatchService, FaceMatchService>();
builder.Services.AddScoped<IAwsFaceService, AwsFaceService>();
builder.Services.AddScoped<IUserIdentificationDataService, UserIdentificationDataService>();
builder.Services.AddScoped<IUserIdentificationTypeService, UserIdentificationTypeService>();
builder.Services.AddScoped<IRecognitionLogService, RecognitionLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Transient Lifetime Repositories
builder.Services.AddTransient<IUserIdentificationDataRepository, UserIdentificationDataRepository>();
builder.Services.AddTransient<IRecognitionLogRepository, RecognitionLogRepository>();
builder.Services.AddTransient<IUserIdentificationTypeRepository, UserIdentificationTypeRepository>();
builder.Services.AddTransient<IDocumentValidationResponseRepository, DocumentValidationResponseRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Add authentication and authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
