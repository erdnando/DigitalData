
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  [RoutePrefix("search")]
  public class SearchTestApiController : BaseApiController
  {
    public SearchTestApiController(IDdocService ddoc, ILogger<SearchTestApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }
/*
    [AsyncIteratorStateMachine(typeof (SearchTestApiController.\u003CGlobalSearch2\u003Ed__1))]
    [HttpGet]
    [Route("global/{collectionType}/{textQuery}")]
    public IAsyncEnumerable<DdocChildEntity> GlobalSearch2(
      string textQuery,
      CollectionType collectionType)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<DdocChildEntity>) new SearchTestApiController.\u003CGlobalSearch2\u003Ed__1(-2)
      {
        \u003C\u003E4__this = this,
        \u003C\u003E3__textQuery = textQuery,
        \u003C\u003E3__collectionType = collectionType
      };
    }*/
  }
}
