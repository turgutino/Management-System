using Management_System.Data;
using Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Management_System.CashierRepository.Concret;

public class CashierFunctions
{
    private readonly AppDbContext _context;

    public CashierFunctions(AppDbContext context)
    {
        _context = context;
    }

    public void ReviewAndApproveOrders(User cashierUser)
    {
        Console.Clear();
        Console.WriteLine("Pending Orders:");

        // Get all orders with status "false" (pending orders)
        var pendingOrders = _context.Orders
            .Where(o => !o.OrderStatus)  // OrderStatus is false (pending)
            .Include(o => o.User)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .ToList();

        if (!pendingOrders.Any())
        {
            Console.WriteLine("No pending orders.");
            Thread.Sleep(1500);
            return;
        }

        // Show all pending orders
        foreach (var order in pendingOrders)
        {
            Console.WriteLine($"ID: {order.Id} | Customer: {order.User.Username} | Date: {order.OrderDate} | Total Price: {order.TotalPrice} AZN");
            foreach (var item in order.OrderProducts)
            {
                Console.WriteLine($"\t- {item.Product.Name} x {item.Quantity}  ({item.Product.Price} AZN)");
            }
            Console.WriteLine("------------------------------------");
        }

        Console.Write("\nEnter the ID of the order you want to confirm (or press Enter to exit): ");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (!int.TryParse(input, out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            Thread.Sleep(1000);
            return;
        }

        var selectedOrder = pendingOrders.FirstOrDefault(o => o.Id == orderId);
        if (selectedOrder == null)
        {
            Console.WriteLine("Order not found.");
            Thread.Sleep(1000);
            return;
        }

        // Create an invoice for the order
        var invoice = new Invoice
        {
            DateCreated = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = Guid.NewGuid(),
            UserId = selectedOrder.User.Id
        };

        // Mark the order as completed
        selectedOrder.OrderStatus = true; // Set OrderStatus to true (approved)
        selectedOrder.IsPaid = true; // Set IsPaid to true (paid)
        selectedOrder.Invoice = invoice; // Associate the invoice with the order

        // Add the invoice to the context
        _context.Invoices.Add(invoice);
        _context.SaveChanges();

        Console.WriteLine("Order confirmed and invoice created successfully.");
        Thread.Sleep(1500);
    }

}
