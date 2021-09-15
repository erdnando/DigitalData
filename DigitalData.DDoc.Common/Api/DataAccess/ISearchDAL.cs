// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.ISearchDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Entities;
using DigitalData.Open.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface ISearchDAL : ICommonDAL
  {
    Task<(long totalRecords, IEnumerable<DdocChildEntity> results)> AllFieldsSearch(
      string searchText,
      string collectionId,
      CollectionType collectionType,
      List<DdocField> collectionField,
      string securityGroupsCsv,
      int page,
      int pageSize,
      string sortBy,
      int sortDirection);

    Task<IEnumerable<DdocChildEntity>> GlobalSearch(
      string searchText,
      CollectionType collectionType,
      string securityGroupsCsv);

    Task<IEnumerable<DdocChildEntity>> GlobalSearch2(
      string searchText,
      CollectionType collectionType,
      string securityGroupsCsv,
      int maxHits);

    Task<IEnumerable<object>> SearchCollectionValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string textQuery);

    Task<IEnumerable<string>> SearchFieldValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string textQuery);
  }
}
