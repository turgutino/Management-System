using Management_System.Models;

namespace Management_System.Admin.Abstract;

public interface IAdminFunctions
{
    void ViewUsers();
    void DeleteUser();

    void AddProduct();

    void ViewProducts();

    void Delete_Product();

    void Update_Product();

    void UpdateProfile(User user2);

}
