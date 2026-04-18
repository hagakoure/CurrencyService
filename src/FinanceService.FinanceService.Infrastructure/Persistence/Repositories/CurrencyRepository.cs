using Microsoft.EntityFrameworkCore;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _context;

    public CurrencyRepository(AppDbContext context) => _context = context;

    public async Task<Currency?> GetByCharCodeAsync(string charCode, CancellationToken ct) =>
        await _context.Currencies.FirstOrDefaultAsync(c => c.CharCode == charCode, ct);

    public async Task<List<Currency>> GetByCodesAsync(List<string> charCodes, CancellationToken ct) =>
        await _context.Currencies.Where(c => charCodes.Contains(c.CharCode)).ToListAsync(ct);

    public async Task AddAsync(Currency currency, CancellationToken ct) =>
        await _context.Currencies.AddAsync(currency, ct);

    public async Task UpdateAsync(Currency currency, CancellationToken ct) =>
        _context.Currencies.Update(currency);

    public async Task SaveChangesAsync(CancellationToken ct) =>
        await _context.SaveChangesAsync(ct);
}