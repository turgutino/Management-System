namespace Management_System.Models;

public class Invoice:BaseEntity
{
    public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public Guid InvoiceNumber { get; set; } = Guid.NewGuid();

    ICollection<Order> Orders { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

}
