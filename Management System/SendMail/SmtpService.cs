using System;
using System.IO;
using System.Collections.Generic;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Net.Mail;
using System.Net;
using System.Linq;
using Management_System.Models;

namespace Management_System.SendMail
{
    public class SmtpService
    {
        public void SendEmail(string recipientEmail, string subject, string body, string pdfFilePath)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("turgut.nitro17@gmail.com", "dnsn dcer bryn zucn");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("turgut.nitro17@gmail.com", "Online Shop Market"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    if (File.Exists(pdfFilePath))
                    {
                        mailMessage.Attachments.Add(new Attachment(pdfFilePath));
                    }
                    else
                    {
                        Console.WriteLine("The PDF file was not found.");
                        return;
                    }

                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email successfully sent!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public string GenerateInvoicePdf(string customerName, List<OrderProduct> orderProducts, decimal totalPrice, string cashierName,string cashierSurname)
        {
            if (orderProducts == null || !orderProducts.Any())
            {
                Console.WriteLine("Error: Order products are missing.");
                return null;
            }

            
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Invoice");

            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            
            string fileName = $"Invoice_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                
                using (PdfDocument pdfDoc = new PdfDocument())
                {
                    PdfPage page = pdfDoc.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont titleFont = new XFont("Arial", 18);
                    XFont regularFont = new XFont("Arial", 12);
                    XBrush darkBlue = new XSolidBrush(XColor.FromArgb(33, 85, 165));
                    XBrush darkRed = new XSolidBrush(XColor.FromArgb(190, 40, 40));
                    XBrush gray = XBrushes.Gray;

                    int y = 40;

                   
                    gfx.DrawString("Online Shop Market", titleFont, darkBlue, 20, y);
                    y += 30;

                    gfx.DrawString($"Müştəri: {customerName}", regularFont, XBrushes.Black, 20, y);
                    y += 20;

                    gfx.DrawString($"Tarix: {DateTime.Now:yyyy-MM-dd}", regularFont, XBrushes.Black, 20, y);
                    y += 25;

                    gfx.DrawString("--------------------------------------------------", regularFont, gray, 20, y);
                    y += 30;

                    
                    gfx.DrawString("Məhsul", regularFont, XBrushes.Black, 20, y);
                    gfx.DrawString("Say", regularFont, XBrushes.Black, 200, y);
                    gfx.DrawString("Qiymət", regularFont, XBrushes.Black, 300, y);
                    gfx.DrawString("Cəmi", regularFont, XBrushes.Black, 400, y);
                    y += 25;

                    
                    foreach (var item in orderProducts)
                    {
                        if (item.Product == null) return null;

                        decimal itemTotal = item.Quantity * item.Product.Price;
                        gfx.DrawString(item.Product.Name, regularFont, XBrushes.Black, 20, y);
                        gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, 200, y);
                        gfx.DrawString($"{item.Product.Price} AZN", regularFont, XBrushes.Black, 300, y);
                        gfx.DrawString($"{itemTotal} AZN", regularFont, XBrushes.Black, 400, y);
                        y += 20;
                    }

                    
                    y += 10;
                    gfx.DrawString("--------------------------------------------------", regularFont, gray, 20, y);
                    y += 30;

                    gfx.DrawString($"Ümumi məbləğ: {totalPrice} AZN", new XFont("Arial", 13), darkRed, 300, y);
                    y += 40;

                    gfx.DrawString("Təşəkkür edirik! Sizinlə əməkdaşlıqdan məmnunuq.", regularFont, XBrushes.SeaGreen, 20, y);
                    y += 25;

                    gfx.DrawString("Əlaqə nömrəsi: 070 878 33 11", regularFont, XBrushes.Black, 20, y);
                    y += 15;
                    gfx.DrawString("E-mail: turgut.nitro17@gmail.com", regularFont, XBrushes.Black, 20, y);
                    y += 15;
                    gfx.DrawString("Sahibkar: Turgut Sofuyev", regularFont, XBrushes.Black, 20, y);
                    y += 15;

                    
                    gfx.DrawString($"Kassir: {cashierName} {cashierSurname}", regularFont, XBrushes.Black, 20, y);

                    
                    pdfDoc.Save(filePath);
                }

                
                Console.WriteLine("Invoice PDF uğurla yaradıldı.Ve userin gmail unvanina gonderildi.QEYD:Bu mesaj spama duse biler spamlar bolmesini yoxlamagi unutmayin");
                Thread.Sleep(3000);
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF yaradılarkən xəta baş verdi: {ex.Message}");
                return null;
            }
        }



        public string GetHtmlEmailBody(string customerName, decimal totalPrice)
        {
            return $@"
                <html>
                <body style='font-family: Arial;'>
                    <h2>Hörmətli {customerName},</h2>
                    <p>Sifarişiniz üçün elektron faktura əlavə edilmişdir.</p>
                    <p>Toplam məbləğ: <strong>{totalPrice} AZN</strong></p>
                    <p>Əlavə sualınız olarsa, bizimlə əlaqə saxlaya bilərsiniz.</p>
                    <br/>
                    <p style='font-size: 12px; color: gray;'>Bu mesaj ClassNet sistemindən avtomatik göndərilmişdir.</p>
                </body>
                </html>";
        }
        public void SendSimpleEmail(string recipientEmail, string subject, string customerName, string messageBody)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("turgut.nitro17@gmail.com", "dnsn dcer bryn zucn");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("turgut.nitro17@gmail.com", "OnlineShop Support"),
                        Subject = subject,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; color: #333;'>
                    <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h2 style='color: #1a73e8;'>Salam {customerName},</h2>
                        <p>{messageBody}</p>
                        <p>Əgər siz bu sorğunu etməmisinizsə, bu mesajı nəzərə almayın.</p>
                        <br/>
                        <p style='font-size: 14px;'>Hörmətlə,<br/><strong>ClassNet Dəstək Komandası</strong></p>
                        <hr style='margin-top: 30px;'/>
                        <p style='font-size: 12px; color: gray;'>Bu mesaj ClassNet sistemi tərəfindən avtomatik olaraq göndərilmişdir.</p>
                    </div>
                </body>
                </html>";

                    mailMessage.Body = body;

                    smtpClient.Send(mailMessage);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nE-poçt ugurla gonderildi!");
                    Console.ResetColor();

                    Console.WriteLine("Zehmet olmasa e-poctunuzu yoxlayın.");
                    Console.WriteLine("QEYD:E-poçt 'Spam' bolmesine duse biler eger mesaj gelmezse o hisseni yoxlayin ve spamda olarsa 'Report Not Spam' secin.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"E-poct gonderilerken xeta bas verdi: {ex.Message}");
                Console.ResetColor();
            }
        }


    }


}
