using Management_System.SendMail;
using Management_System.Models;
using Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Management_System.CashierRepository.Abstract;

namespace Management_System.CashierRepository.Concret
{
    public class CashierFunctions:ICashierFunctions
    {
        private readonly AppDbContext _context;
        SmtpService smtpService;

        public CashierFunctions(AppDbContext context)
        {
            _context = context;
            smtpService = new SmtpService();
        }

        public void ReviewAndApproveOrders(User cashierUser)
        {
            Console.Clear();
            Console.WriteLine("Pending Orders:");

            var pendingOrders = _context.Orders
                .Where(o => !o.OrderStatus)
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

            foreach (var order in pendingOrders)
            {
                Console.WriteLine($"ID: {order.Id} | Customer: {order.User?.Username} | Date: {order.OrderDate} | Total Price: {order.TotalPrice} AZN");
                foreach (var item in order.OrderProducts)
                {
                    Console.WriteLine($"\t- {item.Product?.Name} x {item.Quantity} ({item.Product?.Price} AZN)");
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

            if (selectedOrder.User == null)
            {
                Console.WriteLine("The order does not have an associated user.");
                Thread.Sleep(1000);
                return;
            }

            if (selectedOrder.OrderProducts == null || !selectedOrder.OrderProducts.Any())
            {
                Console.WriteLine("The order does not contain any products.");
                Thread.Sleep(1000);
                return;
            }

            
            var invoice = new Invoice
            {
                DateCreated = DateOnly.FromDateTime(DateTime.Now),
                InvoiceNumber = Guid.NewGuid(),
                UserId = selectedOrder.User.Id
            };

            
            selectedOrder.OrderStatus = true;
            selectedOrder.IsPaid = true;
            selectedOrder.Invoice = invoice;

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            
            string pdfFilePath = smtpService.GenerateInvoicePdf(
                selectedOrder.User.Username,
                selectedOrder.OrderProducts.ToList(),
                selectedOrder.TotalPrice,
                cashierUser.Name,
                cashierUser.Surname
            );

            if (string.IsNullOrEmpty(pdfFilePath))
            {
                Console.WriteLine("Failed to generate PDF invoice.");
                Thread.Sleep(1000);
                return;
            }

            
            string recipientEmail = selectedOrder.User.Email;
            string subject = "Your Order Invoice";
            string body = $"Dear {selectedOrder.User.Username},<br><br>Your order has been confirmed. Please find your invoice attached.<br><br>Thank you!";

            smtpService.SendEmail(recipientEmail, subject, body, pdfFilePath);

            Console.WriteLine("Order confirmed, invoice created, and email sent successfully.");
            Thread.Sleep(1500);
        }

    }
}