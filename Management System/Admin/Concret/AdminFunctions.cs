using Management_System.Admin.Abstract;
using Management_System.Data;
using Management_System.Models;

namespace Management_System.Admin.Concret;

public class AdminFunctions:IAdminFunctions
{
    private readonly AppDbContext _context;

    public AdminFunctions(AppDbContext context)
    {
        _context = context;
    }

    public void ViewUsers()
    {
        Console.Clear();
        Console.WriteLine("List of Active Users:\n");

        var users = _context.Users
                            .Where(u => u.IsDeleted == false) 
                            .ToList();

        if (users.Count == 0)
        {
            Console.WriteLine("No active users found.");
        }
        else
        {
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.Id} | Username: {user.Username}");
            }
        }

        Pause();
    }



    public void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("                                             Delete User\n");

        Console.Write("Enter username to delete: ");
        string username = Console.ReadLine();

        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsDeleted == false);

        if (user == null)
        {
            Console.WriteLine("\nUser not found or already deleted.");
            Thread.Sleep(1500);
            return;
        }

        user.IsDeleted = true;
        _context.SaveChanges();

        Console.WriteLine($"\nUser '{username}' successfully marked as deleted.");
        Thread.Sleep(1500);
        Pause();
    }


    public void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("                                               Add Product\n");

        Console.Write("Enter product name: ");
        string name = Console.ReadLine();

        Console.Write("Enter product description: ");
        string description = Console.ReadLine();

        Console.Write("Enter product count: ");
        if (!int.TryParse(Console.ReadLine(), out int count))
        {
            Console.WriteLine("Invalid count input.");
            Thread.Sleep(1500);
            return;
        }

        Console.Write("Enter product price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price input.");
            Thread.Sleep(1500);
            return;
        }

        Console.Write("Enter product discount (%): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal discount))
        {
            Console.WriteLine("Invalid discount input.");
            Thread.Sleep(1500);
            return;
        }

        var newProduct = new Product
        {
            Name = name,
            Description = description,
            Count = count,
            Price = price,
            Discount = discount
            
        };

        _context.Products.Add(newProduct);
        _context.SaveChanges();

        Console.WriteLine("\nProduct added successfully!");
        Thread.Sleep(1500);
    }


    public void ViewProducts()
    {
        Console.Clear();
        Console.WriteLine("List of Available Products:\n");

        var products = _context.Products
            .Where(p => !p.IsDeleted && p.Count > 0)
            .ToList();

        if (!products.Any())
        {
            Console.WriteLine("No products found.");
            return;
        }

        foreach (var product in products)
        {
            Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Price: {product.Price} | Quantity: {product.Count}");
        }

        Console.WriteLine("\nPress Enter to go back...");
        Console.ReadLine();
    }





    public void Delete_Product()
    {
        Console.Clear();
        Console.WriteLine("                                             Delete Product\n");
        Console.WriteLine();
        int product_id=Correct_Id();


        var product = _context.Products.FirstOrDefault(p => p.Id== product_id && p.IsDeleted == false);

        if (product == null)
        {
            Console.WriteLine("\nProduct not found or already deleted.");
            Thread.Sleep(1500);
            return;
        }

        product.IsDeleted = true;
        _context.SaveChanges();

        Console.WriteLine($"\nUser '{product.Name}' successfully marked as deleted.");
        Thread.Sleep(1500);
        Pause();
    }


    public void Update_Product()
    {
        Console.Clear();
        Console.WriteLine("List of Products:\n");

        var products = _context.Products.Where(u => u.IsDeleted == false).ToList();
        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Count: {p.Count}");
        }

        Console.Write("\nEnter product ID to update (or press Enter to cancel): ");
        string idInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(idInput))
        {
            Console.WriteLine("Update cancelled. Returning to menu...");
            Thread.Sleep(1500);
            return;
        }

        if (!int.TryParse(idInput, out int productId))
        {
            Console.WriteLine("Invalid ID. Returning to menu...");
            Thread.Sleep(1500);
            return;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId && p.IsDeleted == false);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            Thread.Sleep(1500);
            return;
        }

        Console.WriteLine($"\nUpdating Product: {product.Name}");

        Console.Write($"Enter new product name (current: {product.Name}): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
            product.Name = name;

        Console.Write($"Enter new description (current: {product.Description}): ");
        string description = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(description))
            product.Description = description;

        Console.Write($"Enter new count (current: {product.Count}): ");
        if (int.TryParse(Console.ReadLine(), out int count))
            product.Count = count;

        Console.Write($"Enter new price (current: {product.Price}): ");
        if (decimal.TryParse(Console.ReadLine(), out decimal price))
            product.Price = price;

        Console.Write($"Enter new discount (%) (current: {product.Discount}): ");
        if (decimal.TryParse(Console.ReadLine(), out decimal discount))
            product.Discount = discount;

        Console.Write($"Enter new barcode (current: {product.Barcode}): ");
        string barcodeInput = Console.ReadLine();
        if (Guid.TryParse(barcodeInput, out Guid barcode))
            product.Barcode = barcode;

        _context.Products.Update(product);
        _context.SaveChanges();

        Console.WriteLine("\nProduct updated successfully!");
        Thread.Sleep(1500);
    }


    public void Update_Profile(User user2)
    {
        Console.Clear();
        Console.WriteLine("                                             Update Profile\n");
        
        
        var user = _context.Users.FirstOrDefault(u => u.Id == user2.Id); 
        
        if (user == null)
        {
            Console.WriteLine("User not found.");
            Thread.Sleep(1500);
            return;
        }
        
        Console.WriteLine($"Updating Profile: {user.Username}\n");
        
        
        Console.Write($"Enter new username (current: {user.Username}): ");
        string username = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(username))
            user.Username = username;
        
        
        Console.Write($"Enter new email (current: {user.Email}): ");
        string email = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(email))
            user.Email = email;
        
        
        Console.Write($"Enter new password (current: {user.Password}): ");
        string password = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(password))
            user.Password = password;  
        
        _context.Users.Update(user);
        _context.SaveChanges();
        
        Console.WriteLine("\nProfile updated successfully!");
        Thread.Sleep(1500);
    }

    private void Pause()
    {
        Console.Write("\nPress any key to return to the menu...");
        Console.ReadKey();
        
    }

    private int Correct_Id()
    {
        int product_id;

        while (true)
        {
            try
            {
                Console.Write("Please enter product Id : ");
                product_id = int.Parse(Console.ReadLine());
                break;  
            }
            catch (FormatException)
            {
                Console.WriteLine("False symbol");
            }
        }

        return product_id;
    }
}
