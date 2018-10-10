using CakesWebApp.Services;
using IRunesWebApp.Data;
using SIS.HTTP.Cookies;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace IRunesWebApp.Controllers
{
    public abstract class BaseController
    {
        private const string RootDirectoryRelativePath = "../../../";

        private const string ControllerDefaultName = "Controller";

        private const string DirectorySeparator = "/";

        private const string ViewsFolderName = "Views";

        private const string HtmlFileExtensions = ".html";

        private string GetCurrentControllerName() =>
            this.GetType().Name.Replace(ControllerDefaultName, string.Empty);

        protected readonly UserCookieService cookieService;

        protected IDictionary<string,string> ViewBag { get; set; }

        public BaseController()
        {
            this.Context = new IRunesContext();
            this.cookieService = new UserCookieService();
            this.ViewBag = new Dictionary<string, string>();
        }

        public bool IsAuthenticated (IHttpRequest request)
        {
            return request.Session.ContainsParameter("username");
        }

        public void SignInUser(string userName, IHttpRequest request)
        {
            request.Session.AddParameter("username", userName);
            var userCookieValue = this.cookieService.GetUserCookie(userName);
            request.Cookies.Add(new HttpCookie("IRunes_auth", userCookieValue));
        }

        protected IRunesContext Context { get; private set; }

        protected IHttpResponse View([CallerMemberName] string viewName = "")
        {
            string filePath = RootDirectoryRelativePath
                             + ViewsFolderName
                             + DirectorySeparator
                             + this.GetCurrentControllerName()
                             + DirectorySeparator
                             + viewName
                             + HtmlFileExtensions;

            if (!File.Exists(filePath))
            {
                return new BadRequestResult(
                    $"View {viewName} not found.",
                    SIS.HTTP.Enums.HttpResponseStatusCode.NotFound);
            }

            var filecontent = File.ReadAllText(filePath);

            foreach (var viewBagKey  in ViewBag.Keys)
            {
                var dynamicDataPlaceHolder = $"{{{viewBagKey}}}";
                if (filecontent.Contains(dynamicDataPlaceHolder))
                {
                    var viewBagValue = this.ViewBag[viewBagKey];
                    filecontent = filecontent.Replace(
                        dynamicDataPlaceHolder,
                        viewBagValue);
                }
            }

            var response = new HtmlResult(filecontent, SIS.HTTP.Enums.HttpResponseStatusCode.Ok);

            return response;
        }
    }
}
