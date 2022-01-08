namespace BYO.WebServer.Models;

public class Route
{
    public Route(string verb, string path,RouteHandler handler)
    {
        Verb = verb ?? throw new ArgumentNullException(nameof(verb));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Handler = handler;
    }

    public string Verb { get; set; }
    public string Path { get; set; }
    public RouteHandler Handler { get; set; }
}