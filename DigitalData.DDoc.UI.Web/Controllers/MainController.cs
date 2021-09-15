
using DigitalData.Common.Entities;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class MainController : DdocController
  {
    public MainController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public ViewResult Main() => this.View();

    public ViewResult Welcome() => this.View();
  }
}
