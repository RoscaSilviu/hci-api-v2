using System.Text;
using API.DTOs.AI.Request;
using API.DTOs.AI.Response;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIAssistantService _aIAssistantService;
        private readonly ISecretsService _secretsService;
        private readonly IAppConfigurationService _appConfigurationService;

        public AIController(IAIAssistantService aIAssistantService, ISecretsService secretsService, IAppConfigurationService appConfigurationService)
        {
            _aIAssistantService = aIAssistantService;
            _secretsService = secretsService;
            _appConfigurationService = appConfigurationService;
        }

        [HttpPost("assistant/message")]
        public async Task<ActionResult<PostMessageResponseDTO>> PostMessage([FromBody] PostMessageRequestDTO requestBody)
        {
            string textMessageResponse = await _aIAssistantService.SendMessageAndGetResponse(requestBody.TextMessage);
            textMessageResponse = textMessageResponse.ToLower();
            // if (textMessageResponse != "on" && textMessageResponse != "off")
            // {
            //     textMessageResponse = "off";
            // }

            PostMessageResponseDTO response = new()
            {
                TextMessage = textMessageResponse
            };

            // string? ioTHubConnectionString = _secretsService.ConnectionStrings?.IoTHub;
            // if (ioTHubConnectionString != null)
            // {
            //     var serviceClientForIoTHub = ServiceClient.CreateFromConnectionString(ioTHubConnectionString);
            //     var seralizedMessage = JsonConvert.SerializeObject(textMessageResponse);

            //     var ioTMessage = new Message(Encoding.UTF8.GetBytes(seralizedMessage));
            //     await serviceClientForIoTHub.SendAsync(_appConfigurationService.IoTDeviceName, ioTMessage);
            // }

            return Ok(response);
        }
    }
}