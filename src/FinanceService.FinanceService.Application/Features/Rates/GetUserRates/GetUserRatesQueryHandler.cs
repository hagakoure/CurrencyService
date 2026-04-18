using MediatR;
using FinanceService.Application.Common;
using FinanceService.Domain.Interfaces;

namespace FinanceService.Application.Features.Rates.GetUserRates;

public class GetUserRatesQueryHandler : IRequestHandler<GetUserRatesQuery, Result<List<RateResponse>>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserContext _userContext;

    public GetUserRatesQueryHandler(ICurrencyRepository currencyRepository, IUserContext userContext)
    {
        _currencyRepository = currencyRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<RateResponse>>> Handle(GetUserRatesQuery request, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();
        if (userId == null) return Result<List<RateResponse>>.Failure("User not authenticated");

        var favoriteCodes = _userContext.GetUserFavoriteCurrencies();
        
        if (!favoriteCodes.Any())
            return Result<List<RateResponse>>.Success(new());

        var currencies = await _currencyRepository.GetByCodesAsync(favoriteCodes, ct);

        var response = currencies.Select(c => new RateResponse(
            c.CharCode, c.Name, c.RateToRub, c.LastUpdated)).ToList();

        return Result<List<RateResponse>>.Success(response);
    }
}