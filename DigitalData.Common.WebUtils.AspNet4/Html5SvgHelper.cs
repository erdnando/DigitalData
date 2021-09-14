// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Html5SvgHelper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class Html5SvgHelper : IHtml5Extensions
  {
    public Html5SvgHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
      : this(viewContext, viewDataContainer, RouteTable.Routes)
    {
    }

    public Html5SvgHelper(
      ViewContext viewContext,
      IViewDataContainer viewDataContainer,
      RouteCollection routeCollection)
    {
      this.ViewContext = viewContext;
      this.ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
    }

    public ViewContext ViewContext { get; set; }

    public ViewDataDictionary ViewData { get; set; }

    public MvcHtmlString EmptySvg(string name, string notSupportedMessage) => this.EmptySvg(name, notSupportedMessage, (object) null);

    public MvcHtmlString EmptySvg(
      string name,
      string notSupportedMessage,
      object htmlAttributes)
    {
      return MvcHtmlString.Create(this.CreateSvgTag(name, notSupportedMessage, htmlAttributes).ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Rectangle(
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
      return this.CreateSvgRect(name, notSupportedMessage, x, y, width, height, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
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
      return this.CreateSvgRect(name, notSupportedMessage, x, y, width, height, 0, fillColor, htmlAttributes, DrawMode.Fill);
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
      return width == height ? this.CreateSvgCircle(name, notSupportedMessage, cx, cy, (float) (width / 2), thickness, lineColor, htmlAttributes, DrawMode.Stroke) : this.CreateSvgEllipse(name, notSupportedMessage, cx, cy, width, height, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
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
      return width == height ? this.CreateSvgCircle(name, notSupportedMessage, cx, cy, (float) (width / 2), 0, fillColor, htmlAttributes, DrawMode.Fill) : this.CreateSvgEllipse(name, notSupportedMessage, cx, cy, width, height, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Circle(
      string name,
      string notSupportedMessage,
      int cx,
      int cy,
      float radius,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateSvgCircle(name, notSupportedMessage, cx, cy, radius, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledCircle(
      string name,
      string notSupportedMessage,
      int cx,
      int cy,
      float radius,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateSvgCircle(name, notSupportedMessage, cx, cy, radius, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Polygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateSvgPolygon(name, notSupportedMessage, points, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledPolygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateSvgPolygon(name, notSupportedMessage, points, 0, fillColor, htmlAttributes, DrawMode.Fill);
    }

    public MvcHtmlString Text(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      string text,
      string fontFamily,
      int fontSize,
      string fontWeight,
      string fontStyle,
      string textDecoration,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      return this.CreateSvgText(name, notSupportedMessage, x, y, text, fontFamily, fontSize, fontWeight, fontStyle, textDecoration, thickness, lineColor, htmlAttributes, DrawMode.Stroke);
    }

    public MvcHtmlString FilledText(
      string name,
      string notSupportedMessage,
      int x,
      int y,
      string text,
      string fontFamily,
      int fontSize,
      string fontWeight,
      string fontStyle,
      string textDecoration,
      string fillColor,
      object htmlAttributes)
    {
      return this.CreateSvgText(name, notSupportedMessage, x, y, text, fontFamily, fontSize, fontWeight, fontStyle, textDecoration, 0, fillColor, htmlAttributes, DrawMode.Fill);
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
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("line");
      tagBuilder.MergeAttribute("id", name + "line");
      tagBuilder.MergeAttribute(nameof (x1), x1.ToString());
      tagBuilder.MergeAttribute(nameof (y1), y1.ToString());
      tagBuilder.MergeAttribute(nameof (x2), x2.ToString());
      tagBuilder.MergeAttribute(nameof (y2), y2.ToString());
      tagBuilder.MergeAttribute("stroke", lineColor);
      tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      string str = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString PolyLine(
      string name,
      string notSupportedMessage,
      Point[] points,
      int thickness,
      string lineColor,
      object htmlAttributes)
    {
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("polyline");
      tagBuilder.MergeAttribute("id", name + "Polyline");
      string str1 = "";
      foreach (Point point in points)
      {
        string[] strArray = new string[5]
        {
          str1,
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
        strArray[4] = " ";
        str1 = string.Concat(strArray);
      }
      string str2 = str1.TrimEnd(' ');
      tagBuilder.MergeAttribute(nameof (points), str2);
      tagBuilder.MergeAttribute("fill", "#FFFFFF");
      tagBuilder.MergeAttribute("stroke", lineColor);
      tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      string str3 = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str3, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString(TagRenderMode.Normal));
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
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("image");
      tagBuilder.MergeAttribute("id", name + nameof (Image));
      tagBuilder.MergeAttribute(nameof (x), x.ToString());
      tagBuilder.MergeAttribute(nameof (y), y.ToString());
      tagBuilder.MergeAttribute(nameof (width), width.ToString());
      tagBuilder.MergeAttribute(nameof (height), height.ToString());
      string contentUrl = UrlHelper.GenerateContentUrl(imageUrl, this.ViewContext.HttpContext);
      tagBuilder.MergeAttribute("xlink:href", contentUrl);
      string str = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString(TagRenderMode.Normal));
    }

    private MvcHtmlString CreateSvgRect(
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
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("rect");
      tagBuilder.MergeAttribute("id", name + "Rect");
      tagBuilder.MergeAttribute("x", xPosition.ToString());
      tagBuilder.MergeAttribute("y", yPosition.ToString());
      tagBuilder.MergeAttribute("width", rectWidth.ToString());
      tagBuilder.MergeAttribute("height", rectHeight.ToString());
      if (drawMode == DrawMode.Fill)
      {
        tagBuilder.MergeAttribute("fill", color);
      }
      else
      {
        tagBuilder.MergeAttribute("fill", "#FFFFFF");
        tagBuilder.MergeAttribute("stroke", color);
        tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      }
      string str = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString());
    }

    private MvcHtmlString CreateSvgEllipse(
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
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("ellipse");
      tagBuilder.MergeAttribute("id", name + "Ellipse");
      tagBuilder.MergeAttribute("cx", centerX.ToString());
      tagBuilder.MergeAttribute("cy", centerY.ToString());
      tagBuilder.MergeAttribute("rx", (width / 2).ToString());
      tagBuilder.MergeAttribute("ry", (height / 2).ToString());
      if (drawMode == DrawMode.Fill)
      {
        tagBuilder.MergeAttribute("fill", color);
      }
      else
      {
        tagBuilder.MergeAttribute("fill", "#FFFFFF");
        tagBuilder.MergeAttribute("stroke", color);
        tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      }
      string str = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString());
    }

    private MvcHtmlString CreateSvgCircle(
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
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("circle");
      tagBuilder.MergeAttribute("id", name + "Circle");
      tagBuilder.MergeAttribute("cx", centerX.ToString());
      tagBuilder.MergeAttribute("cy", centerY.ToString());
      tagBuilder.MergeAttribute("r", radius.ToString());
      if (drawMode == DrawMode.Fill)
      {
        tagBuilder.MergeAttribute("fill", color);
      }
      else
      {
        tagBuilder.MergeAttribute("fill", "#FFFFFF");
        tagBuilder.MergeAttribute("stroke", color);
        tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      }
      string str = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString());
    }

    private MvcHtmlString CreateSvgPolygon(
      string name,
      string notSupportedMessage,
      Point[] points,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder("polygon");
      tagBuilder.MergeAttribute("id", name + "Polygon");
      string str1 = "";
      foreach (Point point in points)
      {
        string[] strArray = new string[5]
        {
          str1,
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
        strArray[4] = " ";
        str1 = string.Concat(strArray);
      }
      string str2 = str1.TrimEnd(' ');
      tagBuilder.MergeAttribute(nameof (points), str2);
      if (drawMode == DrawMode.Fill)
      {
        tagBuilder.MergeAttribute("fill", color);
      }
      else
      {
        tagBuilder.MergeAttribute("fill", "#FFFFFF");
        tagBuilder.MergeAttribute("stroke", color);
        tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      }
      string str3 = tagBuilder.ToString(TagRenderMode.SelfClosing);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str3, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString());
    }

    private MvcHtmlString CreateSvgText(
      string name,
      string notSupportedMessage,
      int xPosition,
      int yPosition,
      string text,
      string fontFamily,
      int fontSize,
      string fontWeight,
      string fontStyle,
      string textDecoration,
      int thickness,
      string color,
      object htmlAttributes,
      DrawMode drawMode)
    {
      TagBuilder svgTag = this.CreateSvgTag(name, notSupportedMessage, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder(nameof (text));
      tagBuilder.MergeAttribute("id", name + "Text");
      tagBuilder.MergeAttribute("x", xPosition.ToString());
      tagBuilder.MergeAttribute("y", yPosition.ToString());
      tagBuilder.MergeAttribute("text-anchor", "middle");
      tagBuilder.MergeAttribute("text-decoration", textDecoration);
      tagBuilder.MergeAttribute("font-style", fontStyle);
      tagBuilder.MergeAttribute("font-weight", fontWeight);
      tagBuilder.MergeAttribute("font-size", fontSize.ToString() + "px");
      tagBuilder.MergeAttribute("font-family", fontFamily);
      if (drawMode == DrawMode.Fill)
      {
        tagBuilder.MergeAttribute("fill", color);
      }
      else
      {
        tagBuilder.MergeAttribute("fill", "#FFFFFF");
        tagBuilder.MergeAttribute("stroke", color);
        tagBuilder.MergeAttribute("stroke-width", thickness.ToString() + "px");
      }
      tagBuilder.InnerHtml = text;
      string str = tagBuilder.ToString(TagRenderMode.Normal);
      svgTag.InnerHtml = string.Format("{0}{1}{0}{2}", (object) Environment.NewLine, (object) str, (object) svgTag.InnerHtml);
      return MvcHtmlString.Create(svgTag.ToString());
    }

    private TagBuilder CreateSvgTag(
      string name,
      string notSupportedMessage,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("svg");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder.MergeAttribute("xmlns", "http://www.w3.org/2000/svg");
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = notSupportedMessage;
      return tagBuilder;
    }
  }
}
