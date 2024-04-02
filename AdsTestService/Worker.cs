using AdsTestService.Interfaces;
using AdsTestService.Model;
using AdsTestService.Services;
using System.Collections.Concurrent;
using System.Net.Sockets;
using TwinCAT.Ads;

namespace AdsTestService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPlcConnectionService<AdsClient> _plcConnectionService;

        private uint readHandle = 0;
        private uint writeHandle = 0;
        private short pcCheckNetwork = 0;

        private AdsClient _client = new();
        public Worker(ILogger<Worker> logger, IPlcConnectionService<AdsClient> plcConnectionService)
        {
            _logger = logger;
            _plcConnectionService = plcConnectionService;
            
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = _plcConnectionService.Connect().Result;
            
            return base.StartAsync(cancellationToken);  
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _plcConnectionService.Disconnect().Wait();
            FreeHandle();

            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckNetwork().ConfigureAwait(false);


            }

            FreeHandle();
        }

        private async Task CheckNetwork()
        {
            try
            {
                if (_client.IsConnected)
                {
                    if (readHandle != 0 && writeHandle != 0)
                    {
                        pcCheckNetwork = (short)_client.ReadAny(readHandle, typeof(short));
                        await _client.WriteAnyAsync(writeHandle, pcCheckNetwork,CancellationToken.None);
                    }
                    else
                    {
                        SetHandle();
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading from PLC");

            }

        }

        private void SetHandle()
        {
            if(readHandle == 0 )    readHandle  = _client.CreateVariableHandle("Global.stCheckNetwork.nCheckNetwork");
            if(writeHandle == 0 )   writeHandle = _client.CreateVariableHandle("Global.stCheckNetwork.nEchoCheckNetwork");

        }

        private void FreeHandle()
        {
            _client.DeleteVariableHandle(readHandle);
            _client.DeleteVariableHandle(writeHandle);
        }
       
    }
}
