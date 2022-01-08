namespace BYO.WebServer;

public class Session
{
    public DateTime LastConnection { get; set; }
    public bool Authorized { get; set; }
    public Dictionary<string,string> Objects { get; set; }

    public Session()
    {
        Objects = new();
        UpdateLastConnectionTime();
    }

    public bool IsExpired(int expirationInSeconds)
    {
        return (DateTime.Now - LastConnection).TotalSeconds > expirationInSeconds;
    }

    public void UpdateLastConnectionTime()
    {
        LastConnection = DateTime.Now;
    }
}