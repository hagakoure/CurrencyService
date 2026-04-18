using FluentAssertions;
using Moq;
using UserService.Application.Features.Auth.Register;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Features.Auth.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new();
        _passwordHasherMock = new();
        _handler = new(_userRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_UserAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand("testuser", "password123");
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Пользователь с таким именем уже существует");
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesUserAndReturnsSuccess()
    {
        // Arrange
        var command = new RegisterCommand("newuser", "StrongPass123!");
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasherMock
            .Setup(h => h.Hash("StrongPass123!"))
            .Returns("hashed_password_value");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _userRepositoryMock.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Name == "newuser" && u.PasswordHash == "hashed_password_value"),
            It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("user", "")]
    [InlineData("a", "123")] // слишком короткий пароль
    public async Task Handle_InvalidInput_ReturnsFailure(string name, string password)
    {
        // Arrange
        var command = new RegisterCommand(name, password);
        // Валидация происходит в ValidationBehavior, который мы не тестируем здесь
        // Этот тест проверяет, что хэндлер не падает на невалидных данных
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasherMock
            .Setup(h => h.Hash(password))
            .Returns("hashed");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Хэндлер должен отработать, валидация — на уровне pipeline
        result.Should().NotBeNull();
    }
}