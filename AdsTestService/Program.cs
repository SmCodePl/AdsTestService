using AdsTestService;
using Serilog;


IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

if (configuration.GetValue<bool>("UseSqlLogs"))
{ 
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
}
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog(Log.Logger);
builder.Services.AddDomain();


var host = builder.Build();
host.Run();
