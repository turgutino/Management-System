using Management_System.Data;
using Management_System.Enum;
using Management_System.Models;
using Management_System.Securities;
using Management_System.UserRepository.Abstract;
using Microsoft.EntityFrameworkCore;

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

        Console.Write("Enter new password (leave empty to keep current): ");
        var hiddenPassword = new HiddenPassword();
        string newPassword = hiddenPassword.ReadPassword();

        if (!string.IsNullOrWhiteSpace(newPassword))
            user.Password = PasswordHash.HashPassword(newPassword);

        _context.Users.Update(user);
        _context.SaveChanges();

        Console.WriteLine("\nProfile updated successfully!");
        Thread.Sleep(1500);
    }

    public void AddProductsToCart(User user)
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
            Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price} | Available Quantity: {product.Count}");
        }

        var cart = _context.Carts.FirstOrDefault(c => c.UserId == user.Id && !c.IsConfirmed && !c.IsDeleted);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = user.Id,
                IsConfirmed = false,
                CreatedAt = DateTime.Now
            };
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        while (true)
        {
            Console.Write("\nEnter the product ID you want to add (press Enter to finish): ");
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

            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == selectedProduct.Id);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = selectedProduct.Id,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            selectedProduct.Count -= quantity;
            _context.SaveChanges();

            Console.WriteLine($"{selectedProduct.Name} has been added to your cart.");
        }
    }


    public void ViewCart(User user)
    {
        Console.Clear();
        var cart = _context.Carts.FirstOrDefault(c => c.UserId == user.Id && !c.IsConfirmed && !c.IsDeleted);

        if (cart == null)
        {
            Console.WriteLine("Your cart is empty or already confirmed.");
            return;
        }

        Console.WriteLine("\nProducts in your cart:\n");

        var cartItems = _context.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .Include(ci => ci.Product)
            .ToList();

        if (!cartItems.Any())
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

      
        foreach (var item in cartItems)
        {
            Console.WriteLine($"ID: {item.Product.Id} | Product: {item.Product.Name} | Price: {item.Product.Price:C} | Quantity: {item.Quantity}");
        }

        while (true)
        {
            Console.WriteLine("\nDo you want to update or remove an item? (Enter 'update <ID>' or 'remove <ID>' to delete, or press Enter to finish):");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
            {
                break; 
            }

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid command. Please enter 'update <ID>' or 'remove <ID>' (without quotes).");
                continue;
            }

            var command = parts[0];
            if (!int.TryParse(parts[1], out int productId))
            {
                Console.WriteLine("Invalid product ID. Please try again.");
                continue;
            }

            var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                Console.WriteLine("Product not found in your cart.");
                continue;
            }

            if (command == "remove")
            {
                cartItem.Product.Count += cartItem.Quantity; 
                Console.WriteLine($"The product {cartItem.Product.Name} has been removed from your cart.");

                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            else if (command == "update")
            {
                int available = cartItem.Product.Count; 

                Console.Write($"Enter the new quantity for {cartItem.Product.Name} (Available: {available}): ");
                if (!int.TryParse(Console.ReadLine(), out int newQuantity) || newQuantity <= 0 || newQuantity > available)
                {
                    Console.WriteLine("Invalid quantity. Please enter a valid number.");
                    continue;
                }

                int oldQuantity = cartItem.Quantity;
                int difference = newQuantity - oldQuantity;

                cartItem.Quantity = newQuantity;
                cartItem.Product.Count -= difference;

                _context.SaveChanges();
                Console.WriteLine($"The quantity of {cartItem.Product.Name} has been updated to {newQuantity}.");
            }

            else
            {
                Console.WriteLine("Invalid command. Please enter 'update <ID>' or 'remove <ID>' (without quotes).");
            }
        }
    }




    public void ConfirmCart(User user)
    {
        Console.Clear();
        var cart = _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefault(c => c.UserId == user.Id && !c.IsConfirmed && !c.IsDeleted);

        if (cart == null || !cart.CartItems.Any())
        {
            Console.WriteLine("Your cart is empty or has already been confirmed.");
            return;
        }

        Console.Clear();
        Console.WriteLine("Review Your Cart Before Confirming:\n");

        foreach (var item in cart.CartItems)
        {
            Console.WriteLine($"Product: {item.Product.Name} | Price: {item.Product.Price:C} | Quantity: {item.Quantity}");
        }

        Console.Write("\nDo you want to confirm this cart? (y/n) : ");
        var confirmInput = Console.ReadLine()?.Trim().ToLower();

        if (confirmInput != "y")
        {
            Console.WriteLine("Cart confirmation cancelled.");
            return;
        }

        cart.IsConfirmed = true;
        _context.SaveChanges();

        var orderProducts = cart.CartItems.Select(ci => new OrderProduct
        {
            ProductId = ci.ProductId,
            Quantity = ci.Quantity
        }).ToList();

        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.Now,
            OrderStatus = false,
            IsPaid = false,
            OrderProducts = orderProducts,
            TotalPrice = orderProducts.Sum(op => op.Quantity * _context.Products.First(p => p.Id == op.ProductId).Price)
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        Console.WriteLine("Your cart has been confirmed and an order has been created!");
    }


    public void ViewInvoiceHistory(User user)
    {
        Console.Clear();
        var invoices = _context.Invoices
            .Where(i => i.UserId == user.Id)
            .Include(i => i.Orders)
            .OrderByDescending(i => i.DateCreated)
            .ToList();

        if (!invoices.Any())
        {
            Console.WriteLine("You don't have any invoices yet.");
            return;
        }

        Console.WriteLine($"\nInvoice History for {user.Name} {user.Surname}:\n");

        foreach (var invoice in invoices)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Invoice ID      : {invoice.Id}");
            Console.WriteLine($"Invoice Number  : {invoice.InvoiceNumber}");
            Console.WriteLine($"Issued Date     : {invoice.DateCreated}");
            Console.WriteLine($"Order Count     : {invoice.Orders.Count}");

            int count = 1;
            foreach (var order in invoice.Orders)
            {
                Console.WriteLine($"\n   Order {count++}:");
                Console.WriteLine($"     Order ID     : {order.Id}");
                Console.WriteLine($"     Order Date   : {order.OrderDate}");
                Console.WriteLine($"     Total Price  : {order.TotalPrice:C}");
                Console.WriteLine($"     Paid         : {(order.IsPaid ? "Yes" : "No")}");
                Console.WriteLine($"     Status       : {(order.OrderStatus ? "Confirmed" : "Pending")}");
            }

            Console.WriteLine("--------------------------------------------------");
        }
    }

    public void ViewConfirmedCarts(User user)
    {
        Console.Clear();
        var confirmedCarts = _context.Carts
            .Where(c => c.UserId == user.Id && c.IsConfirmed && !c.IsDeleted)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        if (!confirmedCarts.Any())
        {
            Console.WriteLine("You have no confirmed carts.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < confirmedCarts.Count; i++)
        {
            var cart = confirmedCarts[i];
            Console.WriteLine($"\nCart #{i + 1} | Created At: {cart.CreatedAt}");
            foreach (var item in cart.CartItems)
            {
                Console.WriteLine($"  - {item.Product.Name} | Quantity: {item.Quantity} | Price: {item.Product.Price:C}");
            }
        }

        Console.WriteLine("\n(Only viewing. No delete option available.)");
        Console.Write("\nPress any key to return...");
        Console.ReadKey();
    }





}
