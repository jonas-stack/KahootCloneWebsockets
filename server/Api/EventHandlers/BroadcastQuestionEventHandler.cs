using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.ModelDtos;
using Fleck;
using Microsoft.Extensions.Logging;
using Service;
using WebSocketBoilerplate;

namespace Api.EventHandlers;

public class BroadcastQuestionEventHandler : BaseEventHandler<AdminStartsNextRoundDto>
{
    private readonly EventHandlerServices _eventHandlerServices;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<BroadcastQuestionEventHandler> _logger;

    public BroadcastQuestionEventHandler(EventHandlerServices eventHandlerServices, IConnectionManager connectionManager, ILogger<BroadcastQuestionEventHandler> logger)
    {
        _eventHandlerServices = eventHandlerServices;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public override async Task Handle(AdminStartsNextRoundDto dto, IWebSocketConnection socket)
    {
        if (dto == null || string.IsNullOrEmpty(dto.GameId))
        {
            socket.SendDto(new ServerSendsErrorMessageDto
            {
                Error = "Invalid game ID.",
                requestId = dto?.requestId
            });
            return;
        }

        _logger.LogDebug("Admin is starting round {RoundNumber} for game {GameId}", dto.RoundNumber, dto.GameId);

        // ✅ Fetch a new unanswered question from the service layer
        var questionDto = await _eventHandlerServices.GetUnansweredQuestionAsync(Guid.Parse(dto.GameId));

        if (questionDto == null)
        {
            socket.SendDto(new ServerSendsErrorMessageDto
            {
                Error = "No available questions for this game.",
                requestId = dto.requestId
            });
            return;
        }

        // ✅ Map to RoundStartedDto
        var roundStartedDto = new RoundStartedDto
        {
            RoundNumber = dto.RoundNumber,
            Question = questionDto
        };

        // ✅ Broadcast the new round to all players
        await _connectionManager.BroadcastToTopic(dto.GameId, roundStartedDto);
        _logger.LogDebug("Round {RoundNumber} started for game {GameId}", dto.RoundNumber, dto.GameId);
    }
}
