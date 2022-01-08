using BYO.WebServer.Helpers;

namespace BYO.WebServer.Models;

public class AuthenticatedRouteHandler : RouteHandler
{
    public AuthenticatedRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
        : base(handler)
    {
    }

    public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
    {
        ResponsePacket ret;

        if (session.Authorized)
        {
            ret = Handler(session, parms);
        }
        else
        {
            ret = Server.OnError(ServerError.NotAuthorized);
        }

        return ret;
    }
}