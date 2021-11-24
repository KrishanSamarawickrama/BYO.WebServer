using BYO.WebServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer
{
    public class Router
    {
        public string WebsitePath { get; set; }

        private Dictionary<string, ExtensionInfo> extFolderMap;

        public Router()
        {
            extFolderMap = new()
            {
                { "ico" , new() { ContentType = "image/ico", Loader = ImageLoader } },
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
            ResponsePacket output = new();

            string ext = path.RightOfRightmostOf('.');
            if (extFolderMap.TryGetValue(ext, out ExtensionInfo? extInfo))
            {
                string fullPath = (path.Length > 1) ? WebsitePath + path.Replace('/','\\') : WebsitePath;
                output = extInfo.Loader(fullPath, ext, extInfo);
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

                var f = fullPath.RightOf(WebsitePath);
                fullPath = WebsitePath + "\\pages" + fullPath.RightOf(WebsitePath);
                output = FileLoader(fullPath,ext, extInfo);
            }

            return output;
        }

        internal ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new BinaryReader(fileStream);
            var output = new ResponsePacket
            {
                Data = reader.ReadBytes((int)fileStream.Length),
                ContentType = extInfo.ContentType
            };
            return output;
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

    }
}
