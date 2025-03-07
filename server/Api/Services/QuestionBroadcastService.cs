using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Api.EventHandlers.EventMessageDtos;
using Api.WebSockets;
using DataAccess.Models;
using WebSocketBoilerplate;

namespace Api.Services
{
    public class QuestionBroadcastService
    {
        private readonly KahootDbContext _dbContext;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<QuestionBroadcastService> _logger;

        public QuestionBroadcastService(KahootDbContext dbContext, IConnectionManager connectionManager, ILogger<QuestionBroadcastService> logger)
        {
            _dbContext = dbContext;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public async Task BroadcastQuestionsForGameAsync(Guid gameId, string topic = null)
        {
            topic ??= gameId.ToString();

            var questions = await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId)
                .ToListAsync();

            foreach (var question in questions)
            {
                var broadcastQuestionDto = new BroadcastQuestionDto
                {
                    Id = question.Id,
                    QuestionText = question.QuestionText,
                    Topic = topic,
                    QuestionOptions = question.QuestionOptions.Select(option => new QuestionOptionDto
                    {
                        Id = option.Id,
                        OptionText = option.OptionText,
                        IsCorrect = option.IsCorrect
                    }).ToList(),
                    TimeLimitSeconds = 30
                };

                _logger.LogDebug("Broadcasting question {QuestionId} for game {GameId} on topic '{Topic}'", question.Id, gameId, topic);
                await _connectionManager.BroadcastToTopic(topic, broadcastQuestionDto);
                await Task.Delay(broadcastQuestionDto.TimeLimitSeconds * 1000);

                var timeOverDto = new QuestionTimeOverDto
                {
                    QuestionId = question.Id,
                    Message = "Time is up for answering the question."
                };
                timeOverDto.requestId = broadcastQuestionDto.requestId;
                await _connectionManager.BroadcastToTopic(topic, timeOverDto);
                _logger.LogDebug("Broadcasted end-of-question message for question {QuestionId}", question.Id);
            }
        }
    }
}
