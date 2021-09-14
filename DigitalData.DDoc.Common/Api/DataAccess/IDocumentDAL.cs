// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.IDocumentDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api.DataAccess
{
  public interface IDocumentDAL : ICommonDAL
  {
    Task<string> RegisterNewDocument(string collectionId, int securityGroupId);

    Task CommitDocument(string id, string name, string datakey);

    Task UpdateCreationDate(string id, DateTime creationDate);

    Task<string> CreateDocument(DdocDocument document);

    Task CreateDocumentData(
      string documentId,
      string collectionId,
      IEnumerable<DdocField> documentData);

    Task<string> CreatePage(
      string documentId,
      string extension,
      int sequence,
      int? pathId = null,
      bool replaced = false);

    Task<int> DeleteDocument(string documentId, string user);

    Task DeleteDocumentData(string documentId, string collectionId);

    Task<int> DeletePage(string pageId, string user);

    Task<bool> DocumentExists(
      DdocDocument document,
      List<int> includedFields = null,
      List<int> excludedFields = null);

    Task<bool> DocumentExists(string documentId);

    Task<bool> PageExists(string pageId);

    Task<IEnumerable<DdocDocument>> DocumentTextSearch(
      string collectionId,
      List<DdocField> fields,
      string searchText);

    Task<string> GetCollectionId(string documentId);

    Task<DdocDocument> GetDocument(string documentId);

    Task<IEnumerable<DdocField>> GetDocumentData(
      string documentId,
      string collectionId,
      List<DdocField> fields);

    Task<string> GetDocumentId(string pageId);

    Task<int> CountDocuments(string collectionId, IEnumerable<int> securityGroups);

    Task<IEnumerable<DdocDocument>> GetDocuments(
      string collectionId,
      IEnumerable<int> securityGroups,
      int page,
      int pageSize,
      string sortBy,
      int sortDirection);

    Task<IEnumerable<DdocPage>> GetPages(string documentId);

    Task<int> GetPageSequence(string pageId);

    Task<int> GetWarehousePathId(string documentId);

    Task<bool> PathExists(int pathId);

    Task<IEnumerable<DdocDocument>> PrintSearchDocuments(
      DdocSearchParameters searchParameters,
      List<DdocField> fields);

    Task<(int totalRecords, IEnumerable<DdocDocument> results)> SearchDocuments(
      DdocSearchParameters searchParameters,
      List<DdocField> fields,
      int page,
      int pageSize);

    Task UpdateDocumentCollection(string documentId, string collectionId);

    Task UpdateDocumentData(
      string documentId,
      string collectionId,
      IEnumerable<DdocField> documentData,
      bool updateDate = true);

    Task UpdateDocumentDataKey(string documentId, string dataKey);

    Task UpdateDocumentSecurity(string documentId, int securityGroupId);

    Task UpdatePageImageCount(string pageId, int imageCount);

    Task UpdatePageSequence(string pageId, int sequence);

    Task UpdatePageWarehousePathId(string pageId, int pathId);

    Task<IEnumerable<DdocDocument>> GetOrphanDocuments(int timeoutMinutes);

    Task<int> GetCommitTimeout();

    Task<IEnumerable<DdocDocument>> GetExpiredDocuments();
  }
}
