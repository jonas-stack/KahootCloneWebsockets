using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using Fleck;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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
            if (dto == null || string.IsNullOrEmpty(dto.PlayerId) || dto.QuestionId == Guid.Empty)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid answer submission.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("Player {PlayerId} submitted an answer for question {QuestionId}", dto.PlayerId, dto.QuestionId);

            // Create and save the answer.
            var playerAnswer = new PlayerAnswer
            {
                PlayerId = Guid.Parse(dto.PlayerId),
                QuestionId = dto.QuestionId,
                SelectedOptionId = dto.SelectedOptionId,
                AnswerTimestamp = DateTime.UtcNow
            };

            _dbContext.PlayerAnswers.Add(playerAnswer);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Player answer saved for question {QuestionId}", dto.QuestionId);

            // Optionally, send a confirmation back.
            socket.SendDto(new ServerConfirmsPlayerJoinDto
            {
                PlayerId = dto.PlayerId,
                Message = "Answer received.",
                requestId = dto.requestId
            });
        }
    }
}
