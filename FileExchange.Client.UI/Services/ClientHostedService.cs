using MudBlazor;

namespace FileExchange.Client.UI.Services;

public class ClientHostedService(ILogger<ClientHostedService> logger, IServiceProvider services) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var filePath = "fff.png";
    while (stoppingToken.IsCancellationRequested == false)
    {
      using var scope = services.CreateScope();
      var fileUpload = scope.ServiceProvider.GetRequiredService<FileUploadService>();
      await fileUpload.SendFile(filePath, stoppingToken);
      await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }
  }
}