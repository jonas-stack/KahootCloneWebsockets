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
    private readonly QuestionManagementService _questionManagementService;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<BroadcastQuestionEventHandler> _logger;

    public BroadcastQuestionEventHandler(QuestionManagementService questionManagementService, IConnectionManager connectionManager, ILogger<BroadcastQuestionEventHandler> logger)
    {
        _questionManagementService = questionManagementService;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public override async Task Handle(AdminStartsNextRoundDto dto, IWebSocketConnection socket)
    {
        if (dto == null || dto.GameId == Guid.Empty)
        {
            socket.SendDto(new ServerSendsErrorMessageDto
            {
                Error = "Invalid game ID.",
                requestId = dto?.requestId
            });
            return;
        }

        var gameId = dto.GameId;

        _logger.LogDebug("Admin is starting round {RoundNumber} for game {GameId}", dto.RoundNumber, gameId);

        // ✅ Fetch an unanswered question from the database
        var questionDto = await _questionManagementService.GetUnansweredQuestionAsync(gameId);

        if (questionDto == null)
        {
            // ✅ No questions left → Trigger `GameProgressionEventHandler`
            var gameProgressionDto = new GameProgressionDto
            {
                GameId = gameId,
                CurrentRound = dto.RoundNumber,
                TotalRounds = dto.RoundNumber,
                Message = "Game Over! No more questions left."
            };

            await _connectionManager.BroadcastToTopic(gameId.ToString(), gameProgressionDto);
            _logger.LogInformation("Game {GameId} progression updated: No more questions left.", gameId);
            return;
        }

        // ✅ Hide correct answers before broadcasting to players
        foreach (var option in questionDto.QuestionOptions)
        {
            option.IsCorrect = false;
        }

        // ✅ Broadcast game progression update before question
        var gameProgressionUpdate = new GameProgressionDto
        {
            GameId = gameId,
            CurrentRound = dto.RoundNumber,
            TotalRounds = dto.RoundNumber + 1, // Increment for next round
            Message = $"Round {dto.RoundNumber} is starting!"
        };
        await _connectionManager.BroadcastToTopic(gameId.ToString(), gameProgressionUpdate);

        // ✅ Broadcast the question
        var roundStartedDto = new RoundStartedDto
        {
            RoundNumber = dto.RoundNumber,
            Question = questionDto
        };
        await _connectionManager.BroadcastToTopic(gameId.ToString(), roundStartedDto);

        _logger.LogDebug("Broadcasted question {QuestionId} for round {RoundNumber} in game {GameId}",
            questionDto.Id, dto.RoundNumber, gameId);
    }
}