using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer.Helpers
{
    internal static class HttpRequestProcessor
    {
        public static void ProcessRequest(HttpListenerRequest request)
        {
            Logger.LogRequest(request);

            string path = request?.RawUrl?.LeftOf('?') ?? string.Empty;
            string verb = request?.HttpMethod ?? string.Empty;
            string parms = request?.RawUrl?.RightOf('?') ?? string.Empty;

            Dictionary<string, string> kvParams = GetKeyValues(parms);

            Router router = new();
            router.WebsitePath = GetWebsitePath();
            router.Route(verb, path, kvParams);
            
        }

        private static Dictionary<string, string> GetKeyValues(string data, Dictionary<string, string>? kv = null)
        {
            if (kv == null) kv = new();
            if(string.IsNullOrEmpty(data) || data.Length <= 0) return kv;

            foreach (var keyVal in data.Split('&'))
            {
                kv[keyVal.LeftOf('=')] = keyVal.RightOf('=');
            }

            return kv;
        }

        public static string GetWebsitePath()
        {
            // Path of our exe.
            string websitePath = Assembly.GetExecutingAssembly().Location;
            websitePath = websitePath.LeftOfRightmostOf('\\').LeftOfRightmostOf('\\').LeftOfRightmostOf('\\') + "\\Website";

            return websitePath;
        }

    }
}
