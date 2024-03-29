namespace AdsTestService.Interfaces;

public interface IPlcConnectionService<T> where T : class
{
    Task<T> Connect();
    Task Disconnect();
}
