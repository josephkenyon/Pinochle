using webapi.Domain;
using webapi.Domain.PlayerDetails;

namespace webapi.Controllers.PlayerConnection
{
    public interface IPlayerStateController
    {
        HeroPlayerState GetPlayerState(IPlayerDetails playerDetails);

    }
}
