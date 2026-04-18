namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public List<string> FavoriteCurrencyIds { get; set; } = new();

    private User()
    {
    }

    public static User Create(string name, string passwordHash) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = passwordHash
        };
}