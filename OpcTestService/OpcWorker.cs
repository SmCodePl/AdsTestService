

using Opc.Ua.Client;
using OpcTestService.Services;
using PlcDataModel.PlcStructure.Model;

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
            _session = _testService.Connect().Result;

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
                    _testService.NetworkCheck();
                    _testService.DoEvent();
                }
                else 
                {
                    await ConnectToOpcServerAsync();
                }
              
            }
        }

        private async Task ConnectToOpcServerAsync()
        {
          _session =  await _testService.Connect();

        }
    }
}
