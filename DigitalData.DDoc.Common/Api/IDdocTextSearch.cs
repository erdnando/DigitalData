
using DigitalData.Open.Common.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public interface IDdocTextSearch
  {
    Task AddToIndex(string collectionId, string documentId, string pageId, string filename);

    Task AddToIndex(
      string collectionId,
      string documentId,
      string pageId,
      string fileExt,
      Stream fileStream);

    Task CreateIndex(string collectionId);

    Task RemoveFromIndex(string collectionId, string documentId, string pageId);

    Task<List<DDocTextSearchResult>> SearchIndex(
      DdocSearchParameters parameters);
  }
}
