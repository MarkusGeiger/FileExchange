using System.Reflection.Metadata;

var tempFolder = Path.Combine(Path.GetTempPath(), "FileExchangeTarget");
if(Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
Directory.CreateDirectory(tempFolder);

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.Services.AddAntiforgery();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseAntiforgery();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

var summaries = new[]
{
  "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
  {
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
        ))
      .ToArray();
    return forecast;
  })
  .WithName("GetWeatherForecast")
  .WithOpenApi();

app.MapPost("/upload", async (IFormFile file) =>
{
  var fileExtension = Path.GetExtension(file.FileName);
  if (string.IsNullOrWhiteSpace(fileExtension)) fileExtension = ".png";
  var randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
  var tempFile = Path.Combine(tempFolder, randomFileName + fileExtension);
  
  app.Logger.LogInformation($"Random file: {randomFileName}, Random file without extension: {Path.GetFileNameWithoutExtension(randomFileName)}, Temp folder: {Path.GetTempPath()}");
  app.Logger.LogInformation($"Temp file: {tempFile}, Original File: {file.FileName}");
  await using var stream = File.OpenWrite(tempFile);
  await file.CopyToAsync(stream);
}).DisableAntiforgery();

app.MapPost("/upload_many", async (IFormFileCollection myFiles) =>
{
  foreach (var file in myFiles)
  {
    var tempFile = Path.GetTempFileName();
    app.Logger.LogInformation(tempFile);
    await using var stream = File.OpenWrite(tempFile);
    await file.CopyToAsync(stream);
  }
}).DisableAntiforgery();

// app.MapPost("/upload", async (FormFile file) =>
// {
//   var size = file.Length;
//
//   if (file.Length <= 0) return Results.BadRequest("File is empty");
//   var filePath = Path.GetTempFileName();
//   app.Logger.LogInformation("Uploading file {FilePath}", filePath);
//   await using var stream = File.Create(filePath);
//   await file.CopyToAsync(stream);
//   
//
//   // Process uploaded files
//   // Don't rely on or trust the FileName property without validation.
//
//   return Results.Ok(new { size });
// });

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}