namespace webapi.Domain
{
    public class HeroPlayerState : PlayerState
    {
        public int CurrentBid { get; set; }
        public PlayerState AllyState { get; set; }
        public PlayerState LeftOpponentState { get; set; }
        public PlayerState RightOpponentState { get; set; }

        public HeroPlayerState() : base() {
            AllyState = new PlayerState();
            LeftOpponentState = new PlayerState();
            RightOpponentState = new PlayerState();
        }
    }
}
