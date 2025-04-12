using Management_System.Models;

namespace Management_System.UserRepository.Abstract;

public interface IUserFunctions
{
    void UpdateProfile(User user);
    void AddProductsToCart(User user);
    void ViewCart(User user);
    void ConfirmCart(User user);
    void ViewInvoiceHistory(User user);
    void ViewConfirmedCarts(User user);

}
