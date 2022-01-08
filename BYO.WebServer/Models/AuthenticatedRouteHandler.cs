namespace BYO.WebServer.Models;

public class AuthenticatedRouteHandler : RouteHandler
{
    public AuthenticatedRouteHandler(Func<Session, Dictionary<string, string>, string> handler)
        : base(handler)
    {
    }

    public override string Handle(Session session, Dictionary<string, string> parms)
    {
        string ret;

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