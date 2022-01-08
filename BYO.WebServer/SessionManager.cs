using System.Net;

namespace BYO.WebServer;

public class SessionManager
{
    protected Dictionary<IPAddress, Session> SessionMap;

    public SessionManager()
    {
        SessionMap = new();
    }

    public Session GetSession(IPEndPoint remoteEndPoint)
    {
        if (!SessionMap.ContainsKey(remoteEndPoint.Address))
            SessionMap.Add(remoteEndPoint.Address, new());

        return SessionMap[remoteEndPoint.Address];
    }
}

//TODO: We need a way to remove very old sessions so that the server doesn't accumulate thousands of stale endpoints.