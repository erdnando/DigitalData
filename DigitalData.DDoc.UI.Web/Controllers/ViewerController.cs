// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.ViewerController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Cfdi;
using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Extensions;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class ViewerController : DdocController
  {
    public ViewerController(
      IDdocService ddoc,
      IOptions<DiDaSettings> settings,
      ICustomActions customActions)
      : base(ddoc, settings.Value, customActions.CustomActions)
    {
    }

    public async Task<ActionResult> OpenViewer(
      string documentId,
      string file = null,
      string hideNavigation = null,
      string pn = null,
      string search = null,
      string nw = null,
      string apiToken = null)
    {
      ViewerController viewerController = this;
      DdocDocument document = await viewerController.Ddoc.GetDocument(documentId);
      DdocCollection collection = await viewerController.Ddoc.GetCollection(document.CollectionId);
      await viewerController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "OpenDocument",
        User = viewerController.CurrentUserSession.Username,
        Module = viewerController.ModuleName,
        Details = documentId
      });
      if (!collection.Cfdi)
        return viewerController.Settings.GetSetting<bool>("UseNativePdfViewer") ? (ActionResult) viewerController.RedirectToAction("OpenPdf", (object) new
        {
          documentId = documentId,
          file = file,
          hideNavigation = hideNavigation,
          nw = nw,
          apiToken = apiToken
        }) : (ActionResult) viewerController.RedirectToAction("OpenDocument", (object) new
        {
          documentId = documentId,
          file = file,
          pn = pn,
          search = search,
          nw = nw,
          apiToken = apiToken
        });
      viewerController.TempData["CollectionId"] = (object) collection.Id;
      return (ActionResult) viewerController.RedirectToAction("OpenCfdi", (object) new
      {
        documentId = documentId,
        nw = nw
      });
    }

    [NoCache]
    public async Task<ActionResult> OpenPdf(
      string documentId,
      string file,
      string hideNavigation)
    {
      ViewerController viewerController = this;
      Permissions permissions = await viewerController.Ddoc.GetElementPermissions(CollectionType.D, documentId, new GroupFilters()
      {
        UserGroupList = viewerController.CurrentUserSession.UserGroups,
        DdocGroupList = viewerController.CurrentUserSession.DdocGroups
      });
      if (permissions.Equals((object) Permissions.None))
        viewerController.ViewData["NoPermission"] = (object) true;
      DdocDocument document = await viewerController.Ddoc.GetDocument(documentId);
      DdocDocument ddocDocument = document;
      ddocDocument.Pages = await viewerController.Ddoc.GetPages(documentId);
      ddocDocument = (DdocDocument) null;
      viewerController.ViewData["CanRead"] = (object) permissions.HasFlag((Enum) Permissions.Read);
      viewerController.ViewData["CanWrite"] = (object) permissions.HasFlag((Enum) Permissions.Write);
      viewerController.ViewData["CanComment"] = (object) permissions.HasFlag((Enum) Permissions.Comment);
      viewerController.ViewData["CanExport"] = (object) permissions.HasFlag((Enum) Permissions.Export);
      viewerController.ViewData["CanPrint"] = (object) permissions.HasFlag((Enum) Permissions.Print);
      viewerController.ViewData["CanDelete"] = (object) permissions.HasFlag((Enum) Permissions.Delete);
      if (!string.IsNullOrEmpty(file))
        viewerController.ViewData["FileIndex"] = (object) file;
      if (!string.IsNullOrEmpty(hideNavigation))
        viewerController.ViewData["HideNavigation"] = (object) hideNavigation;
      viewerController.ViewData["ShowCustomPanel"] = (object) viewerController.Settings.GetSetting<bool>("ViewerCustomPanel");
      ActionResult actionResult = (ActionResult) viewerController.View("NativePdfViewer", (object) document);
      document = (DdocDocument) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> OpenDocument(string documentId, string file = null)
    {
      ViewerController viewerController = this;
      viewerController.ViewData["DocumentId"] = (object) documentId;
      Permissions permissions = await viewerController.Ddoc.GetElementPermissions(CollectionType.D, documentId, new GroupFilters()
      {
        UserGroupList = viewerController.CurrentUserSession.UserGroups,
        DdocGroupList = viewerController.CurrentUserSession.DdocGroups
      });
      if (permissions.Equals((object) Permissions.None))
        viewerController.ViewData["NoPermission"] = (object) true;
      DdocDocument document = await viewerController.Ddoc.GetDocument(documentId);
      DdocDocument ddocDocument = document;
      ddocDocument.Pages = await viewerController.Ddoc.GetPages(documentId);
      ddocDocument = (DdocDocument) null;
      viewerController.ViewData["CanRead"] = (object) permissions.HasFlag((Enum) Permissions.Read);
      viewerController.ViewData["CanWrite"] = (object) permissions.HasFlag((Enum) Permissions.Write);
      viewerController.ViewData["CanComment"] = (object) permissions.HasFlag((Enum) Permissions.Comment);
      viewerController.ViewData["CanExport"] = (object) permissions.HasFlag((Enum) Permissions.Export);
      viewerController.ViewData["CanPrint"] = (object) permissions.HasFlag((Enum) Permissions.Print);
      viewerController.ViewData["CanDelete"] = (object) permissions.HasFlag((Enum) Permissions.Delete);
      viewerController.ViewData["EnablePrint"] = (object) Convert.ToInt32(permissions.HasFlag((Enum) Permissions.Print));
      viewerController.ViewData["EnableDownload"] = (object) Convert.ToInt32(permissions.HasFlag((Enum) Permissions.Print));
      viewerController.ViewData["EnableText"] = (object) 1;
      if (!string.IsNullOrEmpty(file))
        viewerController.ViewData["FileIndex"] = (object) file;
      viewerController.ViewData["ShowCustomPanel"] = (object) viewerController.Settings.GetSetting<bool>("ViewerCustomPanel");
      ActionResult actionResult = (ActionResult) viewerController.View("DDocViewer", (object) document);
      document = (DdocDocument) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> OpenCfdi(string documentId)
    {
      ViewerController viewerController = this;
      viewerController.ViewData["DocumentId"] = (object) documentId;
      Permissions elementPermissions = await viewerController.Ddoc.GetElementPermissions(CollectionType.D, documentId, new GroupFilters()
      {
        UserGroupList = viewerController.CurrentUserSession.UserGroups,
        DdocGroupList = viewerController.CurrentUserSession.DdocGroups
      });
      if (elementPermissions.Equals((object) Permissions.None))
        viewerController.ViewData["NoPermission"] = (object) true;
      viewerController.ViewData["CanRead"] = (object) elementPermissions.HasFlag((Enum) Permissions.Read);
      viewerController.ViewData["CanWrite"] = (object) elementPermissions.HasFlag((Enum) Permissions.Write);
      viewerController.ViewData["CanComment"] = (object) elementPermissions.HasFlag((Enum) Permissions.Comment);
      viewerController.ViewData["CanExport"] = (object) elementPermissions.HasFlag((Enum) Permissions.Export);
      viewerController.ViewData["CanPrint"] = (object) elementPermissions.HasFlag((Enum) Permissions.Print);
      viewerController.ViewData["CanDelete"] = (object) elementPermissions.HasFlag((Enum) Permissions.Delete);
      DdocDocument document = await viewerController.Ddoc.GetDocument(documentId);
      DdocDocument ddocDocument = document;
      ddocDocument.Pages = await viewerController.Ddoc.GetPages(documentId);
      ddocDocument = (DdocDocument) null;
      CfdiData cfdData = await viewerController.Ddoc.GetCfdData(document.Pages[0].Id);
      viewerController.ViewData["DocumentFiles"] = (object) document.Pages;
      viewerController.ViewData["CollectionId"] = (object) document.CollectionId;
      viewerController.ViewData["ShowCustomPanel"] = (object) viewerController.Settings.GetSetting<bool>("ViewerCustomPanel");
      ActionResult actionResult = (ActionResult) viewerController.View("CfdiViewer", (object) cfdData);
      document = (DdocDocument) null;
      return actionResult;
    }

    public PartialViewResult DownloadMode(string fileType)
    {
      this.ViewData["FileType"] = (object) fileType;
      return this.PartialView();
    }

    public PartialViewResult FileDownloadMode(string fileType)
    {
      this.ViewData["FileType"] = (object) fileType;
      return this.PartialView();
    }

    public PartialViewResult PagesDownloadMode() => this.PartialView();

    public PartialViewResult DocumentDownloadMode() => this.PartialView();

    public PartialViewResult CfdiDownload() => this.PartialView();

    public ActionResult Resources()
    {
      this.Response.ContentType = "text/javascript";
      return (ActionResult) this.View();
    }

    [NoCache]
    public async Task<ActionResult> GetPdfPageCount(string pageId)
    {
      ViewerController viewerController = this;
      Response response = new Response();
      try
      {
        response.Total = (long) await viewerController.Ddoc.GetPdfPageCount(pageId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) viewerController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetPagePath(string pageId)
    {
      ViewerController viewerController = this;
      Response response = new Response();
      try
      {
        response.TextResult = await viewerController.Ddoc.GetPath(pageId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) viewerController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetCfdData(string pageId)
    {
      ViewerController viewerController = this;
      Response<CfdiData> response = new Response<CfdiData>();
      try
      {
        response.ObjectResult = await viewerController.Ddoc.GetCfdData(pageId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) viewerController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response<CfdiData>) null;
      return actionResult;
    }
  }
}
