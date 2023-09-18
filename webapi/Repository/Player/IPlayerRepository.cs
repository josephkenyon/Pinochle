using webapi.Domain.Player;

namespace webapi.Repository.Player
{
    public interface IPlayerRepository
    {
        public void CreatePlayer(string gameName, string playerName);
        public IEnumerable<IPlayer> GetPlayers(string gameName);
        public IPlayer? GetPlayer(string gameName, string playerName);
    }
}
