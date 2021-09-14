// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.HttpRequestExtensions
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Web;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class HttpRequestExtensions
  {
    public static bool IsAjaxRequest(this HttpRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request["X-Requested-With"] == "XMLHttpRequest")
        return true;
      return request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
  }
}
