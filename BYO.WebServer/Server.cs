using BYO.WebServer.Helpers;
using System.Net;
using System.Net.Sockets;
using BYO.WebServer.Constants;
using BYO.WebServer.Models;

namespace BYO.WebServer
{
    public static class Server
    {
        private static readonly Router Router = new();
        private static readonly SessionManager SessionManager = new();
        private static Func<ServerError, string> OnError { get; set; } = ErrorHandler;
        private static readonly int maxSimultaneousConnections = 20;
        private static readonly Semaphore Sem = new(maxSimultaneousConnections, maxSimultaneousConnections);

        public static void Start()
        {
            Server.AddRoute(new Route(Verbs.POST, "/demo/redirect", RedirectMe));
            
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
                response = HttpRequestProcessor.ProcessRequest(Router, context.Request);
                session.UpdateLastConnectionTime();

                if (response.Error != ServerError.Ok)
                {
                    response.Redirect = OnError(response.Error);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ConsoleWriteException(ex);
                response = new ResponsePacket() { Redirect = OnError(ServerError.ServerError) };
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
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.Redirect;
                response.Redirect($"http://{request.UserHostAddress}{resp.Redirect}");
            }

            response.OutputStream.Close();
        }
        
        private static void AddRoute(Route route)
        {
            Router.Routes.Add(route);
        }
        
        private static string RedirectMe(Dictionary<string, string>? parms)
        {
            return "/demo/clicked";
        }
        
        private static string ErrorHandler(ServerError error)
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

            return output;
        }
    }
}