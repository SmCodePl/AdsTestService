using Opc.Ua.Client;
using OpcTestService.Services;
using PlcDataModel.Interfaces;

namespace OpcTestService;
    public static class ServiceRegistration
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {

            services.AddSingleton<OpcUaDoTestService>();
            services.AddHostedService<OpcWorker>();

            return services;
        }
    }
