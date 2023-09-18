using webapi.Domain.PlayerConnection;

namespace webapi.Repository.PlayerConnection
{
    public interface IPlayerConnectionRepository
    {
        public void CreatePlayerConnection(string gameName, string playerName, string connectionId);
        public IPlayerConnection? GetPlayerConnection(string gameName, string playerName);
        public IEnumerable<IPlayerConnection> GetPlayerConnections(string gameName);
    }
}
