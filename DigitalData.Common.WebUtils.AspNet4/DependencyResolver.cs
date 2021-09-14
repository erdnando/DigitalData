// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.DefaultDependencyResolver
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class DefaultDependencyResolver : IDependencyResolver
  {
    protected IServiceProvider serviceProvider;

    public DefaultDependencyResolver(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

    public object GetService(Type serviceType) => this.serviceProvider.GetService(serviceType);

    public IEnumerable<object> GetServices(Type serviceType) => this.serviceProvider.GetServices(serviceType);
  }
}
