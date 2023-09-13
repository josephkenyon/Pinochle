﻿using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class TestCard : ICard
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }

        public TestCard()
        {
        }

        public TestCard(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Card ToCard()
        {
            return new Card(-1, Suit, Rank);
        }
    }
}
