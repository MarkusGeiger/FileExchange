var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FileExchange_Api>("Api");

builder.Build().Run();