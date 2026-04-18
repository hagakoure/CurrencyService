using FluentAssertions;
using Moq;
using FinanceService.Application.Features.Rates.GetUserRates;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;

namespace FinanceService.Tests.Features.Rates.GetUserRates;

public class GetUserRatesQueryHandlerTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly GetUserRatesQueryHandler _handler;

    public GetUserRatesQueryHandlerTests()
    {
        _currencyRepositoryMock = new();
        _userContextMock = new();
        _handler = new(_currencyRepositoryMock.Object, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailure()
    {
        // Arrange
        _userContextMock.Setup(u => u.GetUserId()).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(new GetUserRatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not authenticated");
        _currencyRepositoryMock.Verify(r => r.GetByCodesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoFavoriteCurrencies_ReturnsEmptyList()
    {
        // Arrange
        _userContextMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
        _userContextMock.Setup(u => u.GetUserFavoriteCurrencies()).Returns(new List<string>());

        // Act
        var result = await _handler.Handle(new GetUserRatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithFavorites_ReturnsFilteredRates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var favorites = new List<string> { "USD", "EUR" };
        
        var currencies = new List<Currency>
        {
            Currency.Create("Доллар США", "USD", 92.5m),
            Currency.Create("Евро", "EUR", 99.8m)
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserFavoriteCurrencies()).Returns(favorites);
        _currencyRepositoryMock
            .Setup(r => r.GetByCodesAsync(favorites, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        // Act
        var result = await _handler.Handle(new GetUserRatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(r => r.CharCode == "USD" && r.Rate == 92.5m);
        result.Value.Should().Contain(r => r.CharCode == "EUR" && r.Rate == 99.8m);
    }

    [Fact]
    public async Task Handle_FavoriteNotFoundInDb_ReturnsOnlyExisting()
    {
        // Arrange
        _userContextMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
        _userContextMock.Setup(u => u.GetUserFavoriteCurrencies()).Returns(new List<string> { "USD", "FAKE" });
        
        var foundCurrencies = new List<Currency>
        {
            Currency.Create("Доллар США", "USD", 92.5m)
        };
        
        _currencyRepositoryMock
            .Setup(r => r.GetByCodesAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(foundCurrencies);

        // Act
        var result = await _handler.Handle(new GetUserRatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().OnlyContain(r => r.CharCode == "USD");
    }
}