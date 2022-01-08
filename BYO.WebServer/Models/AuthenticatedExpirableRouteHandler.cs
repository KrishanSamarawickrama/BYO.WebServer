namespace BYO.WebServer.Models;

public class AuthenticatedExpirableRouteHandler : AuthenticatedRouteHandler
{
    public AuthenticatedExpirableRouteHandler(Func<Session, Dictionary<string, string>, string> handler)
        : base(handler)
    {
    }

    public override string Handle(Session session, Dictionary<string, string> parms)
    {
        string ret;

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