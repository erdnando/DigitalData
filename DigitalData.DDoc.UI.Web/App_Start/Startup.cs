// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Startup
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Configuration;
using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.Common.Logging;
using DigitalData.Common.Reflection;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities.Config;
using DigitalData.DDoc.Common.Extensions;
using DigitalData.DDoc.Core.BLL;
using DigitalData.DDoc.UI.Web.Modules;
using DigitalData.DDoc.WebUI;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.DDoc.UI.Web
{
  public class Startup
  {
    private static DiDaSettings settings;

    public static void Init()
    {
      IoCContainer.Initialize(new Action(Startup.ConfigureServices));
      HttpApplication.RegisterModule(typeof (RemoveAspMvcHeadersModule));
      HttpApplication.RegisterModule(typeof (LicenseValidator));
      MvcHandler.DisableMvcResponseHeader = true;
    }

    private static void ConfigureServices()
    {
            IConfiguration config = IoCContainer.Services.AddDidaConfig("config.json", 
                new string[] { 
                "storageConfig.json", 
                "indexingConfig.json", 
                "pluginsConfig.json" },
                null);
            Startup.settings = config.MapDiDaSettings();
            IoCContainer.Services.AddDidaLogging(Startup.settings.LoggingOptions);
            IoCContainer.RegisterMultipleTransients<IController>(typeof(Startup).Assembly);
            IoCContainer.RegisterMultipleTransients<IHttpController>(typeof(Startup).Assembly);
            IoCContainer.RegisterMultipleTransients<IHub>(typeof(Startup).Assembly);
            Startup.ConfigureModules(Startup.settings);
            Startup.ConfigureExtensions(Startup.settings);
        }

    public void Configuration(IAppBuilder app)
    {
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      ModelBinders.Binders.DefaultBinder = (IModelBinder) new NoGetterModelBinder();
      HttpConfiguration httpConfiguration = WebApiConfig.Register(Startup.settings);
      app.UseWebApi(httpConfiguration);

            IoCControllersSupport.EnableIoCMvcControllers();
            IoCControllersSupport.EnableIoCHttpControllers(httpConfiguration);
            IoCSignalRSupport.EnableIoCSignalR();
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "DDocAuth",
                LoginPath = new PathString("/Security/Login"),
                LogoutPath = new PathString("/Security/LogOff")
            });
            app.MapSignalR();
            CustomViewEngine.Enable("Plugins");
     
        }

    private static void ConfigureExtensions(DiDaSettings settings)
    {
            List<Assembly> assemblies = AssemblyScanner.GetAssemblies(searchPattern: "DigitalData.DDoc.Plugins.*.dll");
            IoCContainer.RegisterMultipleTransients<IController>((IEnumerable<Assembly>)assemblies);
            IoCContainer.RegisterMultipleTransients<IHttpController>((IEnumerable<Assembly>)assemblies);
            IoCContainer.RegisterMultipleTransients<IHub>((IEnumerable<Assembly>)assemblies);
            IoCContainer.RegisterSingletonAs<IDocumentCreation>((IEnumerable<Assembly>)assemblies, typeof(NullDocumentCreation));
            IoCContainer.RegisterSingletonAs<ICustomSearch>((IEnumerable<Assembly>)assemblies, typeof(NullCustomSearch));
            IoCContainer.RegisterSingletonAs<IFolderPreProcessor>((IEnumerable<Assembly>)assemblies, typeof(NullFolderPreProcessor));
            IoCContainer.RegisterSingletonAs<IDocumentPreProcessor>((IEnumerable<Assembly>)assemblies, typeof(NullDocumentPreProcessor));
            IoCContainer.RegisterSingletonAs<ICustomReports>((IEnumerable<Assembly>)assemblies, typeof(NullCustomReports));
            IoCContainer.RegisterSingletonAs<ICustomActions>((IEnumerable<Assembly>)assemblies, typeof(NullCustomActions));
        }

    private static void ConfigureModules(DiDaSettings settings)
    {
            IoCContainer.AddSingleton<DdocFeatures>();
            IoCContainer.AddTransient<IDdocService, DDocService>();

            List<Assembly> assemblies = AssemblyScanner.GetAssemblies(searchPattern: "DigitalData.DDoc.Modules.*.dll");

            IoCContainer.RegisterSingletonAs<IDdocAuthentication>((IEnumerable<Assembly>)assemblies, typeof(NullAuthenticationModule));
            IoCContainer.RegisterTransientAs<IDdocDAL>((IEnumerable<Assembly>)assemblies);
            IoCContainer.RegisterTransientAs<IDdocOcr>((IEnumerable<Assembly>)assemblies, typeof(NullOcrModule));
            IoCContainer.RegisterSingletonAs<IDdocStorage>((IEnumerable<Assembly>)assemblies, typeof(NullStorageModule));
            IoCContainer.RegisterSingletonAs<IDdocTextSearch>((IEnumerable<Assembly>)assemblies, typeof(NullTextSearchModule));



        }
  }
}
