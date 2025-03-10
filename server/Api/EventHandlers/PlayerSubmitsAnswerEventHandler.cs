using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using WebSocketBoilerplate;

namespace Api.EventHandlers
{
    public class PlayerSubmitsAnswerEventHandler : BaseEventHandler<PlayerSubmitsAnswerDto>
    {
        private readonly KahootDbContext _dbContext;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<PlayerSubmitsAnswerEventHandler> _logger;

        private static Dictionary<Guid, int> playerScores = new(); // ✅ Store scores temporarily

        public PlayerSubmitsAnswerEventHandler(KahootDbContext dbContext, IConnectionManager connectionManager, ILogger<PlayerSubmitsAnswerEventHandler> logger)
        {
            _dbContext = dbContext;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(PlayerSubmitsAnswerDto dto, IWebSocketConnection socket)
        {
            if (dto.PlayerId == Guid.Empty || dto.QuestionId == Guid.Empty || dto.SelectedOptionId == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid answer submission.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Player {PlayerId} submitted an answer for question {QuestionId}", dto.PlayerId, dto.QuestionId);

            // ✅ Fetch the question and its GameId
            var question = await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

            if (question == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Question does not exist.",
                    requestId = dto.requestId
                });
                return;
            }

            // ✅ Extract GameId from Question
            var gameId = question.GameId;
            if (gameId == null || gameId == Guid.Empty)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Could not determine game for this question.",
                    requestId = dto.requestId
                });
                return;
            }

            // ✅ Check if the selected option exists
            var selectedOption = question.QuestionOptions.FirstOrDefault(o => o.Id == dto.SelectedOptionId);
            if (selectedOption == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid answer option.",
                    requestId = dto.requestId
                });
                return;
            }

            // ✅ Determine if the answer is correct
            bool isCorrect = selectedOption.IsCorrect;
            int score = isCorrect ? 10 : 0;

            // ✅ Track Score in Dictionary
            if (!playerScores.ContainsKey(dto.PlayerId))
                playerScores[dto.PlayerId] = 0;

            playerScores[dto.PlayerId] += score;

            _logger.LogInformation("Player {PlayerId} answered {AnswerCorrect}. Total Score: {Score}",
                dto.PlayerId, isCorrect ? "CORRECTLY" : "INCORRECTLY", playerScores[dto.PlayerId]);

            // ✅ Send Answer Validation Response
            socket.SendDto(new AnswerValidationDto
            {
                PlayerId = dto.PlayerId,
                QuestionId = dto.QuestionId,
                IsCorrect = isCorrect,
                ScoreAwarded = score,
                requestId = dto.requestId
            });

            // ✅ Check if all players have answered
            int totalPlayers = await _dbContext.Players.CountAsync(p => p.GameId == gameId);
            int answeredPlayers = playerScores.Count;

            if (answeredPlayers >= totalPlayers)
            {
                _logger.LogInformation("All players have answered. Broadcasting round results...");

                // ✅ Fetch the current round number dynamically
                int currentRoundNumber = await GetCurrentRoundNumber(gameId.Value);

                // ✅ Broadcast Round Results
                var roundResults = playerScores.Select(kvp => new RoundResultEntry
                {
                    PlayerId = kvp.Key,
                    Score = kvp.Value
                }).ToList();

                await _connectionManager.BroadcastToTopic(gameId.ToString(), new RoundResultDto
                {
                    GameId = gameId.Value,
                    RoundNumber = currentRoundNumber,
                    Results = roundResults
                });

                // ✅ Reset Scores for Next Round
                playerScores.Clear();

                // ✅ Delay before starting the next round
                await Task.Delay(5000);

                // ✅ Broadcast Game Progression
                await _connectionManager.BroadcastToTopic(gameId.ToString(), new GameProgressionDto
                {
                    GameId = gameId.Value,
                    CurrentRound = currentRoundNumber + 1,
                    TotalRounds = 5, // Adjust if needed
                    Message = $"Round {currentRoundNumber + 1} is starting!"
                });
            }
        }

        private async Task<int> GetCurrentRoundNumber(Guid gameId)
        {
            var lastQuestion = await _dbContext.Questions
                .Where(q => q.GameId == gameId)
                .OrderByDescending(q => q.Id)
                .FirstOrDefaultAsync();

            return lastQuestion != null ? lastQuestion.Id.GetHashCode() % 10 + 1 : 1; // Example logic to derive round number
        }
    }
}