namespace FileExchange.Client.UI.Services.UploadQueue;

public class QueuedHostedService(FileWatcherService fileWatcherService, FileUploadQueueService fileUploadQueueService, FileUploadService fileUploadService) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var tempFolder = Path.Combine(Path.GetTempPath(), "FileExchangeSource");
    if(Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
    Directory.CreateDirectory(tempFolder);
    
    
    
    fileWatcherService.FileChanged += FileWatcherServiceOnFileChanged;
    
    fileWatcherService.AddWatchDirectory(tempFolder);
    fileUploadQueueService.FileEnqueued += FileUploadQueueServiceOnFileEnqueued;
    Task.Run(async () =>
    {
      using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
      while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
      {
        var randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        File.Copy("fff.png", Path.Combine(tempFolder, randomFileName + ".png"));
      }
    });
  }

  private void FileUploadQueueServiceOnFileEnqueued(object? sender, string e)
  {
    Task.Run(async () =>
    {
      await fileUploadService.SendFile(e);
    }).Wait();
  }

  private void FileWatcherServiceOnFileChanged(object? sender, string e)
  {
    fileUploadQueueService.Enqueue(e);
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    // TODO
  }
}