namespace webapi.Domain
{
    public class PlayerConnectionData
    {
        public string? Id { get; set; }
        public string GameName { get; set; }
        public string PlayerName { get; set; }

        public PlayerConnectionData(string gameName, string playerName, string? id = null)
        {
            Id = id;
            GameName = gameName;
            PlayerName = playerName;
        }
    }
}

