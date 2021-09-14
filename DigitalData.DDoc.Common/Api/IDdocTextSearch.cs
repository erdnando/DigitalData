// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.IDdocTextSearch
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api
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
