// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Html5MediaHelper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class Html5MediaHelper : IHtml5Extensions
  {
    public Html5MediaHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
      : this(viewContext, viewDataContainer, RouteTable.Routes)
    {
    }

    public Html5MediaHelper(
      ViewContext viewContext,
      IViewDataContainer viewDataContainer,
      RouteCollection routeCollection)
    {
      this.ViewContext = viewContext;
      this.ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
    }

    public ViewContext ViewContext { get; set; }

    public ViewDataDictionary ViewData { get; set; }

    public MvcHtmlString Audio(string name, string source, string notSupportedMessage) => this.Audio(name, source, notSupportedMessage, true, false, false, (object) null);

    public MvcHtmlString Audio(
      string name,
      string source,
      string notSupportedMessage,
      object htmlAttributes)
    {
      return this.Audio(name, source, notSupportedMessage, true, false, false, htmlAttributes);
    }

    public MvcHtmlString Audio(
      string name,
      string source,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop)
    {
      return this.Audio(name, source, notSupportedMessage, showControls, autoPlay, playInLoop, (object) null);
    }

    public MvcHtmlString Audio(
      string name,
      string source,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("audio");
      string contentUrl = UrlHelper.GenerateContentUrl(source, this.ViewContext.HttpContext);
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (showControls)
        tagBuilder.MergeAttribute("controls", "controls");
      if (autoPlay)
        tagBuilder.MergeAttribute("autoplay", "autoplay");
      if (playInLoop)
        tagBuilder.MergeAttribute("loop", "loop");
      tagBuilder.MergeAttribute("src", contentUrl);
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = notSupportedMessage;
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Audio(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage)
    {
      return this.Audio(name, sourceList, notSupportedMessage, true, false, false, (object) null);
    }

    public MvcHtmlString Audio(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      object htmlAttributes)
    {
      return this.Audio(name, sourceList, notSupportedMessage, true, false, false, htmlAttributes);
    }

    public MvcHtmlString Audio(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop)
    {
      return this.Audio(name, sourceList, notSupportedMessage, showControls, autoPlay, playInLoop, (object) null);
    }

    public MvcHtmlString Audio(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("audio");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (showControls)
        tagBuilder.MergeAttribute("controls", "controls");
      if (autoPlay)
        tagBuilder.MergeAttribute("autoplay", "autoplay");
      if (playInLoop)
        tagBuilder.MergeAttribute("loop", "loop");
      tagBuilder.MergeAttribute("id", name);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      foreach (SourceListItem source in sourceList)
        stringBuilder.AppendLine(this.SourceItemToSource(source));
      stringBuilder.AppendLine(notSupportedMessage);
      tagBuilder.InnerHtml = stringBuilder.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Video(string name, string source, string notSupportedMessage) => this.Video(name, source, notSupportedMessage, true, false, false, (object) null);

    public MvcHtmlString Video(
      string name,
      string source,
      string notSupportedMessage,
      object htmlAttributes)
    {
      return this.Video(name, source, notSupportedMessage, true, false, false, htmlAttributes);
    }

    public MvcHtmlString Video(
      string name,
      string source,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop)
    {
      return this.Video(name, source, notSupportedMessage, showControls, autoPlay, playInLoop, (object) null);
    }

    public MvcHtmlString Video(
      string name,
      string source,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("video");
      string contentUrl = UrlHelper.GenerateContentUrl(source, this.ViewContext.HttpContext);
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (showControls)
        tagBuilder.MergeAttribute("controls", "controls");
      if (autoPlay)
        tagBuilder.MergeAttribute("autoplay", "autoplay");
      if (playInLoop)
        tagBuilder.MergeAttribute("loop", "loop");
      tagBuilder.MergeAttribute("src", contentUrl);
      tagBuilder.MergeAttribute("id", name);
      tagBuilder.InnerHtml = notSupportedMessage;
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage)
    {
      return this.Video(name, sourceList, notSupportedMessage, true, false, false, (object) null);
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      object htmlAttributes)
    {
      return this.Video(name, sourceList, notSupportedMessage, true, false, false, htmlAttributes);
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop)
    {
      return this.Video(name, sourceList, notSupportedMessage, showControls, autoPlay, playInLoop, (object) null);
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      string notSupportedMessage,
      bool showControls,
      bool autoPlay,
      bool playInLoop,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("video");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      if (showControls)
        tagBuilder.MergeAttribute("controls", "controls");
      if (autoPlay)
        tagBuilder.MergeAttribute("autoplay", "autoplay");
      if (playInLoop)
        tagBuilder.MergeAttribute("loop", "loop");
      tagBuilder.MergeAttribute("id", name);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      foreach (SourceListItem source in sourceList)
        stringBuilder.AppendLine(this.SourceItemToSource(source));
      stringBuilder.AppendLine(notSupportedMessage);
      tagBuilder.InnerHtml = stringBuilder.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      ObjectType objectType,
      string objectSource)
    {
      return this.Video(name, sourceList, objectType, objectSource, (object) null);
    }

    public MvcHtmlString Video(
      string name,
      IEnumerable<SourceListItem> sourceList,
      ObjectType objectType,
      string objectSource,
      object htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("video");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder.MergeAttribute("id", name);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      foreach (SourceListItem source in sourceList)
        stringBuilder.AppendLine(this.SourceItemToSource(source));
      stringBuilder.AppendLine();
      if (objectType == ObjectType.Flash)
        stringBuilder.AppendLine(this.CreateFlashObject(objectSource, htmlAttributes));
      else
        stringBuilder.AppendLine(this.CreateSilverlightObject(sourceList, objectSource, htmlAttributes));
      tagBuilder.InnerHtml = stringBuilder.ToString();
      stringBuilder.AppendLine();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    private string CreateFlashObject(string flashSource, object htmlAttributes)
    {
      TagBuilder tagBuilder1 = new TagBuilder("object");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      string contentUrl = UrlHelper.GenerateContentUrl(flashSource, this.ViewContext.HttpContext);
      TagBuilder tagBuilder2 = new TagBuilder("param");
      tagBuilder2.MergeAttribute("name", "movie");
      tagBuilder2.MergeAttribute("value", contentUrl);
      string str1 = tagBuilder2.ToString(TagRenderMode.SelfClosing);
      TagBuilder tagBuilder3 = new TagBuilder("embed");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder3.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder3.MergeAttribute("src", contentUrl);
      string str2 = tagBuilder3.ToString(TagRenderMode.Normal);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine(str1);
      stringBuilder.AppendLine(str2);
      tagBuilder1.InnerHtml = stringBuilder.ToString();
      return tagBuilder1.ToString(TagRenderMode.Normal);
    }

    private string CreateSilverlightObject(
      IEnumerable<SourceListItem> sourceList,
      string xapSource,
      object htmlAttributes)
    {
      TagBuilder tagBuilder1 = new TagBuilder("object");
      if (htmlAttributes != null)
      {
        RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
        tagBuilder1.MergeAttributes<string, object>((IDictionary<string, object>) routeValueDictionary);
      }
      tagBuilder1.MergeAttribute("type", "application/x-silverlight-2");
      string contentUrl = UrlHelper.GenerateContentUrl(xapSource, this.ViewContext.HttpContext);
      TagBuilder tagBuilder2 = new TagBuilder("param");
      tagBuilder2.MergeAttribute("name", "source");
      tagBuilder2.MergeAttribute("value", contentUrl);
      string str1 = tagBuilder2.ToString(TagRenderMode.SelfClosing);
      TagBuilder tagBuilder3 = new TagBuilder("param");
      tagBuilder3.MergeAttribute("name", "initParams");
      string str2 = "deferredLoad=true, duration=0, m=" + UrlHelper.GenerateContentUrl(sourceList.Single<SourceListItem>((Func<SourceListItem, bool>) (s => s.Source.ToLower().Contains("mp4"))).Source, this.ViewContext.HttpContext) + ", autostart=false, autohide=true, showembed=true, postid=0";
      tagBuilder3.MergeAttribute("value", str2);
      string str3 = tagBuilder3.ToString(TagRenderMode.SelfClosing);
      TagBuilder tagBuilder4 = new TagBuilder("param");
      tagBuilder4.MergeAttribute("name", "background");
      tagBuilder4.MergeAttribute("value", "#00FFFFFF");
      string str4 = tagBuilder4.ToString(TagRenderMode.SelfClosing);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine(str1);
      stringBuilder.AppendLine(str3);
      stringBuilder.AppendLine(str4);
      tagBuilder1.InnerHtml = stringBuilder.ToString();
      return tagBuilder1.ToString(TagRenderMode.Normal);
    }

    private string SourceItemToSource(SourceListItem sourceItem)
    {
      TagBuilder tagBuilder = new TagBuilder("source");
      tagBuilder.MergeAttribute("type", sourceItem.SourceType);
      tagBuilder.MergeAttribute("src", UrlHelper.GenerateContentUrl(sourceItem.Source, this.ViewContext.HttpContext));
      return tagBuilder.ToString(TagRenderMode.SelfClosing);
    }
  }
}
