using BYO.WebServer.Helpers;
using System.Reflection;
using System.Text;

namespace BYO.WebServer
{
    public class Router
    {
        public string WebsitePath { get; set; } = "/";

        private readonly Dictionary<string, ExtensionInfo> extFolderMap;

        public Router()
        {
            WebsitePath = GetWebsitePath();
            extFolderMap = new()
            {
                { "ico", new() { ContentType = "image/ico", Loader = ImageLoader } },
                { "png", new() { ContentType = "image/png", Loader = ImageLoader } },
                { "jpg", new() { ContentType = "image/jpg", Loader = ImageLoader } },
                { "gif", new() { ContentType = "image/gif", Loader = ImageLoader } },
                { "bmp", new() { ContentType = "image/bmp", Loader = ImageLoader } },
                { "html", new() { ContentType = "text/html", Loader = PageLoader } },
                { "css", new() { ContentType = "text/css", Loader = FileLoader } },
                { "js", new() { ContentType = "text/js", Loader = FileLoader } },
                { "", new() { ContentType = "text/html", Loader = PageLoader } },
            };
        }

        internal ResponsePacket Route(string verb, string path, Dictionary<string, string>? kvParams)
        {
            ResponsePacket output;

            string ext = path.RightOfRightmostOf('.');
            if (extFolderMap.TryGetValue(ext, out ExtensionInfo? extInfo))
            {
                string fullPath = (path.Length > 1) ? WebsitePath + path.Replace('/', '\\') : WebsitePath;
                output = extInfo.Loader(fullPath, ext, extInfo);
            }
            else
            {
                output = new() { Error = ServerError.UnknownTypes };
            }

            return output;
        }

        internal ResponsePacket PageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            ResponsePacket output;

            if (fullPath == WebsitePath)
                output = Route("GET", "/index.html", null);
            else
            {
                if (string.IsNullOrEmpty(ext))
                    fullPath = fullPath + ".html";

                fullPath = WebsitePath + "\\pages" + fullPath.RightOf(WebsitePath);
                output = FileLoader(fullPath, ext, extInfo);
            }

            return output;
        }

        internal ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fileStream);
            return new ResponsePacket
            {
                Data = reader.ReadBytes((int)fileStream.Length),
                ContentType = extInfo.ContentType
            };
        }

        internal ResponsePacket FileLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            return new ResponsePacket()
            {
                ContentType = extInfo.ContentType,
                Data = Encoding.UTF8.GetBytes(File.ReadAllText(fullPath)),
                Encoding = Encoding.UTF8
            };
        }

        internal static string GetWebsitePath()
        {
            string websitePath = Assembly.GetExecutingAssembly().Location;
            websitePath = websitePath.LeftOfRightmostOf('\\').LeftOfRightmostOf('\\').LeftOfRightmostOf('\\').LeftOfRightmostOf('\\') + "\\Website";

            return websitePath;
        }
    }
}