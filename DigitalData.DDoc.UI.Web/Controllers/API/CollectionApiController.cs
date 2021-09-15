// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.CollectionApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Api;
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
  [RoutePrefix("api/collections")]
  public class CollectionApiController : BaseApiController
  {
    public CollectionApiController(IDdocService ddoc, ILogger<CollectionApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }

    [HttpGet]
    [Route("{collectionId}")]
    public async Task<IHttpActionResult> GetCollection([FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (string.IsNullOrEmpty(collectionId))
        return collectionApiController.BadRequest();
      DdocCollection collection;
      try
      {
        collection = await collectionApiController.Ddoc.GetCollection(collectionId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return collection != null ? (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(collection)) : collectionApiController.NotFound();
    }

    [HttpGet]
    [Route("search/{name}")]
    public async Task<IHttpActionResult> SearchCollection([FromUri] string name)
    {
      CollectionApiController collectionApiController = this;
      int num;
      if ( !collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      List<DdocCollection> result;
      try
      {
        result = await collectionApiController.Ddoc.SearchCollection(name);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          name = name
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(result));
    }

    [HttpGet]
    [Route("")]
    public async Task<IHttpActionResult> GetCollections()
    {
      CollectionApiController collectionApiController = this;
     // int num;
      if ( !collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      List<DdocCollection> collections;
      try
      {
        collections = await collectionApiController.Ddoc.GetCollections();
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex);
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(collections));
    }

    [HttpGet]
    [Route("fields/{fieldId}")]
    public async Task<IHttpActionResult> GetCollectionField([FromUri] int fieldId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (fieldId == 0)
        return collectionApiController.BadRequest();
      DdocField collectionField;
      try
      {
        collectionField = await collectionApiController.Ddoc.GetCollectionField(fieldId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          fieldId = fieldId
        });
      }
      return collectionField != null ? (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocField>>(new ApiResponse<DdocField>(collectionField)) : collectionApiController.NotFound();
    }

    [HttpGet]
    [Route("{collectionId}/fields")]
    public async Task<IHttpActionResult> GetCollectionFields(
      [FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!await collectionApiController.Ddoc.CollectionExists(collectionId))
        return collectionApiController.NotFound("No existe la colección " + collectionId);
      List<DdocField> collectionFields;
      try
      {
        collectionFields = await collectionApiController.Ddoc.GetCollectionFields(collectionId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocField>>(new ApiResponse<DdocField>(collectionFields));
    }

    [HttpGet]
    [Route("{collectionId}/type")]
    public async Task<IHttpActionResult> GetCollectionType([FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (string.IsNullOrEmpty(collectionId))
        return collectionApiController.BadRequest();
      DdocCollection collection;
      try
      {
        collection = await collectionApiController.Ddoc.GetCollection(collectionId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return collection != null ? (IHttpActionResult) collectionApiController.Ok<ApiResponse<CollectionType>>(new ApiResponse<CollectionType>(collection.Type)) : collectionApiController.NotFound();
    }

    [HttpGet]
    [Route("searchable")]
    public async Task<IHttpActionResult> GetSearchableCollections(
      [FromUri] string collectionType = null)
    {
      CollectionApiController collectionApiController = this;
      UserSession userSession;
      if (!collectionApiController.ValidateToken(out userSession))
        return collectionApiController.Unauthorized();
      string securityGroupsCsv = userSession.DdocGroups;
      IEnumerable<string> values;
      if (collectionApiController.Request.Headers.TryGetValues("X-Ddoc-SecurityGroupsCsv", out values))
        securityGroupsCsv = values.First<string>();
      List<DdocCollection> searchableCollections;
      try
      {
        searchableCollections = await collectionApiController.Ddoc.GetSearchableCollections(securityGroupsCsv, collectionType);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionType = collectionType
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(searchableCollections));
    }

    [HttpGet]
    [Route("root")]
    public async Task<IHttpActionResult> GetRootCollections()
    {
      CollectionApiController collectionApiController = this;
      UserSession userSession;
      if (!collectionApiController.ValidateToken(out userSession))
        return collectionApiController.Unauthorized();
      string userGroupsCsv = userSession.UserGroups;
      IEnumerable<string> values;
      if (collectionApiController.Request.Headers.TryGetValues("X-Ddoc-UserGroupsCsv", out values))
        userGroupsCsv = values.First<string>();
      List<DdocCollection> rootCollections;
      try
      {
        rootCollections = await collectionApiController.Ddoc.GetRootCollections(userGroupsCsv);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex);
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(rootCollections));
    }

    [HttpGet]
    [Route("{collectionId}/parents")]
    public async Task<IHttpActionResult> GetParentCollections(
      [FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      UserSession userSession;
      if (!collectionApiController.ValidateToken(out userSession))
        return collectionApiController.Unauthorized();
      string userGroupsCsv = userSession.UserGroups;
      IEnumerable<string> values;
      if (collectionApiController.Request.Headers.TryGetValues("X-Ddoc-UserGroupsCsv", out values))
        userGroupsCsv = values.First<string>();
      List<DdocCollection> parentCollections;
      try
      {
        parentCollections = await collectionApiController.Ddoc.GetParentCollections(collectionId, userGroupsCsv);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(parentCollections));
    }

    [HttpGet]
    [Route("{collectionId}/children")]
    public async Task<IHttpActionResult> GetChildCollections(
      [FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      UserSession userSession;
      if (!collectionApiController.ValidateToken(out userSession))
        return collectionApiController.Unauthorized();
      string userGroupsCsv = userSession.UserGroups;
      IEnumerable<string> values;
      if (collectionApiController.Request.Headers.TryGetValues("X-Ddoc-UserGroupsCsv", out values))
        userGroupsCsv = values.First<string>();
      List<DdocCollection> childCollections;
      try
      {
        childCollections = await collectionApiController.Ddoc.GetChildCollections(collectionId, userGroupsCsv);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(childCollections));
    }

    [HttpGet]
    [Route("getId/{collectionName}/{collectionType}")]
    public async Task<IHttpActionResult> GetCollectionId(
      [FromUri] string collectionName,
      [FromUri] char collectionType)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!string.IsNullOrEmpty(collectionName))
      {
        if (((IEnumerable<char>) new char[3]
        {
          'C',
          'D',
          'F'
        }).Contains<char>(collectionType))
        {
          string collectionId;
          try
          {
            collectionId = await collectionApiController.Ddoc.GetCollectionId(collectionName, collectionType);
          }
          catch (Exception ex)
          {
            return collectionApiController.DDocErrorMessage(ex, (object) new
            {
              collectionName = collectionName,
              collectionType = collectionType
            });
          }
          return !string.IsNullOrEmpty(collectionId) ? (IHttpActionResult) collectionApiController.Ok<ApiResponse>(new ApiResponse(collectionId)) : collectionApiController.NotFound();
        }
      }
      return collectionApiController.BadRequest();
    }

    [HttpGet]
    [Route("getSecurityGroupId/{collectionName}/{collectionType}")]
    public async Task<IHttpActionResult> GetSecurityGroupId(
      [FromUri] string collectionName,
      [FromUri] char collectionType)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!string.IsNullOrEmpty(collectionName))
      {
        if (((IEnumerable<char>) new char[3]
        {
          'C',
          'D',
          'F'
        }).Contains<char>(collectionType))
        {
          int securityGroupId;
          try
          {
            securityGroupId = await collectionApiController.Ddoc.GetSecurityGroupId(collectionName, collectionType);
          }
          catch (Exception ex)
          {
            return collectionApiController.DDocErrorMessage(ex, (object) new
            {
              collectionName = collectionName,
              collectionType = collectionType
            });
          }
          return securityGroupId != 0 ? (IHttpActionResult) collectionApiController.Ok<ApiResponse>(new ApiResponse(securityGroupId)) : collectionApiController.NotFound();
        }
      }
      return collectionApiController.BadRequest();
    }

    [HttpGet]
    [Route("rules")]
    public async Task<IHttpActionResult> GetCollectionRules()
    {
      CollectionApiController collectionApiController = this;
      //int num;
      if ( !collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      List<DdocRule> collectionRules;
      try
      {
        collectionRules = await collectionApiController.Ddoc.GetCollectionRules();
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex);
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocRule>>(new ApiResponse<DdocRule>(collectionRules));
    }

    [HttpGet]
    [Route("rules/child/{collectionId}")]
    public async Task<IHttpActionResult> GetRulesForChildCollection(
      [FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!await collectionApiController.Ddoc.CollectionExists(collectionId))
        return collectionApiController.NotFound("No existe la colección " + collectionId);
      List<DdocRule> forChildCollection;
      try
      {
        forChildCollection = await collectionApiController.Ddoc.GetRulesForChildCollection(collectionId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocRule>>(new ApiResponse<DdocRule>(forChildCollection));
    }

    [HttpGet]
    [Route("rules/parent/{collectionId}")]
    public async Task<IHttpActionResult> GetRulesForParentCollection(
      [FromUri] string collectionId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!await collectionApiController.Ddoc.CollectionExists(collectionId))
        return collectionApiController.NotFound("No existe la colección " + collectionId);
      List<DdocRule> parentCollection;
      try
      {
        parentCollection = await collectionApiController.Ddoc.GetRulesForParentCollection(collectionId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocRule>>(new ApiResponse<DdocRule>(parentCollection));
    }

    [HttpGet]
    [Route("rules/childField/{fieldId}")]
    public async Task<IHttpActionResult> GetRulesForChildField([FromUri] int fieldId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!await collectionApiController.Ddoc.CollectionFieldExists(fieldId))
        return collectionApiController.NotFound(string.Format("No existe el campo {0}", (object) fieldId));
      List<DdocRule> rulesForChildField;
      try
      {
        rulesForChildField = await collectionApiController.Ddoc.GetRulesForChildField(fieldId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          fieldId = fieldId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocRule>>(new ApiResponse<DdocRule>(rulesForChildField));
    }

    [HttpGet]
    [Route("rules/parentField/{fieldId}")]
    public async Task<IHttpActionResult> GetRulesForParentField([FromUri] int fieldId)
    {
      CollectionApiController collectionApiController = this;
      if (!collectionApiController.ValidateToken(out UserSession _))
        return collectionApiController.Unauthorized();
      if (!await collectionApiController.Ddoc.CollectionFieldExists(fieldId))
        return collectionApiController.NotFound(string.Format("No existe el campo {0}", (object) fieldId));
      List<DdocRule> rulesForParentField;
      try
      {
        rulesForParentField = await collectionApiController.Ddoc.GetRulesForParentField(fieldId);
      }
      catch (Exception ex)
      {
        return collectionApiController.DDocErrorMessage(ex, (object) new
        {
          fieldId = fieldId
        });
      }
      return (IHttpActionResult) collectionApiController.Ok<ApiResponse<DdocRule>>(new ApiResponse<DdocRule>(rulesForParentField));
    }
  }
}
