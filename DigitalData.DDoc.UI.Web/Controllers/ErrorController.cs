// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.ErrorController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class ErrorController : DdocController
  {
    public ErrorController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public PartialViewResult NotFound()
    {
      this.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (NotFound),
        User = this.CurrentUserSession.Username,
        Module = this.ModuleName
      });
      return this.PartialView();
    }

    public PartialViewResult InternalError()
    {
      this.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (InternalError),
        User = this.CurrentUserSession.Username,
        Module = this.ModuleName
      });
      return this.PartialView();
    }

    [AllowAnonymous]
    public PartialViewResult SessionExpired()
    {
      this.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (SessionExpired),
        User = this.Request.UserHostAddress,
        Module = this.ModuleName
      });
      return this.PartialView();
    }

    [AllowAnonymous]
    public PartialViewResult Unauthorized()
    {
      this.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "UnauthorizedAccess",
        User = this.CurrentUserSession?.Username ?? this.Request.UserHostAddress,
        Module = this.ModuleName
      });
      return this.PartialView();
    }
  }
}
