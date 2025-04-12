using Management_System.Models;
using Management_System.SendMail;

namespace Management_System.Securities;

public class Forgotpassword
{

    HiddenPassword hiddenPassword=new HiddenPassword();
    public void ForgotPassword(User user)
    {
        if (user == null || user.IsDeleted || string.IsNullOrWhiteSpace(user.Email))
        {
            Console.WriteLine("Valid user not provided.");
            return;
        }

        string email = user.Email;

        
        string code = new Random().Next(100000, 999999).ToString();

        
        var smtpService = new SmtpService();
        string htmlBody = $@"
            <p>Sizin şifrə yeniləmə tələbiniz qeydə alındı.</p>
            <p>Təsdiqləmə kodunuz: <strong>{code}</strong></p>
            <p>Əgər bu tələbi siz etməmisinizsə, bu mesajı nəzərə almayın.</p>";

        smtpService.SendSimpleEmail(email, "Şifrə yenilənməsi təsdiq kodu", user.Username, htmlBody);

        
        while (true)
        {
            Console.Write("Enter the verification code: ");
            string inputCode = Console.ReadLine();

            if (inputCode == code)
            {
                break;
            }
            Console.WriteLine("Incorrect code. Please try again.");
        }

        Console.Write("Enter your new password: ");
        string newPassword = hiddenPassword.ReadPassword();

        user.Password = PasswordHash.HashPassword(newPassword);

        Console.WriteLine("Password has been successfully reset!");
    }
}
