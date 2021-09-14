// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.BaseController
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Text;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public abstract class BaseController : Controller
  {
    protected new JsonResult Json(object data, string contentType) => this.Json(data, contentType, (Encoding) null, JsonRequestBehavior.AllowGet);

    protected new JsonResult Json(
      object data,
      string contentType,
      Encoding contentEncoding)
    {
      return this.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
    }

    protected new JsonResult Json(object data, JsonRequestBehavior behavior) => this.Json(data, (string) null, (Encoding) null, behavior);

    protected new JsonResult Json(
      object data,
      string contentType,
      JsonRequestBehavior behavior)
    {
      return this.Json(data, contentType, (Encoding) null, behavior);
    }

    protected override JsonResult Json(
      object data,
      string contentType = null,
      Encoding contentEncoding = null,
      JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
    {
      return new JsonResult()
      {
        MaxJsonLength = new int?(int.MaxValue),
        Data = data,
        ContentType = contentType,
        ContentEncoding = contentEncoding,
        JsonRequestBehavior = behavior
      };
    }
  }
}
