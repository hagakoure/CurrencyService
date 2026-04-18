namespace FinanceService.Domain.Interfaces;

public interface IUserContext
{
    Guid? GetUserId();
    string? GetUserName();
    List<string> GetUserFavoriteCurrencies();
}