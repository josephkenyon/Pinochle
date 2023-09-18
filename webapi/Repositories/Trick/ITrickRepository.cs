using webapi.Domain.GameDetails;
using webapi.Domain.Trick;

namespace webapi.Repositories.Trick
{
    public interface ITrickRepository
    {
        public ITrick? GetTrick(IGameDetails gameDetails);
    }
}
