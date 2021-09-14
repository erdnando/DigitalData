// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Html5Helper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class Html5Helper : IHtml5Extensions
  {
    public Html5Helper(ViewContext viewContext, IViewDataContainer viewDataContainer)
      : this(viewContext, viewDataContainer, RouteTable.Routes)
    {
    }

    public Html5Helper(
      ViewContext viewContext,
      IViewDataContainer viewDataContainer,
      RouteCollection routeCollection)
    {
      this.ViewContext = viewContext;
      this.ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
    }

    public ViewContext ViewContext { get; set; }

    public ViewDataDictionary ViewData { get; set; }

    public MvcHtmlString PlaceholderBox(
      string name,
      string placeholderText,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag(name, "text", htmlAttributes);
      tagBuilder.MergeAttribute("placeholder", placeholderText);
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString EmailBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "email", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString UrlBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "url", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString NumberBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "number", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString NumberBox(string name, double min, object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag(name, "number", htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBox(
      string name,
      double min,
      double max,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag(name, "number", htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBox(
      string name,
      double min,
      double max,
      double step,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag(name, "number", htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if (step != 0.0)
        tagBuilder.MergeAttribute(nameof (step), step.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString Range(
      string name,
      int min = -1,
      int max = -1,
      int step = 0,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag(name, "range", htmlAttributes);
      if (min != -1)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if ((uint) step > 0U)
        tagBuilder.MergeAttribute(nameof (step), step.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString SearchBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "search", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString ColorBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "color", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString DateBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "date", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString MonthBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "month", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString WeekBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "week", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString TimeBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "time", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString DateTimeBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "datetime", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString DateTimeLocalBox(string name, object htmlAttributes = null) => MvcHtmlString.Create(this.BuildInputTag(name, "datetime-local", htmlAttributes).ToString(TagRenderMode.SelfClosing));

    public MvcHtmlString Progress(
      string name,
      string innerText,
      int max = -1,
      int value = -1,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = new TagBuilder("progress");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (value != -1)
        tagBuilder.MergeAttribute(nameof (value), value.ToString());
      if (max != -1)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = innerText;
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Meter(
      string name,
      string innerText,
      double min = -1.0,
      double max = -1.0,
      double value = -1.0,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = new TagBuilder("meter");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (value != -1.0)
        tagBuilder.MergeAttribute(nameof (value), value.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = innerText;
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    private TagBuilder BuildInputTag(
      string name,
      string inputType,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("input");
      tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
      tagBuilder.MergeAttribute("type", inputType);
      string fullHtmlFieldName = this.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      tagBuilder.MergeAttribute(nameof (name), fullHtmlFieldName, true);
      tagBuilder.GenerateId(fullHtmlFieldName);
      return tagBuilder;
    }
  }
}
