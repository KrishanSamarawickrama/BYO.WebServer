namespace BYO.WebServer.Models;

public class AnonymousRouteHandler : RouteHandler
{
    public AnonymousRouteHandler(Func<Session, Dictionary<string, string>, string> handler)
        : base(handler)
    {
    }

    public override string Handle(Session session, Dictionary<string, string> parms)
    {
        return Handler(session, parms);
    }
}