namespace DataAccess.Models;

public class UserEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public List<DocumentEntity> Documents { get; set; }
    
}