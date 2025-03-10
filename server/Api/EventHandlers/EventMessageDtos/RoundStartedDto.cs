using Api.WebSockets;
using DataAccess.ModelDtos;

namespace Api.EventHandlers.EventMessageDtos;

public partial class RoundStartedDto : CustomBaseDto
{
    public RoundStartedDto() => eventType = "RoundStarted";
    public int RoundNumber { get; set; }
    public required QuestionDto Question { get; set; } // ✅ Now uses the correct DTO from DataAccess.ModelDtos
}