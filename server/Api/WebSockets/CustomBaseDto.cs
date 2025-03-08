using WebSocketBoilerplate;

namespace Api.WebSockets
{
    public class CustomBaseDto : BaseDto
    {
        public string? SenderId { get; set; } // The ID of the sender of the message
        public string? RecipientId { get; set; } // The ID of the recipient of the message
        public string? Topic { get; set; } // The topic (game ID or room name)
    }
}