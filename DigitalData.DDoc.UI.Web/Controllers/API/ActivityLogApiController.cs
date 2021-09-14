// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.ActivityLogApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities.Api;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Entities.Security;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api/activityLog")]
  public class ActivityLogApiController : BaseApiController
  {
    public ActivityLogApiController(IDdocService ddoc, ILogger<ActivityLogApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }

    [HttpPost]
    [Route("entry")]
    public async Task<IHttpActionResult> ActivityLog([FromBody] DdocActionLogEntry entry)
    {
      ActivityLogApiController logApiController = this;
      int num;
      if ( !logApiController.ValidateToken(out UserSession _))
        return logApiController.Unauthorized();
      try
      {
        await logApiController.Ddoc.ActivityLog(entry);
      }
      catch (Exception ex)
      {
        return logApiController.DDocErrorMessage(ex, (object) new
        {
          entry = entry
        });
      }
      return (IHttpActionResult) logApiController.Ok<ApiResponse>(new ApiResponse());
    }
  }
}
