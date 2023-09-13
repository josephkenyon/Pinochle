namespace webapi.Domain
{
    public class BidData
    {
        public int Bid { get; set; }
        public string GameName { get; set; }
        public string PlayerName { get; set; }

        public BidData(string gameName, string playerName, int id)
        {
            Bid = id;
            GameName = gameName;
            PlayerName = playerName;
        }
    }
}