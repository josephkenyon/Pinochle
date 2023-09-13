namespace webapi.Domain
{
    public class PlayerState
    {
        public string Name { get; set; }
        public List<CardState> Hand { get; set; }
        public List<CardState> DisplayedCards { get; set; }
        public TrickState TrickState { get; set; }
        public bool IsReady { get; set; }
        public bool ShowReady { get; set; }
        public int LastBid { get; set; }
        public bool ShowLastBid { get; set; }
        public bool ShowBiddingBox { get; set; }
        public bool ShowTrumpSelection { get; set; }
        public bool ShowSwapPlayerIndex { get; set; }

        public PlayerState() {
            Name = "";
            Hand = new List<CardState>();
            DisplayedCards = new List<CardState>();
            TrickState = new TrickState();
        }
    }
}
