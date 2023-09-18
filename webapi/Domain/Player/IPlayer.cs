namespace webapi.Domain.Player
{
    public interface IPlayer
    {
        public void SetHand(List<Card> cards);
        public List<Card> GetHand();
        public void RemoveCard(int id);
    }
}
