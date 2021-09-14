// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.CustomViewEngine
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class CustomViewEngine : RazorViewEngine
  {
    public static void Enable(string path) => ViewEngines.Engines.Add((IViewEngine) new CustomViewEngine(path));

    public string Path { get; }

    public CustomViewEngine(string path)
    {
      this.Path = path;
      this.ViewLocationFormats = this.GetViewLocations();
      this.MasterLocationFormats = this.GetMasterLocations();
      this.PartialViewLocationFormats = this.GetViewLocations();
    }

    public string[] GetViewLocations() => new List<string>()
    {
      "~/Views/{1}/{0}.cshtml",
      "~/" + this.Path + "/Views/{1}/{0}.cshtml"
    }.ToArray();

    public string[] GetMasterLocations() => new List<string>()
    {
      "~/Views/Shared/{0}.cshtml",
      "~/" + this.Path + "/Views/Shared/{0}.cshtml"
    }.ToArray();
  }
}
