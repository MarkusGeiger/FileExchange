using System.Net.Http.Headers;

namespace FileExchange.Client;

public class ClientHostedService(IHttpClientFactory clientFactory) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var httpClient = clientFactory.CreateClient();
    var filePath = "fff.png";
    var url = "http://localhost:5208/upload";
    while (stoppingToken.IsCancellationRequested == false)
    {
      using var form = new MultipartFormDataContent();
      await using var fs = File.OpenRead(filePath);
      using var streamContent = new StreamContent(fs);
      using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync(stoppingToken));
      fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

      // "file" parameter name should be the same as the server side input parameter name
      form.Add(fileContent, "file", Path.GetFileName(filePath));
      HttpResponseMessage response = await httpClient.PostAsync(url, form, stoppingToken);
      
      await Task.Delay(1000, stoppingToken);
    }
  }
}