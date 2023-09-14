﻿using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public static class Utils
    {
        public static int GetWinningCardId(Suit trumpSuit, List<TrickCard> cards)
        {
            var ledSuit = cards.OrderBy(card => card.PlayedIndex).First().Suit;

            var sortedCards = new List<TrickCard>(cards);

            sortedCards.Sort((a,b) => CompareCards(trumpSuit, ledSuit, a, b));

            return sortedCards.Last().Id;
        }

        public static List<Card> GetValidPlays(List<TrickCard> playedCards, List<Card> hand, Suit trumpSuit)
        {
            var ledSuit = playedCards.OrderBy(card => card.PlayedIndex).First().Suit;
            var hasSuit = hand.Any(card => card.Suit == ledSuit);
            var hasTrump = hand.Any(card => card.Suit == trumpSuit);

            var winningCardId = GetWinningCardId(trumpSuit, playedCards);
            var winningCard = playedCards.Single(card => card.Id == winningCardId);

            if (hasSuit)
            {
                var winningCards = hand.Where(card => CompareCards(trumpSuit, ledSuit, card.ToTrickCard(), winningCard) > 0).ToList();

                if (winningCards.Any())
                {
                    return winningCards;
                }
                else
                {
                    return hand.Where(card => card.Suit == ledSuit).ToList();
                }
            }
            else if (hasTrump)
            {
                var winningTrumpCards = hand.Where(card => card.Suit == trumpSuit && CompareCards(trumpSuit, ledSuit, card.ToTrickCard(), winningCard) > 0).ToList();

                if (winningTrumpCards.Any())
                {
                    return winningTrumpCards;
                }
                else
                {
                    return hand.Where(card => card.Suit == trumpSuit).ToList();
                }
            }

            return hand;
        }

        public static int CompareCards(Suit trumpSuit, Suit ledSuit, TrickCard card1, TrickCard card2)
        {
            var suitOneValue = GetSuitValue(trumpSuit, ledSuit, card1.Suit);
            var suitTwoValue = GetSuitValue(trumpSuit, ledSuit, card2.Suit);

            if (suitOneValue > suitTwoValue)
            {
                return 1;
            }
            else if (suitOneValue < suitTwoValue)
            {
                return -1;
            }

            if (card1.Suit != trumpSuit && card1.Suit != ledSuit && card2.Suit != trumpSuit && card2.Suit != ledSuit)
            {
                return 0;
            }
            else
            {
                if (card1.Rank < card2.Rank)
                {
                    return 1;
                }
                else if (card1.Rank > card2.Rank)
                {
                    return -1;
                }
            }

            return card1.PlayedIndex > card2.PlayedIndex ? -1 : 1;
        }

        public static int GetSuitValue(Suit trumpSuit, Suit ledSuit, Suit suit)
        {
            var trumpWasLed = trumpSuit == ledSuit;
            
            if (trumpWasLed)
            {
                return suit == trumpSuit ? 1 : 0;
            }
            else
            {
                if (suit == trumpSuit)
                {
                    return 2;
                }
                else if (suit == ledSuit)
                {
                    return 1;
                }
            }

            return 0;
        }

        public static Card GetCardFromId(int id)
        {
            var suit = (Suit)(id / 12);
            var rank = (Rank)((id - ((int) suit) * 12) / 2);

            return new Card(id, suit, rank);
        }
    }
}