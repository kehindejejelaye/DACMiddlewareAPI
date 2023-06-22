using System.ComponentModel.DataAnnotations;

namespace DACMiddlewareAPI.Models;

public class UserForCreationDto
{
    [Required]
    [MaxLength(25)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(25)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Email { get; set; }

    [Required]
    [MinLength(8), MaxLength(10)]
    public string Password { get; set; }

    [Required]
    [MaxLength(70)]
    public string Address { get; set; }
}
