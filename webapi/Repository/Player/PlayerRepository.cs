using webapi.Data;
using webapi.Domain.Player;

namespace webapi.Repository.Player
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ILogger<PlayerRepository> _logger;
        private readonly GameContext _gameContext;

        public PlayerRepository(ILogger<PlayerRepository> logger, GameContext gameContext)
        {
            _logger = logger;
            _gameContext = gameContext;
        }

        public void CreatePlayer(string gameName, string playerName)
        {
            var playerCount = GetPlayers(gameName).Count();
            var player = new Domain.Player.Player(gameName, playerName)
            {
                PlayerIndex = playerCount
            };

            _gameContext.Players.Add(player);
            _gameContext.SaveChanges();
        }

        public IPlayer? GetPlayer(string gameName, string playerName)
        {
            try
            {
                return _gameContext.Players.SingleOrDefault(player => player.GameName == gameName && player.Name == playerName);
            }
            catch (Exception)
            {
                _logger.LogError("Error retrieving a player with '{GameName}' && '{PlayerName}'", gameName, playerName);
                return null;
            }
        }

        public IEnumerable<IPlayer> GetPlayers(string gameName)
        {
            return _gameContext.Players.Where(player => player.GameName == gameName);
        }
    }
}
