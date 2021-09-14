// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.NoCacheAttribute
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Web;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class NoCacheAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1.0));
      filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
      filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
      filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      filterContext.HttpContext.Response.Cache.SetNoStore();
      base.OnActionExecuting(filterContext);
    }
  }
}
