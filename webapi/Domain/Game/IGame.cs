using webapi.Domain.Bidding;
using webapi.Domain.PlayerDetails;
using static webapi.Domain.Statics.Enums;

namespace webapi.Domain.Game
{
    public interface IGame
    {
        int GetCurrentBid();
        int GetPlayerTurnIndex();
        void SetPlayerTurnIndex(int index);
        int GetTookBidTeamIndex();
        void SetTookBidTeamIndex(int newTeamIndex);
        Phase GetPhase();
        Suit GetTrumpSuit();
        void SetTrumpSuit(Suit suit);
        void IncrementPhase();
        void StartNewRound();
        void AddScore(int teamIndex, int scoreIncrementValue);
        int GetLastMeld(int teamIndex);
        void NullifyMeld(int teamIndex);
        List<string> GetScoreLog(string teamName, int teamIndex);
        int GetTotalScore(int teamIndex);
        List<RoundBidResult> GetRoundBidResults();
        void AddRoundBidResult(Suit trumpSuit, int teamIndex, int bid);
        List<int> GetCardIds(int teamIndex);
        void AddCardIds(int teamIndex, List<int> cardIds);
        void SetCurrentBid(int bid);
    }
}
