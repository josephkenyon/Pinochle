using webapi.Domain.GameDetails;
using webapi.Domain.MessageDetails;
using webapi.Domain.PlayerConnectionDetails;

namespace webapi.Controllers.PlayerConnection
{
    public interface IPlayerConnectionController
    {
        Task<bool> JoinGame(IPlayerConnectionDetails playerConnectionDetails);
        Task UpdateClients(IGameDetails gameDetails);
        Task MessageClients(IMessageDetails messageDetails);
        Task MessageClient(IMessageDetails messageDetails, string playerName);
    }
}
