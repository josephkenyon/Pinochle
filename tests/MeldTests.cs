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

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(7));
                Assert.That(result.MeldValue, Is.EqualTo(17));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(6));
                Assert.That(result.MeldValue, Is.EqualTo(16));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(2));
                Assert.That(result.MeldValue, Is.EqualTo(2));
            });
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

            Assert.That(result.MeldValue, Is.EqualTo(8));
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

            Assert.That(result.MeldValue, Is.EqualTo(16));
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

            Assert.That(result.MeldValue, Is.EqualTo(14));
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

            Assert.That(result.MeldValue, Is.EqualTo(3));
        }
    }
}