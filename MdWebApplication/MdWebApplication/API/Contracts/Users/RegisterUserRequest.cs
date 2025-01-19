using System.ComponentModel.DataAnnotations;

namespace MdWebApplication.API.Contracts.Users;

public record RegisterUserRequest(
    [Required] string Username,
    [Required] string Login,
    [Required] string Password);

