var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FileExchange_Api>("Api");
builder.AddProject<Projects.FileExchange_Client>("Client");

builder.Build().Run();