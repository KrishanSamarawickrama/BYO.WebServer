using BYO.WebServer.Helpers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BYO.WebServer.Constants;
using BYO.WebServer.Models;

namespace BYO.WebServer
{
    public static class Server
    {
        
        private static readonly Router Router = new();
        private static readonly SessionManager SessionManager = new();
        private static Action<Session, HttpListenerContext>? _onRequest;
        public static Func<ServerError, ResponsePacket> OnError { get; set; } = ErrorHandler;
        
        internal const int ExpirationTimeSeconds = 3600;
        private const int MaxSimultaneousConnections = 20;
        private static readonly Semaphore Sem = new(MaxSimultaneousConnections, MaxSimultaneousConnections);

        public static void Start()
        {
            // Never expire, always authorize
            Server._onRequest = (session, context) =>
            {
                session.Authorized = true;
                session.UpdateLastConnectionTime();
            };
            
            Server.AddRoute(new Route(Verbs.POST, "/demo/redirect", new AnonymousRouteHandler(RedirectMe)));
            Server.AddRoute(new Route(Verbs.POST, "/demo/redirect", new AuthenticatedRouteHandler(RedirectMe)));
            Server.AddRoute(new Route(Verbs.POST, "/demo/redirect", new AuthenticatedExpirableRouteHandler(RedirectMe)));
            Server.AddRoute(new Route(Verbs.PUT, "/demo/ajax", new AnonymousRouteHandler(AjaxResponder)));
            Server.AddRoute(new Route(Verbs.GET, "/demo/ajax", new AnonymousRouteHandler(AjaxResponder)));

            var localIPs = GetLocalHostIPs();
            var listener = InitializeListener(localIPs);
            Start(listener);
        }

        private static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> output = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();
            return output;
        }

        private static HttpListener InitializeListener(List<IPAddress> localHostIps)
        {
            HttpListener listener = new();
            listener.Prefixes.Add("http://localhost/");
            listener.Prefixes.Add("http://+:80/");

            // localHostIps.ForEach(ip =>
            // {
            //     Console.WriteLine($"Listening on IP http://{ip}/");
            //     listener.Prefixes.Add($"http://{ip}/");
            // });

            return listener;
        }

        private static void Start(HttpListener listener)
        {
            listener.Start();
            Task.Run(() => RunServer(listener));
        }

        private static void RunServer(HttpListener listener)
        {
            while (true)
            {
                Sem.WaitOne();
                StartConnectionListener(listener);
            }
        }

        private static async void StartConnectionListener(HttpListener listener)
        {
            ResponsePacket response;

            HttpListenerContext context = await listener.GetContextAsync();
            Sem.Release();

            try
            {
                Session session = SessionManager.GetSession(context.Request.RemoteEndPoint);
                response = HttpRequestProcessor.ProcessRequest(Router, context.Request, session);
                session.UpdateLastConnectionTime();
                _onRequest?.Invoke(session, context);

                if (response.Error != ServerError.Ok)
                {
                    response.Redirect = OnError(response.Error).Redirect;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ConsoleWriteException(ex);
                response = new ResponsePacket() {Redirect = OnError(ServerError.ServerError).Redirect};
            }

            Respond(context.Request, context.Response, response);
        }

        private static void Respond(HttpListenerRequest request, HttpListenerResponse response, ResponsePacket resp)
        {
            if (string.IsNullOrEmpty(resp.Redirect) && resp.Data != null)
            {
                response.ContentType = resp.ContentType;
                response.ContentLength64 = resp.Data.Length;
                response.OutputStream.Write(resp.Data, 0, resp.Data.Length);
                response.ContentEncoding = resp.Encoding;
                response.StatusCode = (int) HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (int) HttpStatusCode.Redirect;
                response.Redirect($"http://{request.UserHostAddress}{resp.Redirect}");
            }

            response.OutputStream.Close();
        }

        private static void AddRoute(Route route)
        {
            Router.Routes.Add(route);
        }
        
        public static ResponsePacket Redirect(string url, string? parm = null)
        {
            ResponsePacket ret = new ResponsePacket() { Redirect = url };
            ret.Redirect = (parm != null) ? ret.Redirect += "?" + parm : ret.Redirect;
            return ret;
        }

        private static ResponsePacket RedirectMe(Session session, Dictionary<string, string>? parms)
        {
            return Server.Redirect("/demo/clicked");
        }
        
        private static ResponsePacket AjaxResponder(Session session, Dictionary<string, string>? parms)
        {
            string data = "You said " + parms["number"];
            ResponsePacket ret = new ResponsePacket() { Data = Encoding.UTF8.GetBytes(data), ContentType = "text" };

            return ret;
        }

        private static ResponsePacket ErrorHandler(ServerError error)
        {
            string output = string.Empty;

            switch (error)
            {
                case ServerError.ExpiredSession:
                    output = "/error-pages/expired-session.html";
                    break;

                case ServerError.NotAuthorized:
                    output = "/error-pages/not-authorized.html";
                    break;

                case ServerError.FileNotFound:
                    output = "/error-pages/file-not-found.html";
                    break;

                case ServerError.PageNotFound:
                    output = "/error-pages/page-not-found.html";
                    break;

                case ServerError.ServerError:
                    output = "/error-pages/server-error.html";
                    break;

                case ServerError.UnknownTypes:
                    output = "/error-pages/unknown-types.html";
                    break;
            }

            return new ResponsePacket(){Redirect = output};
        }
    }
}