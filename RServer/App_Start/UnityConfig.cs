using Microsoft.Practices.Unity;
using System.Fabric;
using System.Web.Http;
using Unity.WebApi;

namespace RServer
{
    class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config, ServiceContext serviceContext)
        {
            UnityContainer container = new UnityContainer();

            //container.RegisterType<DefaultApiController>(
            //    new TransientLifetimeManager(),
            //    new InjectionConstructor(serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings));

            config.DependencyResolver = new UnityDependencyResolver(container);
        }

    }
}
