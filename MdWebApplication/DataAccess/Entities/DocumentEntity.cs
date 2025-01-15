namespace DataAccess.Models;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public UserEntity UserEntity { get; set; }
    public Guid UserId { get; set; }
}