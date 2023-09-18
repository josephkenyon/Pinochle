using webapi.Domain.GameDetails;
using webapi.Domain.PlayerConnectionDetails;
using webapi.Domain.PlayerDetails;

namespace webapi.Repositories.PlayerConnection
{
    public interface IPlayerConnectionRepository
    {
        public void CreatePlayerConnection(IPlayerConnectionDetails playerConnection);
        public IPlayerConnectionDetails? GetPlayerConnection(IPlayerDetails playerDetails);
        public IEnumerable<IPlayerConnectionDetails> GetPlayerConnections(IGameDetails gameDetails);
    }
}
