namespace FileExchange.Client.UI.Services.UploadQueue;

public class Channel
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public WatchChannel FileWatcher { get; set; }
  public FileUploadQueueService FileUploadQueueService { get; set; }
  public UploadClient UploadClient { get; set; }
  public event EventHandler<Upload>? ChannelUploadFinished;
  
  public Channel(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, string watchDirectory,
    Uri uploadUri)
  {
    // Create all needed components
    FileWatcher = new WatchChannel(watchDirectory, loggerFactory.CreateLogger<WatchChannel>());
    FileUploadQueueService = new FileUploadQueueService(loggerFactory.CreateLogger<FileUploadQueueService>());
    UploadClient = new UploadClient(loggerFactory.CreateLogger<UploadClient>(), httpClientFactory, uploadUri);
    
    // Wire up the events
    FileWatcher.FileChanged += (sender, file) => FileUploadQueueService.Enqueue(file);
    FileUploadQueueService.FileEnqueued += (sender, file) =>
    {
      Task.Run(async () =>
      {
        await UploadClient.SendFile(file);
      }).Wait();
    };
    UploadClient.UploadChanged += (sender, file) => ChannelUploadFinished?.Invoke(this, file);
  }
}