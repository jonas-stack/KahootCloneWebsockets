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

            // ✅ Send game progression before broadcasting the question
            var gameProgressionDto = new GameProgressionDto
            {
                GameId = gameId,
                CurrentRound = dto.RoundNumber,
                TotalRounds = 5, // Set dynamically if needed
                Message = $"Round {dto.RoundNumber} is starting!"
            };

            await _connectionManager.BroadcastToTopic(dto.GameId, gameProgressionDto);
            _logger.LogDebug("Game progression sent for round {RoundNumber} in game {GameId}", dto.RoundNumber, gameId);

            // ✅ Fetch an unanswered question from the database
            var questionEntity = await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId && !q.Answered)
                .OrderBy(q => Guid.NewGuid()) // Random selection
                .FirstOrDefaultAsync();

            if (questionEntity == null)
            {
                // ✅ If no more questions remain, broadcast "Game Over"
                var gameOverDto = new GameProgressionDto
                {
                    GameId = gameId,
                    CurrentRound = dto.RoundNumber,
                    TotalRounds = dto.RoundNumber,
                    Message = "Game Over! No more questions left."
                };

                await _connectionManager.BroadcastToTopic(dto.GameId, gameOverDto);
                _logger.LogInformation("Game {GameId} has ended. No more questions available.", gameId);
                return;
            }

            // ✅ Convert the question entity into a DTO
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
                    IsCorrect = false // Hide correct answers
                }).ToList()
            };

            // ✅ Use `RoundStartedDto` to broadcast the question
            var roundStartedDto = new RoundStartedDto
            {
                RoundNumber = dto.RoundNumber,
                Question = questionDto
            };

            await _connectionManager.BroadcastToTopic(dto.GameId, roundStartedDto);
            _logger.LogDebug("Round {RoundNumber} started with question {QuestionId} in game {GameId}", 
                dto.RoundNumber, questionDto.Id, gameId);
        }
    }
}
