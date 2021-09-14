// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.MefControllerFactory
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class MefControllerFactory : IControllerFactory
  {
    private readonly DefaultControllerFactory defaultControllerFactory;

    public MefControllerFactory() => this.defaultControllerFactory = new DefaultControllerFactory();

    public IController CreateController(
      RequestContext requestContext,
      string controllerName)
    {
      return Bootstrapper.GetInstance<IController>(controllerName) ?? this.defaultControllerFactory.CreateController(requestContext, controllerName);
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
