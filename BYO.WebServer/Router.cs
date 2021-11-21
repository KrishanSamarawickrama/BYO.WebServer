using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer
{
    public class Router
    {
        public string WebsitePath { get; internal set; }

        internal void Route(string verb, string path, Dictionary<string, string> kvParams)
        {
            throw new NotImplementedException();
        }
    }
}
