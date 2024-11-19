namespace FileExchange.Client.UI.Services.UploadQueue;

public class QueuedHostedService(FileWatcherService fileWatcherService, FileUploadQueueService fileUploadQueueService, FileUploadService fileUploadService) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var tempFolder = Path.Combine(Path.GetTempPath(), "FileExchangeSource");
    
    fileWatcherService.FileChanged += FileWatcherServiceOnFileChanged;
    
    fileWatcherService.AddWatchDirectory(tempFolder);
    fileUploadQueueService.FileEnqueued += FileUploadQueueServiceOnFileEnqueued;
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