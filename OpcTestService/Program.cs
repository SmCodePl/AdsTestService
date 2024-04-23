using OpcTestService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDomain();

var host = builder.Build();
host.Run();
