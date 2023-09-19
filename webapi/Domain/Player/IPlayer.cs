using webapi.Domain.PlayerDetails;

namespace webapi.Domain.Player
{
    public interface IPlayer
    {
        public IPlayerDetails GetPlayerDetails();
        public int GetIndex();
        public int GetTeamIndex();
        public int GetLastBid();
        public bool GetPassed();
        public string GetName();
        public string GetGameName();
        public bool GetIsReady();
        public List<Card> GetHand();
        public void Bid(int bid);
        public void ResetBiddingState();
        public void RemoveCard(int id);
    }
}
