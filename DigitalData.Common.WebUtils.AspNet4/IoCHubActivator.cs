// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.IoCHubActivator
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class IoCHubActivator : IHubActivator
  {
    private readonly IServiceProvider _provider;

    public IoCHubActivator(IServiceProvider provider) => this._provider = provider;

    public IHub Create(HubDescriptor descriptor) => (IHub) this._provider.GetService(descriptor.HubType);
  }
}
