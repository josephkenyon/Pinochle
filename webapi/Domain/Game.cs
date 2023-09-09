using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class Game
    {
        public string Name { get; set; }
        public Phase Phase { get; set; }
        public Suit TrumpSuit { get; set; }
        public int PlayerTurnIndex { get; set; }
        public int StartingPlayerTurnIndex { private get; set; }
        public int CurrentBid { get; set; }
        public int TeamTookBidIndex { get; set; }
        public int TeamOneScore { get; set; }
        public int TeamTwoScore { get; set; }
        public List<Player> Players { get; set; }
        public List<MeldResult> MeldResults { get; set; }
        public List<Card> Cards { get; set; }
        public List<int> Scores { get; set; }

        public Game() {
            Name = "Unknown";
            Phase = Phase.Initializing;
            Players = new List<Player>();
            MeldResults = new List<MeldResult>();
            Cards = new List<Card>();
            Scores = new List<int>();
            StartingPlayerTurnIndex = -1;
        }

        public Game(string gameName, string hostingPlayerName) {
            Name = gameName;
            Phase = Phase.Initializing;
            MeldResults = new List<MeldResult>();
            Players = new List<Player>
            {
                new Player(hostingPlayerName, gameName)
            };
            Scores = new List<int>();
            Cards = new List<Card>();
            var index = 0;
            Enum.GetValues<Suit>().ToList().ForEach(suit =>
            {
                Enum.GetValues<Rank>().ToList().ForEach(rank =>
                {
                    Cards.Add(new Card(gameName, index++, suit, rank));
                    Cards.Add(new Card(gameName, index++, suit, rank));
                });
            });
            StartingPlayerTurnIndex = -1;
        }

        public int IncrementAndGetStartingPlayerTurnIndex()
        {
            StartingPlayerTurnIndex++;

            if (StartingPlayerTurnIndex > 3)
            {
                StartingPlayerTurnIndex = 0;
            }

            return StartingPlayerTurnIndex;
        }
    }
}
