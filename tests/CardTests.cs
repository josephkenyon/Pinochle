using webapi.Domain;
using static webapi.Domain.Enums;

namespace tests
{
    public class CardTests
    {
        [Test]
        public void TrumpBeatsNonTrump()
        {
            var trumpSuit = Suit.Spade;
            var ledSuit = Suit.Heart;

            var card1 = new Card { Rank = Rank.Queen, Suit = Suit.Heart };
            var card2 = new Card { Rank = Rank.Ace, Suit = Suit.Spade };

            var winningCard = Utils.CompareCards(trumpSuit, ledSuit, card1, card2);

            Assert.That(winningCard, Is.EqualTo(-1));
        }

        [Test]
        public void NonLedSuitLosesToAll()
        {
            var trumpSuit = Suit.Spade;
            var ledSuit = Suit.Heart;

            var card1 = new Card { Rank = Rank.Ace, Suit = Suit.Club };
            var card2 = new Card { Rank = Rank.Nine, Suit = Suit.Heart };

            var winningCard = Utils.CompareCards(trumpSuit, ledSuit, card1, card2);

            Assert.That(winningCard, Is.EqualTo(-1));
        }

        [Test]
        public void NonLedSuitsAreEqual()
        {
            var trumpSuit = Suit.Spade;
            var ledSuit = Suit.Heart;

            var card1 = new Card { Rank = Rank.Ace, Suit = Suit.Club };
            var card2 = new Card { Rank = Rank.Nine, Suit = Suit.Diamond };

            var winningCard = Utils.CompareCards(trumpSuit, ledSuit, card1, card2);

            Assert.That(winningCard, Is.EqualTo(0));
        }

        [Test]
        public void SuitsAreEqualCheckRank()
        {
            var trumpSuit = Suit.Spade;
            var ledSuit = Suit.Heart;

            var card1 = new Card { Rank = Rank.Ace, Suit = Suit.Heart };
            var card2 = new Card { Rank = Rank.Nine, Suit = Suit.Heart };

            var winningCard = Utils.CompareCards(trumpSuit, ledSuit, card1, card2);

            Assert.That(winningCard, Is.EqualTo(1));
        }

        [Test]
        public void SuitsAreEqualCheckRankTrump()
        {
            var trumpSuit = Suit.Spade;
            var ledSuit = Suit.Heart;

            var card1 = new Card { Rank = Rank.Queen, Suit = Suit.Spade };
            var card2 = new Card { Rank = Rank.Jack, Suit = Suit.Spade };

            var winningCard = Utils.CompareCards(trumpSuit, ledSuit, card1, card2);

            Assert.That(winningCard, Is.EqualTo(1));
        }
    }
}