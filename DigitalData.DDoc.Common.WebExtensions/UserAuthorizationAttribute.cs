// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.WebExtensions.UserAuthorizationAttribute
// Assembly: DigitalData.DDoc.Common.WebExtensions, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 208B017F-ABB6-4BAD-B6CD-E9A77B49E772
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.WebExtensions.dll

using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Open.Common.WebExtensions
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class UserAuthorizationAttribute : AuthorizeAttribute
  {
    private readonly string[] _allowedRoles;

    public UserAuthorizationAttribute()
    {
      this.Order = 2;
      this._allowedRoles = (string[]) null;
    }

    public UserAuthorizationAttribute(params string[] roles) => this._allowedRoles = roles;

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
      if (filterContext == null)
        throw new ArgumentNullException(nameof (filterContext));
      if (filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true))
        return;
      if (!filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
      {
        string[] values = filterContext.RequestContext.HttpContext.Request.QueryString.GetValues("apiToken");
        if (values != null && ((IEnumerable<string>) values).Any<string>())
        {
          if (this.AuthorizeByApiToken(values[0]))
            return;
          this.HandleUnauthorizedRequest(filterContext);
        }
        else
          this.HandleUnauthenticatedRequest(filterContext);
      }
      else
      {
        if (this.AuthorizeCore(filterContext.HttpContext))
          return;
        this.HandleUnauthorizedRequest(filterContext);
      }
    }

    protected new bool AuthorizeCore(HttpContextBase httpContext)
    {
      bool flag = false;
      UserSession userSession = UserSession.FromClaims(((ClaimsIdentity) httpContext.User.Identity).Claims);
      if (this._allowedRoles != null)
      {
        if (userSession == null)
          return false;
        foreach (string allowedRole in this._allowedRoles)
          flag = userSession.Roles.Contains(allowedRole);
      }
      else
      {
        if (userSession == null)
          return false;
        flag = userSession.Roles.Contains("User");
      }
      return flag;
    }

    protected bool AuthorizeByApiToken(string apiToken)
    {
      bool flag = false;
      UserSession userSession = UserSession.FromToken(apiToken);
      if (this._allowedRoles != null)
      {
        if (userSession == null)
          return false;
        foreach (string allowedRole in this._allowedRoles)
          flag = userSession.Roles.Contains(allowedRole);
      }
      else
      {
        if (userSession == null)
          return false;
        flag = userSession.Roles.Contains("User");
      }
      return flag;
    }

    private void HandleUnauthenticatedRequest(AuthorizationContext filterContext)
    {
      filterContext.HttpContext.Session?.Clear();
      if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
      {
        filterContext.HttpContext.Response.Clear();
        filterContext.HttpContext.Response.StatusCode = new HttpException(303, "See Other").GetHttpCode();
        AuthorizationContext authorizationContext = filterContext;
        JsonResult jsonResult = new JsonResult();
        ApiResponse<int> apiResponse = new ApiResponse<int>();
        apiResponse.Url = "/Error/SessionExpired";
        jsonResult.Data = (object) apiResponse;
        jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        authorizationContext.Result = (ActionResult) jsonResult;
      }
      else
        filterContext.Result = (ActionResult) new RedirectToRouteResult("SessionExpired", (RouteValueDictionary) null);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
      {
        filterContext.HttpContext.Response.Clear();
        filterContext.HttpContext.Response.StatusCode = new HttpException(303, "See Other").GetHttpCode();
        AuthorizationContext authorizationContext = filterContext;
        JsonResult jsonResult = new JsonResult();
        ApiResponse<int> apiResponse = new ApiResponse<int>();
        apiResponse.Url = "/Error/Unauthorized";
        jsonResult.Data = (object) apiResponse;
        jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        authorizationContext.Result = (ActionResult) jsonResult;
      }
      else
        filterContext.Result = (ActionResult) new RedirectToRouteResult("Unauthorized", (RouteValueDictionary) null);
    }
  }
}
