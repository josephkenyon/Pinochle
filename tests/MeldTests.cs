using webapi.Domain;
using webapi.Domain.Meld;
using static webapi.Domain.Statics.Enums;

namespace tests
{
    public class MeldTests
    {
        [Test]
        public void TestSingleRunWithMarriages()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Jack, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Ten, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Spade },
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Heart);

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(7));
                Assert.That(result.MeldValue, Is.EqualTo(17));
            });
        }

        [Test]
        public void TestSingleRun()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Nine, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Ten, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Heart);

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(6));
                Assert.That(result.MeldValue, Is.EqualTo(16));
            });
        }

        [Test]
        public void TestNinesAround()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Nine, Suit = Suit.Club },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Club },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Spade },
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Spade);

            Assert.Multiple(() =>
            {
                Assert.That(result.MeldCards, Has.Count.EqualTo(2));
                Assert.That(result.MeldValue, Is.EqualTo(2));
            });
        }

        [Test]
        public void TestJacksAround()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Jack, Suit = Suit.Club },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Spade);

            Assert.That(result.MeldValue, Is.EqualTo(8));
        }

        [Test]
        public void TestQueensAround()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Jack, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Club },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Spade);

            Assert.That(result.MeldValue, Is.EqualTo(16));
        }

        [Test]
        public void TestKingsAround()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.King, Suit = Suit.Club },
                new TestCard { Rank = Rank.King, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Spade }
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Spade);

            Assert.That(result.MeldValue, Is.EqualTo(14));
        }

        [Test]
        public void Test()
        {
            var testCards = new List<TestCard>
            {
                new TestCard { Rank = Rank.Ten, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Ten, Suit = Suit.Spade },
                new TestCard { Rank = Rank.King, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Club },
                new TestCard { Rank = Rank.Ten, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.King, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Queen, Suit = Suit.Diamond },
                new TestCard { Rank = Rank.Nine, Suit = Suit.Diamond },
            };

            var result = new MeldResult(testCards.Select(testCard => testCard.ToCard()).ToList(), Suit.Heart);

            Assert.That(result.MeldValue, Is.EqualTo(3));
        }
    }
}