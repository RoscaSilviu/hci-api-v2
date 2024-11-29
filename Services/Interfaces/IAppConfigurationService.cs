namespace API.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        public string? KeyVaultName { get; set; }
        public string? KeyVaultPrefix { get; set; }
        public string? IoTDeviceName { get; set; }
    }
}