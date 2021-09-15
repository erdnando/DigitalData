
using DigitalData.Common.Entities;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  [AllowAnonymous]
  public class HelpController : DdocController
  {
    public HelpController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public ActionResult Index() => (ActionResult) this.View();
  }
}
