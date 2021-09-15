// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.WebExtensions.BaseApiController
// Assembly: DigitalData.DDoc.Common.WebExtensions, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 208B017F-ABB6-4BAD-B6CD-E9A77B49E772
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.WebExtensions.dll

using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.Security;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DigitalData.Open.Common.WebExtensions
{
  public abstract class BaseApiController : ApiController
  {
    protected BaseApiController(IDdocService ddoc, ILogger logger)
    {
      this.Ddoc = ddoc;
      this.Logger = logger;
    }

    protected IDdocService Ddoc { get; }

    protected ILogger Logger { get; }

    protected string ModuleName { get; private set; }

    protected override void Initialize(HttpControllerContext controllerContext)
    {
      this.ModuleName = controllerContext.ControllerDescriptor.ControllerName;
      base.Initialize(controllerContext);
    }

    protected bool ValidateToken(out UserSession userSession)
    {
      userSession = (UserSession) null;
      IEnumerable<string> values;
      if (!this.Request.Headers.TryGetValues("X-Ddoc-Auth", out values))
      {
        IEnumerable<KeyValuePair<string, string>> queryNameValuePairs = this.Request.GetQueryNameValuePairs();
        if (!queryNameValuePairs.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key.Equals("apiToken", StringComparison.OrdinalIgnoreCase))))
        {
         // this.Logger.LogWarnParams("Llamada API No Autorizada");
          return false;
        }
        values = (IEnumerable<string>) new List<string>()
        {
          queryNameValuePairs.Single<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key.Equals("apiToken", StringComparison.OrdinalIgnoreCase))).Value
        };
      }
      userSession = UserSession.FromToken(values.First<string>());
      return userSession.Valid;
    }

    protected IHttpActionResult DDocErrorMessage(
      Exception ex,
      object parameters = null,
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
    /*  if (!(ex is DdocException) && !ex.Data.Contains((object) "ErrorId"))
        this.Logger.LogExError(ex, "Error en código de API", parameters);*/

      ApiResponse apiResponse = new ApiResponse(ex);
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(statusCode)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }

    protected IHttpActionResult DDocErrorMessage(
      string errorMessage,
      object parameters = null,
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
      string str = Guid.NewGuid().ToString("D");
      using (LogContext.PushProperty("Parameters", parameters))
      {
        using (LogContext.PushProperty("ErrorId", (object) str))
        {
          using (LogContext.PushProperty("ErrorMessage", (object) errorMessage))
            this.Logger.LogError("Error en código de API");
        }
      }
      ApiResponse apiResponse = new ApiResponse()
      {
        Success = false,
        Message = errorMessage,
        ErrorId = str
      };
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(statusCode)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }

    protected IHttpActionResult Unauthorized()
    {
      ApiResponse apiResponse = new ApiResponse()
      {
        Success = false,
        Message = "Operación no autorizada"
      };
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.Unauthorized)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }

    protected IHttpActionResult NotFound(string message = null)
    {
      ApiResponse apiResponse = new ApiResponse()
      {
        Success = false,
        Message = message ?? "Elemento no encontrado"
      };
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NotFound)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }

    protected IHttpActionResult BadRequest(string message = null)
    {
      ApiResponse apiResponse = new ApiResponse()
      {
        Success = false,
        Message = message ?? "Parámetros incorrectos"
      };
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }

    protected IHttpActionResult StatusCode(
      HttpStatusCode statusCode,
      string message = null)
    {
      ApiResponse apiResponse = new ApiResponse()
      {
        Success = false,
        Message = message
      };
      return (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(statusCode)
      {
        Content = (HttpContent) new StringContent(JsonSerializer.Serialize<ApiResponse>(apiResponse), Encoding.UTF8, "application/json")
      });
    }
  }
}
