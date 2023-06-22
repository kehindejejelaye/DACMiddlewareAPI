using System.ComponentModel.DataAnnotations;

namespace DACMiddlewareAPI.Models;

public class AttachUserDto
{
    public int OwnerId { get; set; }

    [Required]
    public int UserAssigned { get; set; }
}
