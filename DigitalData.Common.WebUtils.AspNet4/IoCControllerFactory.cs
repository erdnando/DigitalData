// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.IoCControllerFactory
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using DigitalData.Common.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class IoCControllerFactory : IControllerFactory
  {
    public IController CreateController(
      RequestContext requestContext,
      string controllerName)
    {
      ServiceDescriptor serviceDescriptor = IoCContainer.Services.SingleOrDefault<ServiceDescriptor>((Func<ServiceDescriptor, bool>) (service => service.ImplementationType != (Type) null && service.ImplementationType.Name.Equals(controllerName + "Controller")));
      IController controller = (IController) null;
      if (serviceDescriptor != null && serviceDescriptor.ImplementationType.GetTypeInfo().ImplementedInterfaces.Contains<Type>(typeof (IController)))
        controller = (IController) IoCContainer.GetService(serviceDescriptor.ImplementationType);
      return controller;
    }

    public SessionStateBehavior GetControllerSessionBehavior(
      RequestContext requestContext,
      string controllerName)
    {
      return SessionStateBehavior.Default;
    }

    public void ReleaseController(IController controller)
    {
      if (!(controller is IDisposable disposable))
        return;
      disposable.Dispose();
    }
  }
}
