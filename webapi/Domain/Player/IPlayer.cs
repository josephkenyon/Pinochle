using webapi.Domain.PlayerDetails;

namespace webapi.Domain.Player
{
    public interface IPlayer
    {
        IPlayerDetails GetPlayerDetails();
        int GetIndex();
        int GetTeamIndex();
        int GetLastBid();
        bool GetPassed();
        string GetName();
        string GetGameName();
        bool GetIsReady();
        void SetIsReady(bool ready);
        void Bid(int bid);
        void ResetBiddingState();
        void RemoveCard(int id);
        List<Card> GetHand();
        void DealCards(List<Card> cards);
    }
}
