namespace FileExchange.Client.UI.Services.UploadQueue;

public class FileUploadQueueService
{
  private readonly TriggeredQueue<TargetFile> _fileQueue;
  private readonly ILogger<FileUploadQueueService> _logger;

  public event EventHandler<string>? FileEnqueued;

  public FileUploadQueueService(ILogger<FileUploadQueueService> logger)
  {
    _logger = logger;
    _fileQueue = new TriggeredQueue<TargetFile>();
    _fileQueue.OnItemEnqueued += FileQueueOnOnItemEnqueued;
    _fileQueue.OnItemDequeued += FileQueueOnOnItemDequeued;
  }

  public void Enqueue(string fileName)
  {
    _fileQueue.Enqueue(new TargetFile(fileName));
  }

  public string Dequeue()
  {
    return _fileQueue.Dequeue().FileName;
  }

  private void FileQueueOnOnItemDequeued(object? sender, TargetFile e)
  {
    _logger.LogTrace($"File '{e.FileName}' is removed from queue.");
  }

  private void FileQueueOnOnItemEnqueued(object? sender, TargetFile e)
  {
    _logger.LogTrace($"File '{e.FileName}' is added to queue.");
    FileEnqueued?.Invoke(this, e.FileName);
  }
}