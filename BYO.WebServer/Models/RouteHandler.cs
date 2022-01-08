namespace BYO.WebServer.Models;

public abstract class RouteHandler
{
    protected Func<Session, Dictionary<string, string>, string> Handler;

    public RouteHandler(Func<Session, Dictionary<string, string>, string> handler)
    {
        Handler = handler;
    }

    public abstract string Handle(Session session, Dictionary<string, string>? kvParams);
}