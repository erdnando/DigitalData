
using DigitalData.Common.Cfdi;
using DigitalData.Common.IO;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.Extensions;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api")]
  public class DocumentApiController : BaseApiController
  {
    private readonly DocumentApiController.DocumentPreProcessorFunction PreProcessDocuments;

    public DocumentApiController(
      IDdocService ddoc,
      ILogger<DocumentApiController> logger,
      IDocumentPreProcessor documentPreProcessor)
      : base(ddoc, (ILogger) logger)
    {
      if (documentPreProcessor.PreProcessDocuments == null)
        return;
      this.PreProcessDocuments = new DocumentApiController.DocumentPreProcessorFunction(documentPreProcessor.PreProcessDocuments.Invoke);
    }

    [HttpGet]
    [Route("document/{documentId}/exists")]
    public async Task<IHttpActionResult> DocumentIdExists([FromUri] string documentId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId))
        return documentApiController.BadRequest();
      bool flag;
      try
      {
        flag = await documentApiController.Ddoc.DocumentIdExists(documentId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Exists = new bool?(flag)
      });
    }

    [HttpGet]
    [Route("page/{pageId}/exists")]
    public async Task<IHttpActionResult> PageIdExists([FromUri] string pageId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(pageId))
        return documentApiController.BadRequest();
      bool flag;
      try
      {
        flag = await documentApiController.Ddoc.PageIdExists(pageId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          pageId = pageId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Exists = new bool?(flag)
      });
    }

    [HttpGet]
    [Route("document/{documentId}")]
    public async Task<IHttpActionResult> GetDocument([FromUri] string documentId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId))
        return documentApiController.BadRequest();
      DdocDocument document;
      try
      {
        document = await documentApiController.Ddoc.GetDocument(documentId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId
        });
      }
      return document != null ? (IHttpActionResult) documentApiController.Ok<ApiResponse<DdocDocument>>(new ApiResponse<DdocDocument>(document)) : documentApiController.NotFound();
    }

    [HttpGet]
    [Route("collection/{collectionId}/documents/count")]
    public async Task<IHttpActionResult> CountDocuments([FromUri] string collectionId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      IEnumerable<string> headerValues;
      if (string.IsNullOrEmpty(collectionId) || !documentApiController.Request.Headers.TryGetValues("X-Ddoc-SecurityGroupsCsv", out headerValues))
        return documentApiController.BadRequest();
      if (await documentApiController.Ddoc.GetCollection(collectionId) == null)
        return documentApiController.NotFound();
      int num;
      try
      {
        num = await documentApiController.Ddoc.CountDocuments(collectionId, headerValues.First<string>());
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Count = new int?(num)
      });
    }

    [HttpGet]
    [Route("collection/{collectionId}/documents")]
    public async Task<IHttpActionResult> GetDocuments(
      [FromUri] string collectionId,
      [FromUri] int page = 1,
      [FromUri] int pageSize = 17,
      [FromUri] string sortBy = null,
      [FromUri] int sortDirection = 0)
    {
      DocumentApiController documentApiController = this;
      UserSession userSession;
      if (!documentApiController.ValidateToken(out userSession))
        return documentApiController.Unauthorized();
      string securityGroupsCsv = userSession.DdocGroups;
      IEnumerable<string> values;
      if (documentApiController.Request.Headers.TryGetValues("X-Ddoc-SecurityGroupsCsv", out values))
        securityGroupsCsv = values.First<string>();
      List<DdocDocument> ddocDocumentList;
      try
      {
        ddocDocumentList = await documentApiController.Ddoc.GetDocuments(collectionId, securityGroupsCsv, page, pageSize, sortBy, sortDirection);
        if (documentApiController.PreProcessDocuments != null)
          ddocDocumentList = await documentApiController.PreProcessDocuments(ddocDocumentList);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          page = page,
          pageSize = pageSize,
          sortBy = sortBy,
          sortDirection = sortDirection
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse<DdocDocument>>(new ApiResponse<DdocDocument>(ddocDocumentList));
    }

    [HttpGet]
    [Route("document/{documentId}/pages")]
    public async Task<IHttpActionResult> GetPages([FromUri] string documentId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound("No existe el documento " + documentId);
      List<DdocPage> pages;
      try
      {
        pages = await documentApiController.Ddoc.GetPages(documentId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse<DdocPage>>(new ApiResponse<DdocPage>(pages));
    }

    [HttpPost]
    [Route("document/{documentId}/page/{extension}")]
    public async Task<IHttpActionResult> CreatePage(
      [FromUri] string documentId,
      [FromUri] string extension,
      [FromUri] int sequence = 0,
      [FromUri] bool replaced = false)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(extension))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      string page;
      try
      {
        page = await documentApiController.Ddoc.CreatePage(documentId, extension, sequence, replaced);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          extension = extension,
          sequence = sequence,
          replaced = replaced
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse(page));
    }

    [HttpPost]
    [Route("document/{documentId}/page/upload/{fileExt}")]
    public async Task<IHttpActionResult> UploadPage(
      [FromUri] string documentId,
      [FromUri] string fileExt,
      [FromUri] string pageId = null,
      [FromUri] int sequence = 0)
    {
      DocumentApiController documentApiController = this;
      HttpRequest httpRequest = HttpContext.Current.Request;
      UserSession userSession;
      if (!documentApiController.ValidateToken(out userSession))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(fileExt) || httpRequest.Files.Count != 1)
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      HttpPostedFile file = httpRequest.Files[0];
      string itemGid;
      int num;
      try
      {
        (itemGid, num) = await documentApiController.Ddoc.UploadPage(documentId, fileExt, file.InputStream.QuickReadToEnd(), pageId, sequence, userSession.Username);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          fileExt = fileExt,
          pageId = pageId,
          sequence = sequence
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse(itemGid)
      {
        Count = new int?(num)
      });
    }

    [HttpPost]
    [Route("document/{documentId}/page/fetch/{fileExt}")]
    public async Task<IHttpActionResult> UploadLocalPage(
      [FromUri] string documentId,
      [FromUri] string fileExt,
      [FromUri] string pageId = null,
      [FromUri] int sequence = 0)
    {
      DocumentApiController documentApiController = this;
      UserSession userSession;
      if (!documentApiController.ValidateToken(out userSession))
        return documentApiController.Unauthorized();
      IEnumerable<string> headerValues;
      if (string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(fileExt) || !documentApiController.Request.Headers.TryGetValues("X-Ddoc-FetchFilePath", out headerValues))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      string itemGid;
      int num;
      try
      {
        (itemGid, num) = await documentApiController.Ddoc.UploadLocalPage(documentId, fileExt, headerValues.First<string>(), pageId, sequence, userSession.Username);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          fileExt = fileExt,
          pageId = pageId,
          sequence = sequence
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse(itemGid)
      {
        Count = new int?(num)
      });
    }

    [HttpPost]
    [Route("document/registerNew")]
    public async Task<IHttpActionResult> RegisterNew(
      [FromUri] string collectionId,
      [FromUri] int securityGroupId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(collectionId) || securityGroupId == 0)
        return documentApiController.BadRequest();
      string itemGid;
      try
      {
        itemGid = await documentApiController.Ddoc.RegisterNewDocument(collectionId, securityGroupId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          securityGroupId = securityGroupId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse(itemGid));
    }

    [HttpPost]
    [Route("document/commit")]
    public async Task<IHttpActionResult> Commit(
      [FromBody] DdocDocument document,
      [FromUri] bool updateDate = true,
      [FromUri] string creationDate = null)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(document.Id) || document.Data == null || !document.Data.Any<DdocField>())
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(document.Id))
        return documentApiController.NotFound();
      try
      {
        await documentApiController.Ddoc.CommitDocument(document.Id, document.Name, document.Data, updateDate, string.IsNullOrEmpty(creationDate) ? new DateTime?() : new DateTime?(DateTime.ParseExact(creationDate, "yyyy-MM-ddTHH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture)));
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          document = document
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPost]
    [Route("document")]
    public async Task<IHttpActionResult> SaveDocument(
      [FromBody] DdocDocument document,
      [FromUri] bool updateDate = true)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (document.Data == null || !document.Data.Any<DdocField>() || (string.IsNullOrEmpty(document.CollectionId) || document.SecurityGroupId == 0))
        return documentApiController.BadRequest();
      string itemGid;
      try
      {
        itemGid = await documentApiController.Ddoc.SaveDocument(document, updateDate);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          document = document,
          updateDate = updateDate
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse(itemGid));
    }

    [HttpPost]
    [Route("document/parentCollections/{collectionId}")]
    public async Task<IHttpActionResult> PopulateParents(
      [FromUri] string collectionId,
      [FromBody] List<DdocField> childFields)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(collectionId) || childFields == null || !childFields.Any<DdocField>())
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.CollectionExists(collectionId))
        return documentApiController.NotFound();
      try
      {
        await documentApiController.Ddoc.PopulateParents(collectionId, childFields);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          childFields = childFields
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPatch]
    [Route("document/{documentId}")]
    public async Task<IHttpActionResult> UpdateDocumentData(
      [FromUri] string documentId,
      [FromBody] List<DdocField> documentData,
      [FromUri] bool updateDate = true)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId) || documentData == null || !documentData.Any<DdocField>())
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      try
      {
        await documentApiController.Ddoc.UpdateDocumentData(documentId, documentData, updateDate);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          documentData = documentData,
          updateDate = updateDate
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPatch]
    [Route("document/{documentId}/security/{securityGroupId}")]
    public async Task<IHttpActionResult> UpdateDocumentSecurity(
      [FromUri] string documentId,
      [FromUri] int securityGroupId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId) || securityGroupId == 0)
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      try
      {
        await documentApiController.Ddoc.UpdateDocumentSecurity(documentId, securityGroupId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          securityGroupId = securityGroupId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpDelete]
    [Route("document/{documentId}")]
    public async Task<IHttpActionResult> DeleteDocument([FromUri] string documentId)
    {
      DocumentApiController documentApiController = this;
      UserSession userSession;
      if (!documentApiController.ValidateToken(out userSession))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      try
      {
        int num = await documentApiController.Ddoc.DeleteDocument(documentId, userSession.Username);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPatch]
    [Route("document/pageSequence")]
    public async Task<IHttpActionResult> UpdatePagesSequence(
      [FromBody] DdocDocument document)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (document.Pages == null || !document.Pages.Any<DdocPage>())
        return documentApiController.BadRequest();
      try
      {
        await documentApiController.Ddoc.UpdatePagesSequence(document);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          document = document
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPatch]
    [Route("document/{documentId}/colection")]
    public async Task<IHttpActionResult> ChangeDocumentCollection(
      [FromUri] string documentId,
      [FromBody] CollectionChangeRequest request,
      [FromUri] int? securityGroupId = null)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(request.CollectionId) || (request.FieldLinks == null || !request.FieldLinks.Any<FieldCorrespondence>()))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.DocumentIdExists(documentId))
        return documentApiController.NotFound();
      try
      {
        await documentApiController.Ddoc.ChangeDocumentCollection(documentId, request, securityGroupId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          documentId = documentId,
          request = request,
          securityGroupId = securityGroupId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpDelete]
    [Route("document/page/{pageId}")]
    public async Task<IHttpActionResult> DeletePage([FromUri] string pageId)
    {
      DocumentApiController documentApiController = this;
      UserSession userSession;
      if (!documentApiController.ValidateToken(out userSession))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(pageId))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.PageIdExists(pageId))
        return documentApiController.NotFound();
      try
      {
        int num = await documentApiController.Ddoc.DeletePage(pageId, userSession.Username);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          pageId = pageId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpGet]
    [Route("document/page/{pageId}/imageCount")]
    public async Task<IHttpActionResult> GetImageCount([FromUri] string pageId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(pageId))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.PageIdExists(pageId))
        return documentApiController.NotFound();
      int pdfPageCount;
      try
      {
        pdfPageCount = await documentApiController.Ddoc.GetPdfPageCount(pageId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          pageId = pageId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Count = new int?(pdfPageCount)
      });
    }

    [HttpGet]
    [Route("document/page/{pageId}/path")]
    public async Task<IHttpActionResult> GetPath(
      [FromUri] string pageId,
      [FromUri] string pageType = null)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(pageId))
        return documentApiController.BadRequest();
      if (!await documentApiController.Ddoc.PageIdExists(pageId))
        return documentApiController.NotFound();
      string path;
      try
      {
        path = await documentApiController.Ddoc.GetPath(pageId, pageType);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          pageId = pageId,
          pageType = pageType
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Text = path
      });
    }

    [HttpGet]
    [Route("document/page/{pageId}/cfdiData")]
    public async Task<IHttpActionResult> GetCfdData([FromUri] string pageId)
    {
      DocumentApiController documentApiController = this;
      if (!documentApiController.ValidateToken(out UserSession _))
        return documentApiController.Unauthorized();
      if (string.IsNullOrEmpty(pageId))
        return documentApiController.BadRequest();
      CfdiData cfdData;
      try
      {
        cfdData = await documentApiController.Ddoc.GetCfdData(pageId);
      }
      catch (Exception ex)
      {
        return documentApiController.DDocErrorMessage(ex, (object) new
        {
          pageId = pageId
        });
      }
      return (IHttpActionResult) documentApiController.Ok<ApiResponse<CfdiData>>(new ApiResponse<CfdiData>(cfdData));
    }

    private delegate Task<List<DdocDocument>> DocumentPreProcessorFunction(
      List<DdocDocument> documents);
  }
}
