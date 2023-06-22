using System.ComponentModel.DataAnnotations.Schema;

namespace DACMiddlewareAPI.Entities;

public class BankAccount
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccountNumber { get; set; }
    public string BankName { get; set; } = "Dipole Diamond";

    public BankAccount()
    {
        AccountNumber = GenerateAndAssignAccountNumber();
    }

    public string GenerateAndAssignAccountNumber()
    {
        Random generator = new Random();
        return generator.Next().ToString("D10");
    }
}
