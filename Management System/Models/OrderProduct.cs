using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderProduct
{
    [Key]
    [Column(Order = 0)]
    public int OrderId { get; set; }
    public Order Order { get; set; }

    [Key]
    [Column(Order = 1)]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
}
