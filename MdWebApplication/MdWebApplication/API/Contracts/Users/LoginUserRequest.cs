using System.ComponentModel.DataAnnotations;

namespace MdWebApplication.API.Contracts.Users;

public record LoginUserRequest(
    [Required] string Login,
    [Required] string Password);