

using AdsTestService.Model;

namespace AdsTestService.Interfaces;

public interface IWorkQueue
{
    void Enqueue(WorkItem workItem);
    bool TryDequeue(out WorkItem? workItem);
    Task DoWork(CancellationToken stoppingToken);
}
