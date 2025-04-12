namespace Management_System.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }

    public User User { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}

