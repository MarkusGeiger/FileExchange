namespace FileExchange.Client.UI.Services.UploadQueue;

public class WatchChannel
{
  private readonly ILogger<WatchChannel> _logger;
  private FileSystemWatcher Watcher { get; }
  
  public string Directory { get; }
  public DateTime LastChangeTime { get; private set; }
  public event EventHandler<string>? FileChanged;
  
  public WatchChannel(string path, ILogger<WatchChannel> logger)
  {
    _logger = logger;
    Directory = path;
    Watcher = new FileSystemWatcher();
    Watcher.IncludeSubdirectories = true;
    //Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
    Watcher.Path = path;
    Watcher.Filter = "*.*";
    Watcher.Changed += WatcherOnChanged;
    Watcher.Created += WatcherOnCreated;
    Watcher.Deleted += WatcherOnDeleted;
    Watcher.Error += WatcherOnError;
    Watcher.Renamed += WatcherOnRenamed;
    Watcher.EnableRaisingEvents = true;
  }

  private void WatcherOnDeleted(object sender, FileSystemEventArgs e)
  {
    _logger.LogInformation($"File {e.FullPath} deleted.");
    LastChangeTime = DateTime.UtcNow;
    FileChanged?.Invoke(this, e.FullPath);
  }

  private void WatcherOnRenamed(object sender, RenamedEventArgs e)
  {
    _logger.LogInformation($"File {e.FullPath} renamed.");
    LastChangeTime = DateTime.UtcNow;
    FileChanged?.Invoke(this, e.FullPath);
  }

  private void WatcherOnError(object sender, ErrorEventArgs e)
  {
    _logger.LogError(e.GetException(), $"Error occured: {e.GetException().Message}");
    LastChangeTime = DateTime.UtcNow;
    FileChanged?.Invoke(this, string.Empty);
  }

  private void WatcherOnCreated(object sender, FileSystemEventArgs e)
  {
    _logger.LogInformation($"File {e.FullPath} created.");
    LastChangeTime = DateTime.UtcNow;
    FileChanged?.Invoke(this, e.FullPath);
  }

  private void WatcherOnChanged(object sender, FileSystemEventArgs e)
  {
    _logger.LogInformation($"File {e.FullPath} changed.");
    LastChangeTime = DateTime.UtcNow;
    FileChanged?.Invoke(this, e.FullPath);
  }
}