﻿using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class Trick
    {
        public string GameName { get; set; }
        public Suit TrumpSuit { get; set; }
        public Suit LedSuit { get; set; }
        public string CardsString { get; set; }

        public Trick()
        {
            CardsString = "";
            GameName = "Unknown";
        }

        public void PlayCard(Card card, int playerId)
        {
            var cardStrings = CardsString.Split(";").ToList();
            cardStrings.RemoveAll(string.IsNullOrEmpty);

            cardStrings.Add($"{playerId}:{card.Id}:{card.Suit}:{card.Rank}");

            CardsString = string.Join(";", cardStrings);
        }

        public List<Card> GetCards()
        {
            var cardsList = new List<Card>();

            var cardStrings = CardsString.Split(";").ToList();
            cardStrings.RemoveAll(string.IsNullOrEmpty);

            cardsList.AddRange(cardStrings.Select(cardString =>
            {
                var cardStringList = cardString.Split(":");
                _ = Enum.TryParse(cardStringList[2], out Suit suit);
                _ = Enum.TryParse(cardStringList[3], out Rank rank);

                return new Card(int.Parse(cardStringList[1]), suit, rank);
            }));

            return cardsList;
        }

        public List<TrickPlay> GetTrickPlays()
        {
            var trickPlayList = new List<TrickPlay>();

            var cardStrings = CardsString.Split(";").ToList();
            cardStrings.RemoveAll(string.IsNullOrEmpty);

            trickPlayList.AddRange(cardStrings.Select(cardString =>
            {
                var cardStringList = cardString.Split(":");
                _ = Enum.TryParse(cardStringList[2], out Suit suit);
                _ = Enum.TryParse(cardStringList[3], out Rank rank);

                return new TrickPlay(new Card(int.Parse(cardStringList[1]), suit, rank), int.Parse(cardStringList[0]));
            }));

            return trickPlayList;
        }
    }
}
