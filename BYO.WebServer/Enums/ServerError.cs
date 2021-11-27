namespace BYO.WebServer
{
    public enum ServerError
    {
        Ok,
        ExpiredSession,
        NotAuthorized,
        FileNotFound,
        PageNotFound,
        ServerError,
        UnknownTypes
    }
}