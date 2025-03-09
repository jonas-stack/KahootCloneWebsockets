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

            var question = await _dbContext.Questions
                .Include(q => q.QuestionOptions)
                .Where(q => q.GameId == gameId && !q.Answered)
                .OrderBy(q => Guid.NewGuid()) // Random selection
                .FirstOrDefaultAsync();

            if (question == null)
            {
                _logger.LogWarning("No unanswered questions found for game {GameId}", gameId);
                return null;
            }

            // ✅ Mark the question as answered
            question.Answered = true;
            await _dbContext.SaveChangesAsync();

            return new QuestionDto
            {
                Id = question.Id,
                GameId = question.GameId,
                QuestionText = question.QuestionText,
                Answered = true, // ✅ Ensure it's marked answered
                QuestionOptions = question.QuestionOptions.Select(opt => new QuestionOptionDto
                {
                    Id = opt.Id,
                    OptionText = opt.OptionText,
                    IsCorrect = opt.IsCorrect
                }).ToList()
            };
        }

    }
}