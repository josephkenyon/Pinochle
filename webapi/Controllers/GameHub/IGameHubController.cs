using webapi.Domain.PlayerConnectionDetails;

namespace webapi.Controllers.GameHub
{
    public interface IGameHubController
    {
        Task<bool> JoinGame(IPlayerConnectionDetails playerConnectionDetails);
        void OnClientDisconnected(string connectionId);
        Task SwapPlayerPosition(string connectionId, string playerName);
        Task DeclareReady(string connectionId, bool ready);
        Task OnBid(string connectionId, int bid);
        Task DeclareTrump(string connectionId, int trumpSuitIndex);
        Task PlayCard(string connectionId, int sentCardId);
        Task CollectTrick(string connectionId, bool updateClients);
    }
}
