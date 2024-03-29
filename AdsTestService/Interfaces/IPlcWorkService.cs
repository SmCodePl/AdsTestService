
using TwinCAT.Ads;

namespace AdsTestService.Interfaces;

public interface IPlcWorkService
{
    Task DoWork(CancellationToken stoppingToken, AdsClient client);
}
