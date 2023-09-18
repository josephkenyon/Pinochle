namespace webapi.Domain.PlayerConnection
{
    public class PlayerConnection : IPlayerConnection
    {
        public string Id { get; set; }
        public string GameName { get; set; }
        public string PlayerName { get; set; }

        public PlayerConnection(string gameName, string playerName, string id)
        {
            Id = id;
            GameName = gameName;
            PlayerName = playerName;
        }

        public string GetId()
        {
            return Id;
        }

        public string GetGameName()
        {
            return GameName;
        }

        public string GetPlayerName()
        {
            return PlayerName;
        }
    }
}

