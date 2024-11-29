using API.Managers;
using API.Services.Concretes;
using API.Services.Interfaces;
using Azure.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORS",
    policy =>
    {
        policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

var keyVaultName = builder.Configuration["AppConfiguration:KeyVaultName"];
if (string.IsNullOrWhiteSpace(keyVaultName))
{
    throw new ArgumentNullException("KeyVaultName", "KeyVaultName is not set in the configuration.");
}
var keyVaultURI = new Uri(
    $"https://{keyVaultName}.vault.azure.net/"
);

var keyVaultPrefix = builder.Configuration["AppConfiguration:KeyVaultPrefix"];
if (string.IsNullOrWhiteSpace(keyVaultPrefix))
{
    throw new ArgumentNullException("KeyVaultPrefix", "KeyVaultPrefix is not set in the configuration.");
}
builder.Configuration.AddAzureKeyVault(
    keyVaultURI,
    new DefaultAzureCredential(),
    new CustomSecretManager(keyVaultPrefix)
);

builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
builder.Services.Configure<AppConfigurationService>(builder.Configuration.GetSection("AppConfiguration"));

builder.Services.AddSingleton<ISecretsService>(provider =>
    provider.GetRequiredService<IOptions<SecretsService>>().Value);
builder.Services.AddSingleton<IAppConfigurationService>(provider =>
    provider.GetRequiredService<IOptions<AppConfigurationService>>().Value);
builder.Services.AddSingleton<IAIAssistantService, AIAssistantService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("CORS");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
