
using DigitalData.Common.Entities;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class SecurityController : DdocController
  {
    public SecurityController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    [AllowAnonymous]
    public ActionResult Login()
    {
      if (this.Request.IsAuthenticated)
      {
        if (!this.CurrentUserSession.Roles.Contains("Admin"))
          return (ActionResult) this.RedirectToAction("Main", "Main");
        this.HttpContext.GetOwinContext().Authentication.SignOut("DDocAuth");
      }
      HttpCookie httpCookie = this.Request.Cookies.Get("__ddocserver");
      if (httpCookie != null)
        this.ViewData["SelectedServer"] = (object) int.Parse(httpCookie.Value);
      this.ViewData["DdocVersion"] = (object) FileVersionInfo.GetVersionInfo(typeof (IDdocService).Assembly.Location).FileVersion;
      return (ActionResult) this.PartialView();
    }

    public async Task<ActionResult> LogOff()
    {
      SecurityController securityController = this;
      await securityController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "LogOut",
        User = securityController.CurrentUserSession.Username,
        Module = securityController.ModuleName
      });
      securityController.HttpContext.GetOwinContext().Authentication.SignOut("DDocAuth");
      return (ActionResult) securityController.RedirectToAction("Login");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(User user)
    {
      SecurityController securityController = this;
      Response<LoginError> response = new Response<LoginError>();
      try
      {
        securityController.HttpContext.GetOwinContext().Authentication.SignOut("DDocAuth");
        user.InstanceType = DdocInstanceType.Web;
        UserSession userSession = await securityController.Ddoc.Login(user);
        if (!userSession.LoginOk)
        {
          await securityController.Ddoc.ActivityLog(new DdocActionLogEntry()
          {
            Action = "LogInError",
            User = user.Username,
            Module = securityController.ModuleName,
            Details = userSession.LoginError.ErrorMessage
          });
          response.Message = userSession.LoginError.ErrorMessage;
          return (ActionResult) securityController.Json((object) response);
        }
        securityController.HttpContext.GetOwinContext().Authentication.SignIn(userSession.ToIdentity());
        await securityController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "LogIn",
          User = userSession.Username,
          Module = securityController.ModuleName,
          Details = "OK"
        });
        response.Url = securityController.Url.Action("Main", "Main");
        response.Result = RequestResult.Success;
        userSession = (UserSession) null;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      return (ActionResult) securityController.Json((object) response);
    }

    [HttpPost]
    [AllowAnonymous]
    public JsonResult AdTest(User user)
    {
      Response<string> response1 = new Response<string>();
      try
      {
        using (PrincipalContext context = new PrincipalContext(ContextType.Domain, this.Settings.GetSetting<string>("DefaultDomain"), this.Settings.GetSetting<string>("LdapPath")))
        {
          Response<string> response2 = response1;
          response2.Message = response2.Message + "PrincipalContext OK: " + context.Name + "\r\n\r\n";
          List<GroupPrincipal> list = new PrincipalSearcher((Principal) new GroupPrincipal(context)).FindAll().Cast<GroupPrincipal>().ToList<GroupPrincipal>();
          Response<string> response3 = response1;
          response3.Message = response3.Message + string.Join(",", list.Select<GroupPrincipal, string>((Func<GroupPrincipal, string>) (g => g.Name))) + "\r\n\r\n";
          foreach (GroupPrincipal groupPrincipal in list)
          {
            PrincipalSearchResult<Principal> members = groupPrincipal.GetMembers();
            Response<string> response4 = response1;
            response4.Message = response4.Message + groupPrincipal.Name + ": " + string.Join(",", (IEnumerable<string>) members.Select<Principal, string>((Func<Principal, string>) (m => m.SamAccountName)).ToList<string>()) + "\r\n\r\n";
          }
        }
      }
      catch (Exception ex)
      {
        response1.Message += ex.Message;
      }
      return this.Json((object) response1);
    }
  }
}
