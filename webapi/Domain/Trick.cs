using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class Trick
    {
        public string GameName { get; set; }
        public Suit TrumpSuit { get; set; }
        public Suit LedSuit { get; set; }
        public int? LedCardId { get; set; }
        public int? SecondCardId { get; set; }
        public int? ThirdCardId { get; set; }
        public int? FourthCardId { get; set; }
        public int? CardIdZero { get; set; }
        public int? CardIdOne { get; set; }
        public int? CardIdTwo { get; set; }
        public int? CardIdThree { get; set; }

        public Trick()
        {
            GameName = "Unknown";
        }

        public void SetCard(int index, int cardId)
        {
            if (index == 0)
            {
                CardIdZero = cardId;
            }
            else if (index == 1)
            {
                CardIdOne = cardId;
            }
            else if (index == 2)
            {
                CardIdTwo = cardId;
            }
            else
            {
                CardIdThree = cardId;
            }

            if (LedCardId == null)
            {
                LedCardId = cardId;
            }
            else if (SecondCardId == null)
            {
                SecondCardId = cardId;
            }
            else if (ThirdCardId == null)
            {
                ThirdCardId = cardId;
            }
            else if (FourthCardId == null)
            {
                FourthCardId = cardId;
            }
        }

        public int? GetCardId(int index)
        {
            if (index == 0)
            {
                return CardIdZero;
            }
            else if (index == 1)
            {
                return CardIdOne;
            }
            else if (index == 2)
            {
                return CardIdTwo;
            }
            else
            {
                return CardIdThree;
            }
        }

        public bool IsFull()
        {
            return CardIdZero != null && CardIdOne != null && CardIdTwo != null && CardIdThree != null;
        }

        public List<int> GetIds()
        {
            var list = new List<int>
            {
                (int)CardIdZero!,
                (int)CardIdOne!,
                (int)CardIdTwo!,
                (int)CardIdThree!
            };
            return list;
        }
    }
}
