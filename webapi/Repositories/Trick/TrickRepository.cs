using webapi.Data;
using webapi.Domain.GameDetails;
using webapi.Domain.Trick;

namespace webapi.Repositories.Trick
{
    public class TrickRepository : ITrickRepository
    {
        private readonly ILogger<TrickRepository> _logger;
        private readonly GameContext _gameContext;

        public TrickRepository(ILogger<TrickRepository> logger, GameContext gameContext)
        {
            _logger = logger;
            _gameContext = gameContext;
        }

        public ITrick? GetTrick(IGameDetails gameDetails)
        {
            var gameName = gameDetails.GetGameName();

            try
            {
                return _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == gameName);
            }
            catch (Exception)
            {
                _logger.LogError("Error retrieving a trick with '{GameName}'", gameName);
                return null;
            }
        }
    }
}
