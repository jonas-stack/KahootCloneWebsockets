using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class GameManagementService
    {
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<GameManagementService> _logger;

        public GameManagementService(KahootDbContext dbContext, ILogger<GameManagementService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Gets the active game. If none exists, create one.
        /// </summary>
        public async Task<Game> GetOrCreateActiveGameAsync()
        {
            var activeGame = await _dbContext.Games
                .OrderByDescending(g => g.CreatedAt) // Use the latest game
                .FirstOrDefaultAsync();

            if (activeGame == null)
            {
                _logger.LogInformation("No active game found. Creating a new one...");

                activeGame = new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Movie Quiz",
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Games.Add(activeGame);
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Active game: {GameId} - {GameName}", activeGame.Id, activeGame.Name);
            return activeGame;
        }
    }
}