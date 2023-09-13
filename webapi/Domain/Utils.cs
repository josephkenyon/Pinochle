using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public static class Utils
    {
        public static int GetWinningCardId(Suit trumpSuit, Suit ledSuit, List<TrickCard> cards)
        {
            return cards[0].Id;
        }

        public static List<ICard> GetValidPlays(ICard ledCard, List<ICard> playedCards, List<ICard> hand, Suit trumpSuit)
        {


            return hand;
        }

        public static int CompareCards(Suit trumpSuit, Suit ledSuit, ICard card1, ICard card2)
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

            return 0;
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
    }
}