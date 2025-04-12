using Management_System.Models;

namespace Management_System.CashierRepository.Abstract;

public interface ICashierFunctions
{
    void ReviewAndApproveOrders(User cashierUser);
}
