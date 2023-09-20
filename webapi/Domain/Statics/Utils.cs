using webapi.Domain.Game;
using webapi.Domain.Player;
using webapi.Domain.Tricks;
using static webapi.Domain.Statics.Enums;

namespace webapi.Domain.Statics
{
    public static class Utils
    {
        public static int GetWinningCardId(Suit trumpSuit, List<TrickCard> cards)
        {
            var ledSuit = cards.OrderBy(card => card.PlayedIndex).First().Suit;

            var sortedCards = new List<TrickCard>(cards);

            sortedCards.Sort((a, b) => CompareCards(trumpSuit, ledSuit, a, b));

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
                var winningCards = hand.Where(card => card.Suit == ledSuit && CompareCards(trumpSuit, ledSuit, card.ToTrickCard(), winningCard) > 0).ToList();

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

        public static void StartNewRound(IGame game, IEnumerable<IPlayer> players)
        {
            DealCards(players);

            game.StartNewRound();

            var player = players.Single(player => player.GetIndex() == game.GetPlayerTurnIndex());

            foreach (var pl in players)
            {
                pl.ResetBiddingState();
            }
        }

        public static void DealCards(IEnumerable<IPlayer> players)
        {
            var deck = new List<Card>();

            var index = 0;
            var rng = new Random();
            var shuffleCount = rng.Next(20, 30);

            Enum.GetValues<Suit>().ToList().ForEach(suit =>
            {
                Enum.GetValues<Rank>().ToList().ForEach(rank =>
                {
                    deck.Add(new Card(index++, suit, rank));
                    deck.Add(new Card(index++, suit, rank));
                });
            });

            for (int i = 0; i < shuffleCount; i++)
            {
                deck = deck.OrderBy(card => rng.Next()).ToList();
            }

            index = 0;
            var startingIndex = 0;
            foreach (var player in players)
            {
                var cards = new List<Card>();
                for (int i = startingIndex; i < startingIndex + 12; i++)
                {
                    cards.Add(deck[i]);
                }

                index++;
                if (index > 3)
                {
                    index = 0;
                }

                cards.Sort((a, b) =>
                {
                    if ((int)a.Suit > (int)b.Suit)
                    {
                        return 1;
                    }
                    else if ((int)a.Suit < (int)b.Suit)
                    {
                        return -1;
                    }

                    if ((int)a.Rank > (int)b.Rank)
                    {
                        return 1;
                    }
                    else if ((int)a.Rank < (int)b.Rank)
                    {
                        return -1;
                    }

                    return 0;
                });

                player.DealCards(cards);

                startingIndex += 12;
            }
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

        public static TrickState GetTrickState(ITrick trick, int playerIndex)
        {
            var trickState = new TrickState();

            if (trick == null)
            {
                return trickState;
            }

            var trickPlays = trick.GetTrickPlays();

            foreach (var trickPlay in trickPlays)
            {
                if (trickPlay.PlayerIndex == playerIndex)
                {
                    trickState.MyCard = trickPlay.Card;
                }
                else if (trickPlay.PlayerIndex == DecrementIndex(playerIndex, 1))
                {
                    trickState.RightOpponentCard = trickPlay.Card;
                }
                else if (trickPlay.PlayerIndex == DecrementIndex(playerIndex, 2))
                {
                    trickState.AllyCard = trickPlay.Card;
                }
                else if (trickPlay.PlayerIndex == DecrementIndex(playerIndex, 3))
                {
                    trickState.LeftOpponentCard = trickPlay.Card;
                }
            }

            return trickState;
        }

        public static int DecrementIndex(int index, int amount = 1)
        {
            var newIndex = index;
            newIndex -= amount;
            if (newIndex < 0)
            {
                newIndex += 4;
            }

            return newIndex;
        }

        public static int IncrementIndex(int index, int amount = 1)
        {
            var newIndex = index;
            newIndex += amount;
            if (newIndex > 3)
            {
                newIndex -= 4;
            }

            return newIndex;
        }

        public static Card GetCardFromId(int id)
        {
            var suit = (Suit)(id / 12);
            var rank = (Rank)((id - (int)suit * 12) / 2);

            return new Card(id, suit, rank);
        }
    }
}