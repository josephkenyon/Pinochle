using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Domain
{
    public class Player
    {
        public string Name { get; set; }
        public string GameName { get; set;}
        public int PlayerIndex { get; set; }
        public bool Passed { get; set; }
        public bool Ready { get; set; }
        public int LastBid { get; set; }
        public Game? Game { get; set; }

        public Player(string name, string gameName) {
            Name = name;
            GameName = gameName;
        }
    }
}
