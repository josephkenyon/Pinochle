using webapi.Domain.PlayerConnectionDetails;

namespace webapi.Controllers.GameHub
{
    public interface IGameHubController
    {
        Task<bool> JoinGame(IPlayerConnectionDetails playerConnectionDetails);
        Task OnBid(string connectionId, int bid);
        Task DeclareTrump(string connectionId, int trumpSuitIndex);
        Task SwapPlayerPosition(string connectionId, string playerName);
        void OnClientDisconnected(string connectionId);
    }
}
