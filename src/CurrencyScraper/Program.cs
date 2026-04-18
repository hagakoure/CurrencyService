using CurrencyScraper.interfaces;
using CurrencyScraper.Persistence;
using CurrencyScraper.Services;
using CurrencyScraper.Workers;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Timeout;
using Serilog;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSerilog(dispose: true);
});

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null)));

builder.Services.AddHttpClient<ICbrXmlParser, CbrXmlParser>(client =>
    {
        client.BaseAddress = new Uri("http://www.cbr.ru");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler((services, request) =>
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync<HttpResponseMessage>(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    });

builder.Services.AddScoped<ICurrencyWriteRepository, CurrencyWriteRepository>();

builder.Services.AddHostedService<CurrencySyncWorker>();

var host = builder.Build();

// миграции при старте
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    Log.Information("Database migrations applied");
}

await host.RunAsync();