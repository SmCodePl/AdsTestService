
using AdsTestService.Services;
using PlcDataModel.Interfaces;
using TwinCAT.Ads;

namespace AdsTestService
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            
            services.AddSingleton<IPlcConnectionService<AdsClient>, PlcConnectionService>();
            services.AddHostedService<Worker>();
           
            return services;
        }
    }
}
