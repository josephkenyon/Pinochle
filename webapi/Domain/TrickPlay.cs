namespace webapi.Domain
{
    public class TrickPlay
    {
        public int PlayerIndex { get; set; }
        public Card Card { get; set; }

        public TrickPlay(Card card, int playerIndex) {
            Card = card;
            PlayerIndex = playerIndex;
        }
    }
}
