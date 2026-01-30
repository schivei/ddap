var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.DdapApi>("ddapapi");

builder.Build().Run();
