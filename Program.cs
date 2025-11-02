using System.Data;
using System.Data.Common;
using System.Text;
using loma_api.Interfaces;
using loma_api.Repositories;
using loma_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Npgsql;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var allowedOrigins = new[]
{
    "http://localhost:3000",
    "https://song-loma.com",
    "https://www.song-loma.com",
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<ShareTokenRepository>();
builder.Services.AddScoped<SurveyRepository>();
builder.Services.AddScoped<AuditLogRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IShareTokenService, ShareTokenService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ must be before HTTPS redirection
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Text("ok")).AllowAnonymous();

app.MapGet("/health/db", async (IDbConnection db) =>
{
    try
    {
        if (db is DbConnection dbc)
            await dbc.OpenAsync();
        else
            db.Open();

        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT 1";
        var result = (await (cmd is DbCommand dcmd ? dcmd.ExecuteScalarAsync() : Task.FromResult(cmd.ExecuteScalar()))) ?? 0;

        return Results.Ok(new { db = "ok", result });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "DB check failed", detail: ex.Message);
    }
    finally
    {
        if (db.State == ConnectionState.Open)
        {
            if (db is DbConnection dbc2)
                await dbc2.CloseAsync();
            else
                db.Close();
        }
    }
}).AllowAnonymous();

app.Run();
