using System.ComponentModel.DataAnnotations;

namespace MdWebApplication.API.Contracts.Users;

public record RegisterUserRequest(
    [Required] string UserName,
    [Required] string Login,
    [Required] string Password);

