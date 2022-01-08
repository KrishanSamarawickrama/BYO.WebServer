using BYO.WebServer.Helpers;

namespace BYO.WebServer.Models;

public class AuthenticatedExpirableRouteHandler : AuthenticatedRouteHandler
{
    public AuthenticatedExpirableRouteHandler(Func<Session, Dictionary<string, string>, ResponsePacket> handler)
        : base(handler)
    {
    }

    public override ResponsePacket Handle(Session session, Dictionary<string, string> parms)
    {
        ResponsePacket ret;

        if (session.IsExpired(Server.ExpirationTimeSeconds))
        {
            session.Authorized = false;
            ret = Server.OnError(ServerError.ExpiredSession);
        }
        else
        {
            ret = base.Handle(session, parms);
        }

        return ret;
    }
}