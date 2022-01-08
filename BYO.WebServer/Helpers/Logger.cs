using System.Net;

namespace BYO.WebServer.Helpers
{
    internal static class Logger
    {
        public static void LogRequest(HttpListenerRequest request)
        {
            Console.WriteLine($"{request.RemoteEndPoint} :: {request.HttpMethod} :: {request?.Url?.AbsoluteUri}");
        }

        public static void LogParams(Dictionary<string, string> kv)
        {
            kv.ToList().ForEach(kvp=>
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(kvp.Key + " : " + kvp.Value);
                Console.ResetColor();
            });
        }
    }
}