namespace FileExchange.Client.UI.Services.UploadQueue;

public class FileWatcherService(ILogger<FileWatcherService> logger, ILoggerFactory loggerFactory)
{
  private List<WatchChannel> WatchChannels { get; } = new();
  public event EventHandler<string>? FileChanged;
  
  public void AddWatchDirectory(string directory)
  {
    var watchChannelLogger = loggerFactory.CreateLogger<WatchChannel>();
    var watchChannel = new WatchChannel(directory, watchChannelLogger);
    watchChannel.FileChanged += WatchChannelOnFileChanged;
    WatchChannels.Add(watchChannel);
  }
  
  public List<WatchChannel> GetWatchChannels() => WatchChannels;

  private void WatchChannelOnFileChanged(object? sender, string e)
  {
    logger.LogInformation($"FileChanged: {e}");
    FileChanged?.Invoke(this, e);
  }
}