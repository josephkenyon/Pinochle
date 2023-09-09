using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class Card
    {
        public string GameName { get; set; }
        public string PlayerName { get; set; }
        public int Id { get; set; }
        public bool BeingPlayed { get; set; }
        public int WonTeamId { get; set; }
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public Game? Game { get; set; }

        public Card() {
            WonTeamId = -1;
            GameName = "Unknown";
            PlayerName = "Unknown";
        }

        public Card(string gameName, int id, Suit suit, Rank rank)
        {
            WonTeamId = -1;
            GameName = gameName;
            PlayerName = "Unknown";
            Id = id;
            Suit = suit;
            Rank = rank;
        }
    }
}
