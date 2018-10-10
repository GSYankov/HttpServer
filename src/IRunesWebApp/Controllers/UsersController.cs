using CakesWebApp.Services;
using IRunesWebApp.Models;
using SIS.HTTP.Cookies;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;
using System;
using System.Linq;

namespace IRunesWebApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly HashService hashService;


        public UsersController()
        {
            this.hashService = new HashService();
        }

        public IHttpResponse Login(IHttpRequest request) => this.View();

        public IHttpResponse Register(IHttpRequest request) => this.View();

        public IHttpResponse PostLogin(IHttpRequest request)
        {

            var username = request.FormData["username"].ToString();
            var password = request.FormData["password"].ToString();
            var hashedPassword = hashService.Hash(password);
            var user = this.Context.Users
                .FirstOrDefault(u => u.Username == username &&
                            u.HashedPassword == hashedPassword);

            if (user == null)
            {
                return new RedirectResult("/Login");
            }

            this.SignInUser(username, request);

            return new RedirectResult("/");
        }

        public IHttpResponse PostRegister(IHttpRequest request)
        {
            var userName = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();

            if (password != confirmPassword)
            {
                return new BadRequestResult(
                    "Passwords do not match."
                    ,SIS.HTTP.Enums.HttpResponseStatusCode.SeeOther);
            }

            // Hash password
            var hashedPassword = this.hashService.Hash(password);

            // Create user
            var user = new User
            {
                Username = userName,
                HashedPassword = hashedPassword
            };
            this.Context.Users.Add(user);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return new BadRequestResult(e.Message,SIS.HTTP.Enums.HttpResponseStatusCode.InternalServerError);
            }

            this.SignInUser(userName, request);

            // Redirect
            return new RedirectResult("/");
        }

    }
}
