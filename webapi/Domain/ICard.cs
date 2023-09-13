using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public interface ICard
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
    }
}
