namespace CurrencyScraper.Models;

public record CbrCurrencyDto
{
    public string CharCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public int Nominal { get; init; } = 1; 
}