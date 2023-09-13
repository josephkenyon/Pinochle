using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class RoundBidResult
    {
        public Suit TrumpSuit { get; set; }
        public int TeamIndex { get; set; }
        public int Bid { get; set; }

        public RoundBidResult(Suit trumpSuit, int teamIndex, int bid)
        {
            TrumpSuit = trumpSuit;
            TeamIndex = teamIndex;
            Bid = bid;
        }
    }
}
