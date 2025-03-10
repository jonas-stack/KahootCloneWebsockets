using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class AdminStartsGameDto : CustomBaseDto
{
    public AdminStartsGameDto() => eventType = "AdminStartsGame";
    public required Guid GameId { get; set; }
}