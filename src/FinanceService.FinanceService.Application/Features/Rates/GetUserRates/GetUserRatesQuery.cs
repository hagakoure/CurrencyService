using FinanceService.Application.Common;
using MediatR;

namespace FinanceService.Application.Features.Rates.GetUserRates;

public record GetUserRatesQuery : IRequest<Result<List<RateResponse>>>;

public record RateResponse(string CharCode, string Name, decimal Rate, DateTime LastUpdated);