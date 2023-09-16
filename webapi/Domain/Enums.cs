namespace webapi.Domain
{
    public static class Enums
    {
        public enum Suit
        {
            Spade,
            Heart,
            Club,
            Diamond
        }

        public enum Rank
        {
            Ace,
            Ten,
            King,
            Queen,
            Jack,
            Nine
        }

        public enum Phase
        {
            Initializing,
            Bidding,
            Declaring_Trump,
            Meld,
            Playing,
            RoundEnd,
            Game_Over
        }

        public enum MessageCode
        {
            TeamOne,
            TeamTwo,
            Error
        }
    }
}
