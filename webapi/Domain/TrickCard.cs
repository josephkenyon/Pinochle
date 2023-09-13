using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class TrickCard
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public int Id { get; set; }
        public int Order { get; set; }

        public TrickCard(Suit suit, Rank rank, int id, int order) {
            Suit = suit;
            Rank = rank;
            Id = id;
            Order = order;
        }
    }
}
