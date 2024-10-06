using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using service.Application.Repository.AccountManagement;
using service.Application.Repository.KeyManagement;
using service.Application.Repository.UserManagement;
using service.Application.Repository.Utility;
using service.Application.Service.AccountManagement;
using service.Application.Service.KeyManagement;
using service.Application.Service.UserManagement;
using service.Application.Service.Utility;
using service.Application.Utility;
using service.Core.Interfaces.AccountManagement;
using service.Core.Interfaces.KeyManagement;
using service.Core.Interfaces.UserManagement;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add serilog services to the container and read config from appsettings
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers();

// Add distributed memory cache (you can switch to Redis or SQL Server as needed)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Smtp settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Configure Dapper with MSSQL
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

#region Dependency Injection Container
builder.Services.AddScoped<ICommonDbHander, CommonDbHander>();
builder.Services.AddHttpClient<IWebRequestHandler, WebRequestHandler>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IEmailOtpRepository, EmailOtpRepository>();
builder.Services.AddScoped<IEmailOtpService, EmailOtpService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IKeyManagementRepository, KeyManagementRepository>();
builder.Services.AddScoped<IKeyManagementService, KeyManagementService>();
#endregion Dependency Injection Container

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cryptography Service", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "service.xml"));
});
#endregion

#region JWT Token Validation Parameters
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
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Add JwtTokenGenerator as a service
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
#endregion

#region CORS Policy settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder => builder.WithOrigins("http://localhost:4200")
                      .AllowCredentials()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
