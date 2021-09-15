// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.DocumentController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Common.IO;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Extensions;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class DocumentController : DdocController
  {
    private readonly DocumentController.DocumentPreProcessorFunction PreProcessDocuments;
    private readonly DocumentController.NewDocumentFunction NewDocumentTrigger;

    public DocumentController(
      IDdocService ddoc,
      IOptions<DiDaSettings> settings,
      IDocumentCreation newDocumentTrigger,
      IDocumentPreProcessor documentPreProcessor)
      : base(ddoc, settings.Value)
    {
      if (documentPreProcessor.PreProcessDocuments != null)
        this.PreProcessDocuments = new DocumentController.DocumentPreProcessorFunction(documentPreProcessor.PreProcessDocuments.Invoke);
      if (newDocumentTrigger.TriggerAction == null)
        return;
      this.NewDocumentTrigger = new DocumentController.NewDocumentFunction(newDocumentTrigger.TriggerAction.Invoke);
    }

    public async Task<ActionResult> New(string collectionId)
    {
      DocumentController documentController = this;
      DdocCollection collection = await documentController.Ddoc.GetCollection(collectionId);
      List<DdocField> collectionFields = await documentController.Ddoc.GetCollectionFields(collectionId);
      DdocDocument ddocDocument1 = new DdocDocument();
      ddocDocument1.CollectionId = collectionId;
      ddocDocument1.Name = collection.Name;
      ddocDocument1.SecurityGroupId = collection.SecurityGroupId;
      ddocDocument1.Data = collectionFields;
      ddocDocument1.IsNew = true;
      DdocDocument newDocument = ddocDocument1;
      DdocDocument ddocDocument = newDocument;
      ddocDocument.Id = await documentController.Ddoc.RegisterNewDocument(collectionId, collection.SecurityGroupId);
      ddocDocument = (DdocDocument) null;
      if (documentController.NewDocumentTrigger != null)
        await documentController.NewDocumentTrigger(newDocument);
      documentController.ViewData["DocumentId"] = (object) newDocument.Id;
      documentController.ViewData["CollectionName"] = (object) collection.Name;
      documentController.ViewData["EnableWebTwain"] = (object) documentController.Ddoc.InstanceFeatures.WebTwain;
      ViewDataDictionary viewDataDictionary = documentController.ViewData;
      viewDataDictionary["SecurityGroups"] = (object) await documentController.Ddoc.GetSecurityGroups(documentController.CurrentUserSession.DdocGroups);
      viewDataDictionary = (ViewDataDictionary) null;
      ActionResult actionResult = (ActionResult) documentController.PartialView("NewDocumentEditor", (object) newDocument);
      collection = (DdocCollection) null;
      newDocument = (DdocDocument) null;
      return actionResult;
    }

    public async Task<ActionResult> RegisterNew(
      string collectionId,
      string apiToken)
    {
      DocumentController documentController = this;
      DdocCollection collection = await documentController.Ddoc.GetCollection(collectionId);
      List<DdocField> collectionFields = await documentController.Ddoc.GetCollectionFields(collectionId);
      DdocDocument ddocDocument1 = new DdocDocument();
      ddocDocument1.CollectionId = collectionId;
      ddocDocument1.Name = collection.Name;
      ddocDocument1.SecurityGroupId = collection.SecurityGroupId;
      ddocDocument1.Data = collectionFields;
      ddocDocument1.IsNew = true;
      DdocDocument newDocument = ddocDocument1;
      DdocDocument ddocDocument = newDocument;
      ddocDocument.Id = await documentController.Ddoc.RegisterNewDocument(collectionId, collection.SecurityGroupId);
      ddocDocument = (DdocDocument) null;
      if (documentController.NewDocumentTrigger != null)
        await documentController.NewDocumentTrigger(newDocument);
      documentController.ViewData["DocumentId"] = (object) newDocument.Id;
      documentController.ViewData["CollectionName"] = (object) collection.Name;
      documentController.ViewData["EnableWebTwain"] = (object) documentController.Ddoc.InstanceFeatures.WebTwain;
      ViewDataDictionary viewDataDictionary = documentController.ViewData;
      viewDataDictionary["SecurityGroups"] = (object) await documentController.Ddoc.GetSecurityGroups(documentController.CurrentUserSession.DdocGroups);
      viewDataDictionary = (ViewDataDictionary) null;
      documentController.ViewData["ApiToken"] = (object) apiToken;
      ActionResult actionResult = (ActionResult) documentController.View("SNewDocumentEditor", (object) newDocument);
      collection = (DdocCollection) null;
      newDocument = (DdocDocument) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string documentId)
    {
      DocumentController documentController = this;
      DdocDocument document = await documentController.Ddoc.GetDocument(documentId);
      ViewDataDictionary viewDataDictionary = documentController.ViewData;
      viewDataDictionary["CollectionName"] = (object) (await documentController.Ddoc.GetCollection(document.CollectionId)).Name;
      viewDataDictionary = (ViewDataDictionary) null;
      viewDataDictionary = documentController.ViewData;
      viewDataDictionary["SecurityGroups"] = (object) await documentController.Ddoc.GetSecurityGroups(documentController.CurrentUserSession.DdocGroups);
      viewDataDictionary = (ViewDataDictionary) null;
      viewDataDictionary = documentController.ViewData;
      IDdocService ddoc = documentController.Ddoc;
      string id = document.Id;
      viewDataDictionary["SecurityPermissions"] = (object) await ddoc.GetElementPermissions(CollectionType.D, id, new GroupFilters()
      {
        UserGroupList = documentController.CurrentUserSession.UserGroups,
        DdocGroupList = documentController.CurrentUserSession.DdocGroups
      });
      viewDataDictionary = (ViewDataDictionary) null;
      documentController.ViewData["UseNativeViewer"] = (object) documentController.Settings.GetSetting<bool>("UseNativePdfViewer");
      ActionResult actionResult = (ActionResult) documentController.PartialView("DocumentEditor", (object) document);
      document = (DdocDocument) null;
      return actionResult;
    }

    [HttpPost]
    public PartialViewResult EditPages(DdocDocument document)
    {
      this.ViewData["EnableWebTwain"] = (object) this.Ddoc.InstanceFeatures.WebTwain;
      return this.PartialView("DocumentPagesEditor", (object) document);
    }

    [HttpPost]
    public PartialViewResult Scan(DdocDocument document)
    {
      this.ViewData["Username"] = (object) this.CurrentUserSession.Username;
      return this.PartialView("ScannerUpload", (object) document);
    }

    [HttpPost]
    public PartialViewResult FileUpload(DdocDocument document) => this.PartialView((object) document);

    [NoCache]
    public async Task<ActionResult> GetDocument(string documentId)
    {
      DocumentController documentController = this;
      Response<DdocDocument> response1 = new Response<DdocDocument>();
      try
      {
        Response<DdocDocument> response2 = response1;
        response2.ObjectResult = await documentController.Ddoc.GetDocument(documentId);
        response2 = (Response<DdocDocument>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocDocument>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> CountDocuments(string collectionId)
    {
      DocumentController documentController = this;
      Response<DdocFolder> response = new Response<DdocFolder>();
      try
      {
        response.Total = (long) await documentController.Ddoc.CountDocuments(collectionId, documentController.CurrentUserSession.DdocGroups);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response<DdocFolder>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetDocuments(
      string collectionId,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0)
    {
      DocumentController documentController = this;
      Response<DdocDocument> response = new Response<DdocDocument>();
      try
      {
        List<DdocDocument> documents = await documentController.Ddoc.GetDocuments(collectionId, documentController.CurrentUserSession.DdocGroups, page, pageSize, sortBy, sortDirection);
        if (documentController.PreProcessDocuments != null)
          documents = await documentController.PreProcessDocuments(documents);
        response.List = documents;
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      JsonResult jsonResult = documentController.Json((object) response, JsonRequestBehavior.AllowGet);
      jsonResult.MaxJsonLength = new int?(int.MaxValue);
      ActionResult actionResult = (ActionResult) jsonResult;
      response = (Response<DdocDocument>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetPages(string documentId)
    {
      DocumentController documentController = this;
      Response<DdocPage> response1 = new Response<DdocPage>();
      try
      {
        Response<DdocPage> response2 = response1;
        response2.List = await documentController.Ddoc.GetPages(documentId);
        response2 = (Response<DdocPage>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocPage>) null;
      return actionResult;
    }

    public async Task<ActionResult> CreatePage(string documentId, string extension)
    {
      DocumentController documentController = this;
      Response response1 = new Response();
      try
      {
        Response response2 = response1;
        response2.TextResult = await documentController.Ddoc.CreatePage(documentId, extension);
        response2 = (Response) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      JsonResult jsonResult = documentController.Json((object) response1, JsonRequestBehavior.AllowGet);
      jsonResult.MaxJsonLength = new int?(int.MaxValue);
      ActionResult actionResult = (ActionResult) jsonResult;
      response1 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> UploadPages(
      string documentId,
      int sequence = 0,
      string pageId = null)
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        foreach (string file1 in (NameObjectCollectionBase) documentController.Request.Files)
        {
          HttpPostedFileBase file2 = documentController.Request.Files[file1];
          if (file2 != null && file2.ContentLength > 0)
          {
            byte[] end = file2.InputStream.QuickReadToEnd();
            (string, int) valueTuple = await documentController.Ddoc.UploadPage(documentId, Path.GetExtension(file1).Trim('.'), end, pageId, sequence, documentController.CurrentUserSession.Username);
          }
        }
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "DocumentPageAdd",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = documentId
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> ScannerUpload()
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        string documentId = documentController.HttpContext.Request["documentId"];
        string pageId = documentController.HttpContext.Request["pageId"];
        string str = documentController.HttpContext.Request["sequence"];
        int sequence = int.Parse(str == null ? "0" : str);
        foreach (string file1 in (NameObjectCollectionBase) documentController.Request.Files)
        {
          HttpPostedFileBase file2 = documentController.Request.Files[file1];
          if (file2 != null && file2.ContentLength > 0)
          {
            byte[] end = file2.InputStream.QuickReadToEnd();
            (string, int) valueTuple = await documentController.Ddoc.UploadPage(documentId, Path.GetExtension(file2.FileName).Trim('.'), end, pageId, sequence, documentController.CurrentUserSession.Username);
          }
        }
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "DocumentPageAdd",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = documentId
        });
        response.Result = RequestResult.Success;
        documentId = (string) null;
        pageId = (string) null;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) new HttpStatusCodeResult(HttpStatusCode.OK);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> RegisterNewDocument(
      string collectionId,
      int securityGroupId)
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        string documentId = await documentController.Ddoc.RegisterNewDocument(collectionId, securityGroupId);
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (RegisterNewDocument),
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = collectionId
        });
        response.TextResult = documentId;
        response.Result = RequestResult.Success;
        documentId = (string) null;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> Commit(DdocDocument document)
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        await documentController.Ddoc.CommitDocument(document.Id, document.Name, document.Data);
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "CommitDocument",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = document.Id
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> Save(DdocDocument document)
    {
      DocumentController documentController = this;
      Response response1 = new Response();
      bool setting = documentController.Settings.GetSetting<bool>("UpdateDateOnWeb");
      Response response2 = response1;
      response2.TextResult = await documentController.Ddoc.SaveDocument(document, setting);
      response2 = (Response) null;
      if (document.IsNew)
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "NewDocument",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = response1.TextResult
        });
      else
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "UpdateDocument",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = document.Id
        });
      response1.Result = RequestResult.Success;
      ActionResult actionResult = (ActionResult) documentController.Json((object) response1);
      response1 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> Delete(string documentId)
    {
      DocumentController documentController = this;
      Response response1 = new Response();
      Response response2 = response1;
      response2.Code = await documentController.Ddoc.DeleteDocument(documentId, documentController.CurrentUserSession.Username);
      Response response3 = response1;
      response2 = (Response) null;
      response1 = (Response) null;
      await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "DeleteDocument",
        User = documentController.CurrentUserSession.Username,
        Module = documentController.ModuleName,
        Details = documentId
      });
      response3.Result = RequestResult.Success;
      ActionResult actionResult = (ActionResult) documentController.Json((object) response3);
      response3 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> UpdatePagesSequence(DdocDocument document)
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        await documentController.Ddoc.UpdatePagesSequence(document);
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "ChangePageOrder",
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = document.Id
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> ChangeDocumentCollection(
      string documentId,
      CollectionChangeRequest request,
      int? securityGroupId = null)
    {
      DocumentController documentController = this;
      Response response = new Response();
      try
      {
        await documentController.Ddoc.ChangeDocumentCollection(documentId, request, securityGroupId);
        await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (ChangeDocumentCollection),
          User = documentController.CurrentUserSession.Username,
          Module = documentController.ModuleName,
          Details = documentId
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) documentController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeletePage(string pageId)
    {
      DocumentController documentController = this;
      Response response1 = new Response();
      Response response2 = response1;
      response2.Code = await documentController.Ddoc.DeletePage(pageId, documentController.CurrentUserSession.Username);
      Response response3 = response1;
      response2 = (Response) null;
      response1 = (Response) null;
      await documentController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (DeletePage),
        User = documentController.CurrentUserSession.Username,
        Module = documentController.ModuleName,
        Details = pageId
      });
      response3.Result = RequestResult.Success;
      ActionResult actionResult = (ActionResult) documentController.Json((object) response3);
      response3 = (Response) null;
      return actionResult;
    }

    private delegate Task<List<DdocDocument>> DocumentPreProcessorFunction(
      List<DdocDocument> documents);

    private delegate Task NewDocumentFunction(DdocDocument document);
  }
}
