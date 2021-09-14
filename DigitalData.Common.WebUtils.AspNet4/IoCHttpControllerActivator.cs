// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.IoCHttpControllerActivator
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using DigitalData.Common.IoC;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class IoCHttpControllerActivator : IHttpControllerActivator
  {
    public IHttpController Create(
      HttpRequestMessage request,
      HttpControllerDescriptor controllerDescriptor,
      Type controllerType)
    {
      IHttpController service = (IHttpController) IoCContainer.GetService(controllerType);
      if (service is IDisposable resource)
        request.RegisterForDispose(resource);
      return service;
    }
  }
}
