using System.Net.Http.Headers;

namespace FileExchange.Client.UI.Services;

public class FileUploadService(ILogger<FileUploadService> logger, IHttpClientFactory httpClientFactory, UploadTrackingService uploadTracking)
{
  public async Task SendFile(string filePath, CancellationToken cancellationToken = default)
  {
    var id = uploadTracking.StartUpload(filePath);
    var isSuccess = false;
    try
    {
      var url = "http://localhost:5208/upload";
      var httpClient = httpClientFactory.CreateClient();
      using var form = new MultipartFormDataContent();
      await using var fs = File.OpenRead(filePath);
      using var streamContent = new StreamContent(fs);
      using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync(cancellationToken));
      fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

      // "file" parameter name should be the same as the server side input parameter name
      form.Add(fileContent, "file", Path.GetFileName(filePath));
      HttpResponseMessage response = await httpClient.PostAsync(url, form, cancellationToken);
      logger.LogInformation($"{response.StatusCode} - {response.ReasonPhrase}");
      isSuccess = response.IsSuccessStatusCode;
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send file to upload API");
      isSuccess = false;
    }
    uploadTracking.FinishUpload(id, isSuccess);
  }
}