namespace webapi.Domain.Player
{
    public interface IPlayer
    {
        public int GetIndex();
        public int GetLastBid();
        public string GetName();
        public bool GetIsReady();
        public void SetHand(List<Card> cards);
        public List<Card> GetHand();
        public void RemoveCard(int id);
    }
}
