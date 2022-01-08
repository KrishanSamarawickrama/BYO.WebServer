namespace BYO.WebServer.Models;

public class Route
{
    public Route(string verb, string path, Func<Dictionary<string, string>?, string> action)
    {
        Verb = verb ?? throw new ArgumentNullException(nameof(verb));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Action = action;
    }

    public string Verb { get; set; }
    public string Path { get; set; }
    public Func<Dictionary<string, string>?, string> Action { get; set; }
}