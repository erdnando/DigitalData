// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.WebExtensions.DdocController
// Assembly: DigitalData.DDoc.Common.WebExtensions, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 208B017F-ABB6-4BAD-B6CD-E9A77B49E772
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.WebExtensions.dll

using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace DigitalData.Open.Common.WebExtensions
{
  [UserAuthorization]
  public abstract class DdocController : BaseController
  {
    protected readonly string ViewerAnchorTarget;

    protected DdocController(
      IDdocService ddoc,
      DiDaSettings settings,
      IEnumerable<DdocCustomAction> customActions = null)
    {
      this.Ddoc = ddoc;
      this.Settings = settings;
      this.ViewerAnchorTarget = this.Settings.GetSetting<string>(nameof (ViewerAnchorTarget));
      List<DdocCustomAction> source = customActions != null ? customActions.ToList<DdocCustomAction>() : (List<DdocCustomAction>) null;
      this.ViewData["IncludeCustomListActions"] = (object) System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Plugins/Scripts/CustomListActions.js"));
      this.ViewData["IncludeCustomGridEvents"] = (object) System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Plugins/Scripts/CustomGridEvents.js"));
      this.ViewData["CustomGridActions"] = source != null ? (object) source.Where<DdocCustomAction>((Func<DdocCustomAction, bool>) (a => a.Type == CustomActionType.GridAction)) : (object) (IEnumerable<DdocCustomAction>) null;
      this.ViewData["CustomMainMenuItems"] = source != null ? (object) source.Where<DdocCustomAction>((Func<DdocCustomAction, bool>) (a => a.Type == CustomActionType.MainMenuAction)) : (object) (IEnumerable<DdocCustomAction>) null;
      this.ViewData["CustomSubMenuItems"] = source != null ? (object) source.Where<DdocCustomAction>((Func<DdocCustomAction, bool>) (a => a.Type == CustomActionType.SubMenuAction)) : (object) (IEnumerable<DdocCustomAction>) null;
      this.ViewData["CustomViewerActions"] = source != null ? (object) source.Where<DdocCustomAction>((Func<DdocCustomAction, bool>) (a => a.Type == CustomActionType.ViewerAction)) : (object) (IEnumerable<DdocCustomAction>) null;
      if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Plugins/pluginsConfig.json")))
      {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.AddJsonFile(System.Web.HttpContext.Current.Server.MapPath("~/Plugins/pluginsConfig.json"));
        builder.Build().Bind((object) this.PluginSettings);
      }
      this.ViewData[nameof (ViewerAnchorTarget)] = (object) this.ViewerAnchorTarget;
      this.ViewData["ProductName"] = this.Settings.AppConfig.ContainsKey("ProductName") ? (object) this.Settings.GetConfig<string>("ProductName") : (object) "DDOC";
      this.ViewData["CompanyName"] = this.Settings.AppConfig.ContainsKey("CompanyName") ? (object) this.Settings.GetConfig<string>("CompanyName") : (object) "Digital Data";
      this.ViewData["IncludeCustomStyles"] = (object)System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Plugins/Styles/Custom.css"));
    }

    protected UserSession CurrentUserSession
    {
      get
      {
        if (this.Request.IsAuthenticated)
          return UserSession.FromClaims(((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims);
        string[] values = this.Request.QueryString.GetValues("apiToken");
        return values != null && ((IEnumerable<string>) values).Any<string>() ? UserSession.FromToken(values[0]) : (UserSession) null;
      }
    }

    protected IDdocService Ddoc { get; }

    public ILogger Logger { get; protected set; }

    protected string ModuleName { get; private set; }

    protected DiDaSettings PluginSettings { get; }

    public DiDaSettings Settings { get; }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      this.ModuleName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
      this.ViewData["Username"] = (object) this.CurrentUserSession?.Username;
      base.OnActionExecuting(filterContext);
    }
  }
}
