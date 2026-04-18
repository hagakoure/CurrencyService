using FinanceService.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<Currency?> GetByCharCodeAsync(string charCode, CancellationToken ct);
    Task<List<Currency>> GetByCodesAsync(List<string> charCodes, CancellationToken ct);
    Task AddAsync(Currency currency, CancellationToken ct);
    Task UpdateAsync(Currency currency, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}