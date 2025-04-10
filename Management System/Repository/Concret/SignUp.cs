using Management_System.Data;
using Management_System.Enum;
using Management_System.Models;
using Management_System.Repository.Abstract;

namespace Management_System.Repository.Concret;

public class SignUp:ISignUp
{
    public void SignUpMenu()
    {
        Console.WriteLine("                                                      Sign Up\n");
        Console.WriteLine("1 - > User\n");
        Console.WriteLine("2 - > Cashier\n");
        Console.WriteLine("Enter - Back\n");
        Console.Write("Please select one : ");
        string select = Console.ReadLine();

        if (select == "1")
        {
            User_SignUp();
            Thread.Sleep(1000);
        }
        else if (select == "2")
        {
            Cashier_SignUp();
            Thread.Sleep(1000);
        }
        else if (string.IsNullOrWhiteSpace(select))
        {
            return; 
        }
        
    }


    public void User_SignUp()
    {
        Console.Clear();
        Console.WriteLine("                                                      User Sign-Up");
        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Surname: ");
        string surname = Console.ReadLine();
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Gmail: ");
        string gmail = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        using (var context = new AppDbContext())
        {
            var user = new User
            {
                Name = name,
                Surname = surname,
                Username = username,
                Email = gmail,
                Password = password,
                UserRole = Role.User 
            };

            context.Users.Add(user);
            context.SaveChanges();
        }

        Console.WriteLine("                                              User registered successfully!");
    }

    public void Cashier_SignUp()
    {
        Console.Clear();
        Console.WriteLine("                                                      User Sign-Up");
        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Surname: ");
        string surname = Console.ReadLine();
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Gmail: ");
        string gmail = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        using (var context = new AppDbContext())
        {
            var user = new User
            {
                Name = name,
                Surname = surname,
                Username = username,
                Email = gmail,
                Password = password,
                UserRole = Role.Cashier
            };

            context.Users.Add(user);
            context.SaveChanges();
        }

        Console.WriteLine("                                             Cashier registered successfully!");
    }

}
