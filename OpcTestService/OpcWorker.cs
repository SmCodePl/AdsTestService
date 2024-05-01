

using Opc.Ua.Client;
using OpcTestService.Services;
using PlcDataModel.PlcStructure.Model;
using System.Diagnostics;

namespace OpcTestService
{
    public class OpcWorker : BackgroundService
    {
        private readonly ILogger<OpcWorker> _logger;
        private readonly OpcUaDoTestService _testService;
        private Session _session = null!;
        public OpcWorker(ILogger<OpcWorker> logger, OpcUaDoTestService testService)
        {
            _logger = logger;
            _testService = testService;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _testService.Disconnect(); 
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(_session != null && _session.Connected)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    //await _testService.NetworkCheck().ConfigureAwait(false);
                    _testService.DoEvent();
                    stopwatch.Stop();

                    TimeSpan timeTaken = stopwatch.Elapsed;
                    Console.WriteLine($"all Operation: {timeTaken}");
                }
                else 
                {
                    await ConnectToOpcServerAsync();
                }
              
            }
        }

        private async Task ConnectToOpcServerAsync()
        {
            try
            {
                _session = await _testService.Connect();
                _ = Task.Run(async () => await _testService.NetworkCheck());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while connecting to OPC server");
            }

        }
    }
}
