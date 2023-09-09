namespace webapi.Domain
{
    public class PlayerState
    {
        public List<Card> Hand { get; set; }
        public List<Card> DisplayedCards { get; set; }
        public Card? PlayedCard { get; set; }
        public bool IsReady { get; set; }
        public bool ShowReady { get; set; }
        public int LastBid { get; set; }
        public bool ShowLastBid { get; set; }
        public bool ShowBiddingBox { get; set; }
        public bool ShowTrumpSelection { get; set; }
        public bool ShowSwapPlayerIndex { get; set; }

        public PlayerState() {
            Hand = new List<Card>();
            DisplayedCards = new List<Card>();
        }
    }
}
