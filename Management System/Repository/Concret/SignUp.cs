using Management_System.Data;
using Management_System.Enum;
using Management_System.Models;
using Management_System.Repository.Abstract;
using Management_System.Securities;

public class SignUp : ISignUp
{
    PasswordHash passwordHash = new PasswordHash();

    public void SignUpMenu()
    {
        Console.WriteLine("                                                      Sign Up\n");
        Console.WriteLine("1 - > User\n");
        Console.WriteLine("2 - > Cashier\n");
        Console.WriteLine("3 - > Admin\n");
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
        else if (select == "3")
        {
            Admin_SignUp();
            Thread.Sleep(1000);
        }
        else if (string.IsNullOrWhiteSpace(select))
        {
            return;
        }
    }

    HiddenPassword hiddenPassword = new HiddenPassword();

    public void User_SignUp()
    {
        Console.Clear();
        Console.WriteLine("                                                      User Sign-Up");

        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Surname: ");
        string surname = Console.ReadLine();
        Console.Write("Enter Gmail: ");
        string gmail = Console.ReadLine();

        using (var context = new AppDbContext())
        {
            string username = GetUniqueUsername(context);

            Console.Write("Enter Password: ");
            string password = hiddenPassword.ReadPassword();
            string hashedPassword = PasswordHash.HashPassword(password);

            var user = new User
            {
                Name = name,
                Surname = surname,
                Username = username,
                Email = gmail,
                Password = hashedPassword,
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
        Console.WriteLine("                                                      Cashier Sign-Up");

        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Surname: ");
        string surname = Console.ReadLine();
        Console.Write("Enter Gmail: ");
        string gmail = Console.ReadLine();

        using (var context = new AppDbContext())
        {
            string username = GetUniqueUsername(context);

            Console.Write("Enter Password: ");
            string password = hiddenPassword.ReadPassword();
            string hashedPassword = PasswordHash.HashPassword(password);

            var user = new User
            {
                Name = name,
                Surname = surname,
                Username = username,
                Email = gmail,
                Password = hashedPassword,
                UserRole = Role.Cashier
            };

            context.Users.Add(user);
            context.SaveChanges();
        }

        Console.WriteLine("                                              Cashier registered successfully!");
    }

    public void Admin_SignUp()
    {
        Console.Clear();
        Console.WriteLine("                                                      Admin Sign-Up");

        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Surname: ");
        string surname = Console.ReadLine();
        Console.Write("Enter Gmail: ");
        string gmail = Console.ReadLine();

        using (var context = new AppDbContext())
        {
            string username = GetUniqueUsername(context);

            Console.Write("Enter Password: ");
            string password = hiddenPassword.ReadPassword();
            string hashedPassword = PasswordHash.HashPassword(password);

            var user = new User
            {
                Name = name,
                Surname = surname,
                Username = username,
                Email = gmail,
                Password = hashedPassword,
                UserRole = Role.Admin
            };

            context.Users.Add(user);
            context.SaveChanges();
        }

        Console.WriteLine("                                              Admin registered successfully!");
    }


    private string GetUniqueUsername(AppDbContext context)
    {
        string username;
        while (true)
        {
            Console.Write("Enter Username: ");
            username = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                continue;
            }

            bool exists = context.Users.Any(u => u.Username == username);
            if (!exists)
                break;

            Console.WriteLine("This username already exists. Please choose another one.");
        }

        return username;
    }

}
