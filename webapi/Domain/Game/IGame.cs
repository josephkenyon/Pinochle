using static webapi.Domain.Enums;

namespace webapi.Domain.Game
{
    public interface IGame
    {
        public int GetCurrentBid();
        public int GetPlayerTurnIndex();
        public Phase GetPhase();
        public Suit GetTrumpSuit();
        public void IncrementPhase();
        public void StartNewRound();
        public void AddScore(int teamIndex, int scoreIncrementValue);
        public int GetLastMeld(int teamIndex);
        public void NullifyMeld(int teamIndex);
        public List<string> GetScoreLog(string teamName, int teamIndex);
        public int GetTotalScore(int teamIndex);
        public List<RoundBidResult> GetRoundBidResults();
        public void AddRoundBidResult(Suit trumpSuit, int teamIndex, int bid);
        public List<int> GetCardIds(int teamIndex);
        public void AddCardIds(int teamIndex, List<int> cardIds);
    }
}
