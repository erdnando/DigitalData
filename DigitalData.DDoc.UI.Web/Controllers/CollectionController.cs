// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.CollectionController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class CollectionController : DdocController
  {
    public CollectionController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    [NoCache]
    public async Task<ActionResult> GetCollection(string collectionId)
    {
      CollectionController collectionController = this;
      Response<DdocCollection> response1 = new Response<DdocCollection>();
      try
      {
        Response<DdocCollection> response2 = response1;
        response2.ObjectResult = await collectionController.Ddoc.GetCollection(collectionId);
        response2 = (Response<DdocCollection>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocCollection>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetCollections()
    {
      CollectionController collectionController = this;
      Response<DdocCollection> response1 = new Response<DdocCollection>();
      try
      {
        Response<DdocCollection> response2 = response1;
        response2.List = await collectionController.Ddoc.GetCollections();
        response2 = (Response<DdocCollection>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocCollection>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetSearchableCollections()
    {
      CollectionController collectionController = this;
      Response<DdocCollection> response1 = new Response<DdocCollection>();
      try
      {
        Response<DdocCollection> response2 = response1;
        response2.List = await collectionController.Ddoc.GetSearchableCollections(collectionController.CurrentUserSession.DdocGroups);
        response2 = (Response<DdocCollection>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocCollection>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetRulesForParentCollection(string collectionId)
    {
      CollectionController collectionController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.List = await collectionController.Ddoc.GetRulesForParentCollection(collectionId);
        response2 = (Response<DdocRule>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetRulesForParentField(int fieldId)
    {
      CollectionController collectionController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.List = await collectionController.Ddoc.GetRulesForParentField(fieldId);
        response2 = (Response<DdocRule>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetRulesForChildCollection(string collectionId)
    {
      CollectionController collectionController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.List = await collectionController.Ddoc.GetRulesForChildCollection(collectionId);
        response2 = (Response<DdocRule>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetRulesForChildField(int fieldId)
    {
      CollectionController collectionController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.List = await collectionController.Ddoc.GetRulesForChildField(fieldId);
        response2 = (Response<DdocRule>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetFields(string collectionId)
    {
      CollectionController collectionController = this;
      Response<DdocField> response1 = new Response<DdocField>();
      try
      {
        Response<DdocField> response2 = response1;
        response2.List = await collectionController.Ddoc.GetCollectionFields(collectionId);
        response2 = (Response<DdocField>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) collectionController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocField>) null;
      return actionResult;
    }
  }
}
