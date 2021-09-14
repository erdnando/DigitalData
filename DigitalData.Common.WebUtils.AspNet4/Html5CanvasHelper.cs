// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Html5CanvasHelper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class Html5CanvasHelper : IHtml5Extensions
  {
    public Html5CanvasHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
      : this(viewContext, viewDataContainer, RouteTable.Routes)
    {
    }

    public Html5CanvasHelper(
      ViewContext viewContext,
      IViewDataContainer viewDataContainer,
      RouteCollection routeCollection)
    {
      this.ViewContext = viewContext;
      this.ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
    }

    public ViewContext ViewContext { get; set; }

    public ViewDataDictionary ViewData { get; set; }

    public MvcHtmlString EmptyCanvas(string name, string notSupportedMessage) => this.EmptyCanvas(name, notSupportedMessage, (object) null);

    public MvcHtmlString EmptyCanvas(
      string name,
      string notSupportedMessage,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = notSupportedMessage;
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString CanvasRectangle(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      int width,
      int height,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasRect(name, notSupportedMessage, x, y, width, height, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledRectangle(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      int width,
      int height,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasRect(name, notSupportedMessage, x, y, width, height, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Ellipse(
      string name,
      string notSupportedMessage,
      int cx,
      int cy,
      int width,
      int height,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return width == height ? this.CreateCanvasCircle(name, notSupportedMessage, cx, cy, (float) (width / 2), thickness, lineColor, htmlAttributes, DrawMode.Stroke) : this.CreateCanvasEllipse(name, notSupportedMessage, cx, cy, width, height, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledEllipse(
      string name,
      string notSupportedMessage,
      int cx,
      int cy,
      int width,
      int height,
      string fillColor,
      object htmlAttributes)
    {
      return width == height ? this.CreateCanvasCircle(name, notSupportedMessage, cx, cy, (float) (width / 2), 0, fillColor, htmlAttributes, DrawMode.Fill) : this.CreateCanvasEllipse(name, notSupportedMessage, cx, cy, width, height, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Circle(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      float radius,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasCircle(name, notSupportedMessage, x, y, radius, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledCircle(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      float radius,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasCircle(name, notSupportedMessage, x, y, radius, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Arc(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      float radius,
      int thickness,
      float startAngle,
      float endAngle,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasArc(name, notSupportedMessage, x, y, radius, thickness, startAngle, endAngle, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledArc(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      float radius,
      float startAngle,
      float endAngle,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasArc(name, notSupportedMessage, x, y, radius, 0, startAngle, endAngle, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString BeizerCurve(
      string name,
      string notSupportedMessage,
      int x1,
      int y1,
      int x2,
      int y2,
      float radius,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasBeizerCurve(name, notSupportedMessage, x1, y1, x2, y2, radius, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledBeizerCurve(
      string name,
      string notSupportedMessage,
      int x1,
      int y1,
      int x2,
      int y2,
      float radius,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasBeizerCurve(name, notSupportedMessage, x1, y1, x2, y2, radius, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString QuadraticCurve(
      string name,
      string notSupportedMessage,
      int x1,
      int y1,
      int x2,
      int y2,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasQuadraticCurve(name, notSupportedMessage, x1, y1, x2, y2, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledQuadraticCurve(
      string name,
      string notSupportedMessage,
      int x1,
      int y1,
      int x2,
      int y2,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasQuadraticCurve(name, notSupportedMessage, x1, y1, x2, y2, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Polygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasPolygon(name, notSupportedMessage, points, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledPolygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasPolygon(name, notSupportedMessage, points, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Text(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      string text,
      string fontFamily,
      int fontSize,
      string fontStyle,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateCanvasText(name, notSupportedMessage, x, y, text, fontFamily, fontSize, fontStyle, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledText(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      string text,
      string fontFamily,
      int fontSize,
      string fontStyle,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateCanvasText(name, notSupportedMessage, x, y, text, fontFamily, fontSize, fontStyle, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Line(
      string name,
      string notSupportedMessage,
      int x1,
      int y1,
      int x2,
      int y2,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) lineColor);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.moveTo({0},{1});", (object) x1, (object) y1);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.lineTo({0},{1});", (object) x2, (object) y2);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("context.stroke();");
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    public MvcHtmlString Image(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      int width,
      int height,
      string imageUrl,
      object htmlAttributes)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      string contentUrl = UrlHelper.GenerateContentUrl(imageUrl, this.ViewContext.HttpContext);
      stringBuilder1.AppendLine("var imgObj = new Image();");
      stringBuilder1.AppendFormat("imgObj.src = '{0}';", (object) contentUrl);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("imgObj.onload = function () {");
      stringBuilder1.AppendFormat("    context.drawImage(imgObj,{0},{1},{2},{3});", (object) x, (object) y, (object) width, (object) height);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("}");
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasRect(
      string name,
      string notSupportedMessage,
      int xPosition,
      int yPosition,
      int rectWidth,
      int rectHeight,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillRect({0},{1},{2},{3});", (object) xPosition, (object) yPosition, (object) rectWidth, (object) rectHeight);
      else
        stringBuilder1.AppendFormat("context.strokeRect({0},{1},{2},{3});", (object) xPosition, (object) yPosition, (object) rectWidth, (object) rectHeight);
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasEllipse(
      string name,
      string notSupportedMessage,
      int centerX,
      int centerY,
      int width,
      int height,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("context.beginPath();");
      stringBuilder1.AppendFormat("context.moveTo({0}, {1} - {2} / 2);", (object) centerX, (object) centerY, (object) height);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.bezierCurveTo({0} + {1} / 2, {2} - {3} / 2, {0} + {1} / 2, {2} + {3} / 2, {0}, {2} + {3} / 2);", (object) centerX, (object) width, (object) centerY, (object) height);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.bezierCurveTo({0} - {1} / 2, {2} + {3} / 2, {0} - {1} / 2, {2} - {3} / 2, {0}, {2} - {3} / 2);", (object) centerX, (object) width, (object) centerY, (object) height);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      stringBuilder1.AppendLine("context.closePath();");
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasCircle(
      string name,
      string notSupportedMessage,
      int centerX,
      int centerY,
      float radius,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.arc({0}, {1}, {2}, 0, Math.PI * 2, true);", (object) centerX, (object) centerY, (object) radius);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasArc(
      string name,
      string notSupportedMessage,
      int centerX,
      int centerY,
      float radius,
      int thickness,
      float startAngle,
      float endAngle,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.arc({0}, {1}, {2}, {3} * Math.PI/180, {4} * Math.PI/180, false);", (object) centerX, (object) centerY, (object) radius, (object) startAngle, (object) endAngle);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasBeizerCurve(
      string name,
      string notSupportedMessage,
      int startX,
      int startY,
      int endX,
      int endY,
      float radius,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("context.beginPath();");
      stringBuilder1.AppendFormat("context.moveTo({0}, {1});", (object) startX, (object) startY);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.bezierCurveTo({0}, {1}, {2}, {3} , {4}* Math.PI/180, {5} * Math.PI/180);", (object) startX, (object) startY, (object) endX, (object) endY, (object) radius, (object) 200);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      stringBuilder1.AppendLine("context.closePath();");
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasQuadraticCurve(
      string name,
      string notSupportedMessage,
      int startX,
      int startY,
      int endX,
      int endY,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("context.beginPath();");
      stringBuilder1.AppendFormat("context.moveTo({0}, {1});", (object) startX, (object) startY);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("context.quadraticCurveTo({0}, {1}, {2}, {3});", (object) startX, (object) startY, (object) endX, (object) endY);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      stringBuilder1.AppendLine("context.closePath();");
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasPolygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("context.beginPath();");
      string str2 = "";
      foreach (Point point in points)
      {
        string[] strArray = new string[5]
        {
          str2,
          null,
          null,
          null,
          null
        };
        int num = point.X;
        strArray[1] = num.ToString();
        strArray[2] = ",";
        num = point.Y;
        strArray[3] = num.ToString();
        strArray[4] = ",";
        str2 = string.Concat(strArray);
      }
      string str3 = "var points = [" + str2.TrimEnd(',') + "];";
      stringBuilder1.AppendLine(str3);
      stringBuilder1.AppendLine("context.moveTo(points[0], points[1]);");
      stringBuilder1.AppendLine("for( item=2 ; item < points.length-1 ; item+=2 ){ context.lineTo( points[item] , points[item+1] ) }");
      stringBuilder1.AppendLine("context.closePath();");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendLine("context.fill();");
      else
        stringBuilder1.AppendLine("context.stroke();");
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str4 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str4);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }

    private MvcHtmlString CreateCanvasText(
      string name,
      string notSupportedMessage,
      int xPosition,
      int yPosition,
      string text,
      string fontFamily,
      int fontSize,
      string fontStyle,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder tagBuilder1 = new TagBuilder("canvas");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("id", name);
      tagBuilder1.InnerHtml = notSupportedMessage;
      string str1 = tagBuilder1.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder2 = new TagBuilder("script");
      tagBuilder2.MergeAttribute("type", "text/javascript");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine();
      stringBuilder1.AppendFormat("var c = document.getElementById('{0}');", (object) name);
      stringBuilder1.AppendLine();
      stringBuilder1.AppendLine("var context=c.getContext('2d');");
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillStyle = '{0}';", (object) color);
      else
        stringBuilder1.AppendFormat("context.strokeStyle = '{0}';", (object) color);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Stroke)
        stringBuilder1.AppendFormat("context.lineWidth = {0};", (object) thickness);
      stringBuilder1.AppendLine("context.textBaseline = 'top';");
      stringBuilder1.AppendFormat("context.font = '{0} {1}px {2}';", (object) fontStyle, (object) fontSize, (object) fontFamily);
      stringBuilder1.AppendLine();
      if (drawMode == DrawMode.Fill)
        stringBuilder1.AppendFormat("context.fillText('{0}', {1}, {2});", (object) text, (object) xPosition, (object) yPosition);
      else
        stringBuilder1.AppendFormat("context.strokeText('{0}', {1}, {2});", (object) text, (object) xPosition, (object) yPosition);
      stringBuilder1.AppendLine();
      tagBuilder2.InnerHtml = stringBuilder1.ToString();
      string str2 = tagBuilder2.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.AppendLine(str1);
      stringBuilder2.AppendLine(str2);
      return MvcHtmlString.Create(stringBuilder2.ToString());
    }
  }
}
