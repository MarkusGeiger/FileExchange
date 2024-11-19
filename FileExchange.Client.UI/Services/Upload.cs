using System.Diagnostics;

namespace FileExchange.Client.UI.Services;

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