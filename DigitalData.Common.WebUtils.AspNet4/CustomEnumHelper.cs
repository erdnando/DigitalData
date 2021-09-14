// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.CustomEnumHelper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using DigitalData.Common.EnumUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class CustomEnumHelper
  {
    public static SelectList SelectListFor<T>(T? selected) where T : struct => !selected.HasValue ? CustomEnumHelper.SelectListFor<T>() : CustomEnumHelper.SelectListFor<T>(selected.Value);

    public static SelectList SelectListFor<T>() where T : struct => typeof (T).IsEnum ? new SelectList((IEnumerable) Enum.GetValues(typeof (T)).Cast<Enum>().Select(e => new
    {
      Id = Convert.ToInt32((object) e),
      Name = e.GetDisplayName<Enum>()
    }), "Id", "Name") : (SelectList) null;

    public static SelectList SelectListFor<T>(T selected) where T : struct
    {
      Type enumType = typeof (T);
      return enumType.IsEnum ? new SelectList((IEnumerable) Enum.GetValues(enumType).Cast<Enum>().Select(e => new
      {
        Id = Convert.ToInt32((object) e),
        Name = e.GetDisplayName<Enum>()
      }), "Id", "Name", (object) Convert.ToInt32((object) selected)) : (SelectList) null;
    }

    public static string VersionedContent(this UrlHelper urlHelper, string filename) => UrlHelper.GenerateContentUrl(filename, urlHelper.RequestContext.HttpContext) + (bool.Parse(ConfigurationManager.AppSettings[nameof (VersionedContent)]) ? string.Format("?v={0:MMddHHmm}", (object) DateTime.Now) : string.Empty);

    public static MvcHtmlString Numeric(
      this HtmlHelper htmlHelper,
      string name,
      object htmlAttributes = null)
    {
      return htmlHelper.Numeric(name, (object) null, htmlAttributes);
    }

    public static MvcHtmlString Numeric(
      this HtmlHelper htmlHelper,
      string name,
      object value,
      object htmlAttributes = null)
    {
      return htmlHelper.Numeric(name, value, new int?(), htmlAttributes);
    }

    public static MvcHtmlString Numeric(
      this HtmlHelper htmlHelper,
      string name,
      object value,
      int? min,
      object htmlAttributes = null)
    {
      return htmlHelper.Numeric(name, value, min, new int?(), htmlAttributes);
    }

    public static MvcHtmlString Numeric(
      this HtmlHelper htmlHelper,
      string name,
      object value,
      int? min,
      int? max,
      object htmlAttributes = null)
    {
      return CustomEnumHelper.Numeric(htmlHelper, name, value, min, max, (IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString Numeric(
      this HtmlHelper htmlHelper,
      string name,
      object value,
      int? min,
      int? max,
      IDictionary<string, object> htmlAttributes)
    {
      return CustomEnumHelper.InputHelper(htmlHelper, (ModelMetadata) null, name, value, value == null, true, true, (string) null, min, max, htmlAttributes);
    }

    public static MvcHtmlString NumericFor<TModel, TProperty>(
      this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> expression,
      object htmlAttributes = null)
    {
      return htmlHelper.NumericFor<TModel, TProperty>(expression, new int?(), htmlAttributes);
    }

    public static MvcHtmlString NumericFor<TModel, TProperty>(
      this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> expression,
      int? min,
      object htmlAttributes = null)
    {
      return htmlHelper.NumericFor<TModel, TProperty>(expression, min, new int?(), htmlAttributes);
    }

    public static MvcHtmlString NumericFor<TModel, TProperty>(
      this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> expression,
      int? min,
      int? max,
      object htmlAttributes = null)
    {
      return CustomEnumHelper.NumericFor<TModel, TProperty>(htmlHelper, expression, min, max, (IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NumericFor<TModel, TProperty>(
      this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> expression,
      int? min,
      int? max,
      IDictionary<string, object> htmlAttributes)
    {
      ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
      return htmlHelper.NumericHelper(metadata, metadata.Model, ExpressionHelper.GetExpressionText((LambdaExpression) expression), min, max, htmlAttributes);
    }

    private static MvcHtmlString NumericHelper(
      this HtmlHelper htmlHelper,
      ModelMetadata metadata,
      object model,
      string expression,
      int? min,
      int? max,
      IDictionary<string, object> htmlAttributes)
    {
      return CustomEnumHelper.InputHelper(htmlHelper, metadata, expression, model, false, true, true, (string) null, min, max, htmlAttributes);
    }

    private static MvcHtmlString InputHelper(
      HtmlHelper htmlHelper,
      ModelMetadata metadata,
      string name,
      object value,
      bool useViewData,
      bool setId,
      bool isExplicitValue,
      string format,
      int? min,
      int? max,
      IDictionary<string, object> htmlAttributes)
    {
      string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      TagBuilder tagBuilder = new TagBuilder("input");
      tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      tagBuilder.MergeAttribute("type", "number");
      tagBuilder.MergeAttribute(nameof (name), fullHtmlFieldName, true);
      if (min.HasValue)
        tagBuilder.MergeAttribute(nameof (min), min.Value.ToString());
      if (max.HasValue)
        tagBuilder.MergeAttribute(nameof (max), max.Value.ToString());
      string str1 = htmlHelper.FormatValue(value, format);
      ModelState modelState;
      string str2 = !htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) || modelState.Value == null ? (string) null : (string) modelState.Value.ConvertTo(typeof (string), (CultureInfo) null);
      tagBuilder.MergeAttribute(nameof (value), str2 ?? (useViewData ? Convert.ToString(htmlHelper.ViewData.Eval(fullHtmlFieldName, format), (IFormatProvider) CultureInfo.CurrentCulture) : str1), isExplicitValue);
      if (setId)
        tagBuilder.GenerateId(fullHtmlFieldName);
      if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
        tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
      tagBuilder.MergeAttributes<string, object>(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
    }
  }
}
