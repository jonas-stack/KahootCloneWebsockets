using DataAccess.ModelDtos;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class EventHandlerServices
    {
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<EventHandlerServices> _logger;

        public EventHandlerServices(KahootDbContext dbContext, ILogger<EventHandlerServices> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<QuestionDto>> GetQuestionsForGameAsync(Guid gameId)
        {
            _logger.LogDebug("Fetching questions for game {GameId}", gameId);

            return await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId)
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionOptions = q.QuestionOptions.Select(opt => new QuestionOptionDto
                    {
                        Id = opt.Id,
                        OptionText = opt.OptionText,
                        IsCorrect = opt.IsCorrect
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}