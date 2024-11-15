using FileExchange.Client.UI;
using FileExchange.Client.UI.Components;
using FileExchange.Client.UI.Services;
using FileExchange.Client.UI.Services.UploadQueue;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddSingleton<UploadTrackingService>();
builder.Services.AddScoped<FileUploadService>();

// Timed uploads
builder.Services.AddHostedService<ClientHostedService>();

// Queued uploads
builder.Services.AddSingleton<FileUploadService>();
builder.Services.AddSingleton<FileWatcherService>();
builder.Services.AddSingleton<FileUploadQueueService>();
builder.Services.AddHostedService<QueuedHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();