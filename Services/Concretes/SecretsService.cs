using API.CustomTypes;
using API.Services.Interfaces;

namespace API.Services.Concretes
{
    public class SecretsService : ISecretsService
    {
        public AssistantInformations? AssistantInformations { get; set; }
        public ConnectionStrings? ConnectionStrings { get; set; }
    }
}