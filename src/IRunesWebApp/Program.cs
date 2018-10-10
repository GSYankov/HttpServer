using IRunesWebApp.Controllers;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;
using System;

namespace IRunesWebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            ConfigureRouring(serverRoutingTable);

            Server server = new Server(8383, serverRoutingTable);

            server.Run();
        }

        private static void ConfigureRouring(ServerRoutingTable serverRoutingTable)
        {
            //Get
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] =
                request => new RedirectResult("/");
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] =
                request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"] =
                request => new UsersController().Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"] =
                request => new UsersController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/All"] =
                request => new AlbumsController().All(request);

            //Post
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] =
                request => new UsersController().PostLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"] =
                request => new UsersController().PostRegister(request);
        }
    }
}
