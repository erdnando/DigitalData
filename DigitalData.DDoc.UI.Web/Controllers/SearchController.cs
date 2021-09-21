
using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Catalogs;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Extensions;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class SearchController : DdocController
  {
    private readonly SearchController.CustomSearchFunction CustomSearch;

    public SearchController(
      IDdocService ddoc,
      IOptions<DiDaSettings> settings,
      ICustomSearch customSearch,
      ICustomActions customActions)
      : base(ddoc, settings.Value, customActions.CustomActions)
    {
      if (customSearch.Search == null)
        return;
      this.CustomSearch = new SearchController.CustomSearchFunction(customSearch.Search.Invoke);
    }

    public async Task<ActionResult> AdvancedSearch(
      DdocSearchParameters searchParameters)
    {
      SearchController searchController = this;
      searchController.ViewData["EnableTextSearch"] = (object) searchController.Ddoc.InstanceFeatures.TextIndexing;
      List<DdocCollection> searchableCollections = await searchController.Ddoc.GetSearchableCollections(searchController.CurrentUserSession.DdocGroups);
      searchController.ViewData["AdvancedSearchableCollections"] = (object) searchableCollections;
      searchController.ViewData["OcrSearchableCollections"] = (object) searchableCollections.Where<DdocCollection>((Func<DdocCollection, bool>) (c => c.Type == CollectionType.D));
      searchController.ViewData["SearchTypeList"] = (object) DdocCatalogs.GetSearchTypeList().ToList<KeyValuePair<int, string>>();
      if (searchController.TempData["SearchParameters"] is DdocSearchParameters searchParameters1)
        searchParameters = searchParameters1;
      searchController.ViewData["TextComparisons"] = (object) DdocCatalogs.GetFilterComparisonList(FieldType.Text);
      searchController.ViewData["NumericComparisons"] = (object) DdocCatalogs.GetFilterComparisonList(FieldType.Number);
      searchController.ViewData["DateComparisons"] = (object) DdocCatalogs.GetFilterComparisonList(FieldType.Date);
      searchController.ViewData["BooleanComparisons"] = (object) DdocCatalogs.GetFilterComparisonList(FieldType.Boolean);
      return (ActionResult) searchController.PartialView((object) searchParameters);
    }

    public PartialViewResult SearchClauseGroup(int tabindex)
    {
      this.ViewData[nameof (tabindex)] = (object) tabindex;
      return this.PartialView();
    }

    public PartialViewResult NewSearchClause(int tabindex)
    {
      this.ViewData[nameof (tabindex)] = (object) tabindex;
      return this.PartialView();
    }

    [HttpPost]
    public async Task<ActionResult> SearchFolders(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17)
    {
      SearchController searchController = this;
      Response<DdocFolder> response1 = new Response<DdocFolder>();
      try
      {
        searchParams.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        Response<DdocFolder> response2 = response1;
        Response<DdocFolder> response3 = response1;
        (response2.Total, response3.List) = await searchController.Ddoc.SearchFolders(searchParams, page, pageSize);
        response2 = (Response<DdocFolder>) null;
        response3 = (Response<DdocFolder>) null;
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (SearchFolders),
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocFolder>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SearchDocuments(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17)
    {
      SearchController searchController = this;
      Response<DdocDocument> response1 = new Response<DdocDocument>();
      try
      {
        searchParams.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        Response<DdocDocument> response2 = response1;
        Response<DdocDocument> response3 = response1;
        (response2.Total, response3.List) = await searchController.Ddoc.SearchDocuments(searchParams, page, pageSize);
        response2 = (Response<DdocDocument>) null;
        response3 = (Response<DdocDocument>) null;
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (SearchDocuments),
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocDocument>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> PrintFolderSearchResults(
      DdocSearchParameters searchParams)
    {
      SearchController searchController = this;
      Response<DdocFolder> response1 = new Response<DdocFolder>();
      try
      {
        searchParams.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        Response<DdocFolder> response2 = response1;
        response2.TextResult = await searchController.Ddoc.PrintFolderSearchResults(searchParams);
        response2 = (Response<DdocFolder>) null;
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "ExportFolderSearch",
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocFolder>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<JsonResult> PrintDocumentSearchResults(
      DdocSearchParameters searchParams)
    {
      SearchController searchController = this;
      Response<DdocDocument> response = new Response<DdocDocument>();
      try
      {
        searchParams.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        response.TextResult = await searchController.Ddoc.PrintDocumentSearchResults(searchParams);
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "ExportDocumentSearch",
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      JsonResult jsonResult = searchController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response<DdocDocument>) null;
      return jsonResult;
    }

    [HttpPost]
    public async Task<ActionResult> DocumentMassDownload(
      DdocSearchParameters searchParams)
    {
      SearchController searchController = this;
      Response<DdocDocument> response = new Response<DdocDocument>();
      try
      {
        searchParams.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        response.TextResult = await searchController.Ddoc.DocumentMassDownload(searchParams);
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (DocumentMassDownload),
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response<DdocDocument>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> TextSeek(
      string collectionId,
      CollectionType collectionType,
      string searchText)
    {
      SearchController searchController = this;
      Response<DdocChildEntity> response = new Response<DdocChildEntity>();
      try
      {
        List<DdocChildEntity> ddocChildEntityList = (List<DdocChildEntity>) null;
        switch (collectionType)
        {
          case CollectionType.F:
            ddocChildEntityList = (await searchController.Ddoc.FolderTextSearch(collectionId, searchText)).Cast<DdocChildEntity>().ToList<DdocChildEntity>();
            break;
          case CollectionType.D:
            ddocChildEntityList = (await searchController.Ddoc.DocumentTextSearch(collectionId, searchText)).Cast<DdocChildEntity>().ToList<DdocChildEntity>();
            break;
        }
        response.Result = RequestResult.Success;
        response.List = ddocChildEntityList;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      JsonResult jsonResult = searchController.Json((object) response, JsonRequestBehavior.AllowGet);
      jsonResult.MaxJsonLength = new int?(int.MaxValue);
      ActionResult actionResult = (ActionResult) jsonResult;
      response = (Response<DdocChildEntity>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> GlobalSearchResults(
      DdocSearchParameters parameters)
    {
      SearchController searchController = this;
      parameters.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
      List<DdocCollection> ddocCollectionList;
      if (searchController.CustomSearch != null)
        ddocCollectionList = await searchController.CustomSearch(parameters);
      else
        ddocCollectionList = await searchController.Ddoc.GlobalSearch(parameters);
      searchController.ViewData["GlobalSearchMaxHits"] = (object) searchController.Settings.GetSetting<int>("GlobalSearchMaxHits");
      
      //new, just to include pdf metadata from solr
      if (ddocCollectionList.Count == 0)
      {
          ddocCollectionList.Add(dummyExpediente(parameters.TextQuery));
          ddocCollectionList.Add(dummydocument(parameters.TextQuery));
      }

      searchController.ViewData["SearchableCollections"] = (object)ddocCollectionList;

      await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "GlobalSearch",
        User = searchController.CurrentUserSession.Username,
        Module = searchController.ModuleName,
        Details = parameters.TextQuery
      });
      return (ActionResult) searchController.PartialView((object) parameters);
    }

    private DdocCollection dummyExpediente(string txtQuery)
        {
            List<DdocField> auxListField = new List<DdocField>
            {
                new DdocField
                {
                    AllowedValuesString="",
                    Computed=false,
                    CreateRule=false,
                    ForceStructChange=false,
                    Hidden=false,
                    InMask="",
                    IncludeInGlobalSearch=true,
                    Inheritable=true,
                    IsNew=false,
                    Nullable=false,
                    OutMask="",
                    Unique=false,
                    Id = 0,//149
                    Name = "",//idExpediente
                    Sequence = 1,
                    Type = FieldType.Text,
                    TypeLength = 50,
                    TypeString = "VARCHAR(50)",
                }
            };

            List<DdocField> auxListFieldData = new List<DdocField>
            {
                new DdocField
                {
                    AllowedValuesString="",
                    Computed=false,
                    CreateRule=false,
                    ForceStructChange=false,
                    Hidden=false,
                    InMask="",
                    IncludeInGlobalSearch=true,
                    Inheritable=false,
                    IsNew=false,
                    Nullable=false,
                    OutMask="",
                    Unique=false,
                    Id = 0,//149
                    Name = "",//idExpedientex
                    Sequence = 0,
                    Type = FieldType.Text,
                    TypeLength = 50,
                    TypeString = "VARCHAR(50)",
                }
            };

            List<DdocChildEntity> auxSearchResult = new List<DdocChildEntity>
            {
                new DdocChildEntity
                {
                    CollectionId = "",  //01C000001G
                    CreationDate=new DateTime(),
                    Id="",//01F00001TI
                    IsNew=false,
                    Name="",
                    SecurityGroupId=0,//45
                    Data=auxListFieldData
                }
            };

            //main object
            return new DdocCollection
            {
                Cfdi = false,
                Description = null,
                Fields = auxListField,
                FieldCount = 1,
                HasChildren=true,
                HasData=false,
                HasDataTable=true,
                Id = "01C000001G",//ok  --> Expediente (UNAM)
                Name = "",//Expediente (UNAM*)
                ParentId = "",//01C000001F ok --> UNAM (D)
                SecurityGroupId = 0,//45
                TotalSearchResults = 1,
                Type = CollectionType.F,
                WarehouseId=1,
                SearchResults = auxSearchResult
            };
        }

    private DdocCollection dummydocument(string txtQuery)
        {
            List<DdocField> auxListField = new List<DdocField>
            {
                new DdocField
                {
                    AllowedValuesString="",
                    Computed=false,
                    CreateRule=false,
                    ForceStructChange=false,
                    Hidden=false,
                    InMask="",
                    IncludeInGlobalSearch=true,
                    Inheritable=true,
                    IsNew=false,
                    Nullable=false,
                    OutMask="",
                    Unique=false,
                    Id = 0,//154
                    Name = "Folio expediente",//titulo col
                    Sequence = 1,
                    Type = FieldType.Text,
                    TypeLength = 50,
                    TypeString = "VARCHAR(50)",
                }
            };

            List<DdocField> auxListFieldData = new List<DdocField>
            {
                new DdocField
                {
                    AllowedValuesString="",
                    Computed=false,
                    CreateRule=false,
                    ForceStructChange=false,
                    Hidden=false,
                    InMask="",
                    IncludeInGlobalSearch=true,
                    Inheritable=false,
                    IsNew=false,
                    Nullable=false,
                    OutMask="",
                    Unique=false,
                    Id = 0,//154
                    Name = "",//idExpediente
                    Sequence = 0,
                    Value= txtQuery,//valor  encontrado o descripcion corta de lo buscado
                    Type = FieldType.Text,
                    TypeLength = 50,
                    TypeString = "VARCHAR(50)",
                }
            };

            List<DdocChildEntity> auxSearchResult = new List<DdocChildEntity>
            {
                new DdocChildEntity
                {
                    CollectionId = "",//01C000001I
                    CreationDate=new DateTime(),
                    Id= "01D00004IB",//01P000043B   id del documento q abrira en el icono d elupita,
                    IsNew=false,
                    Name="",
                    SecurityGroupId=0,//44
                     Data=auxListFieldData
                }
            };

            //main object
            return new DdocCollection
            {
                Cfdi = false,
                HasChildren=false,
                HasData=false,
                HasDataTable=true,
                Description = null,
                Fields = auxListField,
                FieldCount = 1,
                Id = "01C000001H",//tipo documento en el treeview, en este caso, es contrato
                Name = "Documento digital",//titulo encabezado del doc encontrado
                IsNew=false,
                ParentId = "",//01C000001G
                SecurityGroupId = 0,//44
                TotalSearchResults = 1,
                Type = CollectionType.D,
                WarehouseId=2,
                SearchResults= auxSearchResult
            };
        }

        [HttpPost]
    public async Task<ActionResult> GlobalSearch2Results(
      DdocSearchParameters parameters)
    {
      SearchController searchController = this;
      searchController.ViewData["GlobalSearchMaxHits"] = (object) searchController.Settings.GetSetting<int>("GlobalSearchMaxHits");
      searchController.ViewData["TextSearchQuery"] = (object) parameters.TextQuery;
      await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "GlobalSearch2",
        User = searchController.CurrentUserSession.Username,
        Module = searchController.ModuleName,
        Details = parameters.TextQuery
      });
      return (ActionResult) searchController.PartialView();
    }

    [NoCache]
    public async Task<ActionResult> SearchText(DdocSearchParameters parameters)
    {
      SearchController searchController = this;
      Response<DdocChildEntity> response1 = new Response<DdocChildEntity>();
      try
      {
        parameters.SecurityGroupsCsv = searchController.CurrentUserSession.DdocGroups;
        Response<DdocChildEntity> response2 = response1;
        Response<DdocChildEntity> response3 = response1;
        (response2.Total, response3.List) = await searchController.Ddoc.TextSearch(parameters);
        response2 = (Response<DdocChildEntity>) null;
        response3 = (Response<DdocChildEntity>) null;
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = "OcrTextSearch",
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName,
          Details = parameters.TextQuery
        });
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocChildEntity>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveSearchFilters(
      DdocSearchParameters searchParameters)
    {
      SearchController searchController = this;
      Response<DdocChildEntity> response = new Response<DdocChildEntity>();
      try
      {
        response.TextResult = await searchController.Ddoc.SaveSearchFilters(searchParameters);
        await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
        {
          Action = nameof (SaveSearchFilters),
          User = searchController.CurrentUserSession.Username,
          Module = searchController.ModuleName
        });
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response);
      response = (Response<DdocChildEntity>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> LoadSearchFilters()
    {
      SearchController searchController = this;
      Response<DdocSearchParameters> response = new Response<DdocSearchParameters>();
      try
      {
        if (searchController.Request.Files.Count == 1)
        {
          Stream inputStream = searchController.Request.Files[0]?.InputStream;
          DdocSearchParameters searchParameters;
          using (XmlReader xmlReader = XmlReader.Create(inputStream, new XmlReaderSettings()
          {
            ConformanceLevel = ConformanceLevel.Auto
          }))
            searchParameters = (DdocSearchParameters) new XmlSerializer(typeof (DdocSearchParameters), string.Empty).Deserialize(xmlReader);
          response.ObjectResult = searchParameters;
          await searchController.Ddoc.ActivityLog(new DdocActionLogEntry()
          {
            Action = nameof (LoadSearchFilters),
            User = searchController.CurrentUserSession.Username,
            Module = searchController.ModuleName
          });
          response.Result = RequestResult.Success;
        }
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) searchController.Json((object) response);
      response = (Response<DdocSearchParameters>) null;
      return actionResult;
    }

    private delegate Task<List<DdocCollection>> CustomSearchFunction(
      DdocSearchParameters parameters);
  }
}
