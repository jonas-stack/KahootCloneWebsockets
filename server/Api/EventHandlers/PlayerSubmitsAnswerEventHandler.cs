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

        public PlayerSubmitsAnswerEventHandler(KahootDbContext dbContext, IConnectionManager connectionManager, ILogger<PlayerSubmitsAnswerEventHandler> logger)
        {
            _dbContext = dbContext;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(PlayerSubmitsAnswerDto dto, IWebSocketConnection socket)
        {
            if (!Guid.TryParse(dto.PlayerId, out Guid playerId) || dto.QuestionId == Guid.Empty || dto.SelectedOptionId == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid answer submission.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Player {PlayerId} submitted an answer for question {QuestionId}", dto.PlayerId, dto.QuestionId);

            // ✅ Check if the player exists
            var player = await _dbContext.Players.FindAsync(playerId);
            if (player == null)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Player does not exist.",
                    requestId = dto.requestId
                });
                return;
            }

            // ✅ Check if the question exists
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

            // ✅ Check if the selected option exists for the question
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

            // ✅ Check if the answer is correct
            bool isCorrect = selectedOption.IsCorrect;
            int score = isCorrect ? 10 : 0;  // Example scoring logic

            _logger.LogInformation("Player {PlayerId} answered {AnswerCorrect}", playerId, isCorrect ? "CORRECTLY" : "INCORRECTLY");

            // ✅ Send response to player
            socket.SendDto(new AnswerValidationDto
            {
                PlayerId = dto.PlayerId,
                QuestionId = dto.QuestionId,
                IsCorrect = isCorrect,
                ScoreAwarded = score,
                requestId = dto.requestId
            });
        }
    }
}
