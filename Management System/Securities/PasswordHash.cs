namespace Management_System.Securities;

public class PasswordHash
{
    public static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        return HashPassword(enteredPassword) == storedHashedPassword;
    }
}
