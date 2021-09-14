// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.CatalogController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Catalogs;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class CatalogController : DdocController
  {
    public CatalogController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public JsonResult GetFilterOperatorList()
    {
      Response<KeyValuePair<int, string>> response = new Response<KeyValuePair<int, string>>();
      try
      {
        response.List = DdocCatalogs.GetFilterOperatorList().ToList<KeyValuePair<int, string>>();
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      return this.Json((object) response, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetFilterComparisonList(int fieldType)
    {
      Response<KeyValuePair<int, string>> response = new Response<KeyValuePair<int, string>>();
      try
      {
        response.List = DdocCatalogs.GetFilterComparisonList((FieldType) fieldType).ToList<KeyValuePair<int, string>>();
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      return this.Json((object) response, JsonRequestBehavior.AllowGet);
    }
  }
}
