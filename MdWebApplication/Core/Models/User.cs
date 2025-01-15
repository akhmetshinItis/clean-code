namespace Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }

    public static User Create(Guid id, string userName, string login, string passwordHash)
    {
        return new User { Id = id, UserName = userName, Login = login, PasswordHash = passwordHash };
    }
}