// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.API.SearchApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.Extensions;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers.API
{
  [RoutePrefix("api/search")]
  public class SearchApiController : BaseApiController
  {
    private SearchApiController.CustomSearchFunction CustomSearch;

    public SearchApiController(
      IDdocService ddoc,
      ICustomSearch customSearch,
      ILogger<SearchApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
      if (customSearch.Search == null)
        return;
      this.CustomSearch = new SearchApiController.CustomSearchFunction(customSearch.Search.Invoke);
    }

    [HttpPost]
    [Route("global")]
    public async Task<IHttpActionResult> GlobalSearch(
      [FromBody] DdocSearchParameters parameters)
    {
      SearchApiController searchApiController = this;
      if (!searchApiController.ValidateToken(out UserSession _))
        return searchApiController.Unauthorized();
      if (parameters == null)
        return searchApiController.BadRequest("Debe especificar el tipo de grupo");
      List<DdocCollection> result;
      try
      {
        if (searchApiController.CustomSearch != null)
          result = await searchApiController.CustomSearch(parameters);
        else
          result = await searchApiController.Ddoc.GlobalSearch(parameters);
      }
      catch (Exception ex)
      {
        return searchApiController.DDocErrorMessage(ex, (object) new
        {
          parameters = parameters
        });
      }
      return (IHttpActionResult) searchApiController.Ok<ApiResponse<DdocCollection>>(new ApiResponse<DdocCollection>(result));
    }

    [HttpGet]
    [Route("collections/{collectionId}/fieldValues/{fieldId}/{op}/{query}")]
    public async Task<IHttpActionResult> SearchFieldValues(
      [FromUri] string collectionId,
      [FromUri] int fieldId,
      [FromUri] Comparison op,
      [FromUri] string query)
    {
      SearchApiController searchApiController = this;
      if (!searchApiController.ValidateToken(out UserSession _))
        return searchApiController.Unauthorized();
      if (op > Comparison.NotBetween)
        return searchApiController.BadRequest("Operación no reconocida");
      List<string> result;
      try
      {
        result = await searchApiController.Ddoc.SearchFieldValues(collectionId, fieldId, op, query);
      }
      catch (Exception ex)
      {
        return searchApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          fieldId = fieldId,
          op = op,
          query = query
        });
      }
      return (IHttpActionResult) searchApiController.Ok<ApiResponse<string>>(new ApiResponse<string>(result));
    }

    [HttpGet]
    [Route("collections/{collectionId}/values/{fieldId}/{op}/{query}")]
    public async Task<IHttpActionResult> SearchCollectionValues(
      [FromUri] string collectionId,
      [FromUri] int fieldId,
      [FromUri] Comparison op,
      [FromUri] string query)
    {
      SearchApiController searchApiController = this;
      if (!searchApiController.ValidateToken(out UserSession _))
        return searchApiController.Unauthorized();
      if (op > Comparison.NotBetween)
        return searchApiController.BadRequest("Operación no reconocida");
      List<Dictionary<string, object>> result;
      try
      {
        result = await searchApiController.Ddoc.SearchCollectionValues(collectionId, fieldId, op, query);
      }
      catch (Exception ex)
      {
        return searchApiController.DDocErrorMessage(ex, (object) new
        {
          collectionId = collectionId,
          fieldId = fieldId,
          op = op,
          query = query
        });
      }
      return (IHttpActionResult) searchApiController.Ok<ApiResponse<Dictionary<string, object>>>(new ApiResponse<Dictionary<string, object>>(result));
    }

    private delegate Task<List<DdocCollection>> CustomSearchFunction(
      DdocSearchParameters parameters);
  }
}
