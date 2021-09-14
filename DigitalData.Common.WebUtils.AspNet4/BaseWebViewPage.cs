// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.CustomWebViewPage`1
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public abstract class CustomWebViewPage<TModel> : WebViewPage<TModel>
  {
    protected Html5CanvasHelper Canvas => new Html5CanvasHelper(this.ViewContext, (IViewDataContainer) this);

    protected Html5Helper<TModel> Html5 => new Html5Helper<TModel>(this.ViewContext, (IViewDataContainer) this);

    protected Html5MediaHelper Media => new Html5MediaHelper(this.ViewContext, (IViewDataContainer) this);

    protected Html5SvgHelper Svg => new Html5SvgHelper(this.ViewContext, (IViewDataContainer) this);

    protected UrlExtendedHelper Url => new UrlExtendedHelper(this.Request.RequestContext);
  }
}
