using System.ComponentModel.DataAnnotations;

namespace DACMiddlewareAPI.Models;

public class UserForUpdateDto
{
    public int Id { get; set; }
    [MaxLength(25)]
    public string FirstName { get; set; }

    [MaxLength(25)]
    public string LastName { get; set; }

    [MaxLength(50)]
    public string Email { get; set; }

    [MaxLength(70)]
    public string Address { get; set; }
}
