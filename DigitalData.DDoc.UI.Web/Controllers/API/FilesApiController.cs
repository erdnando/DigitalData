// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.FileApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Entities.Security;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api/files")]
  public class FileApiController : BaseApiController
  {
    public FileApiController(IDdocService ddoc, ILogger<FileApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }

    [HttpGet]
    [Route("getThumbnail/{pageId:regex([0-9]{2}P[0-9A-V]{7})}/{width:int?}/{height:int?}")]
    public async Task<IHttpActionResult> GetPageThumbnail(
      [FromUri] string pageId,
      [FromUri] int width = 0,
      [FromUri] int height = 0)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      HttpResponseMessage httpResponseMessage = response;
      httpResponseMessage.Content = (HttpContent) new StreamContent(await fileApiController.Ddoc.GetPageThumbnail(pageId, width, height));
      httpResponseMessage = (HttpResponseMessage) null;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("getDocument/{pageId:regex([0-9]{2}P[0-9A-V]{7})}/{pdfPage?}")]
    public async Task<IHttpActionResult> GetDocument(
      [FromUri] string pageId,
      [FromUri] int pdfPage = 0)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      HttpResponseMessage httpResponseMessage = response;
      httpResponseMessage.Content = (HttpContent) new StreamContent(await fileApiController.Ddoc.GetDocumentStream(pageId, pdfPage));
      httpResponseMessage = (HttpResponseMessage) null;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
      response.Headers.Add("Cache-Control", "no-cache");
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("getFile/{pageId:regex([0-9]{2}P[0-9A-V]{7})}")]
    public async Task<IHttpActionResult> GetFile(
      [FromUri] string pageId,
      [FromUri] string pageType = "pdf")
    {
      FileApiController fileApiController = this;
      if (!fileApiController.ValidateToken(out UserSession _))
        throw new HttpResponseException(HttpStatusCode.Unauthorized);
      if (string.IsNullOrEmpty(pageId))
        throw new HttpResponseException(HttpStatusCode.BadRequest);
      if (!await fileApiController.Ddoc.PageIdExists(pageId))
        throw new HttpResponseException(HttpStatusCode.NotFound);
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      HttpResponseMessage httpResponseMessage = response;
      httpResponseMessage.Content = (HttpContent) new StreamContent(await fileApiController.Ddoc.GetFile(pageId, pageType));
      httpResponseMessage = (HttpResponseMessage) null;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Headers.Add("Cache-Control", "no-cache");
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadFile/{pageId:regex([0-9]{2}P[0-9A-V]{7})}/{downloadFormat:regex([a-z]{3,4})}")]
    public async Task<IHttpActionResult> FileDownload(
      [FromUri] string pageId,
      [FromUri] string downloadFormat,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      DDocFileResponse ddocFileResponse = await fileApiController.Ddoc.FileDownload(pageId, downloadFormat);
      response.Content = (HttpContent) new StreamContent(ddocFileResponse.FileByteStream);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = ddocFileResponse.Filename
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Headers.Add("Cache-Control", "no-cache");
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (FileDownload),
        User = user,
        Module = fileApiController.ModuleName,
        Details = pageId
      });
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadPages/{pageId:regex([0-9]{2}P[0-9A-V]{7})}/{downloadFormat:regex([a-z]{3,4})}/{startPage:int}/{endPage:int}")]
    public async Task<IHttpActionResult> PagesDownload(
      [FromUri] string pageId,
      [FromUri] string downloadFormat,
      [FromUri] int startPage,
      [FromUri] int endPage,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      DDocFileResponse ddocFileResponse = await fileApiController.Ddoc.PagesDownload(pageId, downloadFormat, startPage, endPage);
      response.Content = (HttpContent) new StreamContent(ddocFileResponse.FileByteStream);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = ddocFileResponse.Filename
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Headers.Add("Cache-Control", "no-cache");
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (PagesDownload),
        User = user,
        Module = fileApiController.ModuleName,
        Details = string.Format("{0}, pages {1}-{2}", (object) pageId, (object) startPage, (object) endPage)
      });
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadDocument/{documentId:regex([0-9]{2}D[0-9A-V]{7})}/{downloadFormat:regex([a-z]{3,4})}")]
    public async Task<IHttpActionResult> DocumentDownload(
      [FromUri] string documentId,
      [FromUri] string downloadFormat,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      DDocFileResponse ddocFileResponse = await fileApiController.Ddoc.DocumentDownload(documentId, downloadFormat);
      response.Content = (HttpContent) new StreamContent(ddocFileResponse.FileByteStream);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = ddocFileResponse.Filename
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Headers.Add("Cache-Control", "no-cache");
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (DocumentDownload),
        User = user,
        Module = fileApiController.ModuleName,
        Details = documentId
      });
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadSearchResults/{fileId:regex([0-9a-fA-F]{32})}")]
    public async Task<IHttpActionResult> DownloadSearchResults(
      [FromUri] string fileId,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      response.Content = (HttpContent) new FileApiController.TempFileContent(Path.Combine(Path.GetTempPath(), fileId + ".xlsx"));
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = "Resultados_busqueda_Ddoc.xlsx"
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      response.Headers.Add("Cache-Control", "no-cache");
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "ExportSearchResults",
        User = user,
        Module = fileApiController.ModuleName
      });
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadMassDocuments/{fileId:regex([0-9a-fA-F]{32})}")]
    public async Task<IHttpActionResult> DownloadMassDocuments(
      [FromUri] string fileId,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      HttpResponseMessage response = fileApiController.Request.CreateResponse();
      response.Content = (HttpContent) new FileApiController.TempFileContent(Path.Combine(Path.GetTempPath(), fileId + ".zip"));
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = "DescargaMasiva.zip"
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
      response.Headers.Add("Cache-Control", "no-cache");
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (DownloadMassDocuments),
        User = user,
        Module = fileApiController.ModuleName
      });
      IHttpActionResult httpActionResult = (IHttpActionResult) fileApiController.ResponseMessage(response);
      response = (HttpResponseMessage) null;
      return httpActionResult;
    }

    [HttpGet]
    [Route("downloadSearchFilters/{fileId:regex([0-9a-fA-F]{32})}")]
    public async Task<IHttpActionResult> DownloadSearchFilters(
      [FromUri] string fileId,
      [FromUri] string user)
    {
      FileApiController fileApiController = this;
      await fileApiController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "ExportSearchResults",
        User = user,
        Module = fileApiController.ModuleName
      });
      return (IHttpActionResult) new FileApiController.XmlFileResult(fileId, fileApiController.Request);
    }

    private class XmlFileResult : IHttpActionResult
    {
      private readonly string fileId;
      private readonly HttpRequestMessage request;

      public XmlFileResult(string fileId, HttpRequestMessage request)
      {
        this.fileId = fileId;
        this.request = request;
      }

      public Task<HttpResponseMessage> ExecuteAsync(
        CancellationToken cancellationToken)
      {
        HttpResponseMessage response = this.request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) new FileApiController.TempFileContent(Path.Combine(Path.GetTempPath(), this.fileId + ".dfd"));
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = "searchFilters.dfd"
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        response.Headers.Add("Cache-Control", "no-cache");
        return Task.FromResult<HttpResponseMessage>(response);
      }
    }

    private class TempFileContent : StreamContent
    {
      private readonly string tempFilename;

      public TempFileContent(string tempFilename)
        : base((Stream) System.IO.File.OpenRead(tempFilename))
      {
        this.tempFilename = tempFilename;
      }

      protected override void Dispose(bool disposing)
      {
        base.Dispose(disposing);
        if (!disposing)
          return;
        System.IO.File.Delete(this.tempFilename);
      }
    }
  }
}
