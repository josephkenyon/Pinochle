using webapi.Domain.Game;

namespace webapi.Repository.Game
{
    public interface IGameRepository
    {
        public void CreateGame(string gameName);
        public IGame? GetGame(string gameName);
    }
}
