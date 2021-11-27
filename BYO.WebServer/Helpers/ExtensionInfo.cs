namespace BYO.WebServer.Helpers
{
    internal class ExtensionInfo
    {
        public string ContentType { get; init; } = String.Empty;
        public Func<string, string, ExtensionInfo, ResponsePacket> Loader { get; set; }
    }
}