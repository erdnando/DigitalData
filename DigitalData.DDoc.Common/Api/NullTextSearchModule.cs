
using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public class NullTextSearchModule : IDdocTextSearch
  {
    public Task AddToIndex(
      string collectionId,
      string documentId,
      string pageId,
      string filename)
    {
      return Task.CompletedTask;
    }

    public Task AddToIndex(
      string collectionId,
      string documentId,
      string pageId,
      string fileExt,
      Stream fileStream)
    {
      return Task.CompletedTask;
    }

    public Task CreateIndex(string collectionId) => Task.CompletedTask;

    public Task RemoveFromIndex(string collectionId, string documentId, string pageId) => Task.CompletedTask;

    public Task<List<DDocTextSearchResult>> SearchIndex(
      DdocSearchParameters parameters)
    {
      throw new NotImplementedException();
    }
  }
}
