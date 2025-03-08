using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;

namespace Api.EventHandlers;

public class BroadcastQuestionEventHandler : BaseEventHandler<BroadcastQuestionDto>
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

    public override async Task Handle(BroadcastQuestionDto dto, IWebSocketConnection socket)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Topic))
        {
            socket.SendDto(new ServerSendsErrorMessageDto
            {
                Error = "Invalid game ID.",
                requestId = dto?.requestId
            });
            return;
        }

        // Fetch a random unanswered question from the database
        var question = await _dbContext.Questions
            .Where(q => q.GameId.ToString() == dto.Topic && !q.Answered)
            .OrderBy(q => Guid.NewGuid()) // Random selection
            .Select(q => new BroadcastQuestionDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                QuestionOptions = q.QuestionOptions.Select(opt => new QuestionOptionDto
                {
                    Id = opt.Id,
                    OptionText = opt.OptionText,
                    IsCorrect = opt.IsCorrect
                }).ToList(),
                TimeLimitSeconds = 30, // Set a default time limit
                Topic = dto.Topic
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

        _logger.LogDebug("Broadcasting question {QuestionId} to topic '{Topic}'", question.Id, dto.Topic);
        await _connectionManager.BroadcastToTopic(dto.Topic, question);

        _logger.LogDebug("Question {QuestionId} broadcasted", question.Id);

        // Wait for the question time limit.
        await Task.Delay(question.TimeLimitSeconds * 1000);

        // Send "Time Over" message
        var timeOverDto = new QuestionTimeOverDto
        {
            QuestionId = question.Id,
            Message = "Time is up for answering the question."
        };
        timeOverDto.requestId = dto.requestId;
        await _connectionManager.BroadcastToTopic(dto.Topic, timeOverDto);
        _logger.LogDebug("Time-over message broadcasted for question {QuestionId}", question.Id);
    }
}