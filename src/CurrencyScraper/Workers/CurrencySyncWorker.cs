using CurrencyScraper.interfaces;
using CurrencyScraper.Persistence;
using FinanceService.Domain.Entities;

namespace CurrencyScraper.Workers;

public class CurrencySyncWorker : BackgroundService
{
    private readonly ICurrencyWriteRepository _repo;
    private readonly ICbrXmlParser _parser;
    private readonly ILogger<CurrencySyncWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public CurrencySyncWorker(
        ICurrencyWriteRepository repo,
        ICbrXmlParser parser,
        ILogger<CurrencySyncWorker> logger)
    {
        _repo = repo;
        _parser = parser;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CurrencySyncWorker started. Sync interval: {Interval}", _interval);

        // Первый запуск сразу после старта
        await SyncCurrenciesAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            await SyncCurrenciesAsync(stoppingToken);
        }
    }

    private async Task SyncCurrenciesAsync(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Starting currency sync at {Time}", DateTime.UtcNow);

            var rates = await _parser.FetchRatesAsync(ct);
            var updatedCount = 0;

            foreach (var rate in rates)
            {
                var currency = await _repo.GetByCharCodeAsync(rate.CharCode, ct);
                
                if (currency == null)
                {
                    await _repo.AddAsync(
                        Currency.Create(rate.Name, rate.CharCode, rate.Rate), 
                        ct);
                }
                else
                {
                    currency.UpdateRate(rate.Rate);
                    await _repo.UpdateAsync(currency, ct);
                }
                updatedCount++;
            }

            await _repo.SaveChangesAsync(ct);
            _logger.LogInformation("Sync completed: {Count} currencies updated", updatedCount);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Sync cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync currency rates");
        }
    }
}