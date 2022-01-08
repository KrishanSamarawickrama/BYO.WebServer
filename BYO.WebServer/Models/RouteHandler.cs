using BYO.WebServer.Helpers;

namespace BYO.WebServer.Models;

public abstract class RouteHandler
{
    protected Func<Session, Dictionary<string, string>, ResponsePacket> Handler;

    public RouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
    {
        Handler = handler;
    }

    public abstract ResponsePacket? Handle(Session session, Dictionary<string, string>? kvParams);
}