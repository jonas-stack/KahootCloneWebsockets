using Api.WebSockets;

namespace Api.EventHandlers.EventMessageDtos;

public partial class PlayerSubmitsAnswerDto : CustomBaseDto
{
    public PlayerSubmitsAnswerDto()
    {
        eventType = "PlayerSubmitsAnswer";
    }

    public required Guid PlayerId { get; set; }
    public required Guid QuestionId { get; set; }
    public Guid? SelectedOptionId { get; set; }
}