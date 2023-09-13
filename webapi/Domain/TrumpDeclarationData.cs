using static webapi.Domain.Enums;

namespace webapi.Domain
{
    public class TrumpDeclarationData
    {
        public Suit TrumpSuit { get; set; }
        public string GameName { get; set; }
        public string PlayerName { get; set; }

        public TrumpDeclarationData(string gameName, string playerName, Suit trumpSuit) {
            TrumpSuit = trumpSuit;
            GameName = gameName;
            PlayerName = playerName;
        }
    }
}
