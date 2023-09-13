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
        public Trick? CurrentTrick { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> Cards { get; set; }
        public string TeamOneScoresString { get; set; }
        public string TeamTwoScoresString { get; set; }
        public string RoundBidResults { get; set; }

        public Game() {
            Name = "Unknown";
            Phase = Phase.Initializing;
            Players = new List<Player>();
            Cards = new List<Card>();
            TeamOneScoresString = "";
            TeamTwoScoresString = "";
            RoundBidResults = "";
            StartingPlayerTurnIndex = -1;
        }

        public Game(string gameName, string hostingPlayerName) {
            Name = gameName;
            Phase = Phase.Initializing;
            Players = new List<Player>
            {
                new Player(hostingPlayerName, gameName)
            };
            TeamOneScoresString = "";
            TeamTwoScoresString = "";
            RoundBidResults = "";
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

        public void AddScore(int teamIndex, int scoreIncrementValue)
        {
            var scoreArray = (teamIndex == 0 ? TeamOneScoresString : TeamTwoScoresString).Split(";").ToList();
            scoreArray.RemoveAll(string.IsNullOrEmpty);
            scoreArray.Add(scoreIncrementValue.ToString());

            if (teamIndex == 0)
            {
                TeamOneScoresString = string.Join(";", scoreArray);
            }
            else
            {
                TeamTwoScoresString = string.Join(";", scoreArray);
            }
        }

        public List<string> GetScoreLog(string teamName, int teamIndex)
        {

            var newList = new List<string>
            {
                teamName
            };

            newList.AddRange(teamIndex == 0 ? TeamOneScoresString.Split(";") : TeamTwoScoresString.Split(";"));

            return newList;
        }

        public void AddRoundBidResult(Suit trumpSuit, int teamIndex, int bid)
        {
            var resultsArray = RoundBidResults.Split(";").ToList();
            resultsArray.RemoveAll(string.IsNullOrEmpty);

            var result = $"{trumpSuit}:{teamIndex}:{bid}";

            resultsArray.Add(result);

            RoundBidResults = string.Join(";", resultsArray);
        }

        public List<RoundBidResult> GetRoundBidResults()
        {
            var resultsArray = RoundBidResults.Split(";").ToList();
            resultsArray.RemoveAll(string.IsNullOrEmpty);

            var newList = new List<RoundBidResult>();

            foreach ( var result in resultsArray )
            {
                var resultArray = RoundBidResults.Split(":").ToList();
                _ = Enum.TryParse(resultArray[0], out Suit suit);

                newList.Add(new RoundBidResult(suit, int.Parse(resultArray[1]), int.Parse(resultArray[2])));
            }

            return newList;
        }
    }
}
