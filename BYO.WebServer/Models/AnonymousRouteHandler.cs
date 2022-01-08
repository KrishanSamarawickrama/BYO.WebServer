using BYO.WebServer.Helpers;

namespace BYO.WebServer.Models;

public class AnonymousRouteHandler : RouteHandler
{
    public AnonymousRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
        : base(handler)
    {
    }

    public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
    {
        return Handler(session, parms);
    }
}