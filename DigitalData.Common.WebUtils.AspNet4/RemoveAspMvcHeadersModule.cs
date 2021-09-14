// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.RemoveAspMvcHeadersModule
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Web;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class RemoveAspMvcHeadersModule : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      if (!HttpRuntime.UsingIntegratedPipeline)
        return;

      context.PreSendRequestHeaders += (EventHandler) ((sender, e) =>
      {
        if (sender is HttpApplication httpApplication2)
          {
              httpApplication2.Context.Response.Headers.Remove("Server");

              httpApplication2.Context.Response.Headers.Remove("X-Powered-By");
              httpApplication2.Context.Response.Headers.Remove("X-AspNet-Version");
              httpApplication2.Context.Response.Headers.Remove("X-AspNetMvc-Version");
          }

      });
    }

    public void Dispose()
    {
    }
  }
}
