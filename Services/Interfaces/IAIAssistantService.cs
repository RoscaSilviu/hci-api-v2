namespace API.Services.Interfaces
{
    public interface IAIAssistantService
    {
        public Task<string> SendMessageAndGetResponse(string message);
    }
}