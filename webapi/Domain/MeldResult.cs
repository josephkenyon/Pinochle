using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class MeldResult
    {
        public Game? Game { get; set; }
        public string GameName { get; set; }
        public int PlayerIndex { get; set; }
        public int MeldValue { get; set; }
        public List<Card> MeldCards { get; set; }

        public MeldResult() {
            GameName = "Unknown";
            MeldCards = new List<Card>();
        }

        public MeldResult(string gameName, List<Card> cards, Suit trumpSuit) {
            GameName = gameName;
            MeldValue = 0;
            MeldCards = new List<Card>();

            var numberOfRuns = AddRuns(cards.Where(card => card.Suit == trumpSuit && card.Rank != Rank.Nine).ToList());

            var nines = cards.Where(card => card.Rank == Rank.Nine && card.Suit == trumpSuit).ToList();
            AddNinesOfTrump(nines);

            var jacks = cards.Where(card => card.Rank == Rank.Jack).ToList();
            AddJacksAround(jacks);

            var queens = cards.Where(card => card.Rank == Rank.Queen).ToList();
            AddQueensAround(queens);

            var kings = cards.Where(card => card.Rank == Rank.King).ToList();
            AddKingsAround(kings);

            var kingsAndQueens = cards.Where(card => card.Rank == Rank.King || card.Rank == Rank.Queen).ToList();
            AddMarriages(trumpSuit, kingsAndQueens, numberOfRuns);

            var jacksOfDiamonds = cards.Where(card => card.Rank == Rank.Jack && card.Suit == Suit.Diamond).ToList();
            var queensOfSpades = cards.Where(card => card.Rank == Rank.Queen && card.Suit == Suit.Spade).ToList();
            AddPinochles(jacksOfDiamonds, queensOfSpades);
        }

        private void AddNinesOfTrump(List<Card> ninesOfTrump)
        {
            foreach (var nine in ninesOfTrump)
            {
                MeldCards.Add(nine);
                MeldValue += 1;
            }
        }

        private int AddRuns(List<Card> trumpCards)
        {
            var numberOfRuns = 0;
            if (trumpCards.Where(card => card.Rank == Rank.Ace).Count() == 2 && trumpCards.Where(card => card.Rank == Rank.Ten).Count() == 2
                && trumpCards.Where(card => card.Rank == Rank.King).Count() == 2 && trumpCards.Where(card => card.Rank == Rank.Queen).Count() == 2
                && trumpCards.Where(card => card.Rank == Rank.Jack).Count() == 2)
            {
                MeldCards.AddRange(trumpCards);

                MeldValue += 150;
                numberOfRuns = 2;
            }
            else if (trumpCards.Any(card => card.Rank == Rank.Ace) && trumpCards.Any(card => card.Rank == Rank.Ten)
                && trumpCards.Any(card => card.Rank == Rank.King) && trumpCards.Any(card => card.Rank == Rank.Queen)
                && trumpCards.Any(card => card.Rank == Rank.Jack))
            {
                MeldCards.Add(trumpCards.First(card => card.Rank == Rank.Ace));
                MeldCards.Add(trumpCards.First(card => card.Rank == Rank.Ten));
                MeldCards.Add(trumpCards.First(card => card.Rank == Rank.King));
                MeldCards.Add(trumpCards.First(card => card.Rank == Rank.Queen));
                MeldCards.Add(trumpCards.First(card => card.Rank == Rank.Jack));

                MeldValue += 15;
                numberOfRuns = 1;
            }

            return numberOfRuns;
        }

        private void AddJacksAround(List<Card> jacks)
        {
            if (jacks.Count == 8)
            {
                foreach (var jack in jacks.Where(jack => !MeldCards.Contains(jack)))
                {
                    MeldCards.Add(jack);
                }

                MeldValue += 40;
            }
            else if (jacks.Any(jack => jack.Suit == Suit.Club) && jacks.Any(jack => jack.Suit == Suit.Diamond)
                && jacks.Any(jack => jack.Suit == Suit.Heart) && jacks.Any(jack => jack.Suit == Suit.Spade))
            {
                Enum.GetValues<Suit>().ToList().ForEach(suit =>
                {
                    if (!MeldCards.Any(card => card.Suit == suit && card.Rank == Rank.Jack))
                    {
                        MeldCards.Add(jacks.First(jack => jack.Suit == suit));
                    }
                });

                MeldValue += 4;
            }
        }

        private void AddQueensAround(List<Card> queens)
        {
            if (queens.Count == 8)
            {
                foreach (var queen in queens.Where(queen => !MeldCards.Contains(queen)))
                {
                    MeldCards.Add(queen);
                }

                MeldValue += 60;
            }
            else if (queens.Any(queen => queen.Suit == Suit.Club) && queens.Any(queen => queen.Suit == Suit.Diamond)
                && queens.Any(queen => queen.Suit == Suit.Heart) && queens.Any(queen => queen.Suit == Suit.Spade))
            {
                Enum.GetValues<Suit>().ToList().ForEach(suit =>
                {
                    if (!MeldCards.Any(card => card.Suit == suit && card.Rank == Rank.Queen))
                    {
                        MeldCards.Add(queens.First(queen => queen.Suit == suit));
                    }
                });

                MeldValue += 6;
            }
        }

        private void AddKingsAround(List<Card> kings)
        {
            if (kings.Count == 8)
            {
                foreach (var king in kings.Where(king => !MeldCards.Contains(king)))
                {
                    MeldCards.Add(king);
                }

                MeldValue += 60;
            }
            else if (kings.Any(king => king.Suit == Suit.Club) && kings.Any(king => king.Suit == Suit.Diamond)
                && kings.Any(king => king.Suit == Suit.Heart) && kings.Any(king => king.Suit == Suit.Spade))
            {
                Enum.GetValues<Suit>().ToList().ForEach(suit =>
                {
                    if (!MeldCards.Any(card => card.Suit == suit && card.Rank == Rank.King))
                    {
                        MeldCards.Add(kings.First(king => king.Suit == suit));
                    }
                });

                MeldValue += 8;
            }
        }

        private void AddMarriages(Suit trumpSuit, List<Card> kingsAndQueens, int numberOfRuns)
        {
            var numMarriagesPerSuit = new Dictionary<Suit, int>();
            var numSuitsWithMarriage = 0;
            var numSuitsWithDoubleMarriage = 0;

            Enum.GetValues<Suit>().ToList().ForEach(suit =>
            {
                var kingsAndQueensOfSuit = kingsAndQueens.Where(card => card.Suit == suit);
                var kingsOfSuit = kingsAndQueens.Where(card => card.Rank == Rank.King && card.Suit == suit).ToList();
                var queensOfSuit = kingsAndQueens.Where(card => card.Rank == Rank.Queen && card.Suit == suit).ToList();

                if (kingsOfSuit.Count == 2 && queensOfSuit.Count == 2)
                {
                    foreach (var king in kingsOfSuit.Where(king => !MeldCards.Contains(king)))
                    {
                        MeldCards.Add(king);
                    }

                    foreach (var queen in queensOfSuit.Where(queen => !MeldCards.Contains(queen)))
                    {
                        MeldCards.Add(queen);
                    }

                    numSuitsWithDoubleMarriage += 1;
                    numMarriagesPerSuit.Add(suit, 2);
                }
                else if (kingsOfSuit.Count > 0 && queensOfSuit.Count > 0)
                {
                    if (!MeldCards.Any(card => card.Suit == suit && card.Rank == Rank.King))
                    {
                        MeldCards.Add(kingsOfSuit.First(king => king.Suit == suit));
                    }

                    if (!MeldCards.Any(card => card.Suit == suit && card.Rank == Rank.Queen))
                    {
                        MeldCards.Add(queensOfSuit.First(queen => queen.Suit == suit));
                    }

                    numSuitsWithMarriage += 1;
                    numMarriagesPerSuit.Add(suit, 1);
                }
            });

            if (numSuitsWithDoubleMarriage == 4)
            {
                MeldValue += 250;
            }
            else if (numSuitsWithMarriage == 4)
            {
                MeldValue += 25;

                Enum.GetValues<Suit>().ToList().ForEach(suit =>
                {
                    var unaccountedForMarriages = numMarriagesPerSuit[suit] - 1;
                    if (suit == trumpSuit)
                    {
                        unaccountedForMarriages -= numberOfRuns;
                    }

                    if (unaccountedForMarriages > 0)
                    {
                        MeldValue += suit == trumpSuit ? 4 : 2;
                    }
                });
            }
            else
            {
                foreach (var marriageCount in numMarriagesPerSuit)
                {
                    var suit = marriageCount.Key;
                    var count = marriageCount.Value;

                    if (suit == trumpSuit)
                    {
                        count -= numberOfRuns;
                    }

                    if (count > 0)
                    {
                        MeldValue += ((suit == trumpSuit ? 4 : 2) * count);
                    }
                }
            }
        }

        private void AddPinochles(List<Card> jacksOfDiamonds, List<Card> queensOfSpades)
        {
            if (jacksOfDiamonds.Count == 2 && queensOfSpades.Count == 2)
            {
                foreach (var jackOfDiamonds in jacksOfDiamonds)
                {
                    if (MeldCards.Where(card => card.Rank == Rank.Jack && card.Suit == Suit.Diamond).Count() < 2)
                    {
                        MeldCards.Add(jackOfDiamonds);
                    }
                    else
                    {
                        break;
                    }
                }

                foreach (var queenOfSpades in queensOfSpades)
                {
                    if (MeldCards.Where(card => card.Rank == Rank.Queen && card.Suit == Suit.Spade).Count() < 2)
                    {
                        MeldCards.Add(queenOfSpades);
                    }
                    else
                    {
                        break;
                    }
                }

                MeldValue += 30;
            }
            else if (jacksOfDiamonds.Count > 0 && queensOfSpades.Count > 0)
            {
                if (!MeldCards.Any(card => card.Rank == Rank.Jack && card.Suit == Suit.Diamond))
                {
                    MeldCards.Add(jacksOfDiamonds.First());
                }

                if (!MeldCards.Any(card => card.Rank == Rank.Queen && card.Suit == Suit.Spade))
                {
                    MeldCards.Add(queensOfSpades.First());
                }

                MeldValue += 4;
            }
        }
    }
}
