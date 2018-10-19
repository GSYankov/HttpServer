using IRunesWebApp.ViewModels;

namespace IRunesWebApp.Services.Contracts
{
    public interface IUsersService
    {
        bool ExistsByUsernameAndPassword(string username, string password);

        bool CreateUser(RegisterViewModel model);
    }
}
