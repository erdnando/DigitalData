// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.UrlExtendedHelper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class UrlExtendedHelper : UrlHelper
  {
    public UrlExtendedHelper(RequestContext requestContext)
      : base(requestContext, RouteTable.Routes)
    {
    }

    public override string Content(string contentPath)
    {
      List<string> list = Directory.EnumerateFiles(this.RequestContext.HttpContext.Server.MapPath(this.RequestContext.HttpContext.Request.ApplicationPath), "*.json", SearchOption.TopDirectoryOnly).ToList<string>();
      string appSetting;
      if (list.Any<string>())
      {
        Microsoft.Extensions.Configuration.ConfigurationBuilder builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        foreach (string path in list)
          builder.AddJsonFile(path);
        appSetting = builder.Build()["AppSettings:VersionedContent"];
      }
      else
        appSetting = ConfigurationManager.AppSettings["VersionedContent"];
      bool result;
      return bool.TryParse(appSetting, out result) ? UrlHelper.GenerateContentUrl(contentPath, this.RequestContext.HttpContext) + (result ? string.Format("?v={0:MMddHHmm}", (object) DateTime.Now) : string.Empty) : UrlHelper.GenerateContentUrl(contentPath, this.RequestContext.HttpContext);
    }
  }
}
