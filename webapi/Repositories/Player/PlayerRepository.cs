using webapi.Data;
using webapi.Domain.GameDetails;
using webapi.Domain.Player;
using webapi.Domain.PlayerDetails;

namespace webapi.Repositories.Player
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

        public void CreatePlayer(IPlayerDetails playerDetails)
        {
            var gameName = playerDetails.GetGameName();
            var playerName = playerDetails.GetPlayerName();

            var playerCount = GetPlayers(playerDetails).Count();
            var player = new Domain.Player.Player(gameName, playerName)
            {
                PlayerIndex = playerCount
            };

            _gameContext.Players.Add(player);
            _gameContext.SaveChanges();
        }

        public IPlayer? GetPlayer(IPlayerDetails playerDetails)
        {
            var gameName = playerDetails.GetGameName();
            var playerName = playerDetails.GetPlayerName();

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

        public IEnumerable<IPlayer> GetPlayers(IGameDetails gameDetails)
        {
            var gameName = gameDetails.GetGameName();

            return _gameContext.Players.Where(player => player.GameName == gameName);
        }
    }
}
