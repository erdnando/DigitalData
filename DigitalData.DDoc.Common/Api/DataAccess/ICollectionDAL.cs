// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.ICollectionDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api.DataAccess
{
  public interface ICollectionDAL : ICommonDAL
  {
    Task<int> AddCollectionField(string collectionId, DdocField field);

    Task AlterCollectionField(DdocField field);

    Task<bool> CollectionExists(string collectionId);

    Task<bool> CollectionFieldExists(int fieldId);

    Task<bool> CollectionHasData(string collectionId);

    Task<string> CreateCollection(DdocCollection collection);

    Task CreateCollectionField(int fieldId);

    Task CreateCollectionFieldsTable(string collectionId);

    Task<bool> CollectionRuleExists(int ruleId);

    Task<int> CreateCollectionRule(DdocRule rule);

    Task DeleteCollection(string collectionId);

    Task DeleteCollectionField(int fieldId);

    Task DeleteRule(int id);

    Task DropCollectionField(int fieldId);

    Task FinalizeCollections();

    Task<IEnumerable<DdocCollection>> GetChildCollections(
      string collectionId,
      string userGroupsCsv);

    Task<DdocCollection> GetCollection(string collectionId);

    Task<IEnumerable<DdocField>> GetCollectionFields(string collectionId);

    Task<string> GetCollectionId(int fieldId);

    Task<string> GetCollectionId(string collectionName, char collectionType);

    Task<bool> GetCollectionLockStatus();

    Task<IEnumerable<DdocRule>> GetCollectionRules();

    Task<IEnumerable<DdocCollection>> GetCollections();

    Task<string> GetCollectionType(string collectionId);

    Task<IEnumerable<DdocCollection>> GetParentCollections(
      string collectionId,
      string userGroupsCsv = null);

    Task<IEnumerable<DdocCollection>> GetRootCollections(
      string userGroupsCsv);

    Task<IEnumerable<DdocRule>> GetRulesForChildCollection(
      string collectionId);

    Task<IEnumerable<DdocRule>> GetRulesForChildField(int fieldId);

    Task<IEnumerable<DdocRule>> GetRulesForParentCollection(
      string collectionId);

    Task<IEnumerable<DdocRule>> GetRulesForParentField(int fieldId);

    Task<IEnumerable<DdocCollection>> GetSearchableCollections(
      string securityGroupsCsv,
      string collectionType = null);

    Task<int> GetSecurityGroupId(string collectionName, char collectionType);

    Task MoveCollectionField(int fieldId, bool direction);

    Task<IEnumerable<DdocCollection>> SearchCollection(string name);

    Task<int> UpdateCollection(DdocCollection collection);

    Task<int> UpdateCollectionField(DdocField field);

    Task<int> UpdateCollectionFilenameTemplate(string collectionId, string filenameTemplate);

    Task<int> UpdateCollectionRule(DdocRule rule);

    Task<DdocField> GetCollectionField(int fieldId);
  }
}
