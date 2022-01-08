namespace BYO.WebServer.Helpers
{
    internal class ExtensionInfo
    {
        public ExtensionInfo(string contentType, Func<string, string, ExtensionInfo, ResponsePacket> loader)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public string ContentType { get; init; }
        public Func<string, string, ExtensionInfo, ResponsePacket> Loader { get; init; }
    }
}