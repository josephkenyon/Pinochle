using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class Card : ICard
    {
        public int Id { get; set; }
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }

        public Card(int id, Suit suit, Rank rank)
        {
            Id = id;
            Suit = suit;
            Rank = rank;
        }
    }
}
