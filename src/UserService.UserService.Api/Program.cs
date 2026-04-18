using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Application;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите токен: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<UserService.Infrastructure.Persistence.AppDbContext>();

        var pending = context.Database.GetPendingMigrations();
        if (pending.Any())
        {
            logger.LogInformation("Applying {Count} migrations: {Migrations}",
                pending.Count(), string.Join(", ", pending));
            context.Database.Migrate();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date (no pending migrations)");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration failed");
        throw;
    }
}

app.UseForwardedHeaders();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (exception == null) return;

        context.Response.ContentType = "application/json";

        if (exception.GetType().Name.Contains("Validation", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "Validation Failed", error = exception.Message });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { message = "Internal Server Error", error = exception.Message });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (
    MediatR.ISender mediator,
    UserService.Application.Features.Auth.Register.RegisterCommand command,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Received register request: Name={Name}", command.Name);

    var result = await mediator.Send(command);

    if (result.IsSuccess)
    {
        logger.LogInformation("User registered: {UserId}", result.Value);
        return Results.Ok(new { userId = result.Value });
    }

    logger.LogWarning("Registration failed: {Error}", result.Error);

    return Results.Json(new { error = result.Error }, statusCode: StatusCodes.Status400BadRequest);
});

app.MapPost("/api/auth/login", async (MediatR.ISender mediator,
    UserService.Application.Features.Auth.Login.LoginCommand command,
    ILogger<Program> logger) =>
{
    var result = await mediator.Send(command);
    return result.IsSuccess
        ? Results.Ok(new { token = result.Value })
        : Results.Json(new { error = result.Error }, statusCode: StatusCodes.Status401Unauthorized);
});

app.Run();