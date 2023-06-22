using System.ComponentModel.DataAnnotations.Schema;

namespace DACMiddlewareAPI.Entities;

public class Client
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string AppId { get; set; }
    public string AppKeyHash { get; set; }
}
