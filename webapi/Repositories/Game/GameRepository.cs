using webapi.Data;
using webapi.Domain.Game;

namespace webapi.Repositories.Game
{
    public class GameRepository : IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;
        private readonly GameContext _gameContext;

        public GameRepository(ILogger<GameRepository> logger, GameContext gameContext)
        {
            _logger = logger;
            _gameContext = gameContext;
        }

        public void CreateGame(string gameName)
        {
            var game = new Domain.Game.Game(gameName);

            _gameContext.Add(game);
            _gameContext.SaveChanges();
        }

        public IGame? GetGame(string gameName)
        {
            try
            {
                return _gameContext.Games.SingleOrDefault(game => game.Name == gameName);
            }
            catch (Exception)
            {
                _logger.LogError("Error retrieving a game with '{GameName}'", gameName);
                return null;
            }
        }
    }
}
