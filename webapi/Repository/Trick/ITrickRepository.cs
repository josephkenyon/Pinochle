using webapi.Domain.Trick;

namespace webapi.Repository.Trick
{
    public interface ITrickRepository
    {
        public ITrick? GetTrick(string gameName);
    }
}
