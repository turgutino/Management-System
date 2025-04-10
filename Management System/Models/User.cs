using Management_System.Enum;

namespace Management_System.Models;

public class User:BaseEntity
{
    public string Name { get; set; }

    public string Surname { get; set; }
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Role UserRole { get; set; }

    public ICollection<Order> Orders { get; set; }

    public ICollection<Invoice> Invoices { get; set; }
}


