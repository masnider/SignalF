using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace RServer
{
    class SignalRListener : ICommunicationListener
    {
        private readonly IOwinAppBuilder startup;
        private readonly string appRoot;
        private readonly ServiceContext serviceContext;
        private string listeningAddress;
        private IDisposable signalRHost;

        public SignalRListener(string appRoot, IOwinAppBuilder startup, ServiceContext serviceInitializationParameters)
        {
            this.startup = startup;
            this.appRoot = appRoot;
            this.serviceContext = serviceInitializationParameters;
        }

        public void Abort()
        {
            DisposeR();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return DisposeR();
        }

        private Task DisposeR()
        {
            this.signalRHost.Dispose();
            return Task.FromResult<bool>(true);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            EndpointResourceDescription serviceEndpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = serviceEndpoint.Port;

            if (this.serviceContext is StatefulServiceContext)
            {
                StatefulServiceContext statefulServiceContext =
                    (StatefulServiceContext)this.serviceContext;

                this.listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}/{2}/{3}",
                    port,
                    statefulServiceContext.PartitionId,
                    statefulServiceContext.ReplicaId,
                    Guid.NewGuid().ToString() + '/');
            }
            else if (this.serviceContext is StatelessServiceContext)
            {
                this.listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    string.IsNullOrWhiteSpace(this.appRoot)
                        ? string.Empty
                        : this.appRoot.TrimEnd('/') + '/');
            }
            else
            {
                throw new InvalidOperationException();
            }

            this.signalRHost = WebApp.Start(this.listeningAddress, appBuilder => this.startup.Configuration(appBuilder));

            string resultAddress = this.listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            return Task.FromResult<string>(resultAddress);
        }

    }
}
