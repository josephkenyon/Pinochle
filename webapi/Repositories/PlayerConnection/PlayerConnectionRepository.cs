using webapi.Data;
using webapi.Domain.GameDetails;
using webapi.Domain.PlayerConnectionDetails;
using webapi.Domain.PlayerDetails;

namespace webapi.Repositories.PlayerConnection
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

        public void CreatePlayerConnection(IPlayerConnectionDetails playerConnectionDetails)
        {
            var playerConnection = new PlayerConnectionDetails(
                playerConnectionDetails.GetGameName(),
                playerConnectionDetails.GetPlayerName(),
                playerConnectionDetails.GetConnectionId()
            );

            _gameContext.PlayerConnections.Add(playerConnection);
            _gameContext.SaveChanges();
        }

        public IPlayerConnectionDetails? GetPlayerConnection(IPlayerDetails playerDetails)
        {
            var gameName = playerDetails.GetGameName();
            var playerName = playerDetails.GetPlayerName();

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

        public IEnumerable<IPlayerConnectionDetails> GetPlayerConnections(IGameDetails gameDetails)
        {
            var gameName = gameDetails.GetGameName();

            return _gameContext.PlayerConnections.Where(playerConnection => playerConnection.GameName == gameName);
        }
    }
}
