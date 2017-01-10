using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Web.Http;

namespace RServer
{
    class Startup : IOwinAppBuilder
    {

        private readonly ServiceContext serviceContext;

        public Startup(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();
            FormatterConfig.ConfigureFormatters(config.Formatters);
            UnityConfig.RegisterComponents(config, this.serviceContext);

            try
            {
                appBuilder.UseWebApi(config);
                appBuilder.UseFileServer(
                    new FileServerOptions()
                    {
                        EnableDefaultFiles = true,
                        RequestPath = PathString.Empty,
                        FileSystem = new PhysicalFileSystem(@".\wwwroot"),
                    });

                appBuilder.UseDefaultFiles(
                    new DefaultFilesOptions()
                    {
                        DefaultFileNames = new[] { "signalf/index.html" }
                    });

                appBuilder.UseStaticFiles(
                    new StaticFileOptions()
                    {
                        FileSystem = new PhysicalFileSystem(@".\wwwroot\Content"),
                        RequestPath = PathString.FromUriComponent(@"/Content"),
                        ServeUnknownFileTypes = true
                    });

                appBuilder.UseCors(CorsOptions.AllowAll);
                appBuilder.MapSignalR();

                config.EnsureInitialized();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }



        }
    }
}
