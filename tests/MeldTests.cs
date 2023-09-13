using webapi.Domain;
using static webapi.Domain.Enums;

namespace tests
{
    public class MeldTests
    {
        [Test]
        public void TestSingleRunWithMarriages()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Jack, Suit = Suit.Heart },
                new Card { Rank = Rank.Queen, Suit = Suit.Heart },
                new Card { Rank = Rank.Queen, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Heart },
                new Card { Rank = Rank.Ten, Suit = Suit.Heart },
                new Card { Rank = Rank.Ace, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Spade },
                new Card { Rank = Rank.Queen, Suit = Suit.Spade },
            };

            var result = new MeldResult(cards, Suit.Heart);

            Assert.True(result.MeldCards.Count == 7);
            Assert.True(result.MeldValue == 17);
        }

        [Test]
        public void TestSingleRun()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Nine, Suit = Suit.Heart },
                new Card { Rank = Rank.Jack, Suit = Suit.Heart },
                new Card { Rank = Rank.Queen, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Heart },
                new Card { Rank = Rank.Ten, Suit = Suit.Heart },
                new Card { Rank = Rank.Ace, Suit = Suit.Heart },
            };

            var result = new MeldResult(cards, Suit.Heart);

            Assert.True(result.MeldCards.Count == 6);
            Assert.True(result.MeldValue == 16);
        }

        [Test]
        public void TestNinesAround()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Nine, Suit = Suit.Club },
                new Card { Rank = Rank.Nine, Suit = Suit.Club },
                new Card { Rank = Rank.Nine, Suit = Suit.Diamond },
                new Card { Rank = Rank.Nine, Suit = Suit.Diamond },
                new Card { Rank = Rank.Nine, Suit = Suit.Heart },
                new Card { Rank = Rank.Nine, Suit = Suit.Heart },
                new Card { Rank = Rank.Nine, Suit = Suit.Spade },
                new Card { Rank = Rank.Nine, Suit = Suit.Spade },
            };

            var result = new MeldResult(cards, Suit.Spade);

            Assert.True(result.MeldCards.Count == 2);
            Assert.True(result.MeldValue == 2);
        }

        [Test]
        public void TestJacksAround()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Jack, Suit = Suit.Club },
                new Card { Rank = Rank.Jack, Suit = Suit.Diamond },
                new Card { Rank = Rank.Jack, Suit = Suit.Heart },
                new Card { Rank = Rank.Jack, Suit = Suit.Spade },
                new Card { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(cards, Suit.Spade);

            Assert.True(result.MeldValue == 8);
        }

        [Test]
        public void TestQueensAround()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Jack, Suit = Suit.Diamond },
                new Card { Rank = Rank.Queen, Suit = Suit.Club },
                new Card { Rank = Rank.Queen, Suit = Suit.Diamond },
                new Card { Rank = Rank.King, Suit = Suit.Heart },
                new Card { Rank = Rank.Queen, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Spade },
                new Card { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(cards, Suit.Spade);

            Assert.True(result.MeldValue == 16);
        }

        [Test]
        public void TestKingsAround()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.King, Suit = Suit.Club },
                new Card { Rank = Rank.King, Suit = Suit.Diamond },
                new Card { Rank = Rank.King, Suit = Suit.Heart },
                new Card { Rank = Rank.Queen, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Spade },
                new Card { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(cards, Suit.Spade);

            Assert.True(result.MeldValue == 14);
        }

        [Test]
        public void Test()
        {
            var cards = new List<Card>
            {
                new Card { Rank = Rank.Ten, Suit = Suit.Spade },
                new Card { Rank = Rank.Ten, Suit = Suit.Spade },
                new Card { Rank = Rank.King, Suit = Suit.Spade },
                new Card { Rank = Rank.Jack, Suit = Suit.Spade },
                new Card { Rank = Rank.Jack, Suit = Suit.Heart },
                new Card { Rank = Rank.Nine, Suit = Suit.Heart },
                new Card { Rank = Rank.King, Suit = Suit.Club },
                new Card { Rank = Rank.Ten, Suit = Suit.Diamond },
                new Card { Rank = Rank.King, Suit = Suit.Diamond },
                new Card { Rank = Rank.Queen, Suit = Suit.Diamond },
                new Card { Rank = Rank.Nine, Suit = Suit.Diamond },
            };

            var result = new MeldResult(cards, Suit.Heart);

            Assert.True(result.MeldValue == 3);
        }
    }
}