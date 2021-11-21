using BYO.WebServer.Helpers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BYO.WebServer;

public static class Server
{
    //private static HttpListener listener;

    public static int maxSimultaneousConnections = 20;
    private static Semaphore sem = new(maxSimultaneousConnections, maxSimultaneousConnections);

    private static List<IPAddress> GetLocalHostIPs()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> output = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
        return output;
    }

    private static HttpListener InitializeListener(List<IPAddress> localHostIps)
    {
        HttpListener listener = new();
        listener.Prefixes.Add("http://localhost/");
        listener.Prefixes.Add("http://+:80/");

        localHostIps.ForEach(ip =>
        {
            Console.WriteLine($"Listening on IP http://{ip}/");
            listener.Prefixes.Add($"http://{ip}/");
        });

        return listener;
    }

    private static void Start(HttpListener listener)
    {
        listener.Start();
        Task.Run(() => RunServer(listener));
    }

    private static void RunServer(HttpListener listener)
    {
        while (true)
        {
            sem.WaitOne();
            StartConnectionListener(listener);
        }
    }

    private static async void StartConnectionListener(HttpListener listener)
    {
        HttpListenerContext context = await listener.GetContextAsync();
        sem.Release();

        HttpRequestProcessor.ProcessRequest(context.Request);






        //Do someting
        string response = @"<html><head><meta http-equiv='content-type' content='text/html; charset=utf-8'/>
      </ head > Hello Browser! </ html > ";
        byte[] encoded = Encoding.UTF8.GetBytes(response);
        context.Response.ContentLength64 = encoded.Length;
        context.Response.OutputStream.Write(encoded, 0, encoded.Length);
        context.Response.OutputStream.Close();

    }

    public static void Start()
    {
        var localIPs = GetLocalHostIPs();
        var listener = InitializeListener(localIPs);
        Start(listener);
    }

    
}