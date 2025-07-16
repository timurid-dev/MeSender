using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IPasswordService
{
    public PasswordData HashPassword(string password);

    public bool VerifyPassword(string password, string storedHash, string storedSalt);
}
