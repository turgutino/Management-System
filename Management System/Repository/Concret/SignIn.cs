using Management_System.Admin.Abstract;
using Management_System.Admin.Concret;
using Management_System.CashierRepository.Concret;
using Management_System.Data;
using Management_System.Models;
using Management_System.Repository.Abstract;
using Management_System.UserRepository.Concret;

namespace Management_System.Repository.Concret;

public class SignIn:ISignIn
{
    private readonly AppDbContext _context;
    //AppDbContext context = new AppDbContext();
    AdminFunctions adminFunctions;
    UserFunctions userFunctions;
    CashierFunctions cashierFunctions;
    public SignIn(AppDbContext context)
    {
        _context = context;
        adminFunctions = new AdminFunctions(context);
        userFunctions = new UserFunctions(context);
        cashierFunctions = new CashierFunctions(context);
    }
    public void SignInMenu()
    {
        Console.WriteLine("                                                        Sign In");
        Console.Write("Enter username : ");
        string username = Console.ReadLine();
        Console.Write("Enter password : ");
        string password = Console.ReadLine();

        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password && u.IsDeleted == false);

        if (user != null)
        {
            Console.WriteLine("\n                                                     Login successful!");
            Thread.Sleep(1000);

            if (user.UserRole==Enum.Role.Admin)
            {
                
                Admin_Menu(user);
            }
            else if (user.UserRole==Enum.Role.User)
            {
                
                UserMenu(user);
            }
            else if (user.UserRole == Enum.Role.Cashier)
            {
                CashierMenu(user);

            }
            else
            {
                Console.WriteLine("Undefined role. Please contact admin.");
                Thread.Sleep(1000);
            }

        }
        else
        {
            Console.WriteLine("\nInvalid username or password. Please try again.");
            Thread.Sleep(1000);
            SignInMenu();
        }
    }


    public void UserMenu(User user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Welcome, {user.Username}!");
            Console.WriteLine("1 -> View Products");
            Console.WriteLine("2 -> Add Products to Order"); 
            Console.WriteLine("3 -> Update Profile");
            Console.WriteLine("4 -> Log out");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                adminFunctions.ViewProducts();
                Console.WriteLine("Press Enter to return to menu...");
                Console.ReadLine();
            }
            else if (option == "2")
            {
                userFunctions.AddProductsToOrder(user); 
                Console.WriteLine("Press Enter to return to menu...");
                Console.ReadLine();
            }
            else if (option == "3")
            {
                userFunctions.UpdateProfile(user);
                Thread.Sleep(1000);
            }
            else if (option == "4")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
                Thread.Sleep(1000);
            }
        }
    }


    public void CashierMenu(User user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Welcome, {user.Username}!");
            Console.WriteLine("1 -> Update Profile");
            Console.WriteLine("2 -> View and Approve Orders");
            Console.WriteLine("3 -> Log Out");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                userFunctions.UpdateProfile(user);
                Thread.Sleep(1000);
            }
            else if (option == "2")
            {
                cashierFunctions.ReviewAndApproveOrders(user); // <-- Added here
            }
            else if (option == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
                Thread.Sleep(1000);
            }
        }
    }





    public void Admin_Menu(User user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("                                             Admin Menu\n");
            Console.WriteLine("1 -> View Users");
            Console.WriteLine("2 -> Delete User");
            Console.WriteLine("3 -> Update Product");
            Console.WriteLine("4 -> Add Product");
            Console.WriteLine("5 -> Delete Product");
            Console.WriteLine("6 -> View Products");
            Console.WriteLine("7 -> Update Profile"); 
            Console.WriteLine("0 -> Log Out");
            Console.Write("\nSelect an option: ");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    adminFunctions.ViewUsers();
                    break;
                case "2":
                    adminFunctions.DeleteUser();
                    break;
                case "3":
                    adminFunctions.Update_Product();
                    break;
                case "4":
                    adminFunctions.AddProduct();
                    break;
                case "5":
                    adminFunctions.Delete_Product();
                    break;
                case "6":
                    adminFunctions.ViewProducts();
                    break;
                case "7":  
                    adminFunctions.Update_Profile(user);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }



}
