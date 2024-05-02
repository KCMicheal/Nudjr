using Nudjr_AppCore.Services.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class ServiceFactory : IServiceFactory
    {
        IServiceProvider ServiceProvider { get; set; }
        public ServiceFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }


        public object GetService(Type type)
        {
            var service = ServiceProvider.GetService(type);
            if (service == null)
                throw new InvalidOperationException("Type Not Supported");
            return service;
        }

        public T GetService<T>() where T : class
        {
            T? service = ServiceProvider.GetService(typeof(T)) as T;
            if (service == null)
                throw new InvalidOperationException("Type Not Supported");
            return service;
        }
    }
}
