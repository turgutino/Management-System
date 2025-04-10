using Management_System.Data;
using Management_System.Repository.Concret;
using System.Threading.Channels;

namespace Management_System.Entrance.Concret;

public class Menu
{
    SignUp signUp = new SignUp();
    AppDbContext context = new AppDbContext();
    SignIn signIn;

    public Menu()
    {
        signIn = new SignIn(context);
    }
    public void Introduction()
    {
        while (true) {
            Console.Clear();
            Console.WriteLine("                                                  Welcome Market\n");
            Console.WriteLine("1 - > Sign In\n");
            Console.WriteLine("2 - > Sign Up\n");
            Console.Write("Enter select : ");
            string select = Console.ReadLine();

            if (select == "1")
            {
                Console.Clear();
                signIn.SignInMenu();
            }

            else if (select == "2")
            {
                Console.Clear();
                signUp.SignUpMenu();
            }
            

        }
        
    }
}
