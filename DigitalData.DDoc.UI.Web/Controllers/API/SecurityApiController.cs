// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.SecurityApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api/security")]
  public class SecurityApiController : BaseApiController
  {
    public SecurityApiController(IDdocService ddoc, ILogger<SecurityApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }

    [HttpPost]
    [Route("apiLogin")]
    public async Task<IHttpActionResult> Login([FromBody] User userLogin)
    {
      SecurityApiController securityApiController = this;
      if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrEmpty(userLogin.Password))
        return securityApiController.BadRequest("Debe especificar usuario y contraseña");
      UserSession userSession = await securityApiController.Ddoc.Login(userLogin);
      ApiResponse content = new ApiResponse();
      if (userSession.LoginOk)
      {
        content.ApiToken = userSession.ToToken();
      }
      else
      {
        content.Success = false;
        content.Message = userSession.LoginError.ErrorMessage;
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse>(content);
    }

    [HttpGet]
    [Route("groups/{groupType}/{groupId}")]
    public async Task<IHttpActionResult> GetDdocGroup(
      [FromUri] int groupId,
      [FromUri] GroupType groupType)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (groupId == 0)
        return securityApiController.BadRequest("Debe especificar el Id de grupo");
      if (groupType != GroupType.SecurityGroup && groupType != GroupType.UserGroup)
        return securityApiController.BadRequest("Debe especificar el Tipo de grupo");
      DdocGroup ddocGroup;
      try
      {
        ddocGroup = await securityApiController.Ddoc.GetDdocGroup(groupId, groupType);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          groupId = groupId
        });
      }
      return ddocGroup != null ? (IHttpActionResult) securityApiController.Ok<ApiResponse<DdocGroup>>(new ApiResponse<DdocGroup>(ddocGroup)) : securityApiController.NotFound();
    }

    [HttpGet]
    [Route("groups/{groupType}")]
    public async Task<IHttpActionResult> GetDdocGroups([FromUri] GroupType groupType)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (groupType != GroupType.SecurityGroup && groupType != GroupType.UserGroup)
        return securityApiController.BadRequest("Debe especificar el tipo de grupo");
      List<DdocGroup> ddocGroups;
      try
      {
        ddocGroups = await securityApiController.Ddoc.GetDdocGroups(groupType);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          groupType = groupType
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse<DdocGroup>>(new ApiResponse<DdocGroup>(ddocGroups));
    }

    [HttpPost]
    [Route("group")]
    public async Task<IHttpActionResult> SaveDdocGroup([FromBody] DdocGroup group)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (string.IsNullOrEmpty(group.Name) || group.Type == GroupType.Unknown)
        return securityApiController.BadRequest("Debe especificar el nombre del nuevo grupo y su tipo");
      int itemId;
      try
      {
        itemId = await securityApiController.Ddoc.SaveDdocGroup(group);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          group = group
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse>(new ApiResponse(itemId));
    }

    [HttpDelete]
    [Route("groups/{groupType}/{groupId}")]
    public async Task<IHttpActionResult> DeleteDdocGroup(
      [FromUri] int groupId,
      [FromUri] GroupType groupType)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (groupId == 0 || groupType == GroupType.Unknown)
        return securityApiController.BadRequest("Debe especificar el Id del grupo y su tipo");
      if (await securityApiController.Ddoc.GetDdocGroup(groupId, groupType) == null)
        return securityApiController.NotFound();
      try
      {
        await securityApiController.Ddoc.DeleteDdocGroup(groupId, groupType);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          groupId = groupId,
          groupType = groupType
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse>(new ApiResponse());
    }

    [HttpGet]
    [Route("groups/{groupType}/{groupId}/permissions")]
    public async Task<IHttpActionResult> GetDdocGroupPermissions(
      [FromUri] int groupId,
      [FromUri] GroupType groupType)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (groupId == 0 || groupType == GroupType.Unknown)
        return securityApiController.BadRequest();
      if (await securityApiController.Ddoc.GetDdocGroup(groupId, groupType) == null)
        return securityApiController.NotFound();
      List<DdocPermission> groupPermissions;
      try
      {
        groupPermissions = await securityApiController.Ddoc.GetDdocGroupPermissions(groupId, groupType);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          groupId = groupId,
          groupType = groupType
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse<DdocPermission>>(new ApiResponse<DdocPermission>(groupPermissions));
    }

    [HttpGet]
    [Route("permission/{permissionId}")]
    public async Task<IHttpActionResult> GetPermission(int permissionId)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (permissionId == 0)
        return securityApiController.BadRequest();
      DdocPermission permission;
      try
      {
        permission = await securityApiController.Ddoc.GetPermission(permissionId);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          permissionId = permissionId
        });
      }
      return permission != null ? (IHttpActionResult) securityApiController.Ok<ApiResponse<DdocPermission>>(new ApiResponse<DdocPermission>(permission)) : securityApiController.NotFound();
    }

    [HttpPost]
    [Route("permissions/{itemId}/{type}")]
    public async Task<IHttpActionResult> GetElementPermissions(
      [FromUri] CollectionType type,
      [FromUri] string itemId,
      [FromBody] GroupFilters filters)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (string.IsNullOrEmpty(itemId) || type == CollectionType.R || !filters.UserGroupList.Any<char>())
        return securityApiController.BadRequest();
      switch (type)
      {
        case CollectionType.C:
          if (await securityApiController.Ddoc.GetCollection(itemId) == null)
            return securityApiController.NotFound();
          break;
        case CollectionType.F:
          if (!await securityApiController.Ddoc.DocumentIdExists(itemId))
            return securityApiController.NotFound();
          break;
        case CollectionType.D:
          if (!await securityApiController.Ddoc.FolderIdExists(itemId))
            return securityApiController.NotFound();
          break;
      }
      Permissions elementPermissions;
      try
      {
        elementPermissions = await securityApiController.Ddoc.GetElementPermissions(type, itemId, filters);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          type = type,
          itemId = itemId,
          filters = filters
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse<Permissions>>(new ApiResponse<Permissions>(elementPermissions));
    }

    [HttpGet]
    [Route("permissions")]
    public async Task<IHttpActionResult> GetAllPermissions()
    {
      SecurityApiController securityApiController = this;
      //int num;
      if ( !securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      List<DdocPermission> allPermissions;
      try
      {
        allPermissions = await securityApiController.Ddoc.GetAllPermissions();
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex);
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse<DdocPermission>>(new ApiResponse<DdocPermission>(allPermissions));
    }

    [HttpPost]
    [Route("permissions")]
    public async Task<IHttpActionResult> SavePermission(
      [FromBody] DdocPermission permission)
    {
      SecurityApiController securityApiController = this;
      //int num;
      if ( !securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      int itemId;
      try
      {
        itemId = await securityApiController.Ddoc.SavePermission(permission);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          permission = permission
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse>(new ApiResponse(itemId));
    }

    [HttpDelete]
    [Route("permission/{permissionId}")]
    public async Task<IHttpActionResult> DeletePermission([FromUri] int permissionId)
    {
      SecurityApiController securityApiController = this;
      if (!securityApiController.ValidateToken(out UserSession _))
        return securityApiController.Unauthorized();
      if (permissionId == 0)
        return securityApiController.BadRequest("Debe especificar el Id del permiso");
      if (await securityApiController.Ddoc.GetPermission(permissionId) == null)
        return securityApiController.NotFound();
      try
      {
        await securityApiController.Ddoc.DeletePermission(permissionId);
      }
      catch (Exception ex)
      {
        return securityApiController.DDocErrorMessage(ex, (object) new
        {
          permissionId = permissionId
        });
      }
      return (IHttpActionResult) securityApiController.Ok<ApiResponse>(new ApiResponse());
    }
  }
}
