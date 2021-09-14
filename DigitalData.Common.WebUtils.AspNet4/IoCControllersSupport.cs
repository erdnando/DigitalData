// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.IoCControllersSupport
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class IoCControllersSupport
  {
    public static void EnableIoCMvcControllers() => ControllerBuilder.Current.SetControllerFactory((IControllerFactory) new IoCControllerFactory());

    public static void EnableIoCHttpControllers(HttpConfiguration config) => config.Services.Replace(typeof (IHttpControllerActivator), (object) new IoCHttpControllerActivator());
  }
}
