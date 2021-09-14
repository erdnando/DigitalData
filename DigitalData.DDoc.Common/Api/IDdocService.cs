// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.IDdocService
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Cfdi;
using DigitalData.Common.Entities;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Config;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Entities.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api
{
  public interface IDdocService : IDisposable
  {
    IDdocStorage Storage { get; }

    IDdocAuthentication Authentication { get; }

    DdocFeatures InstanceFeatures { get; }

    Task<DdocGroup> GetDdocGroup(int groupId, GroupType groupType);

    Task<List<DdocGroup>> GetDdocGroups(GroupType groupType);

    Task<int> SaveDdocGroup(DdocGroup group);

    Task DeleteDdocGroup(int groupId, GroupType groupType);

    Task<List<DdocPermission>> GetDdocGroupPermissions(
      int groupId,
      GroupType groupType);

    Task<List<DdocPermission>> GetAllPermissions();

    Task<int> SavePermission(DdocPermission permission);

    Task DeletePermission(int permissionId);

    Task<DdocConfiguration> GetConfiguration();

    Task<int> SaveConfiguration(DdocConfiguration configuration);

    Task<bool> UserExists(string username);

    Task<User> GetUser(string username);

    Task<List<User>> GetUsers();

    Task<int> SaveUser(User user);

    Task UpdateUserProfile(User user);

    Task<int> UpdateUserPassword(User user);

    Task<List<DdocGroup>> GetUserProfile(string username);

    Task<int> UnlockUser(string username);

    Task DeleteUser(string username);

    Task<bool> FileServerExists(string fileServerId);

    Task<List<DdocFileServer>> GetFileServers();

    Task<int> SaveFileServer(DdocFileServer fileServer);

    Task DeleteFileServer(string fileServerId);

    Task<bool> WarehouseExists(int warehouseId);

    Task<bool> WarehousePathExists(int warehousePathId);

    Task<List<DdocWarehouse>> GetWarehouses();

    Task<List<DdocPath>> GetWarehousePaths(int warehouseId);

    Task<int> SaveWarehouse(DdocWarehouse warehouse);

    Task<int> SaveWarehousePath(int warehouseId, DdocPath path);

    Task<int> DeleteWarehousePath(int pathId);

    Task<int> SetWarehouseActivePath(int warehouseId, int pathId);

    Task DeleteWarehouse(int warehouseId);

    Task<DdocDocument> GetDocument(string documentId);

    Task<int> CountDocuments(string collectionId, string securityGroupsCsv);

    Task<List<DdocDocument>> GetDocuments(
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0);

    Task<List<DdocPage>> GetPages(string documentId);

    Task<string> CreatePage(string documentId, string extension, int sequence = 0, bool replaced = false);

    Task<(string newPageId, int imageCount)> UploadPage(
      string documentId,
      string fileExt,
      byte[] byteSource,
      string pageId = null,
      int sequence = 0,
      string user = null);

    Task<(string newPageId, int imageCount)> UploadLocalPage(
      string documentId,
      string fileExt,
      string filePath,
      string pageId = null,
      int sequence = 0,
      string user = null);

    Task PopulateParents(string collectionId, List<DdocField> childFields);

    Task<string> RegisterNewDocument(string collectionId, int securityGroupId);

    Task CommitDocument(
      string documentId,
      string name,
      List<DdocField> documentData,
      bool updateDate = true,
      DateTime? creationDate = null);

    Task<string> SaveDocument(DdocDocument document, bool updateDate = true);

    Task ChangeDocumentCollection(
      string documentId,
      CollectionChangeRequest request,
      int? securityGroupId = null);

    Task UpdateDocumentData(string documentId, List<DdocField> documentData, bool updateDate = true);

    Task UpdateDocumentSecurity(string documentId, int securityGroupId);

    Task<int> DeleteDocument(string documentId, string user = null);

    Task UpdatePagesSequence(DdocDocument document);

    Task<int> DeletePage(string pageId, string user = null);

    Task<(int totalRecords, List<DdocDocument> results)> SearchDocuments(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17);

    Task<string> PrintDocumentSearchResults(DdocSearchParameters searchParams);

    Task<string> DocumentMassDownload(DdocSearchParameters searchParams);

    Task<List<DdocDocument>> DocumentTextSearch(
      string collectionId,
      string searchText);

    Task<bool> DocumentIdExists(string documentId);

    Task<bool> DocumentExists(
      DdocDocument document,
      List<int> includedFields = null,
      List<int> excludedFields = null);

    Task<bool> PageIdExists(string pageId);

    Task<(int totalRecords, List<DdocFolder> results)> SearchFolders(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17);

    Task<string> PrintFolderSearchResults(DdocSearchParameters searchParams);

    Task<List<DdocFolder>> FolderTextSearch(
      string collectionId,
      string searchText);

    Task<bool> FolderIdExists(string folderId);

    Task<bool> FolderExists(
      DdocFolder folder,
      List<int> includedFields = null,
      List<int> excludedFields = null);

    Task<List<DdocCollection>> NavigateFolder(string folderId);

    Task<List<DdocCollection>> GlobalSearch(DdocSearchParameters parameters);

    Task<List<DdocChildEntity>> GlobalSearch2(
      string textQuery,
      CollectionType collectionType,
      string securityGroupsCsv);

    Task<(long totalRecords, List<DdocChildEntity> results)> AllFieldsSearch(
      string searchText,
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0);

    Task<(int totalItems, List<DdocChildEntity> results)> TextSearch(
      DdocSearchParameters parameters);

    Task<List<string>> SearchFieldValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string query);

    Task<List<Dictionary<string, object>>> SearchCollectionValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string query);

    Task<string> SaveSearchFilters(DdocSearchParameters searchParameters);

    Task<Stream> GetDocumentStream(string pageId, int pdfPage = 0);

    Task<Stream> GetFile(string pageId, string pageType = "pdf");

    Task<Stream> GetPageThumbnail(string pageId, int width = 0, int height = 0);

    Task<int> GetPdfPageCount(string pageId);

    Task<string> GetPath(string pageId, string pageType = null);

    Task<DDocFileResponse> DocumentDownload(
      string documentId,
      string downloadFormat);

    Task<DDocFileResponse> PagesDownload(
      string pageId,
      string downloadFormat,
      int startPage,
      int endPage);

    Task<DDocFileResponse> FileDownload(string pageId, string downloadFormat);

    Task<CfdiData> GetCfdData(string pageId);

    Task<UserSession> Login(User userLogin);

    Task<DdocPermission> GetPermission(int permissionId);

    Task<Permissions> GetElementPermissions(
      CollectionType type,
      string itemId,
      GroupFilters filters);

    Task<List<DdocGroup>> GetSecurityGroups(string securityGroupsCsv);

    Task<DdocFolder> GetFolder(string folderId);

    Task<int> CountFolders(string collectionId, string securityGroupsCsv);

    Task<List<DdocFolder>> GetFolders(
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0);

    Task<string> SaveFolder(DdocFolder folder);

    Task UpdateFolderData(string folderId, List<DdocField> folderData);

    Task UpdateFolderSecurity(string folderId, int securityGroupId);

    Task<int> DeleteFolder(string folderId, string user);

    Task<bool> CollectionExists(string collectionId);

    Task<DdocCollection> GetCollection(string collectionId);

    Task<List<DdocCollection>> SearchCollection(string name);

    Task<List<DdocCollection>> GetCollections();

    Task<bool> CollectionFieldExists(int fieldId);

    Task<List<DdocField>> GetCollectionFields(string collectionId);

    Task<string> GetCollectionType(string collectionId);

    Task<List<DdocCollection>> GetSearchableCollections(
      string securityGroupsCsv,
      string collectionType = null);

    Task<List<DdocCollection>> GetRootCollections(string userGroupsCsv);

    Task<List<DdocCollection>> GetParentCollections(
      string collectionId,
      string userGroupsCsv);

    Task<List<DdocCollection>> GetChildCollections(
      string collectionId,
      string userGroupsCsv);

    Task<string> GetCollectionId(string collectionName, char collectionType);

    Task<int> GetSecurityGroupId(string collectionName, char collectionType);

    Task<bool> CollectionRuleExists(int ruleId);

    Task<List<DdocRule>> GetCollectionRules();

    Task<List<DdocRule>> GetRulesForChildCollection(string collectionId);

    Task<List<DdocRule>> GetRulesForParentCollection(string collectionId);

    Task<List<DdocRule>> GetRulesForChildField(int fieldId);

    Task<List<DdocRule>> GetRulesForParentField(int fieldId);

    Task<int> SaveCollectionRule(DdocRule rule);

    Task DeleteCollectionRule(int ruleId);

    Task<string> SaveCollection(DdocCollection collection);

    Task<int> SaveCollectionFilenameTemplate(string collectionId, string filenameTemplate);

    Task DeleteCollection(string collectionId);

    Task<int> SaveCollectionField(string collectionId, DdocField field, bool newCollection = false);

    Task MoveCollectionField(int fieldId, bool direction);

    Task DeleteCollectionField(int fieldId);

    Task FinalizeCollections();

    Task<bool> GetCollectionLockStatus();

    Task<List<DdocReportRecord>> GetDailyReport(
      DateTime fromDate,
      DateTime toDate);

    Task<List<DdocReportRecord>> GetRangeDate(
      string securityGroupsCsv,
      DateTime fromDate,
      DateTime toDate);

    Task<List<DdocReportRecord>> GetTotalReport(string securityGroupsCsv);

    Task ActivityLog(DdocActionLogEntry entry);

    Task<DdocField> GetCollectionField(int fieldId);

    Task<List<DdocDocument>> GetOrphanDocuments();

    Task<List<DdocDocument>> GetExpiredDocuments();
  }
}
