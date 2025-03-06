using Api.EventHandlers.Dtos;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;

namespace Api.EventHandlers;

public class AdminStartsGameEventHandler : BaseEventHandler<GameDto>
{
    private readonly IConnectionManager _connectionManager;

    public AdminStartsGameEventHandler(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public override async Task Handle(GameDto dto, IWebSocketConnection socket)
    {
        await _connectionManager.BroadcastToTopic("lobby", dto);
    }
}