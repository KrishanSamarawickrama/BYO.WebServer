using System.Net;
using System.Reflection;

namespace BYO.WebServer.Helpers
{
    internal static class HttpRequestProcessor
    {
        public static ResponsePacket ProcessRequest(Router router, HttpListenerRequest request)
        {
            Logger.LogRequest(request);

            string path = request?.RawUrl?.LeftOf("?") ?? string.Empty;
            string verb = request?.HttpMethod ?? string.Empty;
            string parms = request?.RawUrl?.RightOf("?") ?? string.Empty;
            string data = string.Empty;
            if (request?.InputStream != null)
            {
                data = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
            }

            Dictionary<string, string> kvParams = GetKeyValues(parms);
            GetKeyValues(data, kvParams);
            Logger.LogParams(kvParams);

            return router.Route(verb, path, kvParams);
        }

        private static Dictionary<string, string> GetKeyValues(string data, Dictionary<string, string>? kv = null)
        {
            if (kv == null) kv = new();
            if (string.IsNullOrEmpty(data) || data.Length <= 0) return kv;

            foreach (var keyVal in data.Split('&'))
            {
                kv[keyVal.LeftOf("=")] = keyVal.RightOf("=");
            }

            return kv;
        }

        
    }
}