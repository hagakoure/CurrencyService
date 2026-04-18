using Microsoft.EntityFrameworkCore;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;

namespace CurrencyScraper.Persistence;

public class CurrencyWriteRepository : ICurrencyWriteRepository
{
    private readonly AppDbContext _context; 

    public CurrencyWriteRepository(AppDbContext context) => _context = context;

    public async Task<Currency?> GetByCharCodeAsync(string charCode, CancellationToken ct) =>
        await _context.Currencies.FirstOrDefaultAsync(c => c.CharCode == charCode, ct);

    public async Task AddAsync(Currency currency, CancellationToken ct) =>
        await _context.Currencies.AddAsync(currency, ct);

    public Task UpdateAsync(Currency currency, CancellationToken ct)
    {
        _context.Currencies.Update(currency);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct) =>
        await _context.SaveChangesAsync(ct);
}