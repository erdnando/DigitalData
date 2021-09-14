// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.FolderApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Api;
using DigitalData.DDoc.Common.Entities.Security;
using DigitalData.DDoc.Common.Extensions;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api")]
  public class FolderApiController : BaseApiController
  {
    private FolderApiController.FolderPreProcessorFunction FolderPreProcessor;

    public FolderApiController(
      IDdocService ddoc,
      IFolderPreProcessor folderPreProcessor,
      ILogger<FolderApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
      if (folderPreProcessor.PreProcessFolders == null)
        return;
      this.FolderPreProcessor = new FolderApiController.FolderPreProcessorFunction(folderPreProcessor.PreProcessFolders.Invoke);
    }

    [HttpGet]
    [Route("folder/{folderId}/exists")]
    public async Task<IHttpActionResult> FolderIdExists([FromUri] string folderId)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (string.IsNullOrEmpty(folderId))
        return folderApiController.BadRequest();
      bool flag;
      try
      {
        flag = await folderApiController.Ddoc.FolderIdExists(folderId);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folderId = folderId
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Exists = new bool?(flag)
      });
    }

    [HttpGet]
    [Route("folder/{folderId}")]
    public async Task<IHttpActionResult> GetFolder([FromUri] string folderId)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (string.IsNullOrEmpty(folderId))
        return folderApiController.BadRequest();
      DdocFolder folder;
      try
      {
        folder = await folderApiController.Ddoc.GetFolder(folderId);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folderId = folderId
        });
      }
      return folder != null ? (IHttpActionResult) folderApiController.Ok<ApiResponse<DdocFolder>>(new ApiResponse<DdocFolder>(folder)) : folderApiController.NotFound();
    }

    [HttpGet]
    [Route("collection/{collectionId}/folders/count")]
    public async Task<IHttpActionResult> CountFolders([FromUri] string collectionId)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      IEnumerable<string> headerValues;
      if (string.IsNullOrEmpty(collectionId) || !folderApiController.Request.Headers.TryGetValues("X-Ddoc-SecurityGroupsCsv", out headerValues))
        return folderApiController.BadRequest();
      if (await folderApiController.Ddoc.GetCollection(collectionId) == null)
        return folderApiController.NotFound();
      int num;
      try
      {
        num = await folderApiController.Ddoc.CountFolders(collectionId, headerValues.First<string>());
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse()
      {
        Count = new int?(num)
      });
    }

    [HttpGet]
    [Route("collection/{collectionId}/folders")]
    public async Task<IHttpActionResult> GetFolders(
      [FromUri] string collectionId,
      [FromUri] int page = 1,
      [FromUri] int pageSize = 17,
      [FromUri] string sortBy = null,
      [FromUri] int sortDirection = 0)
    {
      FolderApiController folderApiController = this;
      UserSession userSession;
      if (!folderApiController.ValidateToken(out userSession))
        return folderApiController.Unauthorized();
      string securityGroupsCsv = userSession.DdocGroups;
      IEnumerable<string> values;
      if (folderApiController.Request.Headers.TryGetValues("X-Ddoc-SecurityGroupsCsv", out values))
        securityGroupsCsv = values.First<string>();
      List<DdocFolder> ddocFolderList;
      try
      {
        ddocFolderList = await folderApiController.Ddoc.GetFolders(collectionId, securityGroupsCsv, page, pageSize, sortBy, sortDirection);
        if (folderApiController.FolderPreProcessor != null)
          ddocFolderList = await folderApiController.FolderPreProcessor(ddocFolderList);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          page = page,
          pageSize = pageSize,
          sortBy = sortBy,
          sortDirection = sortDirection
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse<DdocFolder>>(new ApiResponse<DdocFolder>(ddocFolderList));
    }

    [HttpPost]
    [Route("folder")]
    public async Task<IHttpActionResult> SaveFolder([FromBody] DdocFolder folder)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (folder.Data == null || !folder.Data.Any<DdocField>() || (string.IsNullOrEmpty(folder.CollectionId) || folder.SecurityGroupId == 0))
        return folderApiController.BadRequest();
      string itemGid;
      try
      {
        itemGid = await folderApiController.Ddoc.SaveFolder(folder);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folder = folder
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse(itemGid));
    }

    [HttpPatch]
    [Route("folder/{folderId}")]
    public async Task<IHttpActionResult> UpdateFolderData(
      [FromUri] string folderId,
      [FromBody] List<DdocField> folderData)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (string.IsNullOrEmpty(folderId) || folderData == null || !folderData.Any<DdocField>())
        return folderApiController.BadRequest();
      if (!await folderApiController.Ddoc.FolderIdExists(folderId))
        return folderApiController.NotFound();
      try
      {
        await folderApiController.Ddoc.UpdateFolderData(folderId, folderData);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folderId = folderId,
          folderData = folderData
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpPatch]
    [Route("folder/{folderId}/security/{securityGroupId}")]
    public async Task<IHttpActionResult> UpdateFolderSecurity(
      [FromUri] string folderId,
      [FromUri] int securityGroupId)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (string.IsNullOrEmpty(folderId) || securityGroupId == 0)
        return folderApiController.BadRequest();
      if (!await folderApiController.Ddoc.FolderIdExists(folderId))
        return folderApiController.NotFound();
      try
      {
        await folderApiController.Ddoc.UpdateFolderSecurity(folderId, securityGroupId);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folderId = folderId,
          securityGroupId = securityGroupId
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpDelete]
    [Route("folder/{folderId}")]
    public async Task<IHttpActionResult> DeleteFolder(
      [FromUri] string folderId,
      [FromUri] string user)
    {
      FolderApiController folderApiController = this;
      if (!folderApiController.ValidateToken(out UserSession _))
        return folderApiController.Unauthorized();
      if (string.IsNullOrEmpty(folderId))
        return folderApiController.BadRequest();
      if (!await folderApiController.Ddoc.FolderIdExists(folderId))
        return folderApiController.NotFound();
      try
      {
        int num = await folderApiController.Ddoc.DeleteFolder(folderId, user);
      }
      catch (Exception ex)
      {
        return folderApiController.DDocErrorMessage(ex, (object) new
        {
          folderId = folderId,
          user = user
        });
      }
      return (IHttpActionResult) folderApiController.Ok<ApiResponse>(new ApiResponse());
    }

    private delegate Task<List<DdocFolder>> FolderPreProcessorFunction(
      List<DdocFolder> folders);
  }
}
