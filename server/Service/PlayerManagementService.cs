using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class PlayerManagementService
    {
        private readonly KahootDbContext _dbContext;
        private readonly ILogger<PlayerManagementService> _logger;

        public PlayerManagementService(KahootDbContext dbContext, ILogger<PlayerManagementService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        
        /// Adds a player to the database when they join a game.
        public async Task AddPlayerAsync(string playerId, string nickname, string gameId)
        {
            if (await _dbContext.Players.AnyAsync(p => p.Id.ToString() == playerId))
            {
                _logger.LogInformation("Player {PlayerId} already exists in the database.", playerId);
                return;
            }

            var player = new Player
            {
                Id = Guid.Parse(playerId),
                Nickname = nickname,
                GameId = Guid.Parse(gameId)
            };

            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Player {PlayerId} added to the database.", playerId);
        }

       
        /// Removes a player from the database when they disconnect.
        public async Task RemovePlayerAsync(string playerId)
        {
            var player = await _dbContext.Players.FirstOrDefaultAsync(p => p.Id.ToString() == playerId);
            if (player != null)
            {
                _dbContext.Players.Remove(player);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Player {PlayerId} removed from the database.", playerId);
            }
            else
            {
                _logger.LogWarning("Attempted to remove non-existing player: {PlayerId}", playerId);
            }
        }
    }
}
