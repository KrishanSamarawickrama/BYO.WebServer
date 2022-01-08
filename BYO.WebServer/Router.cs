using BYO.WebServer.Helpers;
using System.Reflection;
using System.Text;
using BYO.WebServer.Models;

namespace BYO.WebServer
{
    public class Router
    {
        public static List<Route> Routes = new();
        private string WebsitePath { get; set; }
        private readonly Dictionary<string, ExtensionInfo> _extFolderMap;

        public Router()
        {
            WebsitePath = GetWebsitePath();
            _extFolderMap = new()
            {
                {"ico", new("image/ico", ImageLoader)},
                {"png", new("image/png", ImageLoader)},
                {"jpg", new("image/jpg", ImageLoader)},
                {"gif", new("image/gif", ImageLoader)},
                {"bmp", new("image/bmp", ImageLoader)},
                {"html", new("text/html", PageLoader)},
                {"css", new("text/css", FileLoader)},
                {"js", new("text/js", FileLoader)},
                {"", new("text/html", PageLoader)},
            };
        }

        internal ResponsePacket Route(Session session, string verb, string path, Dictionary<string, string>? kvParams)
        {
            ResponsePacket output;

            string ext = path.RightOfRightmostOf('.');
            verb = verb.ToLower();
            
            if (_extFolderMap.TryGetValue(ext, out ExtensionInfo? extInfo))
            {
                string fullPath = (path.Length > 1) ? WebsitePath + path.Replace('/', '\\') : WebsitePath;

                var route = Routes.SingleOrDefault(x => x.Verb.ToLower() == verb && x.Path == path);
                if (route != null)
                {
                    var redirect = route.Handler.Handle(session, kvParams);
                    if (string.IsNullOrEmpty(redirect))
                    {
                        output = extInfo.Loader(fullPath, ext, extInfo);
                    }
                    else
                    {
                        output = new() {Redirect = redirect};
                    }
                }
                else
                {
                    output = extInfo.Loader(fullPath, ext, extInfo);
                }
            }
            else
            {
                output = new() {Error = ServerError.UnknownTypes};
            }

            return output;
        }

        private ResponsePacket PageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            ResponsePacket output;

            if (fullPath == WebsitePath)
                output = Route(new(),"GET", "/index.html", null);
            else
            {
                if (string.IsNullOrEmpty(ext))
                    fullPath = fullPath + ".html";

                fullPath = WebsitePath + "\\pages" + fullPath.RightOf(WebsitePath);
                output = FileLoader(fullPath, ext, extInfo);
            }

            return output;
        }

        private ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fileStream);
            return new ResponsePacket
            {
                Data = reader.ReadBytes((int) fileStream.Length),
                ContentType = extInfo.ContentType
            };
        }

        private ResponsePacket FileLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            return new ResponsePacket()
            {
                ContentType = extInfo.ContentType,
                Data = Encoding.UTF8.GetBytes(File.ReadAllText(fullPath)),
                Encoding = Encoding.UTF8
            };
        }

        private static string GetWebsitePath()
        {
            string websitePath = Assembly.GetExecutingAssembly().Location;
            websitePath =
                websitePath.LeftOfRightmostOf('\\').LeftOfRightmostOf('\\').LeftOfRightmostOf('\\')
                    .LeftOfRightmostOf('\\') + "\\Website";

            return websitePath;
        }
    }
}