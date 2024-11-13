using System.Diagnostics;

namespace FileExchange.Client.UI.Services;

public class UploadTrackingService(ILogger<UploadTrackingService> logger)
{
  public List<Upload> Uploads { get; } = [];
  public event EventHandler<Upload>? UploadChanged;

  public Guid StartUpload(string path)
  {
    var id = Guid.CreateVersion7();
    var upload = new Upload(id, path);
    Uploads.Add(upload);
    OnUploadChanged(upload);
    return id;
  }

  public void FinishUpload(Guid id, bool isSuccess)
  {
    var upload = Uploads.Single(u => u.Id == id);
    upload.Stop(isSuccess);
    logger.LogInformation($"Finished upload {upload.Path} in {upload.Duration}ms");
    OnUploadChanged(upload);
  }

  protected virtual void OnUploadChanged(Upload e)
  {
    UploadChanged?.Invoke(this, e);
  }
}

public class Upload
{
  public Guid Id { get; init; }
  public string Path { get; init; }
  public long Duration { get; private set; }
  public UploadStatus Status { get; private set; } = UploadStatus.Started; 
  private readonly Stopwatch _stopWatch = new();

  public Upload(Guid id, string path)
  {
    Id = id;
    Path = path;
    _stopWatch.Start();
  }

  public void Stop(bool isSuccess)
  {
    Status = isSuccess ? UploadStatus.Success : UploadStatus.Failed;
    _stopWatch.Stop();
    Duration = _stopWatch.ElapsedMilliseconds;
  }
}

public enum UploadStatus
{
  Started,
  Success,
  Failed
}