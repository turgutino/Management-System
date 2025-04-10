using Management_System.Models;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public bool OrderStatus { get; set; }
    public bool IsPaid { get; set; }

    public int? UserId { get; set; }
    public User User { get; set; }

    public int? InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public ICollection<OrderProduct> OrderProducts;
}
