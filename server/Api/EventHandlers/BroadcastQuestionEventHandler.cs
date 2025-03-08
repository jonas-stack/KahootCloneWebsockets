using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.ModelDtos;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;


namespace Api.EventHandlers;

public class BroadcastQuestionEventHandler : BaseEventHandler<AdminStartsNextRoundDto>
{
    private readonly IConnectionManager _connectionManager;
    private readonly KahootDbContext _dbContext;
    private readonly ILogger<BroadcastQuestionEventHandler> _logger;

    public BroadcastQuestionEventHandler(IConnectionManager connectionManager,
        ILogger<BroadcastQuestionEventHandler> logger, KahootDbContext dbContext)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _dbContext = dbContext;
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

        // Fetch a new unanswered question from the database
        var question = await _dbContext.Questions
            .Where(q => q.GameId.ToString() == dto.GameId && !q.Answered)
            .OrderBy(q => Guid.NewGuid()) // Random selection
            .Select(q => new RoundStartedDto
            {
                RoundNumber = dto.RoundNumber,
                Question = new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionOptions = q.QuestionOptions.Select(opt => new QuestionOptionDto
                    {
                        Id = opt.Id,
                        OptionText = opt.OptionText,
                        IsCorrect = opt.IsCorrect
                    }).ToList()
                }
            }).FirstOrDefaultAsync();

        if (question == null)
        {
            socket.SendDto(new ServerSendsErrorMessageDto
            {
                Error = "No available questions for this game.",
                requestId = dto.requestId
            });
            return;
        }

        // Broadcast the new round to all players
        await _connectionManager.BroadcastToTopic(dto.GameId, question);
        _logger.LogDebug("Round {RoundNumber} started for game {GameId}", dto.RoundNumber, dto.GameId);
    }
}
