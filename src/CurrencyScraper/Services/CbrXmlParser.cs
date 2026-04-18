using System.Globalization;
using System.Xml.Linq;
using CurrencyScraper.interfaces;
using CurrencyScraper.Models;

namespace CurrencyScraper.Services;

public class CbrXmlParser : ICbrXmlParser
{
    private readonly HttpClient _httpClient;
    private const string CbrUrl = "http://www.cbr.ru/scripts/XML_daily.asp";

    public CbrXmlParser(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<CbrCurrencyDto>> FetchRatesAsync(CancellationToken ct)
    {
        var xml = await _httpClient.GetStringAsync(CbrUrl, ct);
        var doc = XDocument.Parse(xml);

        return doc.Descendants("Valute").Select(x => new CbrCurrencyDto
        {
            CharCode = x.Element("CharCode")?.Value ?? string.Empty,
            Name = x.Element("Name")?.Value ?? string.Empty,
            Nominal = int.TryParse(x.Element("Nominal")?.Value, out var n) ? n : 1,
            Rate = ParseRate(x.Element("Value")?.Value)
        }).ToList();
    }

    private static decimal ParseRate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0m;
        
        return decimal.TryParse(
            value.Replace(',', '.'),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var result) ? result / 1m : 0m;
    }
}