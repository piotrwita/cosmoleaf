using Erp.WmsConnector.Gateway.Api;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddApiDependencies(configuration);

var app = builder.Build();

app.UseApiDependencies(configuration);

await app.TryToMigrateDatabasesAsync();

await app.RunAsync();