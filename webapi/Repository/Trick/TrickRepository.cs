using webapi.Data;
using webapi.Domain.Trick;

namespace webapi.Repository.Trick
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

        public ITrick? GetTrick(string gameName)
        {
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
