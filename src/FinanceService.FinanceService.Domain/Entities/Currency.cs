namespace FinanceService.Domain.Entities;

public class Currency
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string CharCode { get; private set; } = string.Empty;
    public decimal RateToRub { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private Currency()
    {
    }

    public static Currency Create(string name, string charCode, decimal rate) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            CharCode = charCode,
            RateToRub = rate,
            LastUpdated = DateTime.UtcNow
        };

    public void UpdateRate(decimal newRate)
    {
        RateToRub = newRate;
        LastUpdated = DateTime.UtcNow;
    }
}