using static webapi.Domain.Enums;

namespace webapi.Domain.Player
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public string GameName { get; set; }
        public int PlayerIndex { get; set; }
        public bool Passed { get; set; }
        public bool Ready { get; set; }
        public int LastBid { get; set; }
        public string? HandString { get; set; }

        public Player(string gameName, string name)
        {
            GameName = gameName;
            Name = name;
        }

        public int GetIndex()
        {
            return PlayerIndex;
        }

        public int GetLastBid()
        {
            return LastBid;
        }

        public bool GetIsReady()
        {
            return Ready;
        }

        public string GetName()
        {
            return Name;
        }

        public List<Card> GetHand()
        {
            var cardsList = new List<Card>();

            if (HandString == null)
            {
                return cardsList;
            }

            var cardStrings = HandString.Split(";").ToList();
            cardStrings.RemoveAll(string.IsNullOrEmpty);

            cardsList.AddRange(cardStrings.Select(cardString =>
            {
                var cardStringList = cardString.Split(":");
                _ = Enum.TryParse(cardStringList[1], out Suit suit);
                _ = Enum.TryParse(cardStringList[2], out Rank rank);

                return new Card(int.Parse(cardStringList[0]), suit, rank);
            }));

            return cardsList;
        }

        public void SetHand(List<Card> cards)
        {
            var cardStringList = new List<string>();
            cardStringList.AddRange(cards.Select(card => $"{card.Id}:{card.Suit}:{card.Rank}"));

            HandString = string.Join(";", cardStringList);
        }

        public void RemoveCard(int id)
        {
            var cardsList = new List<Card>();

            if (HandString == null)
            {
                throw new InvalidOperationException();
            }

            var cardStringList = HandString.Split(";").ToList();

            var stringToDelete = cardStringList.Single(cardString => cardString.StartsWith($"{id}:"));

            cardStringList.Remove(stringToDelete);

            HandString = string.Join(";", cardStringList);
        }
    }
}
