// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.SearchTestApiController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.WebExtensions;
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
