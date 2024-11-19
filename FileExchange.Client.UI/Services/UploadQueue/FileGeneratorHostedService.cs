namespace FileExchange.Client.UI.Services.UploadQueue;

public class FileGeneratorHostedService() : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var tempFolder = Path.Combine(Path.GetTempPath(), "FileExchangeSource");
    if(Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
    Directory.CreateDirectory(tempFolder);
    
    using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
    while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
    {
      var randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
      File.Copy("fff.png", Path.Combine(tempFolder, randomFileName + ".png"));
    }
  }
}