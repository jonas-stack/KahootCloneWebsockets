using System.Net;
using System.Net.Sockets;
using System.Web;
using Api.EventHandlers.EventMessageDtos;
using Fleck;
using WebSocketBoilerplate;

namespace Api.WebSockets;

public class CustomWebSocketServer(IConnectionManager manager, ILogger<CustomWebSocketServer> logger)
{
    public void Start(WebApplication app)
    {   
        //TODO lokal port
        //var port = GetAvailablePort(5005);
        //TODO deploy port 
        var port = GetAvailablePort(8080);
        Environment.SetEnvironmentVariable("PORT", port.ToString());
        var url = $"ws://0.0.0.0:{port}";
        var server = new WebSocketServer(url);

        server.Start(socket =>
        {
            var queryString = socket.ConnectionInfo.Path.Split('?').Length > 1
                ? socket.ConnectionInfo.Path.Split('?')[1]
                : "";

            var id = HttpUtility.ParseQueryString(queryString)["id"];

            socket.OnOpen = () =>
            {
                logger.LogInformation($"Socket connected: {socket.ConnectionInfo.Id}");
                if (id == null) throw new ArgumentNullException(nameof(id));
                manager.OnOpen(socket, id);
            };

            socket.OnClose = () =>
            {
                logger.LogInformation($"Socket disconnected: {socket.ConnectionInfo.Id}");
                if (id == null) throw new ArgumentNullException(nameof(id));
                manager.OnClose(socket, id);
            };
            
            socket.OnMessage = message =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await app.CallEventHandler(socket, message);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while handling message");
                        socket.SendDto(new ServerSendsErrorMessageDto
                        {
                            Error = e.Message
                        });
                    }
                });
            };
        });
    }

    /// <summary>
    ///     This is relevant for testing since concurrent tests require ports to serve the websocket server on
    /// </summary>
    /// <param name="startPort"></param>
    /// <returns></returns>
    private int GetAvailablePort(int startPort)
    {
        var port = startPort;
        var isPortAvailable = false;

        do
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Loopback, port);
                tcpListener.Start();
                tcpListener.Stop();
                isPortAvailable = true;
            }
            catch (SocketException)
            {
                port++;
            }
        } while (!isPortAvailable);

        return port;
    }
}