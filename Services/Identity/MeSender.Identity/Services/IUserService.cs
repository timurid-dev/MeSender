using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IUserService
{
    void AddUser(string email, string password);
}
