﻿namespace webapi.Domain
{
    public class HeroPlayerState : PlayerState
    {
        public int CurrentBid { get; set; }
        public bool ShowPlayButton { get; set; }
        public bool ShowCollectButton { get; set; }
        public bool ShowBiddingBox { get; set; }
        public bool ShowTrumpSelection { get; set; }
        public List<string> TeamOneScoreList { get; set; }
        public List<string> TeamTwoScoreList { get; set; }
        public List<RoundBidResult> RoundBidResults { get; set; }
        public PlayerState AllyState { get; set; }
        public PlayerState LeftOpponentState { get; set; }
        public PlayerState RightOpponentState { get; set; }
        public TrickState TrickState { get; set; }

        public HeroPlayerState() : base() {
            TeamOneScoreList = new List<string>();
            TeamTwoScoreList = new List<string>();
            RoundBidResults = new List<RoundBidResult>();
            AllyState = new PlayerState();
            LeftOpponentState = new PlayerState();
            RightOpponentState = new PlayerState();
            TrickState = new TrickState();
        }
    }
}
