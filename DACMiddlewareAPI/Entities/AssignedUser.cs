using System.ComponentModel.DataAnnotations.Schema;

namespace DACMiddlewareAPI.Entities;

public class AssignedUser
{ 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public int UserAssigned { get; set; } 
}
