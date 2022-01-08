using System.Text;

namespace BYO.WebServer.Helpers
{
    internal class ResponsePacket
    {
        public string Redirect { get; set; } = string.Empty;
        public byte[]? Data { get; init; } 
        public string ContentType { get; init; } = string.Empty;
        public Encoding? Encoding { get; init; }
        public ServerError Error { get; init; }
    }
}