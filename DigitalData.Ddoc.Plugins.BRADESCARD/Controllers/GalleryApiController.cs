// Decompiled with JetBrains decompiler
// Type: DigitalData.Ddoc.Plugins.GalleryApiController
// Assembly: DigitalData.Ddoc.Plugins.BRADESCARD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 76226255-9B51-4DC8-9719-6E02C225B847
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.Ddoc.Plugins.BRADESCARD.dll

using DigitalData.Common.Conversions;
using DigitalData.Common.IoC;
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

namespace DigitalData.Ddoc.Plugins
{
  [RoutePrefix("api/gallery")]
  public class GalleryApiController : BaseApiController
  {
    public GalleryApiController(IDdocService ddoc, ILogger<GalleryApiController> logger)
      : base(ddoc, (ILogger) logger)
    {
    }

    [HttpPost]
    [Route("expNumber/{expNumber}")]
    public async Task<IHttpActionResult> GetDocuments(string expNumber)
    {
      GalleryApiController galleryApiController = this;
      if (!galleryApiController.ValidateToken(out UserSession _))
        return galleryApiController.Unauthorized();
      if (string.IsNullOrEmpty(expNumber))
        return galleryApiController.BadRequest();
      List<ExpMember> results = new List<ExpMember>();
      try
      {
        GalleryDAL dataAccess = IoCContainer.GetService<GalleryDAL>();
        string folderId;
        try
        {
          folderId = await dataAccess.GetFolderId(expNumber);
        }
        finally
        {
          dataAccess?.Dispose();
        }
        dataAccess = (GalleryDAL) null;
        DdocFolder folder = await galleryApiController.Ddoc.GetFolder(folderId);
        IEnumerable<IGrouping<string, DdocRule>> groupings = (await galleryApiController.Ddoc.GetRulesForParentCollection(folder.CollectionId)).GroupBy<DdocRule, string, DdocRule>((Func<DdocRule, string>) (r => r.ChildId), (Func<DdocRule, DdocRule>) (r => r));
        dataAccess = IoCContainer.GetService<GalleryDAL>();
        try
        {
          foreach (IGrouping<string, DdocRule> grouping in groupings)
          {
            IGrouping<string, DdocRule> ruleGroup = grouping;
            DdocCollection collection = await galleryApiController.Ddoc.GetCollection(ruleGroup.Key);
            ExpMember expMember = new ExpMember()
            {
              CollectionName = collection.Name,
              CollectionId = ruleGroup.Key
            };
            foreach (string str in (await dataAccess.GetFolderContents(ruleGroup.Key, (IEnumerable<KeyValuePair<string, string>>) ruleGroup.Select<DdocRule, KeyValuePair<string, string>>((Func<DdocRule, KeyValuePair<string, string>>) (r => new KeyValuePair<string, string>(string.Format("C{0}", (object) r.ChildField.Value), folder.Data.Single<DdocField>((Func<DdocField, bool>) (f =>
            {
              int id = f.Id;
              int? parentField = r.ParentField;
              int valueOrDefault = parentField.GetValueOrDefault();
              return id == valueOrDefault & parentField.HasValue;
            })).Value))).ToDictionary<string, string>())).ToList<string>())
            {
              string documentId = str;
              List<DdocPage> pages = await galleryApiController.Ddoc.GetPages(documentId);
              expMember.Documents.Add(new ExpDocument()
              {
                DocumentId = documentId,
                PageIds = pages.Select<DdocPage, string>((Func<DdocPage, string>) (p => p.Id)).ToList<string>()
              });
              documentId = (string) null;
            }
            results.Add(expMember);
            expMember = (ExpMember) null;
            ruleGroup = (IGrouping<string, DdocRule>) null;
          }
        }
        finally
        {
          dataAccess?.Dispose();
        }
        dataAccess = (GalleryDAL) null;
      }
      catch (Exception ex)
      {
        return galleryApiController.DDocErrorMessage(ex, (object) new
        {
          expNumber = expNumber
        });
      }
      return (IHttpActionResult) galleryApiController.Ok<ApiResponse<ExpMember>>(new ApiResponse<ExpMember>(results));
    }
  }
}
