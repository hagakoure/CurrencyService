using CurrencyScraper.Models;

namespace CurrencyScraper.interfaces;

public interface ICbrXmlParser
{
    Task<List<CbrCurrencyDto>> FetchRatesAsync(CancellationToken ct);
}