namespace webapi.Domain
{
    public class TrickState
    {
        public CardState? MyCard { get; set; }
        public CardState? MyAllyCard { get; set; }
        public CardState? LeftOpponentCard { get; set; }
        public CardState? RightOpponentCard { get; set; }

        public TrickState()
        {

        }
    }
}
