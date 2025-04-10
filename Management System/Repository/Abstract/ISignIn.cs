using Management_System.Models;

namespace Management_System.Repository.Abstract;

public interface ISignIn
{
    void SignInMenu();
    void UserMenu(User user);
    void CashierMenu(User user);
    void Admin_Menu(User user);
}


