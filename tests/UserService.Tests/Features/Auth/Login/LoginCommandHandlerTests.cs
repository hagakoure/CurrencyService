using FluentAssertions;
using Moq;
using UserService.Application.Features.Auth.Login;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Features.Auth.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new();
        _passwordHasherMock = new();
        _jwtTokenGeneratorMock = new();
        _handler = new(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand("unknown", "password");
        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Пользователь не найден");
        _passwordHasherMock.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var user = User.Create("testuser", "correct_hash");
        var command = new LoginCommand("testuser", "wrong_password");
        
        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.Verify("correct_hash", "wrong_password"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Неверный пароль");
        _jwtTokenGeneratorMock.Verify(g => g.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var user = User.Create("testuser", "hashed_pass");
        var command = new LoginCommand("testuser", "correct_password");
        var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        
        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(h => h.Verify("hashed_pass", "correct_password"))
            .Returns(true);
        _jwtTokenGeneratorMock
            .Setup(g => g.GenerateToken(user.Id, user.Name))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedToken);
    }
}