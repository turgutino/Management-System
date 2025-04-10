using Management_System.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public Guid Barcode { get; set; } = Guid.NewGuid();

    public ICollection<OrderProduct> OrderProducts;
}
