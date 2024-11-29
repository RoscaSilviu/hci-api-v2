using API.CustomTypes;

namespace API.Services.Interfaces
{
    public interface ISecretsService
    {
        public AssistantInformations? AssistantInformations { get; set; }
        public ConnectionStrings? ConnectionStrings { get; set; }
    }
}