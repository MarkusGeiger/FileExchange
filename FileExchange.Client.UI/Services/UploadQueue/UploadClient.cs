using System.Net.Http.Headers;

namespace FileExchange.Client.UI.Services.UploadQueue;

public class UploadClient(ILogger<UploadClient> logger, IHttpClientFactory httpClientFactory, Uri uploadUri)
{
  public List<Upload> Uploads { get; } = [];
  public event EventHandler<Upload>? UploadChanged;
  public Uri UploadUri => uploadUri;

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
  
  public async Task SendFile(string filePath, CancellationToken cancellationToken = default)
  {
    var id = StartUpload(filePath);
    var isSuccess = false;
    try
    {
      var httpClient = httpClientFactory.CreateClient();
      using var form = new MultipartFormDataContent();
      await using var fs = File.OpenRead(filePath);
      using var streamContent = new StreamContent(fs);
      using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync(cancellationToken));
      fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

      // "file" parameter name should be the same as the server side input parameter name
      form.Add(fileContent, "file", Path.GetFileName(filePath));
      HttpResponseMessage response = await httpClient.PostAsync(uploadUri, form, cancellationToken);
      logger.LogInformation($"{response.StatusCode} - {response.ReasonPhrase}");
      isSuccess = response.IsSuccessStatusCode;
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send file to upload API");
      isSuccess = false;
    }
    FinishUpload(id, isSuccess);
  }
}