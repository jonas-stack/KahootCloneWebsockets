using DataAccess.ModelDtos;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class QuestionManagementService
    {
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<QuestionManagementService> _logger;

        public QuestionManagementService(KahootDbContext dbContext, ILogger<QuestionManagementService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<QuestionDto?> GetUnansweredQuestionAsync(Guid gameId)
        {
            _logger.LogDebug("Fetching unanswered question for game {GameId}", gameId);

            return await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId && !q.Answered)
                .OrderBy(q => Guid.NewGuid()) // Random selection
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
                .FirstOrDefaultAsync();
        }

    }
}