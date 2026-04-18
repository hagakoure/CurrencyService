using FinanceService.Domain.Entities;

namespace CurrencyScraper.Persistence;

public interface ICurrencyWriteRepository
{
    Task<Currency?> GetByCharCodeAsync(string charCode, CancellationToken ct);
    Task AddAsync(Currency currency, CancellationToken ct);
    Task UpdateAsync(Currency currency, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}