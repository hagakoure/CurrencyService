using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FinanceService.Domain.Interfaces;

namespace FinanceService.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public Guid? GetUserId()
    {
        var id = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    public string? GetUserName() =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public List<string> GetUserFavoriteCurrencies()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("favorites")?.Value;
        if (string.IsNullOrEmpty(claim))
            return new List<string> { "USD", "EUR" };

        return claim.Split(',').Select(c => c.Trim()).ToList();
    }
}