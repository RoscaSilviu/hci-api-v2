using API.Services.Interfaces;
using Azure;
using Azure.AI.OpenAI.Assistants;

namespace API.Services.Concretes
{
    public class AIAssistantService : IAIAssistantService
    {
        private readonly AssistantsClient? _assistantsClient;
        private readonly ISecretsService _secretsService;
        private readonly string? _id;
        private const int _DELAY_IN_MILLISECONDS = 500;

        public AIAssistantService(ISecretsService secretsService)
        {
            _secretsService = secretsService;

            var endPoint = _secretsService.AssistantInformations?.EndPoint;
            var key = _secretsService.AssistantInformations?.Key;
            _id = _secretsService.AssistantInformations?.Id;
            if (string.IsNullOrWhiteSpace(endPoint) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(_id))
            {
                _assistantsClient = null;
                return;
            }

            _assistantsClient = new AssistantsClient(
                new Uri(endPoint),
                new AzureKeyCredential(key)
            );
        }

        public async Task<string> SendMessageAndGetResponse(string message)
        {
            if (_assistantsClient == null || _id == null)
            {
                return "Error! Client is disabled!";
            }

            AssistantThread assistantThread = await _assistantsClient.CreateThreadAsync();
            ThreadMessage threadMessage = await _assistantsClient.CreateMessageAsync(assistantThread.Id, MessageRole.User, message);
            ThreadRun threadRun = await _assistantsClient.CreateRunAsync(assistantThread.Id, new CreateRunOptions(_id));

            do
            {
                threadRun = await _assistantsClient.GetRunAsync(assistantThread.Id, threadRun.Id);
                await Task.Delay(TimeSpan.FromMilliseconds(_DELAY_IN_MILLISECONDS));
            }
            while (threadRun.Status == RunStatus.Queued || threadRun.Status == RunStatus.InProgress);

            if (threadRun.Status != RunStatus.Completed)
            {
                return "Error!";
            }

            PageableList<ThreadMessage> messagesList = await _assistantsClient.GetMessagesAsync(assistantThread.Id);
            ThreadMessage? lastAssistantMessage = messagesList.FirstOrDefault(
                m => m.Role == MessageRole.Assistant
            );
            if (lastAssistantMessage?.ContentItems?.FirstOrDefault() is not MessageTextContent messageTextContent)
            {
                return "Error!";
            }

            _ = _assistantsClient.DeleteThreadAsync(assistantThread.Id);

            return messageTextContent.Text;
        }
    }
}