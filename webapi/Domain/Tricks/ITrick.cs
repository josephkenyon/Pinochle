namespace webapi.Domain.Tricks
{
    public interface ITrick
    {
        public void PlayCard(Card card, int playerId);
        public List<Card> GetCards();
        public List<TrickPlay> GetTrickPlays();
    }
}
