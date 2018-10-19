using System.Linq;
using IRunesWebApp.Data;
using IRunesWebApp.Models;
using IRunesWebApp.Services.Contracts;
using IRunesWebApp.ViewModels;
using Services;

namespace IRunesWebApp.Services
{
    public class UsersService : IUsersService
    {
        private readonly IRunesContext context;
        private readonly IHashService hashService;

        public UsersService(IRunesContext context, IHashService hashService)
        {
            this.context = context;
            this.hashService = hashService;
        }

        public bool ExistsByUsernameAndPassword(string username, string password)
        {
            var hashedPassword = this.hashService.Hash(password);

            var userExists = this.context.Users
                .Any(u => u.Username == username &&
                          u.HashedPassword == hashedPassword);

            return userExists;
        }

        public bool CreateUser(RegisterViewModel model)
        {
            var user = new User
            {
                Username = model.Username,
                HashedPassword = this.hashService.Hash(model.Password)
            };
            this.context.Users.Add(user);

            try
            {
                this.context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
            

        }
    }
}
