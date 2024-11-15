using System.Collections.Concurrent;

namespace FileExchange.Client.UI.Services.UploadQueue;

public class TriggeredQueue<T>
{
  private readonly BlockingCollection<T> _fileQueue = new BlockingCollection<T>(new ConcurrentQueue<T>());
  public event EventHandler<T>? OnItemEnqueued;
  public event EventHandler<T>? OnItemDequeued;

  public int Count => _fileQueue.Count;
  
  public void Enqueue(T item)
  {
    _fileQueue.Add(item);
    OnItemEnqueued?.Invoke(this, item);
  }

  public T Dequeue()
  {
    var item = _fileQueue.Take();
    OnItemDequeued?.Invoke(this, item);
    return item;
  }
}