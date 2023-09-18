using webapi.Domain.GameDetails;
using static webapi.Domain.Enums;

namespace webapi.Domain.MessageDetails
{
    public interface IMessageDetails : IGameDetails
    {
        MessageCode GetCode();
        string GetContent();
    }
}
