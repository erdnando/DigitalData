// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Html5Helper`1
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class Html5Helper<TModel> : Html5Helper
  {
    public Html5Helper(ViewContext viewContext, IViewDataContainer container)
      : this(viewContext, container, RouteTable.Routes)
    {
    }

    public Html5Helper(
      ViewContext viewContext,
      IViewDataContainer container,
      RouteCollection routeCollection)
      : base(viewContext, container, routeCollection)
    {
      this.ViewData = new ViewDataDictionary<TModel>(container.ViewData);
    }

    public ViewDataDictionary<TModel> ViewData { get; }

    public MvcHtmlString PlaceholderBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string placeholderText,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "text", htmlAttributes: htmlAttributes);
      tagBuilder.MergeAttribute("placeholder", placeholderText);
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString PlaceholderBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string placeholderText,
      string format,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "text", format, htmlAttributes);
      tagBuilder.MergeAttribute("placeholder", placeholderText);
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString EmailBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "email", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString EmailBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "email", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString UrlBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "url", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString UrlBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "url", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "number", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "number", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", htmlAttributes: htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      string format = null,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", format, htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      double max,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", htmlAttributes: htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      double max,
      string format,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", format, htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      double max,
      double step,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", htmlAttributes: htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if (step != 0.0)
        tagBuilder.MergeAttribute(nameof (step), step.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString NumberBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      double min,
      double max,
      double step,
      string format,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "number", format, htmlAttributes);
      if (min != -1.0)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1.0)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if (step != 0.0)
        tagBuilder.MergeAttribute(nameof (step), step.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString RangeFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      int min = -1,
      int max = -1,
      int step = 0,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = this.BuildInputTag<TProperty>(expression, "range", htmlAttributes: htmlAttributes);
      if (min != -1)
        tagBuilder.MergeAttribute(nameof (min), min.ToString());
      if (max != -1)
        tagBuilder.MergeAttribute(nameof (max), max.ToString());
      if ((uint) step > 0U)
        tagBuilder.MergeAttribute(nameof (step), step.ToString());
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString SearchBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "search", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString ColorBox<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "color", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "date", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "date", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString MonthBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "month", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString MonthBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "month", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString WeekBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "week", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString WeekBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "week", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString TimeBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "time", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString TimeBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "time", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateTimeBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "datetime", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateTimeBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "datetime", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateTimeLocalBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "datetime-local", htmlAttributes: htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    public MvcHtmlString DateTimeLocalBoxFor<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string format,
      object htmlAttributes = null)
    {
      return MvcHtmlString.Create(this.BuildInputTag<TProperty>(expression, "datetime-local", format, htmlAttributes).ToString(TagRenderMode.SelfClosing));
    }

    private TagBuilder BuildInputTag<TProperty>(
      Expression<Func<TModel, TProperty>> expression,
      string inputType,
      string format = null,
      object htmlAttributes = null)
    {
      ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, this.ViewData);
      TagBuilder tagBuilder = new TagBuilder("input");
      tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
      tagBuilder.MergeAttribute("type", inputType);
      string fullHtmlFieldName = this.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText((LambdaExpression) expression));
      tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
      tagBuilder.GenerateId(fullHtmlFieldName);
      FormatValue(modelMetadata.Model);
      ModelState modelState;
      string str = !this.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) || modelState.Value == null ? (string) null : (string) modelState.Value.ConvertTo(typeof (string), (CultureInfo) null);
      tagBuilder.MergeAttribute("value", str ?? Convert.ToString(this.ViewData.Eval(fullHtmlFieldName, format), (IFormatProvider) CultureInfo.CurrentCulture), true);
      if (this.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
        tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
      return tagBuilder;

      string FormatValue(object value)
      {
        if (value == null)
          return string.Empty;
        return string.IsNullOrEmpty(format) ? Convert.ToString(value, (IFormatProvider) CultureInfo.CurrentCulture) : string.Format(format, value);
      }
    }
  }
}
