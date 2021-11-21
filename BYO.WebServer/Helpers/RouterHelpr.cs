using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer.Helpers
{
    internal class RouterHelpr
    {
        public ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
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
    }
}
