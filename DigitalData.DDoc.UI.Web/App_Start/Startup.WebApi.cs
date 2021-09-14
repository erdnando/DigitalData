// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.WebApiConfig
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web
{
  public static class WebApiConfig
  {
    public static HttpConfiguration Register(DiDaSettings settings)
    {
      HttpConfiguration configuration = new HttpConfiguration();
      configuration.MessageHandlers.Add((DelegatingHandler) new ChunkedHandler());
      configuration.MapHttpAttributeRoutes();
      if (settings.GetConfig<bool>("UseNewJsonSerializer"))
      {
        configuration.Formatters.RemoveAt(0);
        configuration.Formatters.Insert(0, (MediaTypeFormatter) new JsonFormatter());
      }
      return configuration;
    }
  }
}
