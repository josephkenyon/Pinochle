using webapi.Domain.GameDetails;
using static webapi.Domain.Enums;

namespace webapi.Domain.MessageDetails
{
    public class MessageDetails : GameDetails.GameDetails, IMessageDetails
    {
        public MessageCode Code { get; set; }
        public string Content { get; set; }

        public MessageDetails(IGameDetails gameDetails, MessageCode code, string content) : base(gameDetails.GetGameName())
        {
            Code = code;
            Content = content;
        }

        public MessageCode GetCode()
        {
            return Code;
        }

        public string GetContent()
        {
            return Content;
        }
    }
}
