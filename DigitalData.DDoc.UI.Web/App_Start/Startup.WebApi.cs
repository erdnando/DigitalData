
using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace DigitalData.Open.UI.Web
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
