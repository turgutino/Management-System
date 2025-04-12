using Management_System.Admin.Abstract;
using Management_System.Admin.Concret;
using Management_System.CashierRepository.Concret;
using Management_System.Data;
using Management_System.Models;
using Management_System.Repository.Abstract;
using Management_System.Securities;
using Management_System.UserRepository.Concret;

namespace Management_System.Repository.Concret;

public class SignIn:ISignIn
{
    private readonly AppDbContext _context;
    //AppDbContext context = new AppDbContext();
    AdminFunctions adminFunctions;
    UserFunctions userFunctions;
    CashierFunctions cashierFunctions;
    HiddenPassword hiddenPassword;
    public SignIn(AppDbContext context)
    {
        _context = context;
        adminFunctions = new AdminFunctions(context);
        userFunctions = new UserFunctions(context);
        cashierFunctions = new CashierFunctions(context);
        hiddenPassword = new HiddenPassword();
    }
    public void SignInMenu()
    {
        Console.WriteLine("                                                        Sign In");
        Console.Write("Enter username : ");
        string username = Console.ReadLine();

       
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("\nNo username entered. Returning to main menu...");
            Thread.Sleep(1000);
            return; 
        }

        Console.Write("Enter password : ");
        string password = hiddenPassword.ReadPassword();

        
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("\nNo password entered. Returning to main menu...");
            Thread.Sleep(1000);
            return;
        }

        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.IsDeleted == false);

        if (user != null && PasswordHash.VerifyPassword(password, user.Password))
        {
            Console.WriteLine("\n                                                     Login successful!");
            Thread.Sleep(1000);

            switch (user.UserRole)
            {
                case Enum.Role.Admin:
                    Admin_Menu(user);
                    break;
                case Enum.Role.User:
                    UserMenu(user);
                    break;
                case Enum.Role.Cashier:
                    CashierMenu(user);
                    break;
                default:
                    Console.WriteLine("Undefined role. Please contact admin.");
                    Thread.Sleep(1000);
                    break;
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
            Console.WriteLine("2 -> Add Products to Cart");
            Console.WriteLine("3 -> View/Modify Cart");
            Console.WriteLine("4 -> Confirm Cart");
            Console.WriteLine("5 -> View Confirm Cart");
            Console.WriteLine("6 -> Update Profile");
            Console.WriteLine("7 -> View Invoice History");
            Console.WriteLine("8 -> Log out");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    adminFunctions.ViewProducts();
                    Console.WriteLine("\nPress Enter to return to menu...");
                    Console.ReadLine();
                    break;

                case "2":
                    userFunctions.AddProductsToCart(user);
                    Console.WriteLine("\nPress Enter to return to menu...");
                    Console.ReadLine();
                    break;

                case "3":
                    userFunctions.ViewCart(user);
                    Console.WriteLine("\nPress Enter to return to menu...");
                    Console.ReadLine();
                    break;

                case "4":
                    userFunctions.ConfirmCart(user);
                    Console.WriteLine("\nPress Enter to return to menu...");
                    Console.ReadLine();
                    break;


                case "5":
                    userFunctions.ViewConfirmedCarts(user);
                    
                    Thread.Sleep(1000);
                    break;

                case "6":
                    userFunctions.UpdateProfile(user);
                    Console.WriteLine("\nProfile updated successfully!");
                    Thread.Sleep(1000);
                    break;

                case "7":
                    userFunctions.ViewInvoiceHistory(user);
                    Console.WriteLine("\nPress Enter to return to menu...");
                    Console.ReadLine();
                    break;

                case "8":
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Thread.Sleep(1000);
                    break;
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
                    adminFunctions.UpdateProfile(user);
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
