using webapi.Domain;
using static webapi.Domain.Enums;

namespace tests
{
    public class TrickTests
    {
        [Test]
        public void MustPlayInSuit()
        {
            var trumpSuit = Suit.Spade;

            var ledCard = new TestCard { Rank = Rank.Queen, Suit = Suit.Heart };

            var playedCards = new List<ICard>
            {
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
            };

            var hand = new List<ICard>
            {
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Diamond },
            };

            var validPlays = Utils.GetValidPlays(ledCard, playedCards, hand, trumpSuit);

            Assert.Multiple(() =>
            {
                Assert.That(validPlays, Has.Count.EqualTo(1));
                Assert.That(validPlays[0].Suit, Is.EqualTo(Suit.Heart));
            });
        }

        [Test]
        public void CanPlayAnythingIfVoid()
        {
            var trumpSuit = Suit.Spade;

            var ledCard = new TestCard { Rank = Rank.Queen, Suit = Suit.Heart };

            var playedCards = new List<ICard>
            {
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
            };

            var hand = new List<ICard>
            {
                new TestCard { Rank = Rank.Ace, Suit = Suit.Club },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Club },
            };

            var validPlays = Utils.GetValidPlays(ledCard, playedCards, hand, trumpSuit);

            Assert.Multiple(() =>
            {
                Assert.That(validPlays, Has.Count.EqualTo(2));
            });
        }

        [Test]
        public void MustTrumpIfVoid()
        {
            var trumpSuit = Suit.Spade;

            var ledCard = new TestCard { Rank = Rank.Queen, Suit = Suit.Heart };

            var playedCards = new List<ICard>
            {
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
            };

            var hand = new List<ICard>
            {
                new TestCard { Rank = Rank.Jack, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Club },
            };

            var validPlays = Utils.GetValidPlays(ledCard, playedCards, hand, trumpSuit);

            Assert.Multiple(() =>
            {
                Assert.That(validPlays, Has.Count.EqualTo(1));
                Assert.That(validPlays[0].Rank, Is.EqualTo(Rank.Jack));
            });
        }

        [Test]
        public void MustWinIfYouCan()
        {
            var trumpSuit = Suit.Spade;

            var ledCard = new TestCard { Rank = Rank.Queen, Suit = Suit.Heart };

            var playedCards = new List<ICard>
            {
                new TestCard { Rank = Rank.Queen, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
            };


            var hand = new List<ICard>
            {
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
                new TestCard { Rank = Rank.Jack, Suit = Suit.Heart },
            };

            var validPlays = Utils.GetValidPlays(ledCard, playedCards, hand, trumpSuit);

            Assert.Multiple(() => 
            {
                Assert.That(validPlays, Has.Count.EqualTo(1));
                Assert.That(validPlays[0].Rank, Is.EqualTo(Rank.Ace));
            });

        }

        [Test]
        public void MustWinIfYouCanTrump()
        {
            var trumpSuit = Suit.Spade;

            var ledCard = new TestCard { Rank = Rank.Ace, Suit = Suit.Heart };

            var playedCards = new List<ICard>
            {
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
                new TestCard { Rank = Rank.King, Suit = Suit.Heart },
            };


            var hand = new List<ICard>
            {
                new TestCard { Rank = Rank.Ace, Suit = Suit.Spade },
                new TestCard { Rank = Rank.Ace, Suit = Suit.Heart },
            };

            var validPlays = Utils.GetValidPlays(ledCard, playedCards, hand, trumpSuit);

            Assert.Multiple(() =>
            {
                Assert.That(validPlays, Has.Count.EqualTo(1));
                Assert.That(validPlays[0].Suit, Is.EqualTo(Suit.Spade));
            });
        }
    }
}