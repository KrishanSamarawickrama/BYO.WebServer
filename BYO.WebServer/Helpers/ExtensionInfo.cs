using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer.Helpers
{
    internal class ExtensionInfo
    {
        public string ContentType { get; set; }
        public Func<string,string,ExtensionInfo,ResponsePacket> Loader { get; set; }
    }
}
