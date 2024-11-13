using System.Net.Http.Headers;

namespace FileExchange.Client.UI.Services;

public class ClientHostedService(ILogger<ClientHostedService> logger, IHttpClientFactory clientFactory, UploadTrackingService uploadTracking) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var httpClient = clientFactory.CreateClient();
    var filePath = "fff.png";
    var url = "http://localhost:5208/upload";
    while (stoppingToken.IsCancellationRequested == false)
    {
      var id = uploadTracking.StartUpload(filePath);
      var isSuccess = await SendFile(stoppingToken, filePath, httpClient, url);
      uploadTracking.FinishUpload(id, isSuccess);
      await Task.Delay(10000, stoppingToken);
    }
  }

  private async Task<bool> SendFile(CancellationToken stoppingToken, string filePath, HttpClient httpClient, string url)
  {
    try
    {

      using var form = new MultipartFormDataContent();
      await using var fs = File.OpenRead(filePath);
      using var streamContent = new StreamContent(fs);
      using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync(stoppingToken));
      fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

      // "file" parameter name should be the same as the server side input parameter name
      form.Add(fileContent, "file", Path.GetFileName(filePath));
      HttpResponseMessage response = await httpClient.PostAsync(url, form, stoppingToken);
      logger.LogInformation($"{response.StatusCode} - {response.ReasonPhrase}");
      return response.IsSuccessStatusCode;
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send file to upload API");
      return false;
    }
  }
}