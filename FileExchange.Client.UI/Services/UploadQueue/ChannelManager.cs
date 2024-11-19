namespace FileExchange.Client.UI.Services.UploadQueue;

public class ChannelManager(ILogger<ChannelManager> logger, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
{
  public Dictionary<Guid, Channel> Channels { get; } = new();
  public event EventHandler<Channel>? ChannelUploadFinished;
  
  public void RegisterChannel(string watchDirectory, Uri uploadUri)
  {
    try
    {
      var channel = new Channel(loggerFactory, httpClientFactory, watchDirectory, uploadUri);
      channel.ChannelUploadFinished += OnChannelUploadFinished;
      Channels[channel.Id] = channel;
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to register channel");
    }
  }

  private void OnChannelUploadFinished(object? sender, Upload upload)
  {
    if(sender is not Channel channel) return;
    ChannelUploadFinished?.Invoke(this, channel);
  }

  public void UnregisterChannel(Guid channelId)
  {
    try
    {
      Channels.Remove(channelId);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to unregister channel");
    }
  }
}