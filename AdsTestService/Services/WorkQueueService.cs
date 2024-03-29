using AdsTestService.Interfaces;
using AdsTestService.Model;
using System.Collections.Concurrent;
using TwinCAT.Ads;

namespace AdsTestService.Services
{
    public class WorkQueueService : IWorkQueue
    {
        private ConcurrentQueue<WorkItem> _workItems = new ConcurrentQueue<WorkItem>();
        private readonly ILogger<WorkQueueService> _logger;
        public WorkQueueService(ILogger<WorkQueueService> logger)
        {
            _logger = logger;
        }
        public void Enqueue(WorkItem workItem)
        {
            _workItems.Enqueue(workItem);
        }

        public bool TryDequeue(out WorkItem? workItem)
        {
            return _workItems.TryDequeue(out workItem);
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                if (_workItems.TryDequeue(out var workItem))
                {
                    await ProcessDoWorkAsync(stoppingToken);
                }
            }
        }

        private async Task ProcessDoWorkAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
