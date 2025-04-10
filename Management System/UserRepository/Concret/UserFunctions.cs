using Management_System.Data;
using Management_System.Enum;
using Management_System.Models;
using Management_System.UserRepository.Abstract;

namespace Management_System.UserRepository.Concret;

public class UserFunctions:IUserFunctions
{
    private readonly AppDbContext _context;

    public UserFunctions(AppDbContext context)
    {
        _context = context;
    }

    public void UpdateProfile(User user)
    {
        Console.Clear();
        Console.WriteLine("Update Your Profile\n");

        Console.Write($"Enter new name (current: {user.Name}): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
            user.Name = name;

        Console.Write($"Enter new surname (current: {user.Surname}): ");
        string surname = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(surname))
            user.Surname = surname;

        Console.Write($"Enter new username (current: {user.Username}): ");
        string username = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(username))
            user.Username = username;

        Console.Write($"Enter new email (current: {user.Email}): ");
        string email = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(email))
            user.Email = email;

        Console.Write($"Enter new password: ");
        string password = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(password))
            user.Password = password;

        

        _context.Users.Update(user);
        _context.SaveChanges();

        Console.WriteLine("\nProfile updated successfully!");
        Thread.Sleep(1500);
    }
    public void AddProductsToOrder(User user)
    {
        Console.Clear();
        Console.WriteLine("List of Available Products:\n");

        var products = _context.Products
            .Where(p => !p.IsDeleted && p.Count > 0)
            .ToList();

        if (!products.Any())
        {
            Console.WriteLine("No products available.");
            return;
        }

        foreach (var product in products)
        {
            Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price} | Quantity Available: {product.Count}");
        }

        List<OrderProduct> cart = new List<OrderProduct>();

        while (true)
        {
            Console.Write("\nEnter the Product ID to add to the order (or press Enter to finish): ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                break;

            if (!int.TryParse(input, out int productId))
            {
                Console.WriteLine("Invalid ID format. Please try again.");
                continue;
            }

            var selectedProduct = products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct == null)
            {
                Console.WriteLine("Product not found.");
                continue;
            }

            Console.Write($"Enter the quantity (Available: {selectedProduct.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0 || quantity > selectedProduct.Count)
            {
                Console.WriteLine("Invalid quantity. Please try again.");
                continue;
            }

            cart.Add(new OrderProduct
            {
                ProductId = selectedProduct.Id,
                Quantity = quantity
            });

            Console.WriteLine($"{selectedProduct.Name} has been added to the order.");
        }

        if (cart.Count == 0)
        {
            Console.WriteLine("\nNo products selected. Returning to menu...");
            Thread.Sleep(1500);
            return;
        }

        // Create a new order
        var order = new Order
        {
            OrderDate = DateTime.Now,
            OrderStatus = false,
            IsPaid = false,
            UserId = user.Id,
            OrderProducts = cart,
            TotalPrice = cart.Sum(item => item.Quantity * (products.First(p => p.Id == item.ProductId).Price * (1 - products.First(p => p.Id == item.ProductId).Discount / 100)))
        };

        // Update product counts
        foreach (var item in cart)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.Count -= item.Quantity;
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        Console.WriteLine("\nOrder has been created successfully!");
        Thread.Sleep(1500);
    }


}
