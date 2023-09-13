using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class CardState
    {
        public int Id { get; set; }
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }

        public CardState(Suit suit, Rank rank, int id)
        {
            Suit = suit;
            Rank = rank;
            Id = id;
        }
    }
}
