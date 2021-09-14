// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.IoCSignalRSupport
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using DigitalData.Common.IoC;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class IoCSignalRSupport
  {
    public static void EnableIoCSignalR()
    {
      IoCHubActivator activator = new IoCHubActivator(IoCContainer.ServiceProvider);
      GlobalHost.DependencyResolver.Register(typeof (IHubActivator), (Func<object>) (() => (object) activator));
    }
  }
}
