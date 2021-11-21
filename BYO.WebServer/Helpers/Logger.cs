using System.Net;

namespace BYO.WebServer.Helpers
{
    internal static class Logger
    {
        public static void LogRequest(HttpListenerRequest request)
        {
            Console.WriteLine($"{request.RemoteEndPoint} :: {request.HttpMethod} :: {request?.Url?.AbsoluteUri}");
        }
    }
}