using webapi.Data;
using webapi.Domain.PlayerConnection;

namespace webapi.Repository.PlayerConnection
{
    public class PlayerConnectionRepository : IPlayerConnectionRepository
    {
        private readonly ILogger<PlayerConnectionRepository> _logger;
        private readonly GameContext _gameContext;

        public PlayerConnectionRepository(ILogger<PlayerConnectionRepository> logger, GameContext gameContext)
        {
            _logger = logger;
            _gameContext = gameContext;
        }

        public void CreatePlayerConnection(string gameName, string playerName, string connectionId)
        {
            var playerConnection = new Domain.PlayerConnection.PlayerConnection(gameName, playerName, connectionId);

            _gameContext.PlayerConnections.Add(playerConnection);
            _gameContext.SaveChanges();
        }

        public IPlayerConnection? GetPlayerConnection(string gameName, string playerName)
        {
            try
            {
                return _gameContext.PlayerConnections.SingleOrDefault(playerConnection => playerConnection.GameName == gameName && playerConnection.PlayerName == playerName);
            }
            catch (Exception)
            {
                _logger.LogError("Error retrieving a player connection with '{GameName}' && '{PlayerName}'", gameName, playerName);
                return null;
            }
        }

        public IEnumerable<IPlayerConnection> GetPlayerConnections(string gameName)
        {
            return _gameContext.PlayerConnections.Where(playerConnection => playerConnection.GameName == gameName);
        }
    }
}
