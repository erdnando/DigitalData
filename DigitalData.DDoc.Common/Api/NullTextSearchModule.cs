// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.NullTextSearchModule
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api
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
