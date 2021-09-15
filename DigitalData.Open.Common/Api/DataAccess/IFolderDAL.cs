// Decompiled with JetBrains decompiler

using DigitalData.Open.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IFolderDAL : ICommonDAL
  {
    Task<string> CreateFolder(DdocFolder folder);

    Task CreateFolderData(
      string folderId,
      string collectionId,
      IEnumerable<DdocField> folderData);

    Task<int> DeleteFolder(string folderId, string user);

    Task<bool> FolderExists(
      DdocFolder folder,
      List<int> includedFields = null,
      List<int> excludedFields = null);

    Task<bool> FolderExists(string folderId);

    Task<IEnumerable<DdocFolder>> FolderTextSearch(
      string collectionId,
      List<DdocField> fields,
      string searchText);

    Task<DdocFolder> GetFolder(string folderId);

    Task<IEnumerable<DdocChildEntity>> GetFolderContents(
      string childCollectionId,
      CollectionType childCollectionType,
      Dictionary<string, string> filters,
      List<DdocField> childFields);

    Task<IEnumerable<DdocField>> GetFolderData(
      string folderId,
      string collectionId,
      List<DdocField> fields);

    Task<int> CountFolders(string collectionId, IEnumerable<int> securityGroups);

    Task<IEnumerable<DdocFolder>> GetFolders(
      string collectionId,
      IEnumerable<int> securityGroups,
      int page,
      int pageSize,
      string sortBy,
      int sortDirection);

    Task<(int totalRecords, IEnumerable<DdocFolder> results)> SearchFolders(
      DdocSearchParameters searchParameters,
      List<DdocField> fields,
      int page,
      int pageSize);

    Task<IEnumerable<DdocFolder>> PrintSearchFolders(
      DdocSearchParameters searchParameters,
      List<DdocField> fields);

    Task UpdateFolderData(
      string folderId,
      string collectionId,
      IEnumerable<DdocField> folderData);

    Task UpdateFolderSecurity(string folderId, int securityGroupId);

    Task UpdateFoldertDataKey(string folderId, string dataKey);
  }
}
