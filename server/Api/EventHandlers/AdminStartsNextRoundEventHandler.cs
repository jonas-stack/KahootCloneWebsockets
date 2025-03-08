using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.ModelDtos;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebSocketBoilerplate;

namespace Api.EventHandlers
{
    public class AdminStartsNextRoundEventHandler : BaseEventHandler<AdminStartsNextRoundDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<AdminStartsNextRoundEventHandler> _logger;

        public AdminStartsNextRoundEventHandler(
            IConnectionManager connectionManager, 
            KahootDbContext dbContext, 
            ILogger<AdminStartsNextRoundEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _dbContext = dbContext;
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

            if (!Guid.TryParse(dto.GameId, out Guid gameId))
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid GameId format.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Admin is starting round {RoundNumber} for game {GameId}", dto.RoundNumber, gameId);

            // Fetch a new unanswered question from the database
            var questionEntity = await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId && !q.Answered)
                .OrderBy(q => Guid.NewGuid()) // Random selection
                .FirstOrDefaultAsync();

            if (questionEntity == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "No available questions for this game.",
                    requestId = dto?.requestId
                });
                return;
            }

            // Map to QuestionDto (from DataAccess)
            var questionDto = new QuestionDto
            {
                Id = questionEntity.Id,
                GameId = questionEntity.GameId,
                QuestionText = questionEntity.QuestionText,
                Answered = questionEntity.Answered,
                QuestionOptions = questionEntity.QuestionOptions.Select(opt => new QuestionOptionDto
                {
                    Id = opt.Id,
                    QuestionId = opt.QuestionId,
                    OptionText = opt.OptionText,
                    IsCorrect = opt.IsCorrect
                }).ToList()
            };

            // Map to RoundStartedDto
            var roundStartedDto = new RoundStartedDto
            {
                RoundNumber = dto.RoundNumber,
                Question = questionDto
            };

            // Broadcast the new round to all players in the game
            await _connectionManager.BroadcastToTopic(dto.GameId, roundStartedDto);
            _logger.LogDebug("Round {RoundNumber} started for game {GameId}", dto.RoundNumber, dto.GameId);
        }
    }
}
