using OpcTestService;

var test = WebApplication.CreateBuilder(args);
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDomain();

var host = builder.Build();
host.Run();
